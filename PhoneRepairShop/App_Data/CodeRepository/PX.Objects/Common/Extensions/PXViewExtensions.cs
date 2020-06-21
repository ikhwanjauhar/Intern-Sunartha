using System.Collections;
using System.Collections.Generic;
using PX.Data;


namespace PX.Objects.Common
{
	public static class PXViewExtensions
	{
		public static IEnumerable SelectExternal(this PXView view, ref int startRow, ref int totalRows)
		{
			IEnumerable list = SelectWithExternalParameters(view, ref startRow, ref totalRows);
			PXView.StartRow = 0;
			return list;
		}

		public static IEnumerable SelectExternal(this PXView view)
		{
			int startRow = 0;
			int totalRows = 0;

			IEnumerable list = SelectWithExternalParameters(view, ref startRow, ref totalRows);
			return list;
		}

		private static IEnumerable SelectWithExternalParameters(PXView view, ref int startRow, ref int totalRows)
		{
			IEnumerable list = view.Select(
				PXView.Currents,
				null,
				null,
				view.GetExternalSorts(),
				view.GetExternalDescendings(),
				view.GetExternalFilters(),
				ref startRow,
				PXView.MaximumRows,
				ref totalRows);
			return list;
		}
	}
}
