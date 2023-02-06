using Microsoft.Dynamics.Commerce.Runtime;
using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using Microsoft.Dynamics.Commerce.Runtime.Messages;
using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GHZ.BarcodeMsrDialogSample.CommerceRuntime.Triggers
{
    public class GetCustomersServiceRequestTrigger : IRequestTriggerAsync
    {
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[] { typeof(GetCustomersServiceRequest) };
            }
        }

        public async Task OnExecuted(Request request, Response response)
        {
            GetCustomersServiceResponse getCustomersServiceResponse = response as GetCustomersServiceResponse;
            PagedResult<Customer> customers = getCustomersServiceResponse.Customers;
            Customer customer= customers.FirstOrDefault();

            if(customer != null && String.IsNullOrWhiteSpace(customer.ReceiptEmail) && String.IsNullOrWhiteSpace(customer.Email))
            {
                customer.ReceiptEmail = "guanghui01@microsoft.com";
            }

            await Task.FromResult(getCustomersServiceResponse).ConfigureAwait(false);
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
