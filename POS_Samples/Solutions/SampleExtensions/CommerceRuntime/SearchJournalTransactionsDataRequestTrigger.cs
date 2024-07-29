namespace GasStationSample.CommerceRuntime
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    public class SearchJournalTransactionsDataRequestTrigger : IRequestTriggerAsync
    {
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[] { typeof(SearchJournalTransactionsDataRequest) };
            }
        }

        public Task OnExecuted(Request request, Response response)
        {
            ThrowIf.Null(request, "request");
            ThrowIf.Null(response, "response");

            EntityDataServiceResponse<Transaction> searchJournalTransactions = (EntityDataServiceResponse<Transaction>)response;
            foreach (Transaction transaction in searchJournalTransactions.PagedEntityCollection.Results)
            {
                transaction.ExtensionProperties.Add(new CommerceProperty("CONTOSORETAILSEATNUMBER", 5));
                transaction.ExtensionProperties.Add(new CommerceProperty("CONTOSORETAILSERVERSTAFFID", "000130"));
            }

            return Task.CompletedTask;
        }

        public Task OnExecuting(Request request)
        {
            return Task.CompletedTask;
        }
    }
}
