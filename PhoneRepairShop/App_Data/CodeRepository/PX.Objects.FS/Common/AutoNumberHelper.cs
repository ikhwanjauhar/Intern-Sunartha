using PX.Data;
using PX.Objects.CS;

namespace PX.Objects.FS
{
    public static class AutoNumberHelper
    {
        public static void CheckAutoNumbering(PXGraph graph, string numberingID)
        {
            Numbering numbering = null;

            if (numberingID != null)
            {
                numbering = (Numbering)PXSelect<Numbering,
                                Where<Numbering.numberingID, Equal<Required<Numbering.numberingID>>>>
                                .Select(graph, numberingID);
            }

            if (numbering == null)
            {
                throw new PXSetPropertyException(CS.Messages.NumberingIDNull);
            }

            if (numbering.UserNumbering == true)
            {
                throw new PXSetPropertyException(CS.Messages.CantManualNumber, numbering.NumberingID);
            }
        }
    }
}
