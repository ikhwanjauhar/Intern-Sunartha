using PX.Data;
using PX.Common;
using PX.Objects.AP;
using PX.Objects.GL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PX.Api;
using System.Collections;
using PX.Data.SQLTree;
using System.Reflection;

namespace PX.Objects.TX
{
	public abstract partial class TaxBaseAttribute : PXAggregateAttribute,
													 IPXRowInsertedSubscriber,
													 IPXRowUpdatedSubscriber,
													 IPXRowDeletedSubscriber,
													 IPXRowPersistedSubscriber,
													 IComparable
	{
		private Type _uom;

		/// <summary>
		/// The document line's UOM field for taxes per unit and specific taxes.
		/// </summary>
		public Type UOM
		{
			get => _uom;
			set
			{
				CheckDocLineFieldTypes(value, docLinefieldName: nameof(UOM));
				_uom = value;
			}
		}

		private Type _taxableQty;

		/// <summary>
		/// The document line quantity field that will be used for taxes per unit and specific taxes calculation.
		/// </summary>
		public Type TaxableQty
		{
			get => _taxableQty;
			set
			{
				CheckDocLineFieldTypes(value, docLinefieldName: nameof(TaxableQty));
				_taxableQty = value;
			}
		}

		private void CheckDocLineFieldTypes(Type fieldTypeNewValue, string docLinefieldName)
		{
			fieldTypeNewValue.ThrowOnNull(nameof(fieldTypeNewValue));

			if (!typeof(IBqlField).IsAssignableFrom(fieldTypeNewValue))
			{
				throw new ArgumentException($"The {nameof(docLinefieldName)} should be a type implementing {nameof(IBqlField)}",
											nameof(fieldTypeNewValue));
			}
		}

		protected virtual bool ShouldTaxPerUnitBeUsed(PXCache cache, object row, Tax tax, TaxRev taxRev, TaxDetail taxDetail) =>
			TaxableQty != null && UOM != null;

		protected virtual decimal? GetTaxPerUnitAmount(PXCache cache, object row, Tax tax, TaxRev taxRev, TaxDetail taxDetail)
		{
			decimal? lineQuantity = GetTaxableQty(cache, row);
			string lineUOM = GetUOM(cache, row);

			if (lineQuantity == null || lineUOM == null)
			{
				return null;
			}


			throw new NotImplementedException();
		}

		protected virtual decimal? GetTaxableQty(PXCache cache, object docLine) =>
			TaxableQty != null
				? cache.GetValue(docLine, TaxableQty.Name) as decimal?
				: null;

		protected virtual string GetUOM(PXCache cache, object docLine) =>
			UOM != null
				? cache.GetValue(docLine, UOM.Name) as string
				: null;
	}
}
