using Microsoft.Practices.ServiceLocation;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.GL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.PM.GraphExtensions
{
	public class JournalEntryProjectExt : PXGraphExtension<JournalEntry>
	{
		public PXSelect<PMRegister> ProjectDocs;
		public PXSelect<PMTran> ProjectTrans;
		public PXSelect<PMTaskTotal> ProjectTaskTotals;
		public PXSelect<PMBudgetAccum> ProjectBudget;
		public PXSelect<PMForecastHistoryAccum> ForecastHistory;

		#region Cache Attached Events
		
		#region PMTran
		#region TranID

		[PXDBLongIdentity(IsKey = true)]
		protected virtual void PMTran_TranID_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region RefNbr
		[PXDBString(15, IsUnicode = true)]
		protected virtual void PMTran_RefNbr_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region BatchNbr
		[PXDBLiteDefault(typeof(Batch.batchNbr), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "BatchNbr")]
		protected virtual void PMTran_BatchNbr_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region Date
		[PXDBDate()]
		[PXDefault(typeof(PMRegister.date))]
		public virtual void PMTran_Date_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region FinPeriodID
		[FinPeriodID(typeof(PMRegister.date), typeof(PMTran.branchID))]
		[PXUIField(DisplayName = "Fin. Period", Visibility = PXUIVisibility.SelectorVisible, Required = true)]
		public virtual void PMTran_FinPeriodID_CacheAttached(PXCache sender)
		{
		}
		#endregion
		#region BaseCuryInfoID
		public abstract class baseCuryInfoID : IBqlField { }
		[PXDBLong]
		[PXDBDefault(typeof(CurrencyInfo.curyInfoID))]
		public virtual void PMTran_BaseCuryInfoID_CacheAttached(PXCache sender)
		{
		}

		#endregion
		#region ProjectCuryInfoID
		public abstract class projectCuryInfoID : IBqlField { }
		[PXDBLong]
		[PXDBDefault(typeof(CurrencyInfo.curyInfoID))]
		public virtual void PMTran_ProjectCuryInfoID_CacheAttached(PXCache sender)
		{
		}
		#endregion

		#endregion
		
		#endregion


		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.projectModule>();
		}
		public override void Initialize()
		{
			Base.OnBeforePersist += OnBeforeGraphPersist;
			Base.OnAfterPersist += OnAfterGraphPersist;
		}

		private List<PMTask> autoAllocateTasks;
		private void OnBeforeGraphPersist(PXGraph obj)
		{
			autoAllocateTasks = CreateProjectTrans();
		}

		private void OnAfterGraphPersist(PXGraph obj)
		{
			if (autoAllocateTasks?.Count > 0)
			{
				try
				{
					AutoAllocateTasks(autoAllocateTasks);
				}
				catch (Exception ex)
				{
					throw new PXException(ex, PM.Messages.AutoAllocationFailed);
				}
			}
		}

		#region PMRegister Events

		protected virtual void PMRegister_RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			//CreateProjectTrans() can create more then one PMRegister because of autoallocation
			//hense set the RefNbr Manualy for all child transactios.

			PMRegister row = (PMRegister)e.Row;

			if (e.Operation == PXDBOperation.Insert)
			{
				if (e.TranStatus == PXTranStatus.Open)
				{
					foreach (PMTran tran in ProjectTrans.Cache.Inserted)
					{
						if (tran.TranType == row.Module)
						{
							tran.RefNbr = row.RefNbr;
						}

					}
				}
			}
		}

		#endregion

		protected virtual void AutoAllocateTasks(List<PMTask> tasks)
		{
			PMSetup setup = PXSelect<PMSetup>.Select(Base);
			bool autoreleaseAllocation = setup.AutoReleaseAllocation == true;

			PMAllocator allocator = PXGraph.CreateInstance<PMAllocator>();
			allocator.Clear();
			allocator.TimeStamp = Base.TimeStamp;
			allocator.Execute(tasks);
			allocator.Actions.PressSave();

			if (allocator.Document.Current != null && autoreleaseAllocation)
			{
				List<PMRegister> list = new List<PMRegister>();
				list.Add(allocator.Document.Current);
				List<ProcessInfo<Batch>> batchList;
				bool releaseSuccess = RegisterRelease.ReleaseWithoutPost(list, false, out batchList);
				if (!releaseSuccess)
				{
					throw new PXException(PM.Messages.AutoReleaseFailed);
				}
				foreach (var item in batchList)
				{
					Base.created.AddRange(item.Batches);
				}
			}
		}

		public virtual List<PMTask> CreateProjectTrans()
		{
			List<PMTask> autoAllocateTasks = new List<PMTask>();
			ProjectBalance pb = CreateProjectBalance();


			if (Base.BatchModule.Current != null && Base.BatchModule.Current.Module != GL.BatchModule.GL)
			{
				PXSelectBase<GLTran> select = new PXSelectJoin<GLTran,
					InnerJoin<Account, On<GLTran.accountID, Equal<Account.accountID>>,
					InnerJoin<PMAccountGroup, On<PMAccountGroup.groupID, Equal<Account.accountGroupID>>,
					InnerJoin<PMProject, On<PMProject.contractID, Equal<GLTran.projectID>, And<PMProject.nonProject, Equal<False>>>,
					InnerJoin<PMTask, On<PMTask.projectID, Equal<GLTran.projectID>, And<PMTask.taskID, Equal<GLTran.taskID>>>,
					LeftJoin<PMTran, On<GLTran.origPMTranID, Equal<PMTran.tranID>>>>>>>,
					Where<GLTran.module, Equal<Current<Batch.module>>,
					And<GLTran.batchNbr, Equal<Current<Batch.batchNbr>>,
					And<Account.accountGroupID, IsNotNull,
					And<GLTran.pMTranID, IsNull,
					And<GLTran.isNonPM, NotEqual<True>,
					And<PMAccountGroup.type, NotEqual<PMAccountType.offBalance>>>>>>>>(Base);

				if (Base.BatchModule.Current.Module == GL.BatchModule.AP)
				{
					select.Join<LeftJoin<AP.APTran, On<AP.APTran.refNbr, Equal<GLTran.refNbr>, And<AP.APTran.lineNbr, Equal<GLTran.tranLineNbr>, And<AP.APTran.tranType, Equal<GLTran.tranType>>>>>>();
				}
				else if (Base.BatchModule.Current.Module == GL.BatchModule.AR)
				{
					select.Join<LeftJoin<AR.ARTran, On<AR.ARTran.refNbr, Equal<GLTran.refNbr>, And<AR.ARTran.lineNbr, Equal<GLTran.tranLineNbr>, And<AR.ARTran.tranType, Equal<GLTran.tranType>>>>>>();
				}



				PXResultset<GLTran> resultset = select.Select();

				PMRegister doc = null;
				if (resultset.Count > 0)
				{
					doc = new PMRegister();
					doc.Module = Base.BatchModule.Current.Module;
					doc.Date = Base.BatchModule.Current.DateEntered;
					doc.Description = Base.BatchModule.Current.Description;
					doc.Released = true;
					doc.Status = PMRegister.status.Released;
					ProjectDocs.Insert(doc);
				}


				Dictionary<string, PMTask> tasksToAutoAllocate = new Dictionary<string, PMTask>();
				List<PMTran> sourceForAllocation = new List<PMTran>();

				foreach (PXResult res in resultset)
				{
					GLTran tran = (GLTran)res[0];
					Account acc = (Account)res[1];
					PMAccountGroup ag = (PMAccountGroup)res[2];
					PMProject project = (PMProject)res[3];
					PMTask task = (PMTask)res[4];
					PMTran origTran = (PMTran)res[5];
					AP.APTran apTran = null;
					AR.ARTran arTran = null;

					if (Base.BatchModule.Current.Module == GL.BatchModule.AP)
					{
						apTran = (AP.APTran)res[6];
					}
					else if (Base.BatchModule.Current.Module == GL.BatchModule.AR)
					{
						arTran = (AR.ARTran)res[6];
					}



					PMTran pmt = (PMTran)ProjectTrans.Cache.Insert();

					pmt.BranchID = tran.BranchID;
					pmt.AccountGroupID = acc.AccountGroupID;
					pmt.AccountID = tran.AccountID;
					pmt.SubID = tran.SubID;
					pmt.BAccountID = tran.ReferenceID; //CustomerLocation is lost.
													   //pmt.BatchNbr = tran.BatchNbr;
					pmt.Date = tran.TranDate;
					pmt.TranDate = tran.TranDate;
					pmt.Description = tran.TranDesc;
					pmt.FinPeriodID = tran.FinPeriodID;
					pmt.TranPeriodID = tran.TranPeriodID;
					pmt.InventoryID = tran.InventoryID ?? PM.PMInventorySelectorAttribute.EmptyInventoryID;
					pmt.OrigLineNbr = tran.LineNbr;
					pmt.OrigModule = tran.Module;
					pmt.OrigRefNbr = tran.RefNbr;
					pmt.OrigTranType = tran.TranType;
					pmt.ProjectID = tran.ProjectID;
					pmt.TaskID = tran.TaskID;
					pmt.CostCodeID = tran.CostCodeID;
					if (arTran != null)
					{
						//if this is an invoice transaction force it to be non-billable
						//so that it is not billed again even if the billing rule is configured to bill this account group.
						pmt.Billable = false;

					}
					else
						pmt.Billable = tran.NonBillable != true;

					if (apTran != null && apTran.Date != null)
					{
						pmt.Date = apTran.Date;
					}

					pmt.UseBillableQty = true;
					pmt.UOM = tran.UOM;

					pmt.Amount = tran.DebitAmt - tran.CreditAmt;

					CurrencyInfo projectCuryInfo = null;
					if (PXAccess.FeatureInstalled<FeaturesSet.projectMultiCurrency>())
					{
						pmt.TranCuryID = Base.BatchModule.Current.CuryID;
						pmt.ProjectCuryID = project.CuryID;
						pmt.BaseCuryInfoID = tran.CuryInfoID;
						pmt.TranCuryAmount = tran.CuryDebitAmt - tran.CuryCreditAmt;

						if (project.CuryID == Base.ledger.Current.BaseCuryID)
						{
							pmt.ProjectCuryInfoID = tran.CuryInfoID;
							pmt.ProjectCuryAmount = pmt.Amount;
						}
						else if (project.CuryID == Base.BatchModule.Current.CuryID)
						{
							projectCuryInfo = new CurrencyInfo();
							projectCuryInfo.ModuleCode = GL.BatchModule.PM;
							projectCuryInfo.BaseCuryID = project.CuryID;
							projectCuryInfo.CuryID = project.CuryID;
							projectCuryInfo.CuryRateTypeID = project.RateTypeID ?? Base.CMSetup.Current.PMRateTypeDflt;
							projectCuryInfo.CuryEffDate = tran.TranDate;
							projectCuryInfo.CuryRate = 1;
							projectCuryInfo.RecipRate = 1;
							projectCuryInfo = Base.currencyinfo.Insert(projectCuryInfo);
							pmt.ProjectCuryInfoID = projectCuryInfo.CuryInfoID;
							pmt.ProjectCuryAmount = pmt.TranCuryAmount;
						}
						else
						{
							PX.Objects.CM.Extensions.IPXCurrencyService currencyService = ServiceLocator.Current.GetInstance<Func<PXGraph, PX.Objects.CM.Extensions.IPXCurrencyService>>()(Base);

							projectCuryInfo = new CurrencyInfo();
							projectCuryInfo.ModuleCode = GL.BatchModule.PM;
							projectCuryInfo.BaseCuryID = project.CuryID;
							projectCuryInfo.CuryID = Base.BatchModule.Current.CuryID;
							projectCuryInfo.CuryRateTypeID = project.RateTypeID ?? currencyService.DefaultRateTypeID(GL.BatchModule.PM);
							projectCuryInfo.CuryEffDate = tran.TranDate;

							var rate = currencyService.GetRate(projectCuryInfo.CuryID, projectCuryInfo.BaseCuryID, projectCuryInfo.CuryRateTypeID, projectCuryInfo.CuryEffDate);
							if (rate == null)
							{
								throw new PXException(PM.Messages.FxTranToProjectNotFound, projectCuryInfo.CuryID, projectCuryInfo.BaseCuryID, projectCuryInfo.CuryRateTypeID, tran.TranDate);
							}

							projectCuryInfo = Base.currencyinfo.Insert(projectCuryInfo);
							pmt.ProjectCuryInfoID = projectCuryInfo.CuryInfoID;
							decimal val;
							PXCurrencyAttribute.CuryConvBase(Base.BatchModule.Cache, projectCuryInfo, pmt.TranCuryAmount.GetValueOrDefault(), out val);
							pmt.ProjectCuryAmount = val;
						}
					}
					else
					{
						pmt.TranCuryAmount = pmt.Amount;
						pmt.ProjectCuryAmount = pmt.Amount;
						pmt.TranCuryID = Base.ledger.Current.BaseCuryID;
						pmt.ProjectCuryID = Base.ledger.Current.BaseCuryID;

						if (Base.BatchModule.Current.CuryID != Base.ledger.Current.BaseCuryID)
						{
							CurrencyInfo baseCuryInfo = new CurrencyInfo();
							baseCuryInfo.ModuleCode = GL.BatchModule.PM;
							baseCuryInfo.BaseCuryID = Base.ledger.Current.BaseCuryID;
							baseCuryInfo.CuryID = Base.ledger.Current.BaseCuryID;
							baseCuryInfo.CuryRateTypeID = null;
							baseCuryInfo.CuryEffDate = tran.TranDate;
							baseCuryInfo.CuryRate = 1;
							baseCuryInfo.RecipRate = 1;
							baseCuryInfo = Base.currencyinfo.Insert(baseCuryInfo);
							pmt.ProjectCuryInfoID = baseCuryInfo.CuryInfoID;
							pmt.BaseCuryInfoID = baseCuryInfo.CuryInfoID;
						}
						else
						{
							pmt.ProjectCuryInfoID = tran.CuryInfoID;
							pmt.BaseCuryInfoID = tran.CuryInfoID;
						}
					}


					pmt.Qty = tran.Qty;//pmt.Amount >= 0 ? tran.Qty : (tran.Qty * -1);
					int sign = 1;
					if (acc.Type == AccountType.Income || acc.Type == AccountType.Liability)
					{
						sign = -1;
					}

					if (ProjectBalance.IsFlipRequired(acc.Type, ag.Type))
					{
						pmt.ProjectCuryAmount = -pmt.ProjectCuryAmount;
						pmt.TranCuryAmount = -pmt.TranCuryAmount;
						pmt.Amount = -pmt.Amount;
						pmt.Qty = -pmt.Qty;
					}
					pmt.BillableQty = pmt.Qty;
					pmt.Released = true;
					pmt.Allocated = tran.OrigPMTranID != null;// do not allocate or bill reverse transaction
					pmt.Billed = tran.OrigPMTranID != null;// do not allocate or bill reverse transaction

					Base.GLTranModuleBatNbr.SetValueExt<GLTran.pMTranID>(tran, pmt.TranID);

					if (apTran != null && apTran.NoteID != null)
					{
						PXNoteAttribute.CopyNoteAndFiles(Base.Caches[typeof(AP.APTran)], apTran, ProjectTrans.Cache, pmt);
					}
					else if (arTran != null && arTran.NoteID != null)
					{
						PXNoteAttribute.CopyNoteAndFiles(Base.Caches[typeof(AR.ARTran)], arTran, ProjectTrans.Cache, pmt);
					}

					ProjectBalance.Result balance = pb.Calculate(project, pmt, ag, acc.Type, sign, 1);

					if (balance.Status != null)
					{
						PMBudgetAccum ps = new PMBudgetAccum();
						ps.ProjectID = balance.Status.ProjectID;
						ps.ProjectTaskID = balance.Status.ProjectTaskID;
						ps.AccountGroupID = balance.Status.AccountGroupID;
						ps.InventoryID = balance.Status.InventoryID;
						ps.CostCodeID = balance.Status.CostCodeID;
						ps.UOM = balance.Status.UOM;
						ps.IsProduction = balance.Status.IsProduction;
						ps.Type = balance.Status.Type;
						ps.Description = balance.Status.Description;
						ps.CuryInfoID = balance.Status.CuryInfoID;

						ps = ProjectBudget.Insert(ps);
						ps.ActualQty += balance.Status.ActualQty.GetValueOrDefault();
						ps.CuryActualAmount += balance.Status.CuryActualAmount.GetValueOrDefault();
						ps.ActualAmount += balance.Status.ActualAmount.GetValueOrDefault();

						if (arTran != null && arTran.LineNbr != null && ag.Type == GL.AccountType.Income)
						{
							ps.CuryInvoicedAmount -= balance.Status.CuryActualAmount.GetValueOrDefault();
							ps.InvoicedAmount -= balance.Status.ActualAmount.GetValueOrDefault();
						}
					}

					if (balance.ForecastHistory != null)
					{
						PMForecastHistoryAccum forecast = new PMForecastHistoryAccum();
						forecast.ProjectID = balance.ForecastHistory.ProjectID;
						forecast.ProjectTaskID = balance.ForecastHistory.ProjectTaskID;
						forecast.AccountGroupID = balance.ForecastHistory.AccountGroupID;
						forecast.InventoryID = balance.ForecastHistory.InventoryID;
						forecast.CostCodeID = balance.ForecastHistory.CostCodeID;
						forecast.PeriodID = balance.ForecastHistory.PeriodID;

						forecast = ForecastHistory.Insert(forecast);

						forecast.ActualQty += balance.ForecastHistory.ActualQty.GetValueOrDefault();
						forecast.CuryActualAmount += balance.ForecastHistory.CuryActualAmount.GetValueOrDefault();
						forecast.ActualAmount += balance.ForecastHistory.ActualAmount.GetValueOrDefault();
					}

					if (balance.TaskTotal != null)
					{
						PMTaskTotal ta = new PMTaskTotal();
						ta.ProjectID = balance.TaskTotal.ProjectID;
						ta.TaskID = balance.TaskTotal.TaskID;

						ta = ProjectTaskTotals.Insert(ta);
						ta.CuryAsset += balance.TaskTotal.CuryAsset.GetValueOrDefault();
						ta.Asset += balance.TaskTotal.Asset.GetValueOrDefault();
						ta.CuryLiability += balance.TaskTotal.CuryLiability.GetValueOrDefault();
						ta.Liability += balance.TaskTotal.Liability.GetValueOrDefault();
						ta.CuryIncome += balance.TaskTotal.CuryIncome.GetValueOrDefault();
						ta.Income += balance.TaskTotal.Income.GetValueOrDefault();
						ta.CuryExpense += balance.TaskTotal.CuryExpense.GetValueOrDefault();
						ta.Expense += balance.TaskTotal.Expense.GetValueOrDefault();
					}

					PM.RegisterReleaseProcess.AddToUnbilledSummary(Base, pmt);

					//Create unbilled remainder if any.
					if (origTran != null && origTran.AccountID == tran.AccountID && origTran.Amount > tran.CreditAmt)
					{
						PMTran remainder = (PMTran)ProjectTrans.Cache.CreateCopy(origTran);
						remainder.TranID = null;
						remainder.Billed = false;
						remainder.TranType = null;
						remainder.RefNbr = null;
						remainder.ARRefNbr = null;
						remainder.ARTranType = null;
						remainder.RefLineNbr = null;
						remainder.OrigLineNbr = null;
						remainder.ProformaRefNbr = null;
						remainder.ProformaLineNbr = null;
						remainder.BatchNbr = null;
						remainder.RemainderOfTranID = origTran.TranID;
						remainder.TranCuryAmount = origTran.TranCuryAmount - tran.CuryCreditAmt;
						remainder.Amount = origTran.Amount - tran.CreditAmt;
						if (projectCuryInfo != null)
						{
							decimal val;
							PXCurrencyAttribute.CuryConvCury(Base.BatchModule.Cache, projectCuryInfo, origTran.Amount.GetValueOrDefault() - tran.CreditAmt.GetValueOrDefault(), out val);
							remainder.ProjectCuryAmount = val;
						}

						remainder.Qty = origTran.Qty - tran.Qty;
						remainder.BillableQty = remainder.Qty;
						remainder.NoteID = null;
						ProjectTrans.Insert(remainder);
						PM.RegisterReleaseProcess.AddToUnbilledSummary(Base, remainder);
					}

					sourceForAllocation.Add(pmt);
					if (pmt.Allocated != true && project.AutoAllocate == true)
					{
						if (!tasksToAutoAllocate.ContainsKey(string.Format("{0}.{1}", task.ProjectID, task.TaskID)))
						{
							tasksToAutoAllocate.Add(string.Format("{0}.{1}", task.ProjectID, task.TaskID), task);
						}
					}
				}

				autoAllocateTasks.AddRange(tasksToAutoAllocate.Values);
			}

			return autoAllocateTasks;
		}


		public virtual ProjectBalance CreateProjectBalance()
		{
			return new ProjectBalance(Base);
		}
	}
}
