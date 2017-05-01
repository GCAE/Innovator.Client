using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type State Distribution </summary>
  public class StateDistribution : Item, INullRelationship<StateEMail>, IRelationship<Identity>
  {
    protected StateDistribution() { }
    public StateDistribution(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static StateDistribution() { Innovator.Client.Item.AddNullItem<StateDistribution>(new StateDistribution { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}