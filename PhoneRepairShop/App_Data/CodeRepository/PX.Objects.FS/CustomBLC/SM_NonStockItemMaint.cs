﻿using System;
using System.Collections;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.IN;

namespace PX.Objects.FS
{
    public class SM_NonStockItemMaint : PXGraphExtension<NonStockItemMaint>
    {
        public static bool IsActive()
        {
            return PXAccess.FeatureInstalled<FeaturesSet.serviceManagementModule>();
        }

        private bool baseUnitChanged = false;

        #region Selects

        public class ServiceSkills_View : PXSelectJoin<FSServiceSkill, InnerJoin<FSSkill, On<FSSkill.skillID, Equal<FSServiceSkill.skillID>>>,
                Where<FSServiceSkill.serviceID, Equal<Current<InventoryItem.inventoryID>>>>
        {
            public ServiceSkills_View(PXGraph graph) : base(graph)
            {
            }

            public ServiceSkills_View(PXGraph graph, Delegate handler) : base(graph, handler)
            {
            }
        }

        public class ServiceLicenseTypes_View : PXSelectJoin<FSServiceLicenseType, InnerJoin<FSLicenseType, On<FSLicenseType.licenseTypeID, Equal<FSServiceLicenseType.licenseTypeID>>>,
                Where<FSServiceLicenseType.serviceID, Equal<Current<InventoryItem.inventoryID>>>>
        {
            public ServiceLicenseTypes_View(PXGraph graph) : base(graph)
            {
            }

            public ServiceLicenseTypes_View(PXGraph graph, Delegate handler) : base(graph, handler)
            {
            }
        }

        public class ServiceEquipmentTypes_View : PXSelectJoin<FSServiceEquipmentType, InnerJoin<FSEquipmentType, On<FSEquipmentType.equipmentTypeID, Equal<FSServiceEquipmentType.equipmentTypeID>>>,
                Where<FSServiceEquipmentType.serviceID, Equal<Current<InventoryItem.inventoryID>>>>
        {
            public ServiceEquipmentTypes_View(PXGraph graph) : base(graph)
            {
            }

            public ServiceEquipmentTypes_View(PXGraph graph, Delegate handler) : base(graph, handler)
            {
            }
        }

        public class ServiceInventoryItems_View : PXSelectJoin<FSServiceInventoryItem, InnerJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<FSServiceInventoryItem.inventoryID>>>,
                Where<FSServiceInventoryItem.serviceID, Equal<Current<InventoryItem.inventoryID>>>>
        {
            public ServiceInventoryItems_View(PXGraph graph) : base(graph)
            {
            }

            public ServiceInventoryItems_View(PXGraph graph, Delegate handler) : base(graph, handler)
            {
            }
        }

        public class ServiceVehicleTypes_View : PXSelectJoin<FSServiceVehicleType, 
                                                    InnerJoin<FSVehicleType, On<FSVehicleType.vehicleTypeID, Equal<FSServiceVehicleType.vehicleTypeID>>,
                                                    InnerJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<FSServiceVehicleType.serviceID>>>>,
                Where<FSServiceVehicleType.serviceID, Equal<Current<InventoryItem.inventoryID>>>>
        {
            public ServiceVehicleTypes_View(PXGraph graph) : base(graph)
            {
            }

            public ServiceVehicleTypes_View(PXGraph graph, Delegate handler) : base(graph, handler)
            {
            }
        }

        public ServiceSkills_View ServiceSkills;
        public ServiceLicenseTypes_View ServiceLicenseTypes;
        public ServiceEquipmentTypes_View ServiceEquipmentTypes;
        public ServiceInventoryItems_View ServiceInventoryItems;
        public ServiceVehicleTypes_View ServiceVehicleTypes;

        #endregion

