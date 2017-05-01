﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  [DebuggerTypeProxy(typeof(ElementDebugView))]
  [DebuggerDisplay("{DebuggerDisplay,nq}")]
  public abstract class Element : IElement, ILinkedElement
  {
    protected ElementAttributes _attr;
    protected object _content;
    private ILinkedAnnotation _lastAttr;

    public virtual ElementFactory AmlContext
    {
      get
      {
        if (this.Parent != null)
          return this.Parent.AmlContext;
        return null;
      }
    }
    public virtual bool Exists { get { return Next != null; } }
    /// <summary>
    /// The tag name of the AML element
    /// </summary>
    public abstract string Name { get; }
    public abstract ILinkedElement Next { get; set; }
    public abstract IElement Parent { get; set; }
    IReadOnlyElement IReadOnlyElement.Parent { get { return this.Parent; } }
    IReadOnlyElement ILinkedElement.Parent
    {
      get { return this.Parent; }
      set { this.Parent = (IElement)value; }
    }
    public virtual string Value
    {
      get
      {
        if (_content != null && AmlContext != null && !(_content is ILinkedElement && !(_content is IReadOnlyItem)))
          return AmlContext.LocalizationContext.Format(_content);
        var str = _content as string;
        if (str != null)
          return str;
        return null;
      }
      set
      {
        AssertModifiable();
        _content = value;
      }
    }

    private string DebuggerDisplay
    {
      get
      {
        if (!this.Exists)
          return "{null:" + this.GetType().Name + "}";
        return this.ToAml();
      }
    }

    public bool ReadOnly
    {
      get { return string.IsNullOrEmpty(Name)
          || (_attr & ElementAttributes.ReadOnly) > 0; }
      set
      {
        if (value)
          _attr = _attr | ElementAttributes.ReadOnly;
        else
          _attr = _attr & ~ElementAttributes.ReadOnly;
      }
    }
    protected bool FromDataStore
    {
      get { return (_attr & ElementAttributes.FromDataStore) > 0; }
      set
      {
        if (value)
          _attr = _attr | ElementAttributes.FromDataStore;
        else
          _attr = _attr & ~ElementAttributes.FromDataStore;
      }
    }

    public Element() { }
    public Element(IReadOnlyElement elem)
    {
      CopyData(elem);
    }

    protected void CopyData(IReadOnlyElement elem)
    {
      Add(elem.Attributes());
      Add(elem.Elements());
      if (elem.Value != null)
        Add(elem.Value);
    }

    public virtual IElement Add(object content)
    {
      if (content == null)
        return this;

      AssertModifiable();

      var id = content as IdAnnotation;
      if (id != null)
      {
        QuickAddAttribute(id);
        return this;
      }

      var elem = TryGet(content, this);
      if (elem != null)
      {
        QuickAddElement(elem);
        return this;
      }

      if (content is string)
      {
        _content = content;
        return this;
      }

      var attr = Innovator.Client.Attribute.TryGet(content, this);
      if (attr != null)
      {
        QuickAddAttribute(attr);
        return this;
      }

      var enumerable = content as IEnumerable;
      if (enumerable != null)
      {
        foreach (var curr in enumerable)
        {
          Add(curr);
        }
        return this;
      }

      _content = content;
      return this;
    }

    internal void QuickAddElement(ILinkedElement elem)
    {
      _content = LinkedListOps.Add(_content as ILinkedElement, elem);
    }

    internal void QuickAddAttribute(ILinkedAnnotation attr)
    {
      LinkedListOps.Add(ref _lastAttr, attr);
    }

    static ILinkedElement TryGet(object value, Element newParent)
    {
      var impl = value as ILinkedElement;
      if (impl != null)
      {
        if (impl.Parent == null || !impl.Parent.Exists || impl.Parent == newParent)
        {
          impl.Parent = newParent;
          return impl;
        }

        var item = value as Item;
        if (item != null)
          return (ILinkedElement)item.Clone();
        return new AmlElement(newParent, (IReadOnlyElement)impl);
      }

      var elem = value as IReadOnlyElement;
      if (elem != null)
      {
        return new AmlElement(newParent, elem);
      }

      return null;
    }

    public IAttribute Attribute(string name)
    {
      return (((IReadOnlyElement)this).Attribute(name) as IAttribute)
        ?? Client.Attribute.NullAttr;
    }

    public IEnumerable<IAttribute> Attributes()
    {
      return LinkedListOps.Enumerate(_lastAttr).OfType<IAttribute>();
    }

    public virtual IEnumerable<IElement> Elements()
    {
      return LinkedListOps.Enumerate(_content as ILinkedElement).OfType<IElement>();
    }

    internal Element ElementByName(string name)
    {
      var elem = _content as ILinkedElement;
      if (elem != null)
      {
        var result = (LinkedListOps.Find(elem, name) as Element);
        if (result != null)
          return result;
      }
      return new AmlElement(this, name);
    }

    public void Remove()
    {
      if (Exists)
      {
        var elem = this.Parent as Element;
        if (elem != null && elem.Exists)
        {
          elem.RemoveNode(this);
        }
      }
    }
    internal void RemoveAttribute(Attribute attr)
    {
      AssertModifiable();
      LinkedListOps.Remove(ref _lastAttr, attr);
    }
    public void RemoveAttributes()
    {
      AssertModifiable();
      _lastAttr = null;
    }
    internal void RemoveNode(ILinkedElement elem)
    {
      AssertModifiable();
      var lastElem = _content as ILinkedElement;
      if (lastElem == null)
        return;
      _content = LinkedListOps.Remove(lastElem, elem);
    }
    public void RemoveNodes()
    {
      AssertModifiable();
      _content = null;
    }

    public void ToAml(XmlWriter writer, AmlWriterSettings settings)
    {
      var name = this.Name;
      var i = name.IndexOf(':');
      var attrs = LinkedListOps.Enumerate(_lastAttr).OfType<IReadOnlyAttribute>().ToArray();
      if (attrs.Any(a => a.Name == "xml:lang" && a.Value != AmlContext.LocalizationContext.LanguageCode))
      {
        writer.WriteStartElement("i18n", name, "http://www.aras.com/I18N");
      }
      else if (i > 0)
      {
        var prefix = name.Substring(0, i);
        name = name.Substring(i + 1);
        var ns = "";
        switch (prefix)
        {
          case "SOAP-ENV":
            ns = "http://schemas.xmlsoap.org/soap/envelope/";
            break;
          case "af":
            ns = "http://www.aras.com/InnovatorFault";
            break;
          default:
            throw new NotSupportedException();
        }
        writer.WriteStartElement(prefix, name, ns);
      }
      else
      {
        writer.WriteStartElement(name);
      }

      foreach (var attr in attrs)
      {
        i = attr.Name.IndexOf(':');
        if (i > 0)
        {
          var prefix = attr.Name.Substring(0, i);
          if (prefix == "xmlns")
            continue;
          name = attr.Name.Substring(i + 1);
          var ns = "";
          switch (prefix)
          {
            case "xml":
              ns = "http://www.w3.org/XML/1998/namespace";
              break;
            default:
              throw new NotSupportedException();
          }
          writer.WriteAttributeString(prefix, name, ns, attr.Value);
        }
        else
        {
          writer.WriteAttributeString(attr.Name, attr.Value);
        }

      }
      var elem = _content as ILinkedElement;
      if (elem == null)
      {
        writer.WriteString(this.Value);
      }
      else
      {
        var elems = Elements().ToArray();
        var item = elems.OfType<IReadOnlyItem>().FirstOrDefault();
        if (this is IReadOnlyProperty
          && !settings.ExpandPropertyItems
          && item != null
          && !item.Attribute("action").Exists)
        {
          writer.WriteAttributeString("type", item.TypeName());
          var keyedName = item.KeyedName().Value ?? item.Property("id").KeyedName().Value;
          if (!string.IsNullOrEmpty(keyedName))
            writer.WriteAttributeString("keyed_name", keyedName);
          writer.WriteString(item.Id());
        }
        else
        {
          foreach (var e in elems)
          {
            e.ToAml(writer, settings);
          }
        }
      }
      writer.WriteEndElement();
    }

    IReadOnlyAttribute IReadOnlyElement.Attribute(string name)
    {
      return (LinkedListOps.Find(_lastAttr, name) as IReadOnlyAttribute)
        ?? new Attribute(this, name);
    }

    IEnumerable<IReadOnlyAttribute> IReadOnlyElement.Attributes()
    {
      return LinkedListOps.Enumerate(_lastAttr).OfType<IReadOnlyAttribute>();
    }

    IEnumerable<IReadOnlyElement> IReadOnlyElement.Elements()
    {
      return LinkedListOps.Enumerate(_content as ILinkedElement).OfType<IReadOnlyElement>();
    }

    public override string ToString()
    {
      return this.ToAml();
    }

    protected void AssertModifiable()
    {
      if (this.ReadOnly)
        throw new InvalidOperationException("Cannot modify a read only element");
    }

    private class ElementDebugView
    {
      private Element _elem;

      public ElementFactory AmlContext { get { return _elem.AmlContext; } }
      public IEnumerable<IAttribute> Attributes { get { return _elem.Attributes().ToArray(); } }
      public IEnumerable<IElement> Elements { get { return _elem.Elements().ToArray(); } }
      public bool Exists { get { return _elem.Exists; } }
      public string Name { get { return _elem.Name; } }
      public IElement Parent { get { return _elem.Parent; } }
      public bool ReadOnly { get { return _elem.ReadOnly; } }
      public string Value { get { return _elem.Value; } }

      public ElementDebugView(Element elem)
      {
        _elem = elem;
      }
    }
  }
}
