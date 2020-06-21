using PX.Data;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.CM;
using PX.Objects.Common.Discount;
using PX.Objects.Common.Discount.Attributes;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.SO;
using PX.Objects.TX;
using System;

namespace PX.Objects.FS
{
    [Serializable]
    [PXCacheName(TX.TableName.APPOINTMENTDET_PICKUPDELIVERY)]
    [PXBreakInheritance]
    [PXProjection(typeof(Select<FSAppointmentDet,
                        Where<
                            FSAppointmentDet.lineType, Equal<FSAppointmentInventoryItem.lineType.Pickup_Delivery>>>), Persistent = true)]
    public class FSAppointmentInventoryItem : FSAppointmentDet
    {
        #region AppointmentID
        public new abstract class appointmentID : PX.Data.BQL.BqlInt.Field<appointmentID> { }
        #endregion
        #region LineType
        public new abstract class lineType : ListField_LineType_Pickup_Delivery
        {
        }

        [PXDBString(5, IsFixed = true)]
        [PXUIField(DisplayName = "Line Type", Enabled = false)]
        [lineType.ListAtrribute]
        [PXDefault(ID.LineType_Pickup_Delivery.PICKUP_DELIVERY)]
        public override string LineType { get; set; }
        #endregion

        #region SODetID
        public new abstract class sODetID : PX.Data.BQL.BqlInt.Field<sODetID> { }

        [PXDefault]
        [PXDBInt]
        [PXUIField(DisplayName = "Line Ref.")]
        [FSSelectorServiceSODetIDInAppointment]
        public override int? SODetID { get; set; }
        #endregion
        #region LineRef
        public new abstract class lineRef : PX.Data.BQL.BqlString.Field<lineRef> { }