        #region Virtual Methods
        /// <summary>
        /// Enable or Disable fields.
        /// </summary>
        public virtual void EnableDisable(PXCache cache, InventoryItem inventoryItemRow)
        {
            if (inventoryItemRow == null)
            {
                return;
            }

            FSSetup fsSetupRow = PXSelect<FSSetup>.Select(Base);

            var enabled = inventoryItemRow.ItemType == INItemTypes.ServiceItem;
            PXUIFieldAttribute.SetEnabled<FSxService.billingRule>(cache, inventoryItemRow, enabled);
            PXUIFieldAttribute.SetEnabled<FSxService.estimatedDuration>(cache, inventoryItemRow, enabled);

            ServiceSkills.Cache.AllowInsert = enabled;
            ServiceSkills.Cache.AllowDelete = enabled;
            ServiceSkills.Cache.AllowUpdate = enabled;

            ServiceLicenseTypes.Cache.AllowInsert = enabled;
            ServiceLicenseTypes.Cache.AllowDelete = enabled;
            ServiceLicenseTypes.Cache.AllowUpdate = enabled;

            ServiceEquipmentTypes.Cache.AllowInsert = enabled;
            ServiceEquipmentTypes.Cache.AllowDelete = enabled;
            ServiceEquipmentTypes.Cache.AllowUpdate = enabled;

            if (fsSetupRow != null)
            {
                bool activateEarningType = (bool)fsSetupRow.EnableEmpTimeCardIntegration;
                PXUIFieldAttribute.SetEnabled<FSxService.dfltEarningType>(cache, inventoryItemRow, activateEarningType);
            }
        }

        /// <summary>
        /// Assign the default Billing Rule set in the ItemClass.
        /// </summary>
        public virtual void SetDefaultBillingRule(PXCache cache, InventoryItem nonStockItemRow)
        {
            if (nonStockItemRow == null || nonStockItemRow.ItemClassID == null) 
            { 
                return; 
            }
            
            if (nonStockItemRow.ItemType == INItemTypes.ServiceItem)
            {
                INItemClass inItemClassRow = PXSelect<INItemClass,
                                             Where<
                                                INItemClass.itemClassID, Equal<Required<InventoryItem.itemClassID>>>>
                                             .Select(Base, nonStockItemRow.ItemClassID);

                FSxServiceClass fsxServiceClassRow = PXCache<INItemClass>.GetExtension<FSxServiceClass>(inItemClassRow);

                FSxService service = cache.GetExtension<FSxService>(nonStockItemRow);
                service.BillingRule = fsxServiceClassRow.DfltBillingRule;
            }
        }

        /// <summary>
        /// Set required and visible the PostClassID Field if the distribution module is installed in Acumatica.
        /// </summary>        
        public virtual void SetPostClassIDField(PXCache cache, InventoryItem inventoryItemRow)
        {
            bool isDistributionModuleInstalled = PXAccess.FeatureInstalled<FeaturesSet.distributionModule>();
            PXUIFieldAttribute.SetRequired<InventoryItem.postClassID>(cache, isDistributionModuleInstalled);
            PXUIFieldAttribute.SetVisible<InventoryItem.postClassID>(cache, inventoryItemRow, isDistributionModuleInstalled);
        }

        /// <summary>
        /// Enable/Disable the PickUp/Delivery Grid for a Service if the ActionType is No Items Related or another option.
        /// </summary>
        public virtual void EnableDisablePickUpDelivery(PXCache cache, InventoryItem inventoryItemRow)
        {
            FSxService fsxServiceRow = PXCache<InventoryItem>.GetExtension<FSxService>(inventoryItemRow);

            if (fsxServiceRow != null)
            {
                bool enable = fsxServiceRow.ActionType != ID.Service_Action_Type.NO_ITEMS_RELATED;

                ServiceInventoryItems.Cache.AllowInsert = enable;
                ServiceInventoryItems.Cache.AllowUpdate = enable;
                ServiceInventoryItems.Cache.AllowDelete = enable;
            }
        }

