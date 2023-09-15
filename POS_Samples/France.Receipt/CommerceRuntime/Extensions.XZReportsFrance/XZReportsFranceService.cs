/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

namespace Contoso
{
    namespace Commerce.Runtime.XZReportsFrance
    {
        using System.Text;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Services;
        using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

        /// <summary>
        /// The extended service to process custom French X/Z report data.
        /// </summary>
        public class XZReportsFranceService : SingleAsyncRequestHandler<GetReceiptServiceRequest>
        {
            /// <summary>
            /// Executes the request to get customized French X/Z reports.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <returns>The response.</returns>
            protected override async Task<Response> Process(GetReceiptServiceRequest request)
            {
                ThrowIf.Null(request, "request");

                // The extension should do nothing If fiscal registration is enabled. XZReportsFrance sealed extension should be used.
                if (!string.IsNullOrEmpty(request.RequestContext.GetChannelConfiguration().FiscalRegistrationProcessId))
                {
                    return NotHandledResponse.Instance;
                }

                return await this.GetXAndZReportReceiptAsync(request).ConfigureAwait(false);
            }

            /// <summary>
            /// Gets customized French X/Z reports.
            /// </summary>
            /// <param name="request">The request to get receipts.</param>
            /// <returns>The response containing customized X/Z reports.</returns>
            protected virtual async Task<GetReceiptServiceResponse> GetXAndZReportReceiptAsync(GetReceiptServiceRequest request)
            {
                ThrowIf.Null(request, "request");
                ThrowIf.Null(request.RequestContext, "request.RequestContext");

                RequestContext context = request.RequestContext;

                ReceiptService receiptService = new ReceiptService();
                GetReceiptServiceResponse originalReceiptsResponse = await request.RequestContext.Runtime.ExecuteAsync<GetReceiptServiceResponse>(request, request.RequestContext, receiptService, skipRequestTriggers: false).ConfigureAwait(false);

                if (context.GetChannelConfiguration().CountryRegionISOCode == CountryRegionISOCode.FR)
                {
                    var shift = request.ShiftDetails;

                    foreach (Receipt receipt in originalReceiptsResponse.Receipts)
                    {
                        if (receipt.ReceiptType == ReceiptType.XReport || receipt.ReceiptType == ReceiptType.ZReport)
                        {
                            await CustomizeXZReportAsync(receipt, context, shift).ConfigureAwait(false);
                        }
                    }
                }

                return originalReceiptsResponse;
            }

            /// <summary>
            /// Customizes X/Z reports with France-specific fields.
            /// </summary>
            /// <param name="receipt">The X/Z receipt to customize.</param>
            /// <param name="context">The request context.</param>
            /// <param name="shift">The shift related to X/Z report.</param>
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