        [PXDBString(4, IsFixed = true)]
        [PXFormula(typeof(Selector<sODetID, FSSODet.lineRef>))]
        [PXDefault]
        [PXUIField(DisplayName = "Line Ref.", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
        public override string LineRef { get; set; }
        #endregion
        #region PickupDeliveryServiceID
        public abstract class pickupDeliveryServiceID : PX.Data.BQL.BqlInt.Field<pickupDeliveryServiceID> { }

        [Service(Enabled = false)]        
        [PXFormula(typeof(Selector<sODetID, FSSODet.inventoryID>))]
        [PXDefault]
        public virtual int? PickupDeliveryServiceID { get; set; }
        #endregion
        #region ServiceType
        public new abstract class serviceType : ListField_Appointment_Service_Action_Type
        {
        }

        [PXDBString(1, IsFixed = true)]
        [serviceType.List]
        [PXFormula(typeof(Selector<sODetID, FSAppointmentDetService.serviceType>))]
        [PXDefault]
        [PXUIField(DisplayName = "Pickup/Delivery Action", Enabled = false)]
        public override string ServiceType { get; set; }
        #endregion

        #region BranchID
        public new abstract class branchID : PX.Data.IBqlField
        {
        }

        [Branch(typeof(FSAppointment.branchID))]
        public override int? BranchID { get; set; }
        #endregion
        #region InventoryID
        public new abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }

        [PXDefault]
        [PXCheckUnique(Where = typeof(Where<FSAppointmentInventoryItem.appointmentID, Equal<Current<FSAppointmentInventoryItem.appointmentID>>,
                                                    And<FSAppointmentInventoryItem.sODetID, Equal<Current<FSAppointmentInventoryItem.sODetID>>>>))]
        [PXFormula(typeof(Default<pickupDeliveryServiceID>))]
        [PickupDeliveryItem]
        public override int? InventoryID { get; set; }
        #endregion
        #region SubItemID
        public new abstract class subItemID : PX.Data.BQL.BqlInt.Field<subItemID> { }

        [SubItem(typeof(inventoryID), DisplayName = "Subitem")]
        [PXDefault(typeof(Search<InventoryItem.defaultSubItemID,
                            Where<
                                InventoryItem.inventoryID, Equal<Current<FSAppointmentInventoryItem.inventoryID>>,
                                And<InventoryItem.defaultSubItemOnEntry, Equal<True>>>>),
                    PersistingCheck = PXPersistingCheck.Nothing)]
        [SubItemStatusVeryfier(typeof(inventoryID), typeof(siteID), InventoryItemStatus.Inactive, InventoryItemStatus.NoSales)]
        public override int? SubItemID { get; set; }
        #endregion

        #region UOM
        public new abstract class uOM : PX.Data.BQL.BqlString.Field<uOM> { }

        [INUnit(typeof(inventoryID), DisplayName = "UOM", Enabled = false)]
        [PXDefault(typeof(Search<InventoryItem.salesUnit, Where<InventoryItem.inventoryID, Equal<Current<FSAppointmentInventoryItem.inventoryID>>>>),
                    PersistingCheck = PXPersistingCheck.Nothing)]
        public override string UOM { get; set; }
        #endregion

        #region SiteID
        public new abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID> { }

        [SiteAvail(typeof(inventoryID), typeof(subItemID), DisplayName = "Warehouse")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIRequired(typeof(IIf<Where<
                                    lineType, NotEqual<FSAppointmentDet.lineType.Comment_Service>,
                                    And<lineType, NotEqual<FSAppointmentDet.lineType.Comment_Part>,
                                    And<lineType, NotEqual<FSAppointmentDet.lineType.Instruction_Service>,
                                    And<lineType, NotEqual<FSAppointmentDet.lineType.Instruction_Part>,
                                    And<lineType, NotEqual<FSAppointmentDet.lineType.Service_Template>>>>>>, True, False>))]
        public override int? SiteID { get; set; }
        #endregion
        #region SiteLocationID
        public new abstract class siteLocationID : PX.Data.BQL.BqlInt.Field<siteLocationID> { }

        [Location(typeof(siteID), DescriptionField = typeof(INLocation.descr), DisplayName = "Location")]
        public override int? SiteLocationID { get; set; }
        #endregion

        #region ProjectTaskID
        public new abstract class projectTaskID : PX.Data.BQL.BqlInt.Field<projectTaskID> { }

        [PXDBInt]
        [PXDefault(typeof(Switch<
                                Case<Where<
                                    lineType, Equal<lineType.Pickup_Delivery>>,
                                    Null>,
                                    Current<FSAppointment.dfltProjectTaskID>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Project Task", FieldClass = ProjectAttribute.DimensionName)]
        [FSSelectorActive_AR_SO_ProjectTask(typeof(Where<PMTask.projectID, Equal<Current<FSAppointmentInventoryItem.projectID>>>))]
        public override int? ProjectTaskID { get; set; }
        #endregion

        #region EstimatedQty
        public new abstract class estimatedQty : PX.Data.BQL.BqlDecimal.Field<estimatedQty>
		{
        }

        [PXDBQuantity]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Estimated Quantity")]
        public override decimal? EstimatedQty { get; set; }
        #endregion

        #region Qty
        public new abstract class qty : PX.Data.BQL.BqlDecimal.Field<qty>
		{
        }

        [PXDBQuantity]
        [PXDefault(typeof(Switch<
                                Case<Where<
                                        FSAppointmentInventoryItem.status, NotEqual<FSAppointmentDet.status.Canceled>,
                                        And<
                                            Where<
                                                Current<FSAppointmentInventoryItem.status>, Equal<FSAppointment.status.InProcess>,
                                                Or<Current<FSAppointmentInventoryItem.status>, Equal<FSAppointment.status.Completed>>>>>,
                                    FSAppointmentInventoryItem.estimatedQty>,
                                SharedClasses.decimal_0>))]
        [PXFormula(typeof(Default<status>))]
        [PXUIField(DisplayName = "Actual Quantity")]
        public override decimal? Qty { get; set; }
        #endregion

        #region BillableQty
        public new abstract class billableQty : PX.Data.BQL.BqlDecimal.Field<billableQty>
		{
        }

        [PXDBQuantity]
        [PXFormula(typeof(Default<FSAppointmentInventoryItem.isBillable, FSAppointmentInventoryItem.contractRelated>))]
        [PXFormula(typeof(Switch<
                                Case<
                                    Where<
                                        FSAppointmentInventoryItem.isPrepaid, Equal<True>>,
                                    SharedClasses.decimal_0,
                                Case<
                                    Where<
                                        FSAppointmentInventoryItem.contractRelated, Equal<True>>,
                                    FSAppointmentInventoryItem.extraUsageQty,
                                Case<
                                    Where2<
                                        Where<
                                            Current<FSAppointment.status>, Equal<FSAppointment.status.AutomaticScheduled>,
                                            Or<Current<FSAppointment.status>, Equal<FSAppointment.status.ManualScheduled>>>,
                                        And<contractRelated, Equal<False>>>,
                                    FSAppointmentInventoryItem.estimatedQty>>>,
                            FSAppointmentInventoryItem.qty>))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Billable Quantity", Enabled = false)]
        public override decimal? BillableQty { get; set; }
        #endregion

        #region AcctID
        public new abstract class acctID : PX.Data.BQL.BqlInt.Field<acctID> { }

        [PXFormula(typeof(Default<FSAppointmentInventoryItem.inventoryID>))]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [Account(Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description), Visible = false)]
        public override int? AcctID { get; set; }
        #endregion
        #region SubID
        public new abstract class subID : PX.Data.BQL.BqlInt.Field<subID> { }

        [PXFormula(typeof(Default<FSAppointmentInventoryItem.acctID>))]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [SubAccount(typeof(FSAppointmentInventoryItem.acctID), Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description), Visible = false)]
        public override int? SubID { get; set; }
        #endregion

        #region CuryBillableExtPrice
        public new abstract class curyBillableExtPrice : PX.Data.BQL.BqlDecimal.Field<curyBillableExtPrice> { }

        [PXDBCurrency(typeof(curyInfoID), typeof(billableExtPrice))]
        [PXUIField(DisplayName = "Ext. Price")]
        [PXFormula(typeof(Switch<
                                Case<
                                    Where<
                                        isBillable, Equal<False>>,
                                    SharedClasses.decimal_0,
                                Case<
                                    Where<
                                        contractRelated, Equal<False>,
                                        And<lineType, Equal<lineType.Pickup_Delivery>,
                                        And<billingRule, Equal<billingRule.None>>>>,
                                    SharedClasses.decimal_0,
                                Case<
                                    Where<
                                        contractRelated, Equal<True>,
                                        And<isBillable, Equal<True>>>,
                                    Mult<curyExtraUsageUnitPrice, billableQty>>>>,
                        Mult<curyUnitPrice, billableQty>>))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public override Decimal? CuryBillableExtPrice { get; set; }
        #endregion
        #region CuryExtPrice
        public new abstract class curyExtPrice : PX.Data.BQL.BqlDecimal.Field<curyExtPrice> { }

        [PXDecimal]
        [PXUIField(DisplayName = "Ext. Price")]
        public override Decimal? CuryExtPrice
        {
            get
            {
                return CuryBillableExtPrice;
            }
        }
        #endregion
        #region BillableExtPrice
        public new abstract class billableExtPrice : PX.Data.BQL.BqlDecimal.Field<billableExtPrice> { }

        [PXDBDecimal(4)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public override Decimal? BillableExtPrice { get; set; }
        #endregion

        #region Discount Fields
        #region ManualDisc
        public new abstract class manualDisc : PX.Data.BQL.BqlBool.Field<manualDisc> { }

        [ManualDiscountMode(typeof(curyDiscAmt), typeof(discPct), DiscountFeatureType.CustomerDiscount)]
        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Manual Discount", Visibility = PXUIVisibility.Visible)]
        public override Boolean? ManualDisc { get; set; }
        #endregion
        #region DiscPct
        public new abstract class discPct : PX.Data.BQL.BqlDecimal.Field<discPct> { }

        [PXDBDecimal(6, MinValue = 0, MaxValue = 100)]
        [PXUIField(DisplayName = "Discount Percent")]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXFormula(typeof(Switch<Case<Where<isBillable, Equal<False>>, decimal0>, discPct>))]
        [PXFormula(typeof(Default<inventoryID>))]
        public override Decimal? DiscPct { get; set; }
        #endregion
        #region CuryDiscAmt
        public new abstract class curyDiscAmt : PX.Data.BQL.BqlDecimal.Field<curyDiscAmt> { }

        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXDBCurrency(typeof(CommonSetup.decPlPrcCst), typeof(curyInfoID), typeof(discAmt))]
        [PXFormula(typeof(Div<Mult<curyBillableExtPrice, discPct>, decimal100>))]
        [PXUIField(DisplayName = "Discount Amount")]
        public override Decimal? CuryDiscAmt { get; set; }
        #endregion
        #region DiscAmt
        public new abstract class discAmt : PX.Data.BQL.BqlDecimal.Field<discAmt> { }

        [PXDBDecimal(4)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public override Decimal? DiscAmt { get; set; }
        #endregion
        #region DiscountsAppliedToLine
        public new abstract class discountsAppliedToLine : PX.Data.BQL.BqlByteArray.Field<discountsAppliedToLine> { }

        [PXDBPackedIntegerArray()]
        public override ushort[] DiscountsAppliedToLine { get; set; }
        #endregion
        #region DiscountID
        public new abstract class discountID : PX.Data.BQL.BqlString.Field<discountID> { }

        [PXDBString(10, IsUnicode = true)]
        [PXSelector(typeof(Search<ARDiscount.discountID, Where<ARDiscount.type, Equal<DiscountType.LineDiscount>>>))]
        [PXUIField(DisplayName = "Discount Code", Visible = true, Enabled = true)]
        public override String DiscountID { get; set; }
        #endregion
        #region DiscountSequenceID
        public new abstract class discountSequenceID : PX.Data.BQL.BqlString.Field<discountSequenceID> { }

        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "Discount Sequence", Visible = false, Enabled = false)]
        public override String DiscountSequenceID { get; set; }
        #endregion
        #endregion
        #region Tax Fields
        #region TaxCategoryID
        public new abstract class taxCategoryID : PX.Data.BQL.BqlString.Field<taxCategoryID> { }

        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "Tax Category", Visibility = PXUIVisibility.Visible)]
        [PXSelector(typeof(TaxCategory.taxCategoryID), DescriptionField = typeof(TaxCategory.descr))]
        [PXDefault(typeof(Search<InventoryItem.taxCategoryID,
            Where<InventoryItem.inventoryID, Equal<Current<FSAppointmentInventoryItem.inventoryID>>>>),
            PersistingCheck = PXPersistingCheck.Nothing)]
        [PXFormula(typeof(Default<inventoryID>))]
        public override String TaxCategoryID { get; set; }
        #endregion
        #endregion

