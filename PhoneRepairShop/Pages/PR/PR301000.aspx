<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="PR301000.aspx.cs"
	Inherits="Page_PR301000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.PR.PRPayBatchEntry" PrimaryView="Document">
		<CallbackCommands>
			<px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
			<px:PXDSCallbackCommand Name="Insert" PostData="Self" />
			<px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="True" />
			<px:PXDSCallbackCommand Name="Last" PostData="Self" />
			<px:PXDSCallbackCommand Name="AddEmployee" Visible="False" CommitChanges="true" />
			<px:PXDSCallbackCommand Name="AddSelEmployee" Visible="False" CommitChanges="true" />
			<px:PXDSCallbackCommand Name="ViewEarningDetails" Visible="False" CommitChanges="true" />
            <px:PXDSCallbackCommand Name="ViewPayCheck" Visible="False" 
                DependOnGrid="grid">
            </px:PXDSCallbackCommand>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Document" Width="100%">
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="S" />
			<px:PXSelector ID="edBatchNbr" runat="server" DataField="BatchNbr" AutoRefresh="True" CommitChanges="True" />
			<px:PXDropDown ID="edStatus" runat="server" DataField="Status" Width="100px" />
			<px:PXCheckBox ID="edHold" runat="server" DataField="Hold" CommitChanges="True" />
			<px:PXDropDown ID="edPayrollType" runat="server" DataField="PayrollType" />
			<px:PXSelector ID="edPayGroupID" runat="server" DataField="PayGroupID" CommitChanges="True" />

			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
			<px:PXSelector ID="edPayPeriodID" runat="server" DataField="PayPeriodID" CommitChanges="True" AutoRefresh="True"/>
			<px:PXDateTimeEdit ID="edStartDate" runat="server" DataField="StartDate" />
			<px:PXDateTimeEdit ID="edEndDate" runat="server" DataField="EndDate" />
			<px:PXDateTimeEdit ID="edTransactionDate" runat="server" DataField="TransactionDate" />
			<px:PXLayoutRule runat="server" ColumnSpan="2"></px:PXLayoutRule>
			<px:PXTextEdit ID="edDocDesc" runat="server" DataField="DocDesc" />

			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" StartGroup="True" />
			<px:PXPanel ID="XX" runat="server" RenderStyle="Simple">
				<px:PXLayoutRule runat="server" ControlSize="SM" LabelsWidth="SM" StartColumn="True" />
				<px:PXNumberEdit ID="edNumberOfEmployees" runat="server" DataField="NumberOfEmployees" />
				<px:PXNumberEdit ID="edTotalHourQty" runat="server" DataField="TotalHourQty" />
				<px:PXNumberEdit ID="edTotalEarnings" runat="server" DataField="TotalEarnings" />
			</px:PXPanel>
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXTab ID="tab" runat="server" Width="100%" Height="150px" DataSourceID="ds" DataMember="Transactions">
		<Items>
			<px:PXTabItem Text="Document Details">
				<Template>
					<px:PXGrid ID="PXGrid1" runat="server" DataSourceID="ds" SkinID="DetailsInTab" Width="100%" KeepPosition="True" SyncPosition="True" AllowPaging="True" AdjustPageSize="Auto">
						<Levels>
							<px:PXGridLevel DataMember="Transactions">
                                <Mode AllowAddNew="false" />
								<Columns>
									<px:PXGridColumn DataField="EmployeeID" CommitChanges="True" Width="120px" />
									<px:PXGridColumn DataField="EmployeeID_Description" Width="300px" />
									<px:PXGridColumn DataField="HourQty" Width="60px" CommitChanges="True" TextAlign="Right" />
									<px:PXGridColumn DataField="Rate" TextAlign="Right" CommitChanges="True" />
									<px:PXGridColumn DataField="Amount" Width="100px" TextAlign="Right" />
                                    <px:PXGridColumn DataField="PRPayment__PaymentDocAndRef" Width="140px"
                                        LinkCommand="ViewPayCheck">
                                    </px:PXGridColumn>
								</Columns>
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" />
						<ActionBar>
                            <Actions>
                                <AddNew ToolBarVisible = "False" />
                            </Actions>
							<CustomItems>
								<px:PXToolBarButton Text="Add Employee">
									<AutoCallBack Command="AddEmployee" Target="ds">
										<Behavior CommitChanges="True" PostData="Page" />
									</AutoCallBack>
								</px:PXToolBarButton>
								<px:PXToolBarButton Text="Earning Details">
									<AutoCallBack Command="ViewEarningDetails" Target="ds">
										<Behavior CommitChanges="True" PostData="Page" />
									</AutoCallBack>
								</px:PXToolBarButton>
							</CustomItems>
						</ActionBar>
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Deductions and Benefits">
				<Template>
					<px:PXGrid runat="server" ID="PXGrid2" SkinID="Inquire" SyncPosition="True" DataSourceID="ds" KeepPosition="True" Width="100%" AllowPaging="True" AdjustPageSize="Auto">
						<Levels>
							<px:PXGridLevel DataMember="Deductions">
								<Columns>
                                    <px:PXGridColumn DataField="IsEnabled" Width="70" Type="CheckBox" />
									<px:PXGridColumn DataField="CodeID" Width="140" />
									<px:PXGridColumn DataField="PRDeductCode__Description" Width="220" />
									<px:PXGridColumn DataField="PRDeductCode__ContribType" Width="70" />
									<px:PXGridColumn DataField="PRDeductCode__DedCalcType" Width="70" />
									<px:PXGridColumn DataField="PRDeductCode__DedAmount" Width="100" />
									<px:PXGridColumn DataField="PRDeductCode__DedPercent" Width="100" />
									<px:PXGridColumn DataField="PRDeductCode__CntCalcType" Width="70" />
									<px:PXGridColumn DataField="PRDeductCode__CntAmount" Width="100" />
									<px:PXGridColumn DataField="PRDeductCode__CntPercent" Width="100" />
									<px:PXGridColumn DataField="PRDeductCode__IsGarnishment" Width="60" Type="CheckBox"/>
								</Columns>
								<RowTemplate>
									<px:PXSelector runat="server" ID="codeIDSelector" DataField="CodeID" AllowEdit="True" />
								</RowTemplate>
							</px:PXGridLevel>
						</Levels>
                        <AutoSize Enabled="True" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
            <px:PXTabItem Text="Overtime Rules">
                <Template>
                    <px:PXFormView ID="formOvertimeRules" runat="server" DataSourceID="ds" DataMember="CurrentDocument" RenderStyle="Normal" SkinID="Transparent">
                        <Template>
                            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
                            <px:PXCheckBox ID="chkApplyOvertimeRules" runat="server" DataField="ApplyOvertimeRules" AlignLeft="true" CommitChanges="true"  />
                        </Template>
                    </px:PXFormView>
                    <px:PXGrid ID="PXGridOvertimeRules" runat="server" DataSourceID="ds" SkinID="Inquire" Width="100%" KeepPosition="True" SyncPosition="True" AllowPaging="True" AdjustPageSize="Auto">
                        <Levels>
                            <px:PXGridLevel DataMember="BatchOvertimeRules">
                                <Columns>
                                    <px:PXGridColumn DataField="IsActive" TextAlign="Center" Type="CheckBox" CommitChanges="True" Width="60px" />
                                    <px:PXGridColumn DataField="OvertimeRuleID" Width="170px" />
                                    <px:PXGridColumn DataField="PROvertimeRule__Description" Width="240px" />
                                    <px:PXGridColumn DataField="PROvertimeRule__DisbursingTypeCD" Width="100px" />
                                    <px:PXGridColumn DataField="PROvertimeRule__OvertimeMultiplier" TextAlign="Right" Width="70px" />
                                    <px:PXGridColumn DataField="PROvertimeRule__RuleType" Width="70px" />
                                    <px:PXGridColumn DataField="PROvertimeRule__WeekDay" Width="90px" />
                                    <px:PXGridColumn DataField="PROvertimeRule__OvertimeThreshold" Width="100px" />
                                    <px:PXGridColumn DataField="PROvertimeRule__State" Width="53px" />
                                    <px:PXGridColumn DataField="PROvertimeRule__UnionID" Width="150px" />
                                    <px:PXGridColumn DataField="PROvertimeRule__ProjectID" Width="150px" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
		</Items>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXTab>

	<%-- Employees Lookup --%>
	<px:PXSmartPanel ID="PanelEmployees" runat="server" Key="employees" LoadOnDemand="true" Width="1100px" Height="500px"
		Caption="Add Employee" CaptionVisible="true" AutoRepaint="true" DesignView="Content" ShowAfterLoad="true">
		<px:PXFormView ID="formEmployees" runat="server" CaptionVisible="False" DataMember="AddEmployeeFilter" DataSourceID="ds"
			Width="100%" SkinID="Transparent">
			<Template>
				<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
				<px:PXSelector ID="edEmployeeClassID" runat="server" DataField="EmployeeClassID" CommitChanges="True" />

				<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
				<px:PXCheckBox CommitChanges="True" ID="chkUseQuickPay" AlignLeft="true" runat="server" Checked="True" DataField="UseQuickPay" />
				<px:PXCheckBox CommitChanges="True" ID="chkUseTimeSheets" AlignLeft="true" runat="server" Checked="True" DataField="UseTimeSheets" />
				<px:PXCheckBox CommitChanges="True" ID="chkUseSalesComm" AlignLeft="true" runat="server" Checked="True" DataField="UseSalesComm" />
			</Template>
		</px:PXFormView>
		<px:PXGrid ID="gridEmployees" runat="server" DataSourceID="ds" Style="height: 189px;"
			AutoAdjustColumns="true" Width="100%" SkinID="Inquire" AdjustPageSize="Auto" AllowSearch="True" FastFilterID="edEmployeeClassID"
			FastFilterFields="EmployeeClassID" BatchUpdate="true">
			<CallbackCommands>
				<Refresh CommitChanges="true"></Refresh>
			</CallbackCommands>
			<ActionBar PagerVisible="False">
				<PagerSettings Mode="NextPrevFirstLast" />
			</ActionBar>
			<Levels>
				<px:PXGridLevel DataMember="Employees">
					<Mode AllowUpdate="false" />
					<RowTemplate>
					</RowTemplate>
					<Columns>
						<px:PXGridColumn AllowNull="False" DataField="Selected" TextAlign="Center" Type="CheckBox" AutoCallBack="true"
							AllowCheckAll="true" />
						<px:PXGridColumn DataField="AcctCD" />
						<px:PXGridColumn DataField="AcctName" />
						<px:PXGridColumn DataField="PREmployee__EmployeeClassID" />
                        <px:PXGridColumn DataField="EPEmployeePosition__PositionID" />
                        <px:PXGridColumn DataField="EPEmployee__DepartmentID" />
					</Columns>
				</px:PXGridLevel>
			</Levels>
			<AutoSize Enabled="true" />
		</px:PXGrid>
		<px:PXPanel ID="PXPanel6" runat="server" SkinID="Buttons">
			<px:PXButton ID="PXButton5" runat="server" CommandName="AddSelEmployee" CommandSourceID="ds" Text="Add" SyncVisible="false" />
			<px:PXButton ID="PXButton4" runat="server" Text="Add & Close" DialogResult="OK" />
			<px:PXButton ID="PXButton6" runat="server" DialogResult="Cancel" Text="Cancel" />
		</px:PXPanel>
	</px:PXSmartPanel>

	<%-- Earning Details Lookup --%>
	<px:PXSmartPanel ID="PanelEarningDetails" runat="server" Key="EarningDetails" Width="1800px" Height="700px"
		Caption="Earning Details" CaptionVisible="true" AutoRepaint="true" DesignView="Content" ShowAfterLoad="true">
		<px:PXFormView ID="formEarningDetails" runat="server" CaptionVisible="False" DataMember="CurrentTransaction" DataSourceID="ds"
			Width="100%" SkinID="Transparent">
			<Template>
				<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
				<px:PXSelector ID="edEmployeeID" runat="server" DataField="EmployeeID" Enabled="False" />
				<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
				<px:PXNumberEdit ID="edHourQty" runat="server" DataField="HourQty" Enabled="False" />
				<px:PXNumberEdit ID="edAmount" runat="server" DataField="Amount" Enabled="False" />
			</Template>
		</px:PXFormView>
		<px:PXGrid ID="gridEarningDetails" runat="server" DataSourceID="ds" Style="height: 189px;"
			AutoAdjustColumns="true" Width="100%" SkinID="Details" AdjustPageSize="Auto" AllowSearch="True">
			<CallbackCommands>
				<Refresh CommitChanges="true"></Refresh>
			</CallbackCommands>
			<ActionBar PagerVisible="False">
				<PagerSettings Mode="NextPrevFirstLast" />
			</ActionBar>
			<Levels>
				<px:PXGridLevel DataMember="EarningDetails">
					<Mode InitNewRow="True" />
					<RowTemplate>
					</RowTemplate>
					<Columns>
                        <px:PXGridColumn DataField="BranchID" Width="90px" CommitChanges="True" />
                        <px:PXGridColumn DataField="Date" Width="80px" CommitChanges="True" />
                        <px:PXGridColumn DataField="TypeCD" Width="65px" CommitChanges="True" />
                        <px:PXGridColumn DataField="TypeCD_EPEarningType_Description" Width="110px" />
                        <px:PXGridColumn DataField="LocationID" Width="70px" CommitChanges="True" />
                        <px:PXGridColumn DataField="Hours" Width="60px" CommitChanges="True" TextAlign="Right" />
                        <px:PXGridColumn DataField="Units" Width="60px" CommitChanges="True" TextAlign="Right" />
                        <px:PXGridColumn DataField="UnitType" Width="60px" />
                        <px:PXGridColumn DataField="Rate" Width="60" TextAlign="Right" CommitChanges="True" />
                        <px:PXGridColumn DataField="ManualRate" Type="CheckBox" CommitChanges="True" />
                        <px:PXGridColumn DataField="Amount" Width="100px" TextAlign="Right" />
                        <px:PXGridColumn DataField="AccountID" Width="75px" CommitChanges="True" />
                        <px:PXGridColumn DataField="SubID" Width="100px" CommitChanges="True" />
                        <px:PXGridColumn DataField="ProjectID" Width="150px" CommitChanges="True" />
                        <px:PXGridColumn DataField="ProjectTaskID" Width="150px" CommitChanges="True" />
                        <px:PXGridColumn DataField="CertifiedJob" Width="70px" TextAlign="Center" Type="CheckBox" CommitChanges="True" />
                        <px:PXGridColumn DataField="CostCodeID" Width="100px" CommitChanges="True" />
                        <px:PXGridColumn DataField="UnionID" Width="70px" CommitChanges="True" />
                        <px:PXGridColumn DataField="LabourItemID" Width="70px" CommitChanges="True" />
                        <px:PXGridColumn DataField="WorkCodeID" Width="70px" CommitChanges="True" />
					</Columns>
				</px:PXGridLevel>
			</Levels>
			<AutoSize Enabled="true" />
		</px:PXGrid>
		<px:PXPanel ID="PXPanel1" runat="server" SkinID="Buttons">
			<px:PXButton ID="PXButton2" runat="server" Text="OK" DialogResult="OK" />
		</px:PXPanel>
	</px:PXSmartPanel>
</asp:Content>