        /// <summary>
        /// Enable/Disable the ActionType field for a Service depending on the appointments related to that service.
        /// </summary>
        public virtual void EnableDisableActionType(PXCache cache, InventoryItem inventoryItemRow)
        {
            bool enableActionType = true;
            FSxService fsxServiceRow = PXCache<InventoryItem>.GetExtension<FSxService>(inventoryItemRow);

            if (cache.GetStatus(inventoryItemRow) != PXEntryStatus.Inserted)
            {
                int rowCount = PXSelect<FSAppointmentInventoryItem,
                               Where<
                                    FSAppointmentInventoryItem.lineType, Equal<ListField_LineType_Pickup_Delivery.Pickup_Delivery>,
                                    And<FSAppointmentInventoryItem.pickupDeliveryServiceID, Equal<Required<FSAppointmentInventoryItem.pickupDeliveryServiceID>>>>>
                               .SelectSingleBound(cache.Graph, null, inventoryItemRow.InventoryID).Count;

                enableActionType = rowCount == 0;
            }

            PXUIFieldAttribute.SetEnabled<FSxService.actionType>(cache, inventoryItemRow, enableActionType);

            if (enableActionType == false)
            {
                cache.RaiseExceptionHandling<FSxService.actionType>(
                                                                    inventoryItemRow,
                                                                    null,
                                                                    new PXSetPropertyException(
                                                                        PXMessages.LocalizeFormatNoPrefix(TX.Warning.CANNOT_MODIFY_FIELD, "Appoinments", "Service"),
                                                                        PXErrorLevel.Warning));
            }
        }

        public virtual void HideOrShowTabs(PXCache cache, InventoryItem inventoryItemRow)
        {
            bool isService = inventoryItemRow.ItemType == INItemTypes.ServiceItem;
            this.ServiceSkills.AllowSelect = isService;
            this.ServiceLicenseTypes.AllowSelect = isService;
            this.ServiceInventoryItems.AllowSelect = isService;
            this.ServiceEquipmentTypes.AllowSelect = isService;

            PXUIFieldAttribute.SetVisible<FSxService.actionType>(cache, inventoryItemRow, isService);
            PXUIFieldAttribute.SetVisible<FSxService.estimatedDuration>(cache, inventoryItemRow, isService);
            PXUIFieldAttribute.SetVisible<FSxService.billingRule>(cache, inventoryItemRow, isService);
            PXUIFieldAttribute.SetVisible<FSxServiceClass.mem_RouteService>(Base.ItemClass.Cache, Base.ItemClass.Current, isService);
        }
        #endregion

        #region Event Handlers
        #region InventoryItem Events
        protected virtual void InventoryItem_EstimatedDuration_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            if (e.Row == null)
            {
                return;
            }

            InventoryItem inventoryItemRow = (InventoryItem)e.Row;
            FSxService fsxServiceRow = PXCache<InventoryItem>.GetExtension<FSxService>(inventoryItemRow);

            PXSetPropertyException exception = null;

            if (fsxServiceRow.EstimatedDuration < 1)
            {
                exception = new PXSetPropertyException(TX.Warning.INVALID_SERVICE_DURATION, PXErrorLevel.Error);
            }

            cache.RaiseExceptionHandling<FSxService.estimatedDuration>(
                                                                       inventoryItemRow,
                                                                       fsxServiceRow.EstimatedDuration,
                                                                       exception);
        }

