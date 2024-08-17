namespace CommerceRuntime.Triggers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
    public class GetSalesOrderDetailsByTransactionIdServiceRequestTrigger : IRequestTriggerAsync
    {
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[] { typeof(GetSalesOrderDetailsByTransactionIdServiceRequest) };
            }
        }

        public async  Task OnExecuted(Request request, Response response)
        {
            ThrowIf.Null(request, "request");
            ThrowIf.Null(response, "response");

            GetSalesOrderDetailsServiceResponse getSalesOrderDetailsServiceResponse = (GetSalesOrderDetailsServiceResponse)response;
            GetSalesOrderDetailsByTransactionIdServiceRequest getSalesOrderDetailsByTransactionIdServiceRequest = (GetSalesOrderDetailsByTransactionIdServiceRequest)request;
            if (getSalesOrderDetailsServiceResponse.SalesOrder != null)
            {
                getSalesOrderDetailsServiceResponse.SalesOrder.ExtensionProperties.Add(
                    new CommerceProperty("QRCode",
                    await GetQrCodeByTransactionId(getSalesOrderDetailsByTransactionIdServiceRequest.TransactionId).ConfigureAwait(false)));
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }

        private async Task<String> GetQrCodeByTransactionId(string transactionId)
        {
            return await Task.FromResult("https://www.bing.com").ConfigureAwait(false); ;
        }

        public Task OnExecuting(Request request)
        {
            return Task.CompletedTask;
        }
    }
}
