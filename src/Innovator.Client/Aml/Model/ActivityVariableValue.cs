using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Activity Variable Value </summary>
  public class ActivityVariableValue : Item, INullRelationship<ActivityAssignment>
  {
    protected ActivityVariableValue() { }
    public ActivityVariableValue(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ActivityVariableValue() { Innovator.Client.Item.AddNullItem<ActivityVariableValue>(new ActivityVariableValue { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>value</c> property of the item</summary>
    public IProperty_Text ValueProp()
    {
      return this.Property("value");
    }
    /// <summary>Retrieve the <c>variable</c> property of the item</summary>
    public IProperty_Item<ActivityVariable> Variable()
    {
      return this.Property("variable");
    }
  }
}