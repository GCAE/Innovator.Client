﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  class AmlElement : Element
  {
    private ElementFactory _amlContext;
    private string _name;
    private ILinkedElement _next;
    private IElement _parent;

    public override ElementFactory AmlContext { get { return _amlContext; } }
    public override bool Exists { get { return Next != null || _parent == _nullElem; } }
    public override string Name { get { return _name; } }
    public override ILinkedElement Next
    {
      get { return _next; }
      set { _next = value; }
    }
    public override IElement Parent
    {
      get { return _parent ?? NullElem; }
      set { _parent = value; }
    }

    private AmlElement() { }
    public AmlElement(ElementFactory amlContext, string name, params object[] content)
    {
      _amlContext = amlContext;
      _name = name;
      _parent = NullElem;
      Add(content);
    }
    public AmlElement(IElement parent, string name)
    {
      _amlContext = parent.AmlContext;
      _name = name;
      _parent = parent;
    }
    public AmlElement(IElement parent, IReadOnlyElement elem) : base()
    {
      _amlContext = parent.AmlContext;
      _name = elem.Name;
      _parent = parent;
      CopyData(elem);
    }

    private static AmlElement _nullElem = new AmlElement();
    public static AmlElement NullElem { get { return _nullElem; } }
  }
}
