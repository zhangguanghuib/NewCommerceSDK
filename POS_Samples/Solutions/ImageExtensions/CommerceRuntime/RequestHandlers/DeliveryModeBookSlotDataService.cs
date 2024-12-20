

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
    using Microsoft.Dynamics.Commerce.Runtime.TransactionService;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel.DataContractSurrogates;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Contoso.GasStationSample.CommerceRuntime.Entities;

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

            ReadOnlyCollection<object> results;

            DlvModeBookSlotSearchCriteria searchCriteria = new DlvModeBookSlotSearchCriteria();
            searchCriteria.DlvModeCode = request.DlvModeCode;
            searchCriteria.PagingInfo = request.QueryResultSettings.Paging;
            //searchCriteria.Sorting = request.QueryResultSettings.Sorting;
            try
            {
                bool useJson = true; // Use JSON by deafult
                searchCriteria.SerializationFormat = useJson ? 1 /*Json*/ : 0 /*Xml*/;

                //InvokeExtensionMethodRealtimeRequest extensionRequest = new InvokeExtensionMethodRealtimeRequest(
                //    "contosoGetDlvModeBookSlotXml",
                //    SerializationHelper.SerializeObjectToXml(searchCriteria)
                //);

                //InvokeExtensionMethodRealtimeRequest extensionRequest = new InvokeExtensionMethodRealtimeRequest(
                //    "contosoGetDlvModeBookSlotJson",
                //     Newtonsoft.Json.JsonConvert.SerializeObject(searchCriteria)
                //);

                InvokeExtensionMethodRealtimeRequest extensionRequest = new InvokeExtensionMethodRealtimeRequest(
                    "contosoGetDlvModeBookSlotJson",
                    SerializationHelper.SerializeObjectToJson(searchCriteria)
                );

                InvokeExtensionMethodRealtimeResponse response = await request.RequestContext.ExecuteAsync<InvokeExtensionMethodRealtimeResponse>(extensionRequest).ConfigureAwait(false);

                results = response.Result;

                string resXmlValue = (string)results[0];
             
                IEnumerable<DlvModeBookSlot> dlvModeBookSlots;
                if (useJson)
                {
                    dlvModeBookSlots = SerializationHelper.DeserializeObjectDataContractFromJson<DlvModeBookSlot[]>(resXmlValue);                 
                }
                else
                {
                    dlvModeBookSlots =
                        SerializationHelper.DeserializeObjectDataContractFromXml<DlvModeBookSlot[]>(resXmlValue);
                }
                return new GetDlvModeBookSlotsResponse(dlvModeBookSlots.AsPagedResult());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
               
            }     
        }
    }
}
