using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Innovator.Client
{
  internal class Result : IResult
  {
    private ElementFactory _amlContext;
    private object _content;
    private string _database;
    private IReadOnlyItem _errorContext;
    private List<string> _errors;
    private List<string> _properties;
    private Command _query;
    private IReadOnlyElement _message = AmlElement.NullElem;

    public ServerException Exception
    {
      get
      {
        if (_errors != null && _errors.Any())
        {
          if (_errorContext == null)
          {
            return _amlContext.ServerException(_errors.GroupConcat(Environment.NewLine))
              .SetDetails(_database, _query);
          }
          else
          {
            var props = (_properties ?? Enumerable.Empty<string>()).ToArray();
            return _amlContext.ValidationException(_errors.GroupConcat(Environment.NewLine), _errorContext, props)
              .SetDetails(_database, _query);
          }
        }

        return _content as ServerException;
      }
      set
      {
        _content = value;
      }
    }
    public IReadOnlyElement Message
    {
      get { return _message; }
    }
    public string Value
    {
      get { return _content as string; }
      set { _content = value; }
    }

    public Result(ElementFactory amlContext)
    {
      _amlContext = amlContext;
    }
    public Result(ElementFactory amlContext, string database, Command query) : this(amlContext)
    {
      _database = database;
      _query = query;
    }

    public IResult Add(IReadOnlyItem content)
    {
      AddReadOnly(content);
      return this;
    }
    internal void AddReadOnly(IReadOnlyItem content)
    {
      var list = _content as IList<IReadOnlyItem>;
      if (list == null)
      {
        list = new List<IReadOnlyItem>();
        _content = list;
      }
      list.Add(content);
    }

    public void AddMessage(params object[] content)
    {
      if (!_message.Exists)
        _message = _amlContext.Element("Message", content);
      else
        ((IElement)_message).Add(content);
    }
    internal void AddMessageNode(IReadOnlyElement message)
    {
      _message = message ?? AmlElement.NullElem;
    }

    public IItem AssertItem(string type = null)
    {
      var exception = this.Exception;
      if (exception != null) throw exception;
      var items = _content as IEnumerable<IReadOnlyItem>;
      if (items == null)
        throw _amlContext.NoItemsFoundException("?", _query).SetDetails(_database, _query);
      var item = items.Single(i => true, i => i < 1
          ? (Exception)NewNoItemsException()
          : new InvalidOperationException("Multiple items were found when only one was expected.")) as IItem;
      if (item != null && (string.IsNullOrEmpty(type) || item.TypeName() == type))
        return item;
      throw new InvalidOperationException(string.Format("An item of type '{0}' was found while an item of type '{1}' was expected.", item.Type().Value, type));
    }

    public IEnumerable<IReadOnlyItem> AssertItems()
    {
      var exception = this.Exception;
      if (exception != null) throw exception;
      var items = _content as IEnumerable<IReadOnlyItem>;
      if (items == null)
        throw _amlContext.NoItemsFoundException("?", _query).SetDetails(_database, _query);
      return items;
    }

    /// <summary>
    /// Do nothing other than throw an exception if present
    /// </summary>
    public IReadOnlyResult AssertNoError()
    {
      this.AssertNoError(false);
      return this;
    }

    /// <summary>
    /// Do nothing other than throw an exception if present, optionally ignoring <see cref="NoItemsFoundException"/>
    /// </summary>
    public IReadOnlyResult AssertNoError(bool ignoreNoItemsFound)
    {
      var exception = this.Exception;
      if (exception is NoItemsFoundException && ignoreNoItemsFound) return this;
      if (exception != null) throw exception;
      return this;
    }

    public IErrorBuilder ErrorContext(IReadOnlyItem item)
    {
      _errorContext = item;
      return this;
    }

    public IErrorBuilder ErrorMsg(string message)
    {
      if (_errors == null) _errors = new List<string>();
      _errors.Add(message);
      return this;
    }

    public IErrorBuilder ErrorMsg(string message, params string[] properties)
    {
      return ErrorMsg(message, (IEnumerable<string>)properties);
    }

    public IErrorBuilder ErrorMsg(string message, IEnumerable<string> properties)
    {
      if (_errors == null) _errors = new List<string>();
      _errors.Add(message);
      if (properties.Any())
      {
        if (_properties == null) _properties = new List<string>();
        _properties.AddRange(properties);
      }
      return this;
    }

    public IEnumerable<IReadOnlyItem> Items()
    {
      var exception = this.Exception;
      if (exception is NoItemsFoundException) return Enumerable.Empty<IReadOnlyItem>();
      if (exception != null) throw exception;
      var items = _content as IEnumerable<IReadOnlyItem>;
      return items ?? Enumerable.Empty<IReadOnlyItem>();
    }

    public void ToAml(XmlWriter writer, AmlWriterSettings settings)
    {
      var ex = _content as ServerException;
      if (ex != null)
      {
        ex.ToAml(writer, settings);
      }
      else
      {
        writer.WriteStartElement("SOAP-ENV", "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
        writer.WriteStartElement("SOAP-ENV", "Body", "http://schemas.xmlsoap.org/soap/envelope/");
        writer.WriteStartElement("Result");
        var items = _content as IEnumerable<IReadOnlyItem>;
        if (items != null)
        {
          foreach (var item in items)
          {
            item.ToAml(writer, settings);
          }
        }
        else if (_content != null)
        {
          writer.WriteString(_amlContext.LocalizationContext.Format(_content));
        }
        writer.WriteEndElement();
        if (_message.Exists)
        {
          _message.ToAml(writer, settings);
        }
        writer.WriteEndElement();
        writer.WriteEndElement();
      }
    }

    IReadOnlyItem IReadOnlyResult.AssertItem(string type)
    {
      return AssertItem(type);
    }

    private ServerException NewNoItemsException()
    {
      return (this.Exception as NoItemsFoundException) ??
        _amlContext.NoItemsFoundException("?", _query).SetDetails(_database, _query);
    }

    public override string ToString()
    {
      return this.ToAml();
    }
  }
}
