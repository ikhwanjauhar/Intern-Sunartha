using PX.Api;
using PX.Api.ContractBased;
using PX.Api.ContractBased.Models;
using PX.Data;
using System.Linq;
using System;
using PX.Data.BQL.Fluent;
using PX.SM;
using PX.Data.BQL;

namespace PX.Objects.EndpointAdapters
{
	[PXVersion("19.200.001", "DeviceHub")]
	public class DeviceHubEndpoint19 : DefaultEndpointImpl
	{
		[FieldsProcessed(new[] {
			"JobID",
			"Printer",
			"ReportID",
			"Status"
		})]
		protected void PrintJob_Update(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
		{
			var status = targetEntity.Fields.SingleOrDefault(f => f.Name == "Status") as EntityValueField;
			int jobID;
			int.TryParse(entity.InternalKeys["Job"]["JobID"], out jobID);

			if (jobID != 0 && status != null && status.Value != null)
			{
				foreach (PX.SM.SMPrintJob existingPrintJob in PXSelect<PX.SM.SMPrintJob, Where<PX.SM.SMPrintJob.jobID, Equal<Required<PX.SM.SMPrintJob.jobID>>>>.Select(graph, jobID))
				{
					((PX.SM.SMPrintJobMaint)graph).Job.SetValueExt<SMPrintJob.status>(existingPrintJob, status.Value);
				}
			}
		}

	}
}