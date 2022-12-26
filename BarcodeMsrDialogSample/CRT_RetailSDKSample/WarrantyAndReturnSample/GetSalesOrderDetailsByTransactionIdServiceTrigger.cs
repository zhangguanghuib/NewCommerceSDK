

namespace CRT_RetailSDKSample.WarrantyAndReturnSample
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

    public class GetSalesOrderDetailsByTransactionIdServiceTrigger : IRequestTriggerAsync
    {
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[] { typeof(GetSalesOrderDetailsByTransactionIdServiceRequest) };
            }
        }

        public async Task OnExecuted(Request request, Response response)
        {
            ThrowIf.Null(request, "request");
            ThrowIf.Null(response, "response");

            SalesOrder SalesOrder = ((GetSalesOrderDetailsServiceResponse)response).SalesOrder;

            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task OnExecuting(Request request)
        {
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
