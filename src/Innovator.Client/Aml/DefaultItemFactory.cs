﻿using Innovator.Client.Model;
using M = Innovator.Client.Model;

namespace Innovator.Client
{
  /// <summary>
  /// Creates an instance of a strongly-typed class (which inherits from <see cref="Item"/>) that 
  /// represents a specific item type
  /// </summary>
  /// <seealso cref="Innovator.Client.IItemFactory" />
  /// <remarks>
  /// For any core Aras item type, a model from the <see cref="Innovator.Client.Model"/> namespace
  /// will be returned.  In all other cases, a <c>null</c> will be returned and an 
  /// <see cref="Item"/> will be created
  /// </remarks>
  public class DefaultItemFactory : IItemFactory
  {
    /// <summary>
    /// Creates an instance of a strongly-typed class (which inherits from <see cref="Item" />) that
    /// represents the item type <paramref name="type" />
    /// </summary>
    /// <param name="factory">Factory to be passed to the <see cref="Item" /> constructor</param>
    /// <param name="type">Item type name</param>
    /// <returns>
    /// For any core Aras item type, a model from the <see cref="Innovator.Client.Model"/> namespace
    /// will be returned.  In all other cases, an <see cref="Item"/> will be returned
    /// </returns>
    public Item NewItem(ElementFactory factory, string type)
    {
      switch (type)
      {
        case "Access": return new Access(factory);
        case "Action": return new M.Action(factory);
        case "Activity": return new Activity(factory);
        case "Activity Assignment": return new ActivityAssignment(factory);
        case "Activity EMail": return new ActivityEMail(factory);
        case "Activity Method": return new ActivityMethod(factory);
        case "Activity Task": return new ActivityTask(factory);
        case "Activity Task Value": return new ActivityTaskValue(factory);
        case "Activity Template": return new ActivityTemplate(factory);
        case "Activity Template Assignment": return new ActivityTemplateAssignment(factory);
        case "Activity Template EMail": return new ActivityTemplateEMail(factory);
        case "Activity Template Method": return new ActivityTemplateMethod(factory);
        case "Activity Template Task": return new ActivityTemplateTask(factory);
        case "Activity Template Transition": return new ActivityTemplateTransition(factory);
        case "Activity Template Variable": return new ActivityTemplateVariable(factory);
        case "Activity Transition": return new ActivityTransition(factory);
        case "Activity Variable": return new ActivityVariable(factory);
        case "Activity Variable Value": return new ActivityVariableValue(factory);
        case "Alias": return new Alias(factory);
        case "Allowed Permission": return new AllowedPermission(factory);
        case "Allowed Workflow": return new AllowedWorkflow(factory);
        case "Applied Updates": return new AppliedUpdates(factory);
        case "Board": return new Board(factory);
        case "Body": return new Body(factory);
        case "Business Calendar Exception": return new BusinessCalendarException(factory);
        case "Business Calendar Year": return new BusinessCalendarYear(factory);
        case "Can Add": return new CanAdd(factory);
        case "Chart": return new Chart(factory);
        case "Chart Series": return new ChartSeries(factory);
        case "Client Event": return new ClientEvent(factory);
        case "cmf_AdditionalPropertyType": return new cmf_AdditionalPropertyType(factory);
        case "cmf_BaseView": return new cmf_BaseView(factory);
        case "cmf_ComputedProperty": return new cmf_ComputedProperty(factory);
        case "cmf_ComputedPropertyDependency": return new cmf_ComputedPropertyDependency(factory);
        case "cmf_ContentElementItems": return new cmf_ContentElementItems(factory);
        case "cmf_ContentItems": return new cmf_ContentItems(factory);
        case "cmf_ContentPropertyItems": return new cmf_ContentPropertyItems(factory);
        case "cmf_ContentType": return new cmf_ContentType(factory);
        case "cmf_ContentTypeExportRel": return new cmf_ContentTypeExportRel(factory);
        case "cmf_ContentTypeExportSetting": return new cmf_ContentTypeExportSetting(factory);
        case "cmf_ContentTypeExportToExcel": return new cmf_ContentTypeExportToExcel(factory);
        case "cmf_ContentTypeGridLayout": return new cmf_ContentTypeGridLayout(factory);
        case "cmf_ContentTypeView": return new cmf_ContentTypeView(factory);
        case "cmf_DocumentLifeCycleState": return new cmf_DocumentLifeCycleState(factory);
        case "cmf_ElementAllowedPermission": return new cmf_ElementAllowedPermission(factory);
        case "cmf_ElementBinding": return new cmf_ElementBinding(factory);
        case "cmf_ElementType": return new cmf_ElementType(factory);
        case "cmf_ExportToExcelViewConfig": return new cmf_ExportToExcelViewConfig(factory);
        case "cmf_PropertyAllowedPermission": return new cmf_PropertyAllowedPermission(factory);
        case "cmf_PropertyBinding": return new cmf_PropertyBinding(factory);
        case "cmf_PropertyType": return new cmf_PropertyType(factory);
        case "cmf_Style": return new cmf_Style(factory);
        case "cmf_TabularView": return new cmf_TabularView(factory);
        case "cmf_TabularViewColumn": return new cmf_TabularViewColumn(factory);
        case "cmf_TabularViewColumnGroups": return new cmf_TabularViewColumnGroups(factory);
        case "cmf_TabularViewHeaderRow": return new cmf_TabularViewHeaderRow(factory);
        case "cmf_TabularViewHeaderRows": return new cmf_TabularViewHeaderRows(factory);
        case "cmf_TabularViewTree": return new cmf_TabularViewTree(factory);
        case "Column Event": return new ColumnEvent(factory);
        case "CommandBarButton": return new CommandBarButton(factory);
        case "CommandBarCheckbox": return new CommandBarCheckbox(factory);
        case "CommandBarDropDown": return new CommandBarDropDown(factory);
        case "CommandBarItem": return new CommandBarItem(factory);
        case "CommandBarMenu": return new CommandBarMenu(factory);
        case "CommandBarMenuButton": return new CommandBarMenuButton(factory);
        case "CommandBarMenuCheckbox": return new CommandBarMenuCheckbox(factory);
        case "CommandBarSection": return new CommandBarSection(factory);
        case "CommandBarSectionItem": return new CommandBarSectionItem(factory);
        case "CommandBarSeparator": return new CommandBarSeparator(factory);
        case "Configurable Grid Event": return new ConfigurableGridEvent(factory);
        case "ConversionRule": return new ConversionRule(factory);
        case "ConversionRuleEventHandler": return new ConversionRuleEventHandler(factory);
        case "ConversionRuleEventTemplate": return new ConversionRuleEventTemplate(factory);
        case "ConversionRuleFileType": return new ConversionRuleFileType(factory);
        case "ConversionServer": return new ConversionServer(factory);
        case "ConversionServerConverterType": return new ConversionServerConverterType(factory);
        case "ConversionServerPriority": return new ConversionServerPriority(factory);
        case "ConversionTask": return new ConversionTask(factory);
        case "ConversionTaskDependency": return new ConversionTaskDependency(factory);
        case "ConversionTaskEventHandler": return new ConversionTaskEventHandler(factory);
        case "ConversionTaskHandlerError": return new ConversionTaskHandlerError(factory);
        case "ConversionTaskResult": return new ConversionTaskResult(factory);
        case "ConverterType": return new ConverterType(factory);
        case "Core_GlobalLayout": return new Core_GlobalLayout(factory);
        case "Core_ItemGridLayout": return new Core_ItemGridLayout(factory);
        case "Core_RelGridLayout": return new Core_RelGridLayout(factory);
        case "Dashboard": return new Dashboard(factory);
        case "Dashboard Chart": return new DashboardChart(factory);
        case "DatabaseUpgrade": return new DatabaseUpgrade(factory);
        case "DatabaseUpgradeLogFile": return new DatabaseUpgradeLogFile(factory);
        case "DatabaseUpgradePatch": return new DatabaseUpgradePatch(factory);
        case "Desktop": return new Desktop(factory);
        case "Discussion": return new Discussion(factory);
        case "DiscussionDefinition": return new DiscussionDefinition(factory);
        case "DiscussionDefinitionView": return new DiscussionDefinitionView(factory);
        case "DiscussionTemplate": return new DiscussionTemplate(factory);
        case "DiscussionTemplateView": return new DiscussionTemplateView(factory);
        case "EMail Message": return new EMailMessage(factory);
        case "Exclusion": return new Exclusion(factory);
        case "Feature License": return new FeatureLicense(factory);
        case "Feed": return new Feed(factory);
        case "FeedTemplate": return new FeedTemplate(factory);
        case "Field": return new Field(factory);
        case "Field Event": return new FieldEvent(factory);
        case "File": return new File(factory);
        case "FileContainerItems": return new FileContainerItems(factory);
        case "FileContainerLocator": return new FileContainerLocator(factory);
        case "FileExchangePackage": return new FileExchangePackage(factory);
        case "FileExchangePackageFile": return new FileExchangePackageFile(factory);
        case "FileExchangeTxn": return new FileExchangeTxn(factory);
        case "FileExchangeTxnState": return new FileExchangeTxnState(factory);
        case "FileGroup": return new FileGroup(factory);
        case "FileGroupFile": return new FileGroupFile(factory);
        case "FileSelector": return new FileSelector(factory);
        case "FileSelectorTemplate": return new FileSelectorTemplate(factory);
        case "FileType": return new FileType(factory);
        case "Filter Value": return new FilterValue(factory);
        case "Form": return new Form(factory);
        case "Form Event": return new FormEvent(factory);
        case "Forum": return new Forum(factory);
        case "ForumItem": return new ForumItem(factory);
        case "ForumMessageGroup": return new ForumMessageGroup(factory);
        case "ForumMustViewBy": return new ForumMustViewBy(factory);
        case "ForumSearch": return new ForumSearch(factory);
        case "ForumSharedWith": return new ForumSharedWith(factory);
        case "ForumWantViewBy": return new ForumWantViewBy(factory);
        case "Frame": return new Frame(factory);
        case "Frameset": return new Frameset(factory);
        case "GlobalPresentationConfig": return new GlobalPresentationConfig(factory);
        case "Grid": return new Grid(factory);
        case "Grid Column": return new GridColumn(factory);
        case "Grid Event": return new GridEvent(factory);
        case "Help": return new Help(factory);
        case "Help See Also": return new HelpSeeAlso(factory);
        case "Hide In": return new HideIn(factory);
        case "Hide Related In": return new HideRelatedIn(factory);
        case "History": return new History(factory);
        case "History Action": return new HistoryAction(factory);
        case "History Container": return new HistoryContainer(factory);
        case "History Template": return new HistoryTemplate(factory);
        case "History Template Action": return new HistoryTemplateAction(factory);
        case "History Template Where Used": return new HistoryTemplateWhereUsed(factory);
        case "HistorySecureMessage": return new HistorySecureMessage(factory);
        case "Identity": return new Identity(factory);
        case "Implements": return new Implements(factory);
        case "InBasket Task": return new InBasketTask(factory);
        case "Inherited Server Events": return new InheritedServerEvents(factory);
        case "Item Action": return new ItemAction(factory);
        case "Item Report": return new ItemReport(factory);
        case "ItemType": return new ItemType(factory);
        case "ItemType Life Cycle": return new ItemTypeLifeCycle(factory);
        case "ITPresentationConfiguration": return new ITPresentationConfiguration(factory);
        case "Language": return new Language(factory);
        case "Life Cycle Map": return new LifeCycleMap(factory);
        case "Life Cycle State": return new LifeCycleState(factory);
        case "Life Cycle Transition": return new LifeCycleTransition(factory);
        case "List": return new List(factory);
        case "Locale": return new Locale(factory);
        case "Located": return new Located(factory);
        case "LockedItems": return new LockedItems(factory);
        case "Member": return new Member(factory);
        case "Message": return new Message(factory);
        case "Message Acknowledgement": return new MessageAcknowledgement(factory);
        case "Method": return new Method(factory);
        case "Metric": return new Metric(factory);
        case "Metric Value": return new MetricValue(factory);
        case "Morphae": return new Morphae(factory);
        case "MyReports": return new MyReports(factory);
        case "Old Password": return new OldPassword(factory);
        case "PackageDefinition": return new PackageDefinition(factory);
        case "PackageDependsOn": return new PackageDependsOn(factory);
        case "PackageElement": return new PackageElement(factory);
        case "PackageGroup": return new PackageGroup(factory);
        case "PackageReferencedElement": return new PackageReferencedElement(factory);
        case "Permission": return new Permission(factory);
        case "Preference": return new Preference(factory);
        case "PreferenceTabTypes": return new PreferenceTabTypes(factory);
        case "PreferenceTypes": return new PreferenceTypes(factory);
        case "PresentationCommandBarSection": return new PresentationCommandBarSection(factory);
        case "PresentationConfiguration": return new PresentationConfiguration(factory);
        case "Property": return new M.Property(factory);
        case "ReadPriority": return new ReadPriority(factory);
        case "Relationship Grid Event": return new RelationshipGridEvent(factory);
        case "Relationship View": return new RelationshipView(factory);
        case "RelationshipType": return new RelationshipType(factory);
        case "ReplicationRule": return new ReplicationRule(factory);
        case "ReplicationRuleFileType": return new ReplicationRuleFileType(factory);
        case "ReplicationRuleTargetVault": return new ReplicationRuleTargetVault(factory);
        case "ReplicationTxn": return new ReplicationTxn(factory);
        case "ReplicationTxnLog": return new ReplicationTxnLog(factory);
        case "Report": return new Report(factory);
        case "Revision": return new Revision(factory);
        case "RunReportByUser": return new RunReportByUser(factory);
        case "SavedSearch": return new SavedSearch(factory);
        case "Scheduled Task": return new ScheduledTask(factory);
        case "Search": return new Search(factory);
        case "Search Center": return new SearchCenter(factory);
        case "Search Criteria": return new SearchCriteria(factory);
        case "SearchMode": return new SearchMode(factory);
        case "SecureMessage": return new SecureMessage(factory);
        case "SecureMessageAudio": return new SecureMessageAudio(factory);
        case "SecureMessageFlaggedBy": return new SecureMessageFlaggedBy(factory);
        case "SecureMessageMarkup": return new SecureMessageMarkup(factory);
        case "SecureMessageVideo": return new SecureMessageVideo(factory);
        case "SecureMessageViewTemplate": return new SecureMessageViewTemplate(factory);
        case "SelfServiceReport": return new SelfServiceReport(factory);
        case "SelfServiceReportFeatured": return new SelfServiceReportFeatured(factory);
        case "SelfServiceReportHelp": return new SelfServiceReportHelp(factory);
        case "SelfServiceReportSettings": return new SelfServiceReportSettings(factory);
        case "SelfServiceReportSharedWith": return new SelfServiceReportSharedWith(factory);
        case "Sequence": return new Sequence(factory);
        case "Server Event": return new ServerEvent(factory);
        case "ServiceProvider": return new ServiceProvider(factory);
        case "SPDocumentLibraryDefinition": return new SPDocumentLibraryDefinition(factory);
        case "SPField": return new SPField(factory);
        case "SQL": return new SQL(factory);
        case "SQL Dependencies": return new SQLDependencies(factory);
        case "SSVCItems": return new SSVCItems(factory);
        case "SSVCPresentationConfiguration": return new SSVCPresentationConfiguration(factory);
        case "SSVC_Preferences": return new SSVC_Preferences(factory);
        case "State Distribution": return new StateDistribution(factory);
        case "State EMail": return new StateEMail(factory);
        case "State Notification": return new StateNotification(factory);
        case "StoredSecureMessage": return new StoredSecureMessage(factory);
        case "SystemEvent": return new SystemEvent(factory);
        case "SystemEventHandler": return new SystemEventHandler(factory);
        case "SystemEventLog": return new SystemEventLog(factory);
        case "SystemEventLogDescriptor": return new SystemEventLogDescriptor(factory);
        case "SystemFileContainer": return new SystemFileContainer(factory);
        case "Team": return new Team(factory);
        case "Team Identity": return new TeamIdentity(factory);
        case "Time to Manufacturing": return new TimetoManufacturing(factory);
        case "TOC Access": return new TOCAccess(factory);
        case "TOC View": return new TOCView(factory);
        case "tp_Block": return new tp_Block(factory);
        case "tp_BlockReference": return new tp_BlockReference(factory);
        case "tp_Image": return new tp_Image(factory);
        case "tp_ImageReference": return new tp_ImageReference(factory);
        case "tp_Item": return new tp_Item(factory);
        case "tp_ItemReference": return new tp_ItemReference(factory);
        case "tp_LinkReference": return new tp_LinkReference(factory);
        case "tp_Stylesheet": return new tp_Stylesheet(factory);
        case "tp_XmlSchema": return new tp_XmlSchema(factory);
        case "tp_XmlSchemaElement": return new tp_XmlSchemaElement(factory);
        case "tp_XmlSchemaOutputSetting": return new tp_XmlSchemaOutputSetting(factory);
        case "Transition Distribution": return new TransitionDistribution(factory);
        case "Transition EMail": return new TransitionEMail(factory);
        case "User": return new User(factory);
        case "UserMessage": return new UserMessage(factory);
        case "Value": return new Value(factory);
        case "Variable": return new Variable(factory);
        case "Vault": return new M.Vault(factory);
        case "View": return new View(factory);
        case "View With": return new ViewWith(factory);
        case "Viewer": return new Viewer(factory);
        case "Workflow": return new Workflow(factory);
        case "Workflow Map": return new WorkflowMap(factory);
        case "Workflow Map Activity": return new WorkflowMapActivity(factory);
        case "Workflow Map Path": return new WorkflowMapPath(factory);
        case "Workflow Map Path Post": return new WorkflowMapPathPost(factory);
        case "Workflow Map Path Pre": return new WorkflowMapPathPre(factory);
        case "Workflow Map Variable": return new WorkflowMapVariable(factory);
        case "Workflow Process": return new WorkflowProcess(factory);
        case "Workflow Process Activity": return new WorkflowProcessActivity(factory);
        case "Workflow Process Path": return new WorkflowProcessPath(factory);
        case "Workflow Process Path Post": return new WorkflowProcessPathPost(factory);
        case "Workflow Process Path Pre": return new WorkflowProcessPathPre(factory);
        case "Workflow Process Variable": return new WorkflowProcessVariable(factory);
        case "Workflow Task": return new WorkflowTask(factory);
        case "WSConfiguration": return new WSConfiguration(factory);
        case "WSType": return new WSType(factory);
        case "WSTypeAction": return new WSTypeAction(factory);
        case "WSTypeAssociate": return new WSTypeAssociate(factory);
        case "WSTypeProperty": return new WSTypeProperty(factory);
      }
      return null;
    }
  }
}
