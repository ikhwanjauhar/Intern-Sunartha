using PX.Data;
using System;

namespace PX.Objects.FS
{
    public static class ServiceOrderHandlers
    {

        [Obsolete("If this method is empty on 2020r1 please delete it.")]
        public static void FSSODet_RowSelecting(PXCache cache, PXRowSelectingEventArgs e)
        {
        }

        public static void FSSODet_RowInserting(PXCache cache, PXRowInsertingEventArgs e)
        {
            if (e.Row == null)
            {
                return;
            }

            var row = (FSSODet)e.Row;

            if (row.LineRef == null)
            {
                row.LineRef = row.LineNbr.Value.ToString("0000");
            }
        }
    }
}
