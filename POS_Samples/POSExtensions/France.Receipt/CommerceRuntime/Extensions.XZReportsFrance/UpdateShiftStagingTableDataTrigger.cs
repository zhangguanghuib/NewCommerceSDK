/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

namespace Contoso
{
    namespace Commerce.Runtime.XZReportsFrance
    {
        using System;
        using System.Collections.Generic;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.Data;
        using Microsoft.Dynamics.Commerce.Runtime.DataAccess.SqlServer;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// The extended service to execute custom action before/after updating shift staging table.
        /// </summary>
        public class UpdateShiftStagingTableDataTrigger : IRequestTriggerAsync, ICountryRegionAware
        {
            // Parameter names
            private const string TerminalIdParameterName = "@nvc_TerminalId";
            private const string StoreIdParameterName = "@nvc_StoreId";
            private const string DataAreaIdParameterName = "@nvc_DataAreaId";
            private const string ShiftSalesCounterParameterName = "@nu_ShiftSalesCounterValue";
            private const string ShiftReturnsCounterParameterName = "@nu_ShiftReturnsCounterValue";

            private const string UpsertGrandTotalsSprocName = "UPSERTGRANDTOTALS";

            /// <summary>
            /// Gets the supported requests for this trigger.
            /// </summary>
            public IEnumerable<Type> SupportedRequestTypes
            {
                get
                {
                    return new[]
                    {
                        typeof(UpdateShiftStagingTableDataRequest),
                    };
                }
            }

            /// <summary>
            /// Gets a collection of companies supported by this request handler.
            /// </summary>
            public IEnumerable<string> SupportedCountryRegions
            {
                get
                {
                    return new[]
                    {
                        nameof(CountryRegionISOCode.FR),
                    };
                }
            }

            /// <summary>
            /// Runs pre trigger logic.
            /// </summary>
            /// <param name="request">The request.</param>
            public async Task OnExecuting(Request request)
            {
                ThrowIf.Null(request, "request");

                Type requestedType = request.GetType();

                // The extension should do nothing If fiscal registration is enabled. XZReportsFrance sealed extension should be used.
                if (requestedType == typeof(UpdateShiftStagingTableDataRequest) &&
                    string.IsNullOrEmpty(request.RequestContext.GetChannelConfiguration().FiscalRegistrationProcessId))
                {
                    UpdateShiftStagingTableDataRequest updateShiftStagingTableDataRequest = request as UpdateShiftStagingTableDataRequest;
                    await this.FillShiftFranceDetailsAsync(updateShiftStagingTableDataRequest).ConfigureAwait(false);
                }
            }

            /// <summary>
            /// Runs post trigger logic.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <param name="response">The response.</param>
            public async Task OnExecuted(Request request, Response response)
            {
                ThrowIf.Null(request, "request");

                Type requestedType = request.GetType();

                // The extension should do nothing If fiscal registration is enabled. XZReportsFrance sealed extension should be used.
                if (requestedType == typeof(UpdateShiftStagingTableDataRequest) &&
                    string.IsNullOrEmpty(request.RequestContext.GetChannelConfiguration().FiscalRegistrationProcessId))
                {
                    UpdateShiftStagingTableDataRequest updateShiftStagingTableDataRequest = request as UpdateShiftStagingTableDataRequest;
                    await this.UpdateOrInsertShiftGrandTotalsAsync(updateShiftStagingTableDataRequest).ConfigureAwait(false);
                }
            }

            /// <summary>
            /// Update the shift entity with France-specific details.
            /// </summary>
            /// <param name="request">The update shift staging table data request.</param>
            private async Task FillShiftFranceDetailsAsync(UpdateShiftStagingTableDataRequest request)
            {
                ThrowIf.Null(request.Shift, "request.Shift");

                await ShiftFranceCalculator.FillShiftFranceDetailsAsync(request.RequestContext, request.Shift, request.Shift.TerminalId, request.Shift.ShiftId).ConfigureAwait(false);
            }

            /// <summary>
            /// Update the shift entity with France-specific details.
            /// </summary>
            /// <param name="request">The update shift staging table data request.</param>
            private async Task UpdateOrInsertShiftGrandTotalsAsync(UpdateShiftStagingTableDataRequest request)
            {
                ThrowIf.Null(request.Shift, "request.Shift");
                var shift = request.Shift;

                if (shift.Status == ShiftStatus.Closed)
                {
                    var parameters = new ParameterSet();
                    parameters[DataAreaIdParameterName] = request.RequestContext.GetChannelConfiguration().InventLocationDataAreaId;
                    parameters[StoreIdParameterName] = shift.StoreId;
                    parameters[TerminalIdParameterName] = shift.TerminalId;
                    parameters[ShiftSalesCounterParameterName] = shift.ShiftSalesTotal;
                    parameters[ShiftReturnsCounterParameterName] = shift.ShiftReturnsTotal;

                    int errorCode;
                    using (var databaseContext = new SqlServerDatabaseContext(request.RequestContext))
                    {
                        errorCode = await databaseContext.ExecuteStoredProcedureScalarAsync(UpsertGrandTotalsSprocName, parameters, request.QueryResultSettings).ConfigureAwait(false);
                    }

                    if (errorCode != (int)DatabaseErrorCodes.Success)
                    {
                        throw new StorageException(
                            StorageErrors.Microsoft_Dynamics_Commerce_Runtime_CriticalStorageError,
                            errorCode,
                            string.Format("Unable to execute the stored procedure {0}.", UpsertGrandTotalsSprocName));
                    }
                }
            }
        }
    }
}
