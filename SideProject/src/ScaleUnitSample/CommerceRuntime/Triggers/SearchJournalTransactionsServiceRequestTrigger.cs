namespace GasStationSample.CommerceRuntime
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

    public class SearchJournalTransactionsServiceRequestTrigger : IRequestTriggerAsync
    {
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[] { typeof(SearchJournalTransactionsServiceRequest) };
            }
        }


        public Task OnExecuted(Request request, Response response)
        {
            ThrowIf.Null(request, "request");
            ThrowIf.Null(response, "response");
            List<string> transIds = new List<string>();

            SearchJournalTransactionsServiceResponse searchJournalTransactionsResponse = (SearchJournalTransactionsServiceResponse)response;
            
            foreach (Transaction transaction in searchJournalTransactionsResponse.Transactions)
            {
                transIds.Add(transaction.Id);

                transaction.ExtensionProperties.Add(new CommerceProperty("QRCode", "https://test-api-open.chinaums.com/fapiao-portal/view/index.html#/invoicedetail?qrCodeId=20240816af3dbb9374fa4fa5babecf7c944089ca"));
                transaction.ExtensionProperties.Add(new CommerceProperty("QRCodeId", "Inv0001"));
            }

            return Task.CompletedTask;
        }

        public Task OnExecuting(Request request)
        {
            return Task.CompletedTask;
        }


    }
}
