using Microsoft.Dynamics.Commerce.Runtime;
using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
using Microsoft.Dynamics.Commerce.Runtime.Messages;
using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace GHZ.BarcodeMsrDialogSample.CommerceRuntime.Triggers
{
    public class GetReasonCodesServiceRequestTrigger : IRequestTriggerAsync
    {
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[] { typeof(GetReasonCodesServiceRequest) };
            }
        }

        public async Task OnExecuted(Request request, Response response)
        {
            GetReasonCodesServiceResponse getReasonCodesServiceResponse = response as GetReasonCodesServiceResponse;
            PagedResult<ReasonCode> reasonCodes = getReasonCodesServiceResponse.ReasonCodes;
            foreach (ReasonCode reasonCode in reasonCodes)
            {
                if (reasonCode.ReasonCodeId.Equals("MatchPrice"))
                {
                    reasonCode.ExtensionProperties.Add(new CommerceProperty("BeginWith", "LOY"));
                }
            }

            await Task.FromResult(getReasonCodesServiceResponse).ConfigureAwait(false);
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