        protected virtual void InventoryItem_ItemClassID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            SetDefaultBillingRule(sender, (InventoryItem)e.Row);
        }

        protected virtual void InventoryItem_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            if (e.Row == null)
            {
                return;
            }

            InventoryItem inventoryItemRow = (InventoryItem)e.Row;

            EnableDisable(cache, inventoryItemRow);
            SetPostClassIDField(cache, inventoryItemRow);
            EnableDisablePickUpDelivery(cache, inventoryItemRow);
            EnableDisableActionType(cache, inventoryItemRow);
            HideOrShowTabs(cache, inventoryItemRow);
        }

        protected virtual void InventoryItem_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
        {
            if (e.Row == null)
            {
                return;
            }

            InventoryItem item = (InventoryItem)e.Row;
            InventoryItem oldItem = SharedFunctions.GetInventoryItemRow(cache.Graph, item.InventoryID);

            if (oldItem != null && e.Operation == PXDBOperation.Update
                   && item.BaseUnit?.Equals(oldItem.BaseUnit) == false
                       && SharedFunctions.IsServiceRelatedToAnyContract(cache, item.InventoryID) == true)
            {
                if (WebDialogResult.Yes == ServiceInventoryItems.Ask(TX.WebDialogTitles.CONFIRM_CHANGE_FSSALESPRICE_UOM, TX.Messages.ASK_CONFIRM_CHANGE_UOM_FSSALESPRICE, MessageButtons.YesNo))
                {
                    baseUnitChanged = true;
                }
            }

            if (item.ItemType == INItemTypes.ServiceItem)
            {
                FSxService service = cache.GetExtension<FSxService>(e.Row);

                if (service.EstimatedDuration == null || service.EstimatedDuration == 0)
                {
                    cache.RaiseExceptionHandling<FSxService.estimatedDuration>(
                                                                            e.Row,
                                                                            service.EstimatedDuration,
                                                                            new PXSetPropertyException(TX.Warning.INVALID_SERVICE_DURATION, PXErrorLevel.Error));
                }

                if (PXAccess.FeatureInstalled<FeaturesSet.distributionModule>())
                {
                    if (item.ItemClassID == null)
                    {
                        cache.RaiseExceptionHandling<InventoryItem.itemClassID>(
                                                                                e.Row,
                                                                                null,
                                                                                new PXSetPropertyException<InventoryItem.itemClassID>(TX.Error.FIELD_EMPTY, PXErrorLevel.Error));
                    }
                }
            }
        }

        public virtual void InventoryItem_RowPersisted(PXCache cache, PXRowPersistedEventArgs e)
        {
            if (e.Row == null)
            {
                return;
            }

            SharedFunctions.PropagateBaseUnitToContracts(cache, (InventoryItem)e.Row, e.TranStatus, e.Operation, baseUnitChanged);
        }
        #endregion
        #region FSServiceSkill Events
        protected virtual void FSServiceSkill_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            if (e.Row == null)
            {
                return;
            }

            FSServiceSkill fsServiceSkillRow = (FSServiceSkill)e.Row;
            PXUIFieldAttribute.SetEnabled<FSServiceSkill.skillID>
                (cache, fsServiceSkillRow, string.IsNullOrEmpty(fsServiceSkillRow.SkillID.ToString()));
        }
        #endregion
        #region FSServiceLicenseType Events
        protected virtual void FSServiceLicenseType_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            if (e.Row == null)
            {
                return;
            }

            FSServiceLicenseType fsServiceLicenseTypeRow = (FSServiceLicenseType)e.Row;
            PXUIFieldAttribute.SetEnabled<FSServiceLicenseType.licenseTypeID>
                (cache, fsServiceLicenseTypeRow, string.IsNullOrEmpty(fsServiceLicenseTypeRow.LicenseTypeID.ToString()));
        }
        #endregion
        #region FSServiceEquipmentType Events
        protected virtual void FSServiceEquipmentType_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            if (e.Row == null)
            {
                return;
            }

            FSServiceEquipmentType fsServiceEquipmentTypeRow = (FSServiceEquipmentType)e.Row;
            PXUIFieldAttribute.SetEnabled<FSServiceEquipmentType.equipmentTypeID>
                (cache, fsServiceEquipmentTypeRow, string.IsNullOrEmpty(fsServiceEquipmentTypeRow.EquipmentTypeID.ToString()));
        }
        #endregion
        #endregion
    }
}
