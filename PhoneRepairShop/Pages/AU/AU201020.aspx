<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="AU201020.aspx.cs" Inherits="Page_AU201020"
	Title="Workflows" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXFormView runat="server" SkinID="transparent" ID="formTitle" 
                   DataSourceID="ds" DataMember="ViewPageTitle" Width="100%">
        <Template>
            <px:PXTextEdit runat="server" ID="PageTitle" DataField="PageTitle" SelectOnFocus="False"
                           SkinID="Label" SuppressLabel="true"
                           Width="90%"
                           style="padding: 10px">
                <font size="14pt" names="Arial,sans-serif;"/>
            </px:PXTextEdit>
        </Template>
    </px:PXFormView>
	<pxa:AUDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.SM.AUWorkflowDefinitionMaint"
		PrimaryView="Definition" PageLoadBehavior="SearchSavedKeys">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Cancel" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" /> 
		</CallbackCommands>
	</pxa:AUDataSource>
</asp:Content>

<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Width="100%" 
                   DataMember="Definition">
        <Template>
            <px:PXLayoutRule runat="server" StartRow="True" LabelsWidth="SM" ControlSize="M" />
            <px:PXDropDown ID="edStateField" runat="server" DataField="StateField" AllowNull="True" CommitChanges="True" />
            <px:PXDropDown ID="edFlowTypeField" runat="server" DataField="FlowTypeField" AllowNull="True" CommitChanges="True" />
            <px:PXCheckBox ID="chkEnableWorkflowIDField" runat="server" DataField="EnableWorkflowIDField" />
        </Template>
    </px:PXFormView>
</asp:Content>

<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid ID="gridWorkflows" runat="server" DataSourceID="ds" Height="150px"
               Width="100%" AdjustPageSize="Auto" SkinID="Details" SyncPosition="True" 
               MatrixMode="true" AllowPaging="false" AutoAdjustColumns="true" >					
        <ActionBar>
            <Actions>
                <AdjustColumns ToolBarVisible = "false" />
                <ExportExcel ToolBarVisible ="false" />
            </Actions>
        </ActionBar> 
        <Levels>
            <px:PXGridLevel DataMember="Workflows" >
                <Mode InitNewRow="True" AutoInsert="True"  />
                <Columns>
                    <%--<px:PXGridColumn DataField="IsOverride" Type="CheckBox" Width="60px" TextAlign="Center" />--%>
                    <px:PXGridColumn DataField="IsActive" Type="CheckBox" Width="60px" TextAlign="Center" CommitChanges="True" />
                    <px:PXGridColumn DataField="WorkflowID" Width="50px" CommitChanges="True" AllowNull="True" NullText="(Empty)" />
                    <px:PXGridColumn DataField="Description" Width="200px" LinkCommand="navigateWorkflow" />
                    <%--<px:PXGridColumn DataField="IsDefault" Type="CheckBox" Width="60px" TextAlign="Center" CommitChanges="True" />--%>
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Enabled="True" MinHeight="150" Container="Window" />      
        <CallbackCommands>
            <InitRow CommitChanges="True"/>
            <Refresh CommitChanges="True"></Refresh>
        </CallbackCommands>
    </px:PXGrid>
</asp:Content>
