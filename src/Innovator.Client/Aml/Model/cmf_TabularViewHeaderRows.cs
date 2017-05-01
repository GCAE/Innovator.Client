using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type cmf_TabularViewHeaderRows </summary>
  public class cmf_TabularViewHeaderRows : Item, INullRelationship<cmf_TabularView>, IRelationship<cmf_TabularViewHeaderRow>
  {
    protected cmf_TabularViewHeaderRows() { }
    public cmf_TabularViewHeaderRows(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static cmf_TabularViewHeaderRows() { Innovator.Client.Item.AddNullItem<cmf_TabularViewHeaderRows>(new cmf_TabularViewHeaderRows { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

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
  }
}