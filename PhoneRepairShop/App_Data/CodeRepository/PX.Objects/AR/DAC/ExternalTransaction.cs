using System;
using PX.Data;
using PX.Objects.AR.CCPaymentProcessing.Interfaces;
namespace PX.Objects.AR
{
	[PXCacheName("External Transaction")]
	public class ExternalTransaction : PX.Data.IBqlTable, IExternalTransaction
	{
		#region TransactionID
		public abstract class transactionID : PX.Data.BQL.BqlInt.Field<transactionID> { }
		[PXDBIdentity(IsKey = true)]
		[PXUIField(DisplayName = "Ext. Tran. ID", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual int? TransactionID { get;set; }
		#endregion

		#region PMInstanceID
		public abstract class pMInstanceID : PX.Data.BQL.BqlInt.Field<pMInstanceID> { }
		[PXDBInt]
		[PXDefault(0)]
		public virtual int? PMInstanceID { get; set; }
		#endregion

		#region DocType
		public abstract class docType : PX.Data.BQL.BqlString.Field<docType> { }
		[PXDBString(3, IsFixed = true)]
		[PXUIField(DisplayName = "Doc. Type", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual string DocType { get; set; }
		#endregion

		#region RefNbr
		public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Doc Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual string RefNbr { get; set; }
		#endregion

		#region OrigDocType
		public abstract class origDocType : PX.Data.BQL.BqlString.Field<origDocType> { }
		[PXDBString(3, IsFixed = true)]
		[PXUIField(DisplayName = "Orig. Doc. Type")]
		public virtual string OrigDocType { get; set; }
		#endregion

		#region OrigRefNbr
		public abstract class origRefNbr : PX.Data.BQL.BqlString.Field<origRefNbr> { }
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Orig. Doc. Ref. Nbr.")]
		public virtual string OrigRefNbr { get; set; }
		#endregion

		#region TranNumber
		public abstract class tranNumber : PX.Data.BQL.BqlString.Field<tranNumber> { }
		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "Proc. Center Tran. Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual string TranNumber { get; set; }
		#endregion

		#region AuthNumber
		public abstract class authNumber : PX.Data.BQL.BqlString.Field<authNumber> { }
		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "Proc. Center Auth. Nbr.")]
		public virtual string AuthNumber { get; set; }
		#endregion

		#region Amount
		public abstract class amount : PX.Data.BQL.BqlDecimal.Field<amount> { }
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Tran. Amount", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual decimal? Amount { get; set; }
		#endregion

		#region ProcessingStatus
		public abstract class processingStatus : PX.Data.BQL.BqlDecimal.Field<processingStatus> { }
		[PXDBString(3, IsFixed = true)]
		[ExtTransactionProcStatusCode.List()]
		[PXUIField(DisplayName = "Proc. Status", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual string ProcessingStatus { get; set; }
		#endregion

		#region LastActivityDate
		public abstract class lastActivityDate : PX.Data.BQL.BqlDateTime.Field<lastActivityDate> { }
		[PXDBDate(PreserveTime = true)]
		[PXUIField(DisplayName = "Last Activity Date")]
		public virtual DateTime? LastActivityDate { get; set; }
		#endregion

		#region Direction
		public abstract class direction : PX.Data.BQL.BqlString.Field<direction> { }
		[PXDBString(1, IsFixed = true)]
		public virtual string Direction { get; set; }
		#endregion

		#region Active
		public abstract class active : PX.Data.BQL.BqlBool.Field<active> { }
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Active")]
		public virtual bool? Active { get; set; }
		#endregion

		#region Completed
		public abstract class completed : PX.Data.BQL.BqlBool.Field<completed> { }
		[PXDBBool]
		[PXDefault(false)]
		public virtual bool? Completed { get; set; }
		#endregion

		#region ParentTranID
		public abstract class parentTranID : PX.Data.BQL.BqlInt.Field<parentTranID> { }
		[PXDBInt]
		public virtual int? ParentTranID { get; set; }
		#endregion

		#region ExpirationDate
		public abstract class expirationDate : PX.Data.BQL.BqlDateTime.Field<expirationDate> { }
		[PXDBDate(PreserveTime = true)]
		public virtual DateTime? ExpirationDate { get; set; }
		#endregion

		#region CVVVerification
		public abstract class cVVVerification : PX.Data.BQL.BqlString.Field<cVVVerification> { }
		[PXDBString(3, IsFixed = true)]
		[PXDefault(CVVVerificationStatusCode.NotVerified)]
		[CVVVerificationStatusCode.List()]
		[PXUIField(DisplayName = "CVV Verification")]
		public virtual string CVVVerification { get; set; }
		#endregion

		#region tstamp
		public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }
		[PXDBTimestamp()]
		public virtual byte[] tstamp { get; set; }
		#endregion

		public static class TransactionDirection
		{
			public const string Debet = "D";
			public const string Credit = "C";
		}
	}
}
