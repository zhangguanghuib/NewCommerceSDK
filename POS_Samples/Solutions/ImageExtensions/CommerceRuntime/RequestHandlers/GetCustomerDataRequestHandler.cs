namespace Contoso.GasStationSample.CommerceRuntime.RequestHandlers
{
    using System.Threading.Tasks;
    using System.Transactions;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Data;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages;

    public sealed class GetCustomerDataRequestHandler : SingleAsyncRequestHandler<GetCustomerDataRequest>
    {
        // Need Disable this feature Dynamics.AX.Application.RetailCustomerSimpleOptimizedOrderManagementFeature
        protected override async Task<Response> Process(GetCustomerDataRequest request)
        {
            ThrowIf.Null(request, "request");
            
            if(request.QueryResultSettings == null)
            {
                request.QueryResultSettings = new QueryResultSettings(new ColumnSet(), new PagingInfo(10), new SortingInfo());
            }

            var customerSearchRealtimeRequest = new SearchCustomersRealtimeRequest(
                                                          request.AccountNumber,
                                                          request.QueryResultSettings);

            GlobalCustomer retGlobalCustomer = null;

            var realtimeServiceResponse = await request.RequestContext.ExecuteAsync<EntityDataServiceResponse<GlobalCustomer>>(customerSearchRealtimeRequest).ConfigureAwait(false);
            foreach(GlobalCustomer globalCustomer in realtimeServiceResponse.PagedEntityCollection)
            {
                if(globalCustomer.AccountNumber == request.AccountNumber)
                {
                    retGlobalCustomer = globalCustomer;
                    break;
                }
            }

            Customer customer = new Customer();
            if (retGlobalCustomer != null)
            {
                customer.AccountNumber = retGlobalCustomer.AccountNumber;
                customer.FirstName = retGlobalCustomer.FullName;
                customer.Email = retGlobalCustomer.Email;
                customer.Phone = retGlobalCustomer.Phone;
                customer.RecordId = retGlobalCustomer.RecordId;
            }

            return new SingleEntityDataServiceResponse<Customer>(customer);
        }
    }
}
