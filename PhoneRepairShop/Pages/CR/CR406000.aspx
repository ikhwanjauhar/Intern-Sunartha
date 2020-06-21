<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="CR406000.aspx.cs" Inherits="Page_CR406000"
	Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.CR.CampaignEnq"
		PrimaryView="Filter" PageLoadBehavior="PopulateSavedValues">
		<CallbackCommands>
			<px:PXDSCallbackCommand Visible="false" DependOnGrid="grid" Name="FilteredItems_ViewDetails" />
			<pxa:PXExtendedDSCallbackCommand Name="AddNewLead" ForDashboard="true" />
		</CallbackCommands>
		<DataTrees>
			<px:PXTreeDataMember TreeView="_EPCompanyTree_Tree_" TreeKeys="WorkgroupID" />
		</DataTrees>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Width="100%" DataMember="Filter" Caption="Selection">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartColumn="True"/>
			<px:PXLayoutRule ID="PXLayoutRule3" runat="server" Merge="True" ControlSize="M"/>
			<px:PXSelector ID="edOwnerID" runat="server" DataField="OwnerID" AutoRefresh="True" CommitChanges="True"/>
			<px:PXCheckBox  ID="chkMyOwner" runat="server" Checked="True" DataField="MyOwner" CommitChanges="True"/>
			<px:PXLayoutRule ID="PXLayoutRule4" runat="server" Merge="False" />
			<px:PXLayoutRule ID="PXLayoutRule5" runat="server" Merge="True" ControlSize="M"/>
			<px:PXSelector ID="edWorkGroupID" runat="server" DataField="WorkGroupID" CommitChanges="True" AutoRefresh="True"/>
			<px:PXCheckBox ID="chkMyWorkGroup" runat="server" DataField="MyWorkGroup" CommitChanges="True"/>
			<px:PXLayoutRule ID="PXLayoutRule7"  runat="server" Merge="False" />
			<px:PXLayoutRule ID="PXLayoutRule2" runat="server" StartColumn="True" LabelsWidth="S"
				ControlSize="XM" SuppressLabel="False" />
			<px:PXSelector CommitChanges="True" ID="edCampaignID" runat="server" DataField="CampaignID"
				AllowEdit="true" DisplayMode="Hint" />			
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Height="150px" Style="z-index: 100"
		Width="100%" ActionsPosition="Top" Caption="Campaign Members" AllowPaging="true"
		AdjustPageSize="auto" SkinID="Inquire" FastFilterFields="DisplayName,FullName" RestrictFields="True">
		<Levels>
			<px:PXGridLevel DataMember="FilteredItems">
				<Columns>
					<px:PXGridColumn DataField="CRCampaign__CampaignID" />
					<px:PXGridColumn DataField="CRCampaign__CampaignName" />
					<px:PXGridColumn DataField="CRCampaign__Status" />
					<px:PXGridColumn DataField="ContactType" RenderEditorText="True" />
					<px:PXGridColumn DataField="DisplayName" LinkCommand="FilteredItems_ViewDetails" />
					<px:PXGridColumn DataField="Title" />
					<px:PXGridColumn DataField="FirstName" />
					<px:PXGridColumn DataField="MidName" Visible="false" SyncVisible="false"/>
					<px:PXGridColumn DataField="LastName" />
					<px:PXGridColumn DataField="Salutation" />
					<px:PXGridColumn DataField="BAccount__AcctCD" LinkCommand="FilteredItems_BAccount_ViewDetails" />
					<px:PXGridColumn DataField="FullName" />
					<px:PXGridColumn DataField="IsActive" Type="CheckBox"/>
					<px:PXGridColumn DataField="Status" />
					<px:PXGridColumn DataField="Resolution" />
					<px:PXGridColumn DataField="ClassID" />
					<px:PXGridColumn DataField="Source" />
					<px:PXGridColumn DataField="Address__PostalCode" />
					<px:PXGridColumn DataField="Address__CountryID" />
					<px:PXGridColumn DataField="State__name" />
					<px:PXGridColumn DataField="Address__City" />
					<px:PXGridColumn DataField="Address__AddressLine1" />
					<px:PXGridColumn DataField="Address__AddressLine2" />
					<px:PXGridColumn DataField="EMail" />
					<px:PXGridColumn DataField="Phone1" DisplayFormat="+#(###) ###-####" />
					<px:PXGridColumn DataField="Phone2" DisplayFormat="+#(###) ###-####" />
					<px:PXGridColumn DataField="Phone3" DisplayFormat="+#(###) ###-####" />
					<px:PXGridColumn DataField="Fax" DisplayFormat="+#(###) ###-####" />
					<px:PXGridColumn DataField="WorkgroupID" />
					<px:PXGridColumn DataField="OwnerID" DisplayMode="Text"/>
					<px:PXGridColumn DataField="CreatedByID_Creator_Username" SyncVisible="False" SyncVisibility="False" Visible="False" />
					<px:PXGridColumn DataField="CreatedDateTime" />
					<px:PXGridColumn DataField="LastModifiedByID_Modifier_Username" SyncVisible="False" SyncVisibility="False" Visible="False" />
					<px:PXGridColumn DataField="LastModifiedDateTime" />
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<ActionBar DefaultAction="cmdItemDetails" PagerVisible="False">
			<PagerSettings Mode="NextPrevFirstLast" />
			<CustomItems>
				<px:PXToolBarButton Text="View Details" Tooltip="View Details" Key="cmdItemDetails"
					Visible="false">
					<Images Normal="main@DataEntry" />
					<AutoCallBack Command="FilteredItems_ViewDetails" Target="ds">
						<Behavior CommitChanges="True" />
					</AutoCallBack>	 
					<ActionBar GroupIndex="0" />
				</px:PXToolBarButton> 
			</CustomItems>
		</ActionBar>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
		<CallbackCommands>
			<Refresh CommitChanges="True" PostData="Page" />
		</CallbackCommands>
	</px:PXGrid>
</asp:Content>
