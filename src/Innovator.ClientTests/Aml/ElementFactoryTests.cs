﻿using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Innovator.Client.Tests
{
  [TestClass()]
  public class ElementFactoryTests
  {
    [TestMethod()]
    public void AmlTest()
    {
      var aml = ElementFactory.Local;
      var str = aml.Aml(aml.Item(aml.Type("stuff"))).ToAml();
      Assert.AreEqual("<AML><Item type=\"stuff\" /></AML>", str);

      str = aml.Item(aml.Type("Type"), aml.Id(default(string)), aml.KeyedName(null)).ToAml();
      Assert.AreEqual("<Item type=\"Type\" id=\"\"><keyed_name is_null=\"1\" /></Item>", str);
    }

    [TestMethod()]
    public void ElementAdd()
    {
      var aml = ElementFactory.Local;
      var query = aml.Aml(aml.Item(aml.Type("stuff")));
      query.Add(aml.Item(aml.Type("important")));
      Assert.AreEqual("<AML><Item type=\"stuff\" /><Item type=\"important\" /></AML>", query.ToAml());
    }

    [TestMethod()]
    public void CalcMd5()
    {
      var aml = ElementFactory.Local;
      var token = (SecureToken)"12#$%testing";
      Assert.AreEqual("ee39c527616bef9142992bf65135ca00", aml.CalcMd5("12#$%testing"));
      Assert.AreEqual("ee39c527616bef9142992bf65135ca00", aml.CalcMd5(token));
      token = (SecureToken)"foo-Ã¤";
      Assert.AreEqual("d57d3ca7f60c8e5e1dca90c08ee64f6c", aml.CalcMd5("foo-Ã¤"));
      Assert.AreEqual("d57d3ca7f60c8e5e1dca90c08ee64f6c", aml.CalcMd5(token));
    }

    [TestMethod()]
    public void SubSelect_EnsurePath()
    {
      var subSelect = new SubSelect();
      subSelect.EnsurePath("first");
      subSelect.EnsurePath("second", "thing");
      subSelect.EnsurePath("second", "another2", "id");
      subSelect.EnsurePath("second", "another2", "config_id");
      subSelect.EnsurePath("no_paren");
      subSelect.EnsurePath("third");
      subSelect.EnsurePath("third", "stuff");
      subSelect.EnsurePath("another", "id");
      var actual = subSelect.ToString();
      Assert.AreEqual("first,second(thing,another2(id,config_id)),no_paren,third(stuff),another(id)", actual);
    }

    [TestMethod()]
    public void RelationshipsTest()
    {
      var aml = ElementFactory.Local;
      var query = aml.Item(aml.Type("Method"), aml.Action("GetMultiple"),
        aml.Relationships(GetItems()));
      Assert.AreEqual("<Item type=\"Method\" action=\"GetMultiple\"><Relationships><Item type=\"thing\" id=\"1234\" /><Item type=\"thing\" id=\"5678\" /><Item type=\"thing\" id=\"9012\" /></Relationships></Item>", query.ToAml());
    }

    [TestMethod()]
    public void ExtraRelationshipsTags()
    {
      var item = ElementFactory.Local.FromXml(@"<Item type='Tree Node'>
  <itemtype_id />
  <classification>Tree Node/TocCategory</classification>
  <name>Administration</name>
  <label>Administration</label>
  <Relationships>
    <Item type='Tree Node Child'>
      <related_id>
        <Item type='Tree Node'>
          <itemtype_id>B19D349CC6FC44BC97D50A6D70AE79CB</itemtype_id>
          <open_icon>../images/WorkflowMap.svg</open_icon>
          <close_icon>../images/WorkflowMap.svg</close_icon>
          <name>Administration/Workflow Map</name>
          <label>Workflow Maps</label>
          <classification>Tree Node/ItemTypeInToc</classification>
        </Item>
      </related_id>
    </Item>
    <Item type='Tree Node Child'>
      <related_id>
        <Item type='Tree Node'>
          <itemtype_id>261EAC08AE9144FC95C49182ACE0D3FE</itemtype_id>
          <open_icon>../images/WorkflowProcess.svg</open_icon>
          <close_icon>../images/WorkflowProcess.svg</close_icon>
          <name>Administration/Workflow Process</name>
          <label>Workflow Processes</label>
          <classification>Tree Node/ItemTypeInToc</classification>
        </Item>
      </related_id>
    </Item>
  </Relationships>
  <Relationships />
  <Relationships />
  <Relationships />
  <Relationships />
  <Relationships />
  <Relationships />
  <Relationships />
  <Relationships />
</Item>").AssertItem();
      Assert.AreEqual(2, item.Relationships().Count());
    }

    private IEnumerable<IReadOnlyItem> GetItems()
    {
      var aml = ElementFactory.Local;
      yield return aml.Item(aml.Type("thing"), aml.Id("1234"));
      yield return aml.Item(aml.Type("thing"), aml.Id("5678"));
      yield return aml.Item(aml.Type("thing"), aml.Id("9012"));
    }

    [TestMethod()]
    public void FormatAmlTest_FullInClause()
    {
      var aml = ElementFactory.Local;
      Assert.AreEqual(@"<Item type=""Part"" action=""get"" select=""id,config_id,keyed_name,state""><config_id condition=""in"">'1','2'</config_id></Item>", aml.FormatAml(@"<Item type='@0' action='get' select='id,config_id,keyed_name,state'>
  <config_id condition='in'>@1</config_id>
</Item>", "Part", "'1','2'"));
    }

    [TestMethod()]
    public void FormatAmlTest()
    {
      Assert.AreEqual("<Item><name>first &amp; second &gt; third</name><is_current>1</is_current><date>2015-01-01T00:00:00</date></Item>",
        new Command("<Item><name>@0</name><is_current>@1</is_current><date>@2</date></Item>",
          "first & second > third", true, new DateTime(2015, 1, 1)).ToNormalizedAml(ElementFactory.Local.LocalizationContext));
    }

    /*[TestMethod()]
    public void FormatAmlTest_NamedParam()
    {
      Assert.AreEqual("<Item><name>first &amp; second &gt; third</name><is_current>1</is_current><date>2015-01-01T00:00:00</date></Item>",
        new Command("<Item><name>@string</name><is_current>@bool</is_current><date>@dateTime</date></Item>",
          new Connection.DbParams() {
            {"string", "first & second > third"},
            {"bool", true},
            {"dateTime", new DateTime(2015, 1, 1)}
          }).ToNormalizedAml(ElementFactory.Local.LocalizationContext));
    }*/

    [TestMethod()]
    public void FormatAmlTest_EmptyXmlElement()
    {
      Assert.AreEqual(@"<Item type=""PCO Task"" action=""add""><source_id>1234</source_id><related_id><Item type=""Task"" action=""add""><date_due_target>2020-01-14T16:38:12</date_due_target><indent>0</indent><is_complete is_null=""1"" /><name>Update the documentation</name><date_start_target>2020-01-13T16:38:12</date_start_target><owned_by_id>F13AF7BC3D7A4084AF67AB7BF938C409</owned_by_id></Item></related_id></Item>",
        new Command(@"<Item type='PCO Task' action='add'><source_id>@0</source_id><related_id><Item type='Task' action='add'><date_due_target>2020-01-14T16:38:12</date_due_target><indent>0</indent><is_complete is_null='1' /><name>Update the documentation</name><date_start_target>2020-01-13T16:38:12</date_start_target><owned_by_id>F13AF7BC3D7A4084AF67AB7BF938C409</owned_by_id></Item></related_id></Item>"
          , "1234").ToNormalizedAml(ElementFactory.Local.LocalizationContext));
    }

    [TestMethod()]
    public void FormatAmlTest_EnumTest()
    {
      Assert.AreEqual("<Item><name condition=\"in\">N'1',N'2',N'3'</name><is_current>1</is_current><date>2015-01-01T00:00:00</date></Item>",
        new Command("<Item><name condition='in'>@0</name><is_current>@1</is_current><date>@2</date></Item>",
          new string[] { "1", "2", "3" }, true, new DateTime(2015, 1, 1)).ToNormalizedAml(ElementFactory.Local.LocalizationContext));
    }

    [TestMethod()]
    public void FormatAmlTest_IdListTest()
    {
      Assert.AreEqual("<Item idlist=\"1,2,3\"><is_current>1</is_current><date>2015-01-01T00:00:00</date></Item>",
        new Command("<Item idlist='@0'><is_current>@1</is_current><date>@2</date></Item>",
          new string[] { "1", "2", "3" }, true, new DateTime(2015, 1, 1)).ToNormalizedAml(ElementFactory.Local.LocalizationContext));
    }

    [TestMethod()]
    public void FormatAmlTest_SqlParam()
    {
      Assert.AreEqual("<Item><is_current condition=\"in\">(select '@0' \"@0\" from thing where stuff = N'test ''&gt;'' thing')</is_current></Item>",
        new Command("<Item><is_current condition='in'>(select '@0' \"@0\" from thing where stuff = @0)</is_current></Item>",
          "test '>' thing").ToNormalizedAml(ElementFactory.Local.LocalizationContext));
    }

    [TestMethod()]
    public void FormatAmlTest_SqlOnly()
    {
      Assert.AreEqual("select '@0' \"@0\" from thing where stuff = N'test ''>'' thing'",
        new Command("select '@0' \"@0\" from thing where stuff = @0",
          "test '>' thing").ToNormalizedAml(ElementFactory.Local.LocalizationContext));
    }

    [TestMethod()]
    public void FormatAmlTest_SqlEnumString()
    {
      Assert.AreEqual("select '@0' \"@0\" from thing where stuff = N'1,2,3'",
        new Command("select '@0' \"@0\" from thing where stuff = @0",
          new List<string>() { "1", "2", "3" }).ToNormalizedAml(ElementFactory.Local.LocalizationContext));
    }

    [TestMethod()]
    public void FormatAmlTest_SqlEnumInClause()
    {
      Assert.AreEqual("select '@0' \"@0\" from thing where stuff in (N'1',N'2',N'3')",
        new Command("select '@0' \"@0\" from thing where stuff in @0",
          new List<string>() { "1", "2", "3" }).ToNormalizedAml(ElementFactory.Local.LocalizationContext));
    }

    [TestMethod()]
    public void FormatAmlTest_ApplySql()
    {
      Assert.AreEqual("<sql>select '@0' \"@0\" from thing where stuff = N'test ''&gt;'' thing'</sql>",
        new Command("<sql>select '@0' \"@0\" from thing where stuff = @0</sql>",
          "test '>' thing").ToNormalizedAml(ElementFactory.Local.LocalizationContext));
    }

    [TestMethod()]
    public void FormatAmlTest_WhereClause()
    {
      Assert.AreEqual(@"<Item type=""Method"" action=""Get Address Book"" where=""[user_id] = N'2D246C5838644C1C8FD34F8D2796E327'"" />",
        new Command(@"<Item type='Method' action='Get Address Book' where=""[user_id] = @0""></Item>",
        "2D246C5838644C1C8FD34F8D2796E327").ToNormalizedAml(ElementFactory.Local.LocalizationContext));
    }

    [TestMethod()]
    public void FormatAmlTest_RawSupport()
    {
      Assert.AreEqual("<Item><name>&lt;Item /&gt;</name><is_current><Item /></is_current></Item>",
        new Command("<Item><name>@0</name><is_current>@0!</is_current></Item>",
          "<Item />", true, new DateTime(2015, 1, 1)).ToNormalizedAml(ElementFactory.Local.LocalizationContext));
    }

    [TestMethod()]
    public void FormatAmlTest_DynamicDate()
    {
      DynamicDateTimeRange._clock = () => DateTimeOffset.FromFileTime(131109073341417792);
      Assert.AreEqual("<Item action=\"get\" type=\"Part\"><created_on origDateRange=\"Dynamic|Month|-1|Month|-1\" condition=\"between\">2016-05-01T00:00:00 and 2016-05-31T23:59:59</created_on></Item>",
        new Command("<Item action='get' type='Part'><created_on origDateRange='Dynamic|Month|-1|Month|-1'>random query</created_on></Item>")
          .ToNormalizedAml(ElementFactory.Local.LocalizationContext));
      Assert.AreEqual("<Item action=\"get\" type=\"Part\"><created_on origDateRange=\"Month|-1|Month|-1|Sunday\" condition=\"between\">2016-05-01T00:00:00 and 2016-05-31T23:59:59</created_on></Item>",
        new Command("<Item action='get' type='Part'><created_on origDateRange='Month|-1|Month|-1|Sunday'>random query</created_on></Item>")
          .ToNormalizedAml(ElementFactory.Local.LocalizationContext));
      Assert.AreEqual("<Item action=\"get\" type=\"Part\"><created_on condition=\"between\" origDateRange=\"Month|-1|Month|-1|Sunday\">2016-05-01T00:00:00 and 2016-05-31T23:59:59</created_on></Item>",
        new Command("<Item action='get' type='Part'><created_on>@0</created_on></Item>", new DynamicDateTimeRange()
        {
          EndMagnitude = DateMagnitude.Month,
          EndOffset = -1,
          StartMagnitude = DateMagnitude.Month,
          StartOffset = -1
        }).ToNormalizedAml(ElementFactory.Local.LocalizationContext));
    }

    [TestMethod()]
    public void FormatAmlTest_DynamicDate2()
    {
      DynamicDateTimeRange._clock = () => DateTimeOffset.FromFileTime(131232574142744075);

      Assert.AreEqual("<Item action=\"get\" type=\"Part\"><created_on condition=\"le\" origDateRange=\"Dynamic|Year|-1000|Week|2\">2016-11-26T23:59:59</created_on></Item>",
        new Command("<Item action='get' type='Part'><created_on condition='between' origDateRange='Dynamic|Year|-1000|Week|2'>random query</created_on></Item>")
          .ToNormalizedAml(ElementFactory.Local.LocalizationContext));
      Assert.AreEqual("<Item action=\"get\" type=\"Part\"><created_on condition=\"le\" origDateRange=\"Dynamic|Year|-1000|Week|2\">2016-11-26T23:59:59</created_on></Item>",
        new Command("<Item action='get' type='Part'><created_on>@0</created_on></Item>", new DynamicDateTimeRange()
        {
          EndMagnitude = DateMagnitude.Week,
          EndOffset = 2,
        }).ToNormalizedAml(ElementFactory.Local.LocalizationContext));
    }

    [TestMethod()]
    public void FormatAmlTest_DynamicDate3()
    {
      DynamicDateTimeRange._clock = () => DateTimeOffset.FromFileTime(131232574142744075);
      Assert.AreEqual("<Item action=\"get\" type=\"Part\"><created_on condition=\"ge\" origDateRange=\"Dynamic|Week|-2|Year|1000\">2016-10-23T00:00:00</created_on></Item>",
        new Command("<Item action='get' type='Part'><created_on condition='between' origDateRange='Dynamic|Week|-2|Year|1000'>random query</created_on></Item>")
          .ToNormalizedAml(ElementFactory.Local.LocalizationContext));
      Assert.AreEqual("<Item action=\"get\" type=\"Part\"><created_on condition=\"ge\" origDateRange=\"Dynamic|Week|-2|Year|1000\">2016-10-23T00:00:00</created_on></Item>",
        new Command("<Item action='get' type='Part'><created_on>@0</created_on></Item>", new DynamicDateTimeRange()
        {
          StartMagnitude = DateMagnitude.Week,
          StartOffset = -2,
        }).ToNormalizedAml(ElementFactory.Local.LocalizationContext));
    }

    [TestMethod()]
    public void CreateAmlTest_DateRangeProp()
    {
      DynamicDateTimeRange._clock = () => DateTimeOffset.FromFileTime(131109073341417792);

      var aml = ElementFactory.Local;
      var item = aml.Item(aml.Action("get"), aml.Type("Part"), aml.CreatedOn(new DynamicDateTimeRange() {
        StartMagnitude = DateMagnitude.Month,
        StartOffset = -1,
        EndMagnitude = DateMagnitude.Month,
        EndOffset = -1
      }));
      Assert.AreEqual("<Item action=\"get\" type=\"Part\"><created_on condition=\"between\" origDateRange=\"Month|-1|Month|-1|Sunday\">2016-05-01T00:00:00 and 2016-05-31T23:59:59</created_on></Item>", item.ToAml());

      item = aml.Item(aml.Action("get"), aml.Type("Part"), aml.CreatedOn(new StaticDateTimeRange()
      {
        StartDate = DateTime.Parse("2017-01-01"),
        EndDate = DateTime.Parse("2017-01-31")
      }));
      Assert.AreEqual("<Item action=\"get\" type=\"Part\"><created_on condition=\"between\">2017-01-01T00:00:00 and 2017-01-31T00:00:00</created_on></Item>", item.ToAml());

      item = aml.Item(aml.Action("get"), aml.Type("Part"), aml.CreatedOn(new StaticDateTimeRange()
      {
        StartDate = DateTime.Parse("2017-01-01")
      }));
      Assert.AreEqual("<Item action=\"get\" type=\"Part\"><created_on condition=\"ge\">2017-01-01T00:00:00</created_on></Item>", item.ToAml());

      item = aml.Item(aml.Action("get"), aml.Type("Part"), aml.CreatedOn(new StaticDateTimeRange()
      {
        EndDate = DateTime.Parse("2017-01-31")
      }));
      Assert.AreEqual("<Item action=\"get\" type=\"Part\"><created_on condition=\"le\">2017-01-31T00:00:00</created_on></Item>", item.ToAml());
    }

    [TestMethod()]
    public void FormatEarlyDate()
    {
      var str = "0018-03-07T00:00:00";
      var date = DateTime.Parse("0018-03-07T00:00:00");
      Assert.AreEqual(str, ElementFactory.Local.LocalizationContext.Format(date));
    }
  }
}
