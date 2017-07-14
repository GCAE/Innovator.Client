﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  public enum ParameterSubstitutionMode
  {
    Aml,
    Sql
  }

  public enum ParameterStyle
  {
    Sql,
    CSharp
  }

  /// <summary>
  /// Class for substituting @-prefixed parameters with their values
  /// </summary>
  /// <remarks>This class will escape values thereby preventing SQL/AML injection unless the
  /// parameter name ends with an exclamtion mark (e.g. @fileItem!)</remarks>
  public class ParameterSubstitution : IEnumerable<KeyValuePair<string, object>>
  {
    private IServerContext _context;
    private SqlFormatter _sqlFormatter;
    private Dictionary<string, object> _parameters = new Dictionary<string, object>();
    private int _itemCount = 0;

    public int ItemCount { get { return _itemCount; } }
    public ParameterSubstitutionMode Mode { get; set; }
    public int ParamCount { get { return _parameters.Count; } }
    public Action<string> ParameterAccessListener { get; set; }
    public ParameterStyle Style { get; set; }

    public ParameterSubstitution()
    {
      this.Style = ParameterStyle.Sql;
      this.Mode = ParameterSubstitutionMode.Aml;
    }

    public void AddIndexedParameters(object[] values)
    {
      if (values == null)
      {
        AddParameter("0", null);
        return;
      }

      if (values.GetType().GetElementType().Name != "Object")
        values = new object[] { values };

      for (var i = 0; i < values.Length; i++)
      {
        AddParameter(i.ToString(), values[i]);
      }
    }
    public void AddParameter(string name, object value)
    {
      _parameters[name] = value;
    }

    public void ClearParameters()
    {
      _parameters.Clear();
    }

    public void Substitute(string query, IServerContext context, XmlWriter writer)
    {
      if (_context != context)
      {
        _context = context;
        _sqlFormatter = new SqlFormatter(context);
      }

      if (string.IsNullOrEmpty(query)) return;
      var i = 0;
      while (i < query.Length && char.IsWhiteSpace(query[i])) i++;
      if (i >= query.Length) return;

      SubstituteAml(query, context, writer);
    }
    public string Substitute(string query, IServerContext context)
    {
      if (_context != context)
      {
        _context = context;
        _sqlFormatter = new SqlFormatter(context);
      }

      if (string.IsNullOrEmpty(query)) return query;
      var i = 0;
      while (i < query.Length && char.IsWhiteSpace(query[i])) i++;
      if (i >= query.Length) return query;

      if (query[i] == '<' && this.Mode == ParameterSubstitutionMode.Aml)
      {
        var builder = new StringBuilder(query.Length);
        using (var writer = new StringWriter(builder))
        using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings() { OmitXmlDeclaration = true }))
        {
          SubstituteAml(query, context, xmlWriter);
        }

        return builder.ToString();
      }
      else
      {
        return SqlReplace(query);
      }
    }

    private void SubstituteAml(string query, IServerContext context, XmlWriter xmlWriter)
    {
      using (var reader = new StringReader(query))
      using (var xmlReader = XmlReader.Create(reader))
      {
        Parameter condition = null;
        DynamicDateTimeRange dateRange = null;
        var tagNames = new List<string>();
        string tagName;
        Parameter param;
        var attrs = new List<Parameter>();

        while (xmlReader.Read())
        {
          switch (xmlReader.NodeType)
          {
            case XmlNodeType.CDATA:
              param = RenderValue((string)condition, xmlReader.Value);
              if (param.IsRaw)
                throw new InvalidOperationException("Can't have a raw parameter in a CDATA section");
              xmlWriter.WriteCData(param.Value);
              break;
            case XmlNodeType.Comment:
              xmlWriter.WriteComment(xmlReader.Value);
              break;
            case XmlNodeType.Element:
              dateRange = null;
              xmlWriter.WriteStartElement(xmlReader.Prefix, xmlReader.LocalName
                , xmlReader.NamespaceURI);
              tagName = xmlReader.LocalName;

              var isEmpty = xmlReader.IsEmptyElement;

              attrs.Clear();
              for (var i = 0; i < xmlReader.AttributeCount; i++)
              {
                xmlReader.MoveToAttribute(i);
                param = RenderValue(xmlReader.LocalName, xmlReader.Value);
                param.ContextName = xmlReader.LocalName;
                param.Prefix = xmlReader.Prefix;
                param.NsUri = xmlReader.NamespaceURI;
                if (param.IsRaw)
                  throw new InvalidOperationException("Can't have a raw parameter in an attribute");
                attrs.Add(param);
                if (xmlReader.LocalName == "condition")
                {
                  condition = param;
                }
                else if (xmlReader.LocalName == "origDateRange" && !DynamicDateTimeRange.TryDeserialize(xmlReader.Value, out dateRange))
                {
                  dateRange = null;
                }
              }

              // Deal with date ranges
              if (dateRange != null)
              {
                if (condition == null)
                {
                  condition = new Parameter() { ContextName = "condition", Value = "between" };
                  attrs.Add(condition);
                }

                var dateCondition = dateRange.Condition();
                if (dateCondition != Condition.Undefined)
                  condition.Value = context.Format(dateCondition);
              }

              foreach (var attr in attrs)
              {
                xmlWriter.WriteAttributeString(attr.Prefix, attr.ContextName, attr.NsUri, attr.Value);
              }

              switch (tagName)
              {
                case "Item":
                  if (!tagNames.Any(n => n == "Item")) _itemCount++;
                  break;
                case "sql":
                case "SQL":
                  condition = "sql";
                  break;
              }

              if (isEmpty)
              {
                xmlWriter.WriteEndElement();
              }
              else
              {
                tagNames.Add(tagName);
              }
              break;
            case XmlNodeType.EndElement:
              xmlWriter.WriteEndElement();
              tagNames.RemoveAt(tagNames.Count - 1);
              condition = null;
              break;
            case XmlNodeType.SignificantWhitespace:
              xmlWriter.WriteWhitespace(xmlReader.Value);
              break;
            case XmlNodeType.Text:
              param = RenderValue((string)condition, dateRange == null ? xmlReader.Value : _context.Format(dateRange));

              var dynRange = param.Original as DynamicDateTimeRange;
              var statRange = param.Original as StaticDateTimeRange;
              if (dynRange != null)
              {
                if (!attrs.Any(p => p.Name == "condition"))
                  xmlWriter.WriteAttributeString("condition", context.Format(dynRange.Condition()));
                if (!attrs.Any(p => p.Name == "origDateRange"))
                  xmlWriter.WriteAttributeString("origDateRange", dynRange.Serialize());
              }
              else if (statRange != null)
              {
                if (!attrs.Any(p => p.Name == "condition"))
                  xmlWriter.WriteAttributeString("condition", context.Format(statRange.Condition()));
              }

              if (param.IsRaw)
              {
                xmlWriter.WriteRaw(param.Value);
              }
              else
              {
                xmlWriter.WriteValue(param.Value);
              }
              break;
          }

        }
      }
    }

    internal string RenderParameter(string name, IServerContext context)
    {
      _context = context;
      return RenderValue("", name).Value;
    }
    private Parameter RenderValue(string context, string content)
    {
      object value;
      var param = new Parameter();
      if (content.IsNullOrWhiteSpace())
      {
        return param.WithValue(content);
      }
      else if (TryFillParameter(content, param) && TryGetParamValue(param.Name, out value))
      {
        param.Original = value;
        if (param.IsRaw) return param.WithValue((value ?? "").ToString());

        switch (context)
        {
          case "idlist":
            return param.WithValue(RenderSqlEnum(value, false, o => _context.Format(o)));
          case "in":
          case "not in":
            return param.WithValue(RenderSqlEnum(value, true, o => _context.Format(o)));
          case "like":
          case "not like":
            // Do something useful with context
            return param.WithValue(RenderSqlEnum(value, false, o => _context.Format(o)));
          default:
            return param.WithValue(RenderSqlEnum(value, false, o => _context.Format(o)));
        }
      }
      else if (context == "between" || context == "not between")
      {
        // Do something useful here
        return param.WithValue(content);
      }
      else if (context == "sql"
        || context == "where")
      {
        return param.WithValue(SqlReplace(content));
      }
      else if ((context == "in" || context == "not in")
            && content.TrimStart()[0] == '(')
      {
        content = content.Trim();
        if (content.Length > 2 && content[1] == '@')
        {
          // Dapper is trying to be too helpful with parameter expansion
          return param.WithValue(SqlReplace(content.TrimStart('(').TrimEnd(')')));
        }
        else
        {
          return param.WithValue(SqlReplace(content));
        }
      }
      else
      {
        return param.WithValue(content);
      }
    }

    private bool TryGetNumericEnumerable(object value, out IEnumerable enumerable)
    {
      if (value is IEnumerable<short>
        || value is IEnumerable<int>
        || value is IEnumerable<long>
        || value is IEnumerable<ushort>
        || value is IEnumerable<uint>
        || value is IEnumerable<ulong>
        || value is IEnumerable<byte>
        || value is IEnumerable<decimal>)
      {
        enumerable = (IEnumerable)value;
        return true;
      }
      else if (value is IEnumerable<float>)
      {
        enumerable = ((IEnumerable<float>)value).Cast<decimal>();
        return true;
      }
      else if (value is IEnumerable<double>)
      {
        enumerable = ((IEnumerable<double>)value).Cast<decimal>();
        return true;
      }
      enumerable = null;
      return false;
    }

    private bool TryGetParamValue(string name, out object value)
    {
      if (ParameterAccessListener != null)
        ParameterAccessListener.Invoke(name);

      return _parameters.TryGetValue(name, out value);
    }

    private bool TryFillParameter(string value, Parameter param)
    {
      if (value == null || value.Length < 2) return false;
      if (this.Style == ParameterStyle.Sql)
      {
        if (value[0] != '@') return false;

        var end = value.Length;
        if (value[value.Length - 1] == '!')
        {
          param.IsRaw = true;
          end--;
        }
        for (var i = 1; i < end; i++)
        {
          if (!char.IsLetterOrDigit(value[i]) && value[i] != '_') return false;
        }
        param.Name = value.Substring(1, end - 1);
        return true;
      }
      else
      {
        if (value[0] != '{' || value[value.Length - 1] != '}') return false;
        var end = value.Length - 1;
        var i = value.IndexOf(':');
        if (i > 0)
          end = i;
        int index;
        if (!int.TryParse(value.Substring(1, end - 1), out index))
          return false;

        param.Name = index.ToString();
        return true;
      }
    }

    private string RenderSqlEnum(object value, bool quoteStrings, Func<object, string> format)
    {
      IEnumerable enumerable = value as IEnumerable;
      bool first = true;
      var builder = new StringBuilder();
      if (value is string)
      {
        // Deal with strings which technically are enumerables
        builder.Append(format.Invoke(value));
      }
      else if ((!quoteStrings && enumerable != null) || TryGetNumericEnumerable(value, out enumerable))
      {
        foreach (var item in enumerable)
        {
          if (!first) builder.Append(",");
          builder.Append(format.Invoke(item));
          first = false;
        }
      }
      else
      {
        enumerable = value as IEnumerable;
        if (enumerable != null)
        {
          foreach (var item in enumerable)
          {
            if (!first) builder.Append(",");
            builder.Append("N'").Append(format.Invoke(item)).Append("'");
            first = false;
          }

          // Nothing was written as there were not values in the IEnumerable
          // Therefore, write a bogus value to match zero results
          if (quoteStrings && first)
          {
            builder.Append("N'").Append(format.Invoke("`EMTPY_VALUE_LIST`")).Append("'");
          }
        }
        else
        {
          builder.Append(format.Invoke(value));
        }
      }
      return builder.ToString();
    }

    private string SqlReplace(string query)
    {
      var builder = new StringBuilder(query.Length);

      SqlReplace(query, '@', builder, p =>
      {
        object value;

        if (TryGetParamValue(p, out value))
        {
          Func<string, string> finalAction = s => s;
          var inClause = false;

          if (builder.ToString().EndsWith(" in "))
          {
            finalAction = s => "(" + s + ")";
            inClause = true;
          }
          else if (builder.ToString().EndsWith(" in ("))
          {
            inClause = true;
          }


          IFormattable num;
          if (value == null
#if DBDATA
            || value == DBNull.Value
#endif
          )
          {
            return finalAction("null");
          }
          else if (ServerContext.TryCastNumber(value, out num))
          {
            return finalAction(_sqlFormatter.Format(num));
          }
          else if(value is string)
          {
            return finalAction("N'" + RenderSqlEnum(value, false, o => _sqlFormatter.Format(o)) + "'");
          }
          else if (inClause && value is IEnumerable)
          {
            return finalAction(RenderSqlEnum(value, true, o => _sqlFormatter.Format(o)));
          }
          else
          {
            return finalAction("N'" + RenderSqlEnum(value, false, o => _sqlFormatter.Format(o)) + "'");
          }
        }
        else
        {
          return "@" + p;
        }
      });
      return builder.ToString();
    }

    private void SqlReplace(string sql, char paramPrefix, StringBuilder builder, Func<string, string> replace)
    {
      char endChar = '\0';
      int i = 0;
      var paramName = new StringBuilder(32);
      int lastWrite = 0;

      while (i < sql.Length)
      {
        if (endChar == '\0')
        {
          switch (sql[i])
          {
            case '\'':
              endChar = '\'';
              break;
            case '"':
              endChar = '"';
              break;
            case '[':
              endChar = ']';
              break;
            case '-':
              if (i + 1 < sql.Length && sql[i + 1] == '-')
              {
                endChar = '\n';
              }
              break;
            case '/':
              if (i + 1 < sql.Length && sql[i + 1] == '*')
              {
                endChar = '/';
              }
              break;
          }

          if (sql[i] == paramPrefix)
          {
            builder.Append(sql.Substring(lastWrite, i - lastWrite));
            i++;
            paramName.Length = 0;
            while (i < sql.Length && (Char.IsLetterOrDigit(sql[i]) || sql[i] == '_'))
            {
              paramName.Append(sql[i]);
              i++;
            }
            builder.Append(replace.Invoke(paramName.ToString()));
            lastWrite = i;
            i--;
          }
        }
        else if ((endChar == '\n' && sql[i] == '\r') ||
                 (endChar == '/' && sql[i] == '*' && i + 1 < sql.Length && sql[i + 1] == '/') ||
                 (sql[i] == endChar))
        {
          endChar = '\0';
        }
        i++;
      }

      if ((i - lastWrite) > 0) builder.Append(sql.Substring(lastWrite, i - lastWrite));
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
      return _parameters.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    private class Parameter
    {
      public string ContextName { get; set; }
      public bool IsRaw { get; set; }
      public string Name { get; set; }
      public string NsUri { get; set; }
      public object Original { get; set; }
      public string Prefix { get; set; }
      public string Value { get; set; }

      public Parameter WithValue(string value)
      {
        this.Value = value;
        return this;
      }

      public static implicit operator Parameter(string value)
      {
        return new Parameter() { Value = value };
      }
      public static explicit operator string(Parameter value)
      {
        if (value == null)
          return null;
        return value.Value;
      }
    }
  }
}
