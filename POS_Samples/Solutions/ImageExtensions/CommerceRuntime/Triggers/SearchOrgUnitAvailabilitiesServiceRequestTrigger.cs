using Microsoft.Dynamics.Commerce.Runtime;
using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
using Microsoft.Dynamics.Commerce.Runtime.Messages;
using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages;
using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.GasStationSample.CommerceRuntime.Triggers
{
    public class SearchOrgUnitAvailabilitiesServiceRequestTrigger : IRequestTriggerAsync
    {
        private const string rtsMethodName = "getInventoryStockByWareHouse";
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[] { typeof(SearchOrgUnitAvailabilitiesServiceRequest) };
            }
        }

        /// <summary>
        /// Post trigger code.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        public async Task OnExecuted(Request request, Response response)
        {
            ThrowIf.Null(request, "request");
            ThrowIf.Null(response, "response");

            if ((request is SearchOrgUnitAvailabilitiesServiceRequest) && (response is EntityDataServiceResponse<OrgUnitAvailability>))
            {
                var OrgUnitAvailabilitiesServiceResponse = response as EntityDataServiceResponse<OrgUnitAvailability>;

                PagedResult<OrgUnitAvailability> pagedResult = OrgUnitAvailabilitiesServiceResponse.PagedEntityCollection;
                foreach (OrgUnitAvailability item in pagedResult) 
                {
                    foreach (ItemAvailability itemAvailability in item.ItemAvailabilities)
                    {
                        InvokeExtensionMethodRealtimeRequest extensionRequest = new InvokeExtensionMethodRealtimeRequest(rtsMethodName, itemAvailability.ItemId, itemAvailability.InventoryLocationId);
                        InvokeExtensionMethodRealtimeResponse extensionResponse = await request.RequestContext.ExecuteAsync<InvokeExtensionMethodRealtimeResponse>(extensionRequest).ConfigureAwait(false);
                        ReadOnlyCollection<object> results = extensionResponse.Result;
                    }
                }
            }
            
            await Task.CompletedTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Pre trigger code
        /// </summary>
        /// <param name="request">The request.</param>
        public async Task OnExecuting(Request request)
        {
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
