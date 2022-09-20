using Microsoft.Dynamics.Commerce.Runtime;
using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
using Microsoft.Dynamics.Commerce.Runtime.Messages;
using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHZ.StoreHoursSample.CommerceRuntime.RequestHandlers
{
    public sealed class GetCustomersServiceRequestHandler : SingleAsyncRequestHandler<GetCustomersServiceRequest>
    {
        protected override async Task<Response> Process(GetCustomersServiceRequest request)
        {
            GetCustomersServiceResponse response;

            // When scan customer account in Numpad, the below logic will be called.
            var requestHandler = request.RequestContext.Runtime.GetNextAsyncRequestHandler(request.GetType(), this);
            response = await request.RequestContext.Runtime.ExecuteAsync<GetCustomersServiceResponse>(request, request.RequestContext, requestHandler, false).ConfigureAwait(false);

            // If scan customer account in Numpad, there are customer returned, then call API again to see if customer is availale in the current store.
            if(response.Customers.SingleOrDefault() != null)
            {
                var customerSearchCriteria = new CustomerSearchCriteria()
                {
                    Keyword = request.CustomerAccountNumbers.SingleOrDefault(),
                    SearchLocation = SearchLocation.Local,
                };

                CustomersSearchRequest customersSearchRequest = new CustomersSearchRequest(customerSearchCriteria, QueryResultSettings.AllRecords);
                CustomersSearchResponse customersSearchResponse = await request.RequestContext.Runtime.ExecuteAsync<CustomersSearchResponse>(customersSearchRequest, request.RequestContext).ConfigureAwait(false);
                
                // If the customer is not in current store, the return null for this request.
                if (customersSearchResponse.Customers.Count() <= 0)
                {
                    response = new GetCustomersServiceResponse(null);
                }
            }

            return response;
        }
    }
}
