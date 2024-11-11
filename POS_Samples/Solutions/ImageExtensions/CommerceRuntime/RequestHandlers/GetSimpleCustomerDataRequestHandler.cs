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


    public sealed class GetSimpleCustomerDataRequestHandler : SingleAsyncRequestHandler<GetSimpleCustomerDataRequest>
    {
        protected override async Task<Response> Process(GetSimpleCustomerDataRequest request)
        {
            ThrowIf.Null(request, "request");

            if (request.QueryResultSettings == null)
            {
                request.QueryResultSettings = new QueryResultSettings(new ColumnSet(), new PagingInfo(10), new SortingInfo());
            }

            var customerSearchRealtimeRequest = new SearchCustomersRealtimeRequest(
                                                          request.AccountNumber,
                                                          request.QueryResultSettings);

            GlobalCustomer retGlobalCustomer = null;

            var realtimeServiceResponse = await request.RequestContext.ExecuteAsync<EntityDataServiceResponse<GlobalCustomer>>(customerSearchRealtimeRequest).ConfigureAwait(false);
            foreach (GlobalCustomer globalCustomer in realtimeServiceResponse.PagedEntityCollection)
            {
                if (globalCustomer.AccountNumber == request.AccountNumber)
                {
                    retGlobalCustomer = globalCustomer;
                    break;
                }
            }

            SimpleCustomer simpleCustomer = new SimpleCustomer();
            if (retGlobalCustomer != null)
            {
                simpleCustomer.AccountNumber = retGlobalCustomer.AccountNumber;
                simpleCustomer.CustomerType  = retGlobalCustomer.CustomerType;
                simpleCustomer.PartyNumber   = retGlobalCustomer.PartyNumber;
                simpleCustomer.FirstName     = retGlobalCustomer.FullName;
                simpleCustomer.RecordId      = retGlobalCustomer.RecordId;
            }

            return new SingleEntityDataServiceResponse<SimpleCustomer>(simpleCustomer);
        }
    }
}
