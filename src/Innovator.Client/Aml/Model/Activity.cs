using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Activity </summary>
  [ArasName("Activity")]
  public class Activity : Item
  {
    protected Activity() { }
    public Activity(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static Activity() { Innovator.Client.Item.AddNullItem<Activity>(new Activity { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>active_date</c> property of the item</summary>
    [ArasName("active_date")]
    public IProperty_Date ActiveDate()
    {
      return this.Property("active_date");
    }
    /// <summary>Retrieve the <c>can_delegate</c> property of the item</summary>
    [ArasName("can_delegate")]
    public IProperty_Boolean CanDelegate()
    {
      return this.Property("can_delegate");
    }
    /// <summary>Retrieve the <c>can_refuse</c> property of the item</summary>
    [ArasName("can_refuse")]
    public IProperty_Boolean CanRefuse()
    {
      return this.Property("can_refuse");
    }
    /// <summary>Retrieve the <c>cloned_as</c> property of the item</summary>
    [ArasName("cloned_as")]
    public IProperty_Item<Activity> ClonedAs()
    {
      return this.Property("cloned_as");
    }
    /// <summary>Retrieve the <c>closed_date</c> property of the item</summary>
    [ArasName("closed_date")]
    public IProperty_Date ClosedDate()
    {
      return this.Property("closed_date");
    }
    /// <summary>Retrieve the <c>consolidate_ondelegate</c> property of the item</summary>
    [ArasName("consolidate_ondelegate")]
    public IProperty_Boolean ConsolidateOndelegate()
    {
      return this.Property("consolidate_ondelegate");
    }
    /// <summary>Retrieve the <c>escalate_to</c> property of the item</summary>
    [ArasName("escalate_to")]
    public IProperty_Item<Identity> EscalateTo()
    {
      return this.Property("escalate_to");
    }
    /// <summary>Retrieve the <c>expected_duration</c> property of the item</summary>
    [ArasName("expected_duration")]
    public IProperty_Number ExpectedDuration()
    {
      return this.Property("expected_duration");
    }
    /// <summary>Retrieve the <c>icon</c> property of the item</summary>
    [ArasName("icon")]
    public IProperty_Text Icon()
    {
      return this.Property("icon");
    }
    /// <summary>Retrieve the <c>is_auto</c> property of the item</summary>
    [ArasName("is_auto")]
    public IProperty_Boolean IsAuto()
    {
      return this.Property("is_auto");
    }
    /// <summary>Retrieve the <c>is_end</c> property of the item</summary>
    [ArasName("is_end")]
    public IProperty_Boolean IsEnd()
    {
      return this.Property("is_end");
    }
    /// <summary>Retrieve the <c>is_escalated</c> property of the item</summary>
    [ArasName("is_escalated")]
    public IProperty_Boolean IsEscalated()
    {
      return this.Property("is_escalated");
    }
    /// <summary>Retrieve the <c>is_start</c> property of the item</summary>
    [ArasName("is_start")]
    public IProperty_Boolean IsStart()
    {
      return this.Property("is_start");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    [ArasName("label")]
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>message</c> property of the item</summary>
    [ArasName("message")]
    public IProperty_Text Message()
    {
      return this.Property("message");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    [ArasName("name")]
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>priority</c> property of the item</summary>
    [ArasName("priority")]
    public IProperty_Number Priority()
    {
      return this.Property("priority");
    }
    /// <summary>Retrieve the <c>reminder_count</c> property of the item</summary>
    [ArasName("reminder_count")]
    public IProperty_Number ReminderCount()
    {
      return this.Property("reminder_count");
    }
    /// <summary>Retrieve the <c>reminder_interval</c> property of the item</summary>
    [ArasName("reminder_interval")]
    public IProperty_Number ReminderInterval()
    {
      return this.Property("reminder_interval");
    }
    /// <summary>Retrieve the <c>role</c> property of the item</summary>
    [ArasName("role")]
    public IProperty_Item<Identity> Role()
    {
      return this.Property("role");
    }
    /// <summary>Retrieve the <c>subflow</c> property of the item</summary>
    [ArasName("subflow")]
    public IProperty_Item<WorkflowMap> Subflow()
    {
      return this.Property("subflow");
    }
    /// <summary>Retrieve the <c>timeout_duration</c> property of the item</summary>
    [ArasName("timeout_duration")]
    public IProperty_Number TimeoutDuration()
    {
      return this.Property("timeout_duration");
    }
    /// <summary>Retrieve the <c>wait_for_all_inputs</c> property of the item</summary>
    [ArasName("wait_for_all_inputs")]
    public IProperty_Boolean WaitForAllInputs()
    {
      return this.Property("wait_for_all_inputs");
    }
    /// <summary>Retrieve the <c>wait_for_all_votes</c> property of the item</summary>
    [ArasName("wait_for_all_votes")]
    public IProperty_Boolean WaitForAllVotes()
    {
      return this.Property("wait_for_all_votes");
    }
    /// <summary>Retrieve the <c>x</c> property of the item</summary>
    [ArasName("x")]
    public IProperty_Number X()
    {
      return this.Property("x");
    }
    /// <summary>Retrieve the <c>y</c> property of the item</summary>
    [ArasName("y")]
    public IProperty_Number Y()
    {
      return this.Property("y");
    }
  }
}