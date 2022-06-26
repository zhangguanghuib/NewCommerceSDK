namespace Contoso
{
    namespace Commerce.Runtime.StoreHoursSample
    {
        using System;
        using System.Collections.Generic;
        using System.Collections.ObjectModel;
        using System.Globalization;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.Data;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages;
        using StoreHoursSample.Messages;

        /// <summary>
        /// Sample service to demonstrate returning an array of a new entity.
        /// </summary>
        public class StoreHoursDataService : IRequestHandlerAsync
        {
            /// <summary>
            /// Gets the collection of supported request types by this handler.
            /// </summary>
            public IEnumerable<Type> SupportedRequestTypes
            {
                get
                {
                    return new[]
                    {
                        typeof(GetStoreHoursDataRequest),
                        typeof(UpdateStoreDayHoursDataRequest),
                    };
                }
            }

            /// <summary>
            /// Entry point to StoreHoursDataService service.
            /// </summary>
            /// <param name="request">The request to execute.</param>
            /// <returns>Result of executing request, or null object for void operations.</returns>
            public async Task<Response> Execute(Request request)
            {
                if (request == null)
                {
                    throw new ArgumentNullException("request");
                }

                Type reqType = request.GetType();
                if (reqType == typeof(GetStoreHoursDataRequest))
                {
                    return await this.GetStoreDayHoursAsync((GetStoreHoursDataRequest)request).ConfigureAwait(false);
                }
                else if (reqType == typeof(UpdateStoreDayHoursDataRequest))
                {
                    return await this.UpdateStoreDayHoursAsync((UpdateStoreDayHoursDataRequest)request).ConfigureAwait(false);
                }
                else
                {
                    string message = string.Format(CultureInfo.InvariantCulture, "Request '{0}' is not supported.", reqType);
                    Console.WriteLine(message);
                    throw new NotSupportedException(message);
                }
            }

            private async Task<Response> GetStoreDayHoursAsync(GetStoreHoursDataRequest request)
            {
                ThrowIf.Null(request, "request");

                using (DatabaseContext databaseContext = new DatabaseContext(request.RequestContext))
                {
                    var query = new SqlPagedQuery(request.QueryResultSettings)
                    {
                        DatabaseSchema = "ext",
                        Select = new ColumnSet("DAY", "OPENTIME", "CLOSINGTIME", "RECID"),
                        From = "CONTOSORETAILSTOREHOURSVIEW",
                        Where = "STORENUMBER = @storeNumber",
                    };

                    query.Parameters["@storeNumber"] = request.StoreNumber;
                    return new GetStoreHoursDataResponse(await databaseContext.ReadEntityAsync<DataModel.StoreDayHours>(query).ConfigureAwait(false));
                }
            }

            private async Task<Response> UpdateStoreDayHoursAsync(UpdateStoreDayHoursDataRequest request)
            {
                ThrowIf.Null(request, "request");
                ThrowIf.Null(request.StoreDayHours, "request.StoreDayHours");
                if (request.StoreDayHours.DayOfWeek < 1 || request.StoreDayHours.DayOfWeek > 7)
                {
                    throw new DataValidationException(DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_ValueOutOfRange);
                }

                InvokeExtensionMethodRealtimeRequest extensionRequest = new InvokeExtensionMethodRealtimeRequest(
                    "ContosoRetailStoreHours_UpdateStoreHours",
                    request.StoreDayHours.Id,
                    request.StoreDayHours.DayOfWeek,
                    request.StoreDayHours.OpenTime,
                    request.StoreDayHours.CloseTime);
                InvokeExtensionMethodRealtimeResponse response = await request.RequestContext.ExecuteAsync<InvokeExtensionMethodRealtimeResponse>(extensionRequest).ConfigureAwait(false);
                ReadOnlyCollection<object> results = response.Result;

                long recId = Convert.ToInt64(results[0]);

                using (var databaseContext = new DatabaseContext(request.RequestContext))
                {
                    ParameterSet parameters = new ParameterSet();
                    parameters["@bi_Id"] = recId;
                    parameters["@i_Day"] = request.StoreDayHours.DayOfWeek;
                    parameters["@i_OpenTime"] = request.StoreDayHours.OpenTime;
                    parameters["@i_ClosingTime"] = request.StoreDayHours.CloseTime;
                    await databaseContext.ExecuteStoredProcedureNonQueryAsync("[ext].UPDATESTOREDAYHOURS", parameters, resultSettings: null).ConfigureAwait(false);
                }

                return new UpdateStoreDayHoursDataResponse(request.StoreDayHours);
            }
        }
    }
}