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
        using Commerce.Runtime.XZReportsFrance.Messages;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.Data;
        using Microsoft.Dynamics.Commerce.Runtime.DataAccess.SqlServer;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// The data request handler for custom France X/Z report data in SQLServer.
        /// </summary>
        public sealed class XZReportsFranceSqlServerDataService : IRequestHandlerAsync, ICountryRegionAware
        {
            // Parameter names
            private const string ChannelIdVariableName = "@bi_ChannelId";
            private const string TerminalIdVariableName = "@nvc_TerminalId";
            private const string StoreIdVariableName = "@nvc_StoreId";
            private const string DataAreaIdVariableName = "@nvc_DataAreaId";
            private const string ShiftIdVariableName = "@bi_ShiftId";

            private const string CalculateShiftDetailsFranceSprocName = "GETSHIFTDETAILSFRANCE";

            /// <summary>
            /// Gets the collection of supported request types by this handler.
            /// </summary>
            public IEnumerable<Type> SupportedRequestTypes
            {
                get
                {
                    return new[]
                    {
                        typeof(GetShiftDetailsFranceDataRequest),
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
            /// Gets the sales or payment transaction extension to be saved.
            /// </summary>
            /// <param name="request">The request message.</param>
            /// <returns>The response message.</returns>
            public async Task<Response> Execute(Request request)
            {
                ThrowIf.Null(request, "request");

                // The extension should do nothing If fiscal registration is enabled. XZReportsFrance sealed extension should be used.
                if (!string.IsNullOrEmpty(request.RequestContext.GetChannelConfiguration().FiscalRegistrationProcessId))
                {
                    return NotHandledResponse.Instance;
                }

                Response response = NullResponse.Instance;

                if (request is GetShiftDetailsFranceDataRequest)
                {
                    response = await this.GetShiftFranceDetailsAsync((GetShiftDetailsFranceDataRequest)request).ConfigureAwait(false);
                }
                else
                {
                    throw new NotSupportedException(string.Format("Request '{0}' is not supported.", request.GetType()));
                }

                return response;
            }

            /// <summary>
            /// Loads the shift transactions data.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>A single entity data service response.</returns>
            private async Task<SingleEntityDataServiceResponse<Shift>> GetShiftFranceDetailsAsync(GetShiftDetailsFranceDataRequest request)
            {
                ThrowIf.Null(request, "request");
                ThrowIf.Null(request.TerminalId, "request.TerminalId");

                ParameterSet parameters = new ParameterSet();
                parameters[ChannelIdVariableName] = request.RequestContext.GetPrincipal().ChannelId;
                parameters[DataAreaIdVariableName] = request.RequestContext.GetChannelConfiguration().InventLocationDataAreaId;
                parameters[StoreIdVariableName] = request.RequestContext.GetOrgUnit().OrgUnitNumber ?? string.Empty;
                parameters[TerminalIdVariableName] = request.TerminalId;
                parameters[ShiftIdVariableName] = request.ShiftId;

                Shift shift;
                using (var databaseContext = new SqlServerDatabaseContext(request.RequestContext))
                {
                    var result = await databaseContext.ExecuteStoredProcedureAsync<Shift, ShiftTaxLine>(CalculateShiftDetailsFranceSprocName, parameters, request.QueryResultSettings).ConfigureAwait(false);
                    shift = result.Item1.SingleOrDefault();
                    if (shift != null)
                    {
                        shift.TaxLines = result.Item2;
                    }

                    return new SingleEntityDataServiceResponse<Shift>(shift);
                }
            }
        }
    }
}