        #region BillableTranAmt
        public new abstract class billableTranAmt : PX.Data.BQL.BqlDecimal.Field<billableTranAmt> { }

        [PXDBBaseCury]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Base Billable Amount", Enabled = false)]
        public override Decimal? BillableTranAmt { get; set; }
        #endregion
        #region CuryBillableTranAmt
        public new abstract class curyBillableTranAmt : PX.Data.BQL.BqlDecimal.Field<curyBillableTranAmt> { }

        [PXDBCurrency(typeof(curyInfoID), typeof(billableTranAmt))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXFormula(typeof(Switch<
                                Case<
                                    Where<
                                        isPrepaid, Equal<True>,
                                        Or<contractRelated, Equal<True>>>,
                                    curyBillableExtPrice>,
                                Sub<curyBillableExtPrice, curyDiscAmt>>),
                            typeof(SumCalc<FSAppointment.curyBillableLineTotal>))]
        [PXUIField(DisplayName = "Amount", Enabled = false)]
        public override Decimal? CuryBillableTranAmt { get; set; }
        #endregion
        #region CuryLineAmt
        public new abstract class curyLineAmt : PX.Data.BQL.BqlDecimal.Field<curyLineAmt> { }

        [PXDecimal]
        [PXUnboundDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Amount", Enabled = false)]

        public override Decimal? CuryLineAmt
        {
            get { return CuryBillableTranAmt; }
        }
        #endregion

        #region ContractRelated
        public new abstract class contractRelated : PX.Data.BQL.BqlBool.Field<contractRelated> { }

        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Service Contract Item", IsReadOnly = true)]
        public override bool? ContractRelated { get; set; }
        #endregion
        #region ExtraUsageUnitPrice 
        public new abstract class extraUsageUnitPrice : PX.Data.BQL.BqlDecimal.Field<extraUsageUnitPrice> { }

        [PXDBPriceCost]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Base Overage Unit Price", Enabled = false)]
        public override Decimal? ExtraUsageUnitPrice { get; set; }
        #endregion
        #region CuryExtraUsageUnitPrice
        public new abstract class curyExtraUsageUnitPrice : PX.Data.BQL.BqlDecimal.Field<curyExtraUsageUnitPrice> { }

        [PXDBCurrency(typeof(curyInfoID), typeof(extraUsageUnitPrice))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Overage Unit Price", Enabled = false, Visible = false)]
        public override Decimal? CuryExtraUsageUnitPrice { get; set; }
        #endregion

        #region SourceSalesOrderRefNbr
        public new abstract class sourceSalesOrderRefNbr : PX.Data.BQL.BqlString.Field<sourceSalesOrderRefNbr> { }

        [PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "Sales Order Ref. Nbr.", Enabled = false)]
        [PXSelector(typeof(Search<SOOrder.orderNbr, Where<SOOrder.orderType, Equal<Current<FSAppointmentInventoryItem.sourceSalesOrderType>>>>))]
        public override string SourceSalesOrderRefNbr { get; set; }
        #endregion

        #region PostID
        public new abstract class postID : PX.Data.BQL.BqlInt.Field<postID> { }
        #endregion

        #region MemoryHelper

        #region Selected
        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }

        [PXBool]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        #endregion

        #region Mem_BatchNbr
        public abstract class mem_BatchNbr : PX.Data.BQL.BqlString.Field<mem_BatchNbr> { }

        [PXString(15, IsFixed = true)]
        [PXUIField(DisplayName = "Batch Nbr.", Enabled = false)]
        public virtual string Mem_BatchNbr { get; set; }
        #endregion
        #region Mem_ServiceType
        public abstract class mem_ServiceType : ListField_Appointment_Service_Action_Type
        {
        }

        [PXString(1, IsFixed = true)]
        [mem_ServiceType.List]
        [PXFormula(typeof(Selector<sODetID, FSAppointmentDet.serviceType>))]
        [PXUIField(DisplayName = "Pickup/Deliver Items", Enabled = false)]
        public virtual string Mem_ServiceType { get; set; }
        #endregion

        #endregion

        #region Methods
        public virtual int? GetINItemID()
        {
            return this.InventoryID;
        }
        #endregion
    }
}