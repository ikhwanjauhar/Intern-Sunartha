using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.CS;

namespace PX.Objects.FS
{
    public class SM_VendorMaint : PXGraphExtension<VendorMaint>
    {
        public static bool IsActive()
        {
            return PXAccess.FeatureInstalled<FeaturesSet.serviceManagementModule>();
        }

        #region VendorEventHandlers
        protected virtual void Vendor_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
        {
            if (e.Row == null)
            {
                return;
            }

            var vendorRow = (Vendor)e.Row;
            FSxVendor fsxVendorRow = cache.GetExtension<FSxVendor>(vendorRow);

            if (e.Operation != PXDBOperation.Delete)
            {
                LicenseHelper.CheckStaffMembersLicense(cache.Graph, vendorRow.BAccountID, fsxVendorRow.SDEnabled, vendorRow.Status);
            }
        }
        #endregion  
    }
}