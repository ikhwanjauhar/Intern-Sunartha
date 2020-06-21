<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="SM206505.aspx.cs" Inherits="Page_SM206505"
	Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="ScanJob"
		TypeName="PX.SM.SMScanJobMaint">
		<CallbackCommands>
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" Visible="true" />
			<px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="true" Visible="false" />
			<px:PXDSCallbackCommand Name="Last" PostData="Self" Visible="false" />
            <px:PXDSCallbackCommand Name="Next" PostData="Self" Visible="false" />
            <px:PXDSCallbackCommand Name="Previous" PostData="Self" Visible="false" />
            <px:PXDSCallbackCommand Name="CopyPaste" PostData="Self" Visible="false" />
            <px:PXDSCallbackCommand Name="Delete" PostData="Self" Visible="false" />
            <px:PXDSCallbackCommand Name="Insert" PostData="Self" Visible="false" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:content id="cont2" contentplaceholderid="phF" runat="Server">   
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Width="100%" Visible="true" DataMember="Filter" AllowCollapse="false">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
            <px:PXDateTimeEdit ID="edStartDate" runat="server" DataField="StartDate" CommitChanges="true" />
            <px:PXDateTimeEdit ID="edEndDate" runat="server" DataField="EndDate" CommitChanges="true" />
            <px:PXCheckBox ID="chkHideProcessed" runat="server" DataField="HideProcessed" CommitChanges="true" />
        </Template>
    </px:PXFormView>
</asp:content>
<asp:content id="cont3" contentplaceholderid="phG" runat="Server">
    <px:PXSplitContainer runat="server" ID="spScanJobInfo" SplitterPosition="350" SkinID="Horizontal" Height="500px">
		<AutoSize Enabled="true" Container="Window" />
        <ContentLayout AutoSizeControls="True" Orientation="Horizontal" />
		<Template1>
            <px:PXGrid ID="grdScanMaster" runat="server" DataSourceID="ds" Width="100%" SkinID="Details" SyncPosition="True" KeepPosition="True"
                       Height="400px" AdjustPageSize="Auto" TabIndex="700" MatrixMode="True" Caption="Scan Jobs" CaptionVisible="true">
                <Levels>
                    <px:PXGridLevel DataMember="ScanJob">
                        <Columns>
                            <px:PXGridColumn DataField="ScanJobID" Width="80px" TextAlign="Left" />
                            <px:PXGridColumn DataField="DeviceHubID" Width="200px" />
							<px:PXGridColumn DataField="ScannerName" Width="200px" />
                            <px:PXGridColumn DataField="ScannerName_SMScanner_description" Width="220px" />
                            <px:PXGridColumn DataField="Status" Width="100px" />
                            <px:PXGridColumn DataField="EntityScreenID" Width="120px" />
                            <px:PXGridColumn DataField="PaperSource" TextAlign="Left" Width="100px"/>
                            <px:PXGridColumn DataField="PixelType" TextAlign="Left" Width="100px"/>
                            <px:PXGridColumn DataField="Resolution" TextAlign="Left" Width="100px"/>
                            <px:PXGridColumn DataField="FileType" TextAlign="Left" Width="180px"/>
                            <px:PXGridColumn DataField="FileName" Width="280px" />
                            <px:PXGridColumn DataField="Error" Width="280px" />
                            <px:PXGridColumn DataField="CreatedByID" Width="120px" />
                            <px:PXGridColumn DataField="CreatedDateTime" DisplayFormat="g" Width="150px">
                            </px:PXGridColumn>
                        </Columns>
                    </px:PXGridLevel>
                </Levels>
                <AutoCallBack Target="grdScanDetail" Command="Refresh"/>
                <AutoSize Enabled="True"/>
            </px:PXGrid>
		</Template1>
        <Template2>
            <px:PXGrid ID="grdScanDetail" runat="server" DataSourceID="ds" Width="100%" SkinID="Inquire" AdjustPageSize="Auto" Caption="Document Parameters" CaptionVisible="true">
                <Levels>
                    <px:PXGridLevel DataMember="Parameters" DataKeyNames="ScanJobID,LineNbr">
                        <Columns>
                            <px:PXGridColumn DataField="LineNbr" Width="1px" />
                            <px:PXGridColumn DataField="ParameterName" Width="200px" />
                            <px:PXGridColumn DataField="ParameterValue" Width="400px" />
                        </Columns>
                    </px:PXGridLevel>
                </Levels>
                <AutoSize Enabled="True"/>
            </px:PXGrid>
        </Template2>
    </px:PXSplitContainer>
</asp:content>