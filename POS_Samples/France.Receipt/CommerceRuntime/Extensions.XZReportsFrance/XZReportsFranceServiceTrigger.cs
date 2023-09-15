using Microsoft.Dynamics.Commerce.Runtime;
using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using Microsoft.Dynamics.Commerce.Runtime.Messages;
using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contoso
{
    namespace Commerce.Runtime.XZReportsFrance
    {
        public class XZReportsFranceServiceTrigger : IRequestTriggerAsync
        {

            public IEnumerable<Type> SupportedRequestTypes
            {
                get
                {
                    return new[] { typeof(GetReceiptServiceRequest) };
                }
            }

            /// <summary>
            /// Pre trigger code.
            /// </summary>
            /// <param name="request">The request.</param>
            public async Task OnExecuting(Request request)
            {
                await Task.CompletedTask.ConfigureAwait(false);
            }

            /// <summary>
            /// Post trigger code.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <param name="response">The response.</param>
            public async Task OnExecuted(Request request, Response response)
            {
                if (request is GetReceiptServiceRequest getReceiptServiceRequest && response is GetReceiptServiceResponse getReceiptServiceResponse)
                {
                    RequestContext context = request.RequestContext;

                    if (context.GetChannelConfiguration().CountryRegionISOCode == CountryRegionISOCode.FR)
                    {
                        var shift = getReceiptServiceRequest.ShiftDetails;

                        foreach (Receipt receipt in getReceiptServiceResponse.Receipts)
                        {
                            if (receipt.ReceiptType == ReceiptType.XReport || receipt.ReceiptType == ReceiptType.ZReport)
                            {
                                await CustomizeXZReportAsync(receipt, context, shift).ConfigureAwait(false);
                            }
                        }
                    }
                }
            }

            private static async Task CustomizeXZReportAsync(Receipt receipt, RequestContext context, Shift shift)
            {
                if (receipt.ReceiptType == ReceiptType.XReport)
                {
                    await ShiftFranceCalculator.FillShiftFranceDetailsAsync(context, shift, shift.TerminalId, shift.ShiftId).ConfigureAwait(false);
                }

                // Sanitize text group separator. Trim trailing whitespaces and append new line.
                StringBuilder reportLayout = new StringBuilder(receipt.Body.TrimEnd());
                reportLayout.AppendLine();
                reportLayout.AppendLine();

                await ReceiptHelper.AppendFranceShiftDetailsAsync(context, shift, reportLayout).ConfigureAwait(false);

                receipt.Body = reportLayout.ToString();
            }
        }
    }
}
