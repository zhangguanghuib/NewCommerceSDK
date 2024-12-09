

namespace Contoso.GasStationSample.CommerceRuntime.RequestHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Data;
    using Microsoft.Dynamics.Commerce.Runtime.DataAccess.SqlServer;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages;
    using Contoso.GasStationSample.CommerceRuntime.Messages;
    using System.Collections.ObjectModel;
    using Contoso.GasStationSample.CommerceRuntime.TransactionService;
    using Microsoft.Dynamics.Commerce.Runtime.TransactionService.Serialization;

    public class DeliveryModeBookSlotDataService : IRequestHandlerAsync
    {
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[]
                {
                    typeof(GetDlvModeBookSlotsRequest),
                };
            }
        }

        public async Task<Response> Execute(Request request)
        {
            ThrowIf.Null(request, nameof(request));
            switch (request)
            {
                case GetDlvModeBookSlotsRequest getDlvModeBookSlotsRequest:
                    return await this.GetDlvModeBookSlots(getDlvModeBookSlotsRequest).ConfigureAwait(false);
                default:
                    throw new NotSupportedException($"Request '{request.GetType()}' is not supported.");
            }
        }

        private async Task<Response> GetDlvModeBookSlots(GetDlvModeBookSlotsRequest request)
        {
            ThrowIf.Null(request, "request");
            ThrowIf.Null(request.DlvModeCode, "request.DlvModeCode");

            DlvModeBookSlotSearchCriteria searchCriteria = new DlvModeBookSlotSearchCriteria(
                request.DlvModeCode,
                request.QueryResultSettings.Paging,
                request.QueryResultSettings.Sorting);

            //InvokeExtensionMethodRealtimeRequest extensionRequest = new InvokeExtensionMethodRealtimeRequest(
            //    "contosoGetDlvModeBookSlot",
            //    SerializationHelper.SerializeObjectToJson(searchCriteria)
            //);

            //InvokeExtensionMethodRealtimeResponse response = await request.RequestContext.ExecuteAsync<InvokeExtensionMethodRealtimeResponse>(extensionRequest).ConfigureAwait(false);

            //ReadOnlyCollection<object> results = response.Result;

            InvokeExtensionMethodRealtimeRequest extensionRequest = new InvokeExtensionMethodRealtimeRequest("SerialCheck", "123");
            InvokeExtensionMethodRealtimeResponse response = await request.RequestContext.ExecuteAsync<InvokeExtensionMethodRealtimeResponse>(extensionRequest).ConfigureAwait(false);
            ReadOnlyCollection<object> results = response.Result;

            string resValue = (string)results[0];

            return new GetDlvModeBookSlotsResponse(null);
        }
    }
}
