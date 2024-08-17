namespace Contoso
{
    namespace Commerce.Runtime.ReceiptsSample
    {
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Text;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Services;
        using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

        /// <summary>
        /// The extended service to get custom receipts.
        /// </summary>
        public class CustomReceiptService : IRequestHandlerAsync
        {
            /// <summary>
            /// Gets the collection of supported request types by this handler.
            /// </summary>
            public IEnumerable<Type> SupportedRequestTypes
            {
                get
                {
                    return new[]
                    {
                        typeof(GetReceiptServiceRequest),
                        typeof(GetEmailReceiptServiceRequest),
                    };
                }
            }

            /// <summary>
            /// Executes the custom receipt requests.
            /// </summary>
            /// <param name="request">The request to get receipts.</param>
            /// <returns>The response containing customized receipts.</returns>
            public async Task<Response> Execute(Request request)
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                Type requestedType = request.GetType();

                if (requestedType == typeof(GetReceiptServiceRequest))
                {
                    return await this.GetCustomReceiptsAsync((GetReceiptServiceRequest)request).ConfigureAwait(false);
                }
                else if (requestedType == typeof(GetEmailReceiptServiceRequest))
                {
                    return await this.GetCustomEmailReceiptsAsync((GetEmailReceiptServiceRequest)request).ConfigureAwait(false);
                }

                throw new NotSupportedException(string.Format("Request '{0}' is not supported.", request.GetType()));
            }

            /// <summary>
            /// The extended service to get custom receipt. This is a example of how to customize X/Z report.
            /// </summary>
            /// <param name="request">The service request to get custom receipt.</param>
            /// <returns>The value of custom receipt with customized X/Z report.</returns>
            private async Task<Response> GetCustomReceiptsAsync(GetReceiptServiceRequest request)
            {
                // 1. First we need to get the original receipts.
                //ReceiptService receiptService = new ReceiptService();

                var requestHandler = request.RequestContext.Runtime.GetNextAsyncRequestHandler(request.GetType(), this);

                GetReceiptServiceResponse originalReceiptsResponse = await request.RequestContext.Runtime.ExecuteAsync<GetReceiptServiceResponse>(request, request.RequestContext, requestHandler, skipRequestTriggers: false).ConfigureAwait(false);

                // 2. Find the X/Z report from the original receipts.
                foreach (Receipt receipt in originalReceiptsResponse.Receipts)
                {
                    if (receipt.ReceiptType == ReceiptType.XReport)
                    {
                        this.CustomizeXReport(receipt);
                    }
                    else if (receipt.ReceiptType == ReceiptType.ZReport)
                    {
                        this.CustomizeZReport(receipt);
                    }
                }

                return await Task.FromResult<Response>(originalReceiptsResponse).ConfigureAwait(false);
            }

            /// <summary>
            /// The extended service to get custom email receipts. This is an example of how to add custom receipts to the list of email receipts.
            /// </summary>
            /// <param name="request">The service request to get custom email receipts.</param>
            /// <remarks>
            /// When adding support for new receipt type here, please make sure to mark the new receipt type as email compatible by updating 'RetailReceiptTypeConfiguration' table in HQ.
            /// To update, override 'populateRetailReceiptTypeConfigurationTable' method on the table and use Commerce Parameters -> General -> Initialize.
            /// </remarks>
            private async Task<Response> GetCustomEmailReceiptsAsync(GetEmailReceiptServiceRequest request)
            {
                // 1. First we need to get the original email receipts.
                //ReceiptService receiptService = new ReceiptService();

                var requestHandler = request.RequestContext.Runtime.GetNextAsyncRequestHandler(request.GetType(), this);

                GetEmailReceiptServiceResponse emailReceiptsResponse = await request.RequestContext.Runtime.ExecuteAsync<GetEmailReceiptServiceResponse>(
                    request, request.RequestContext, requestHandler, skipRequestTriggers: false).ConfigureAwait(false);

                List<Receipt> receipts = emailReceiptsResponse.Receipts.ToList();
                ReceiptType receiptType = request.ReceiptTypes.ToList()[0];

                // 2. Build the request to retrieve the custom receipts
                ReceiptRetrievalCriteria retrievalCriteria = new ReceiptRetrievalCriteria();
                GetCustomReceiptsRequest getCustomReceiptsRequest = new GetCustomReceiptsRequest(request.SalesOrder.Id, retrievalCriteria);

                // 3. Add custom receipts as desired.
                switch (receiptType)
                {
                    case ReceiptType.CustomReceipt1:
                        {
                            var customReceipts = (await request.RequestContext.Runtime.ExecuteAsync<GetReceiptResponse>(
                                getCustomReceiptsRequest, request.RequestContext).ConfigureAwait(false)).Receipts;

                            receipts.AddRange(customReceipts);
                        }
                        break;
                }

                return new GetEmailReceiptServiceResponse(receipts.AsPagedResult());
            }

            /// <summary>
            /// Customizes the X report.
            /// </summary>
            /// <param name="report">The original X report.</param>
            private void CustomizeXReport(Receipt report)
            {
                // You can also customize the Body and Footer part in this way.
                StringBuilder newHeader = new StringBuilder(report.Header);

                newHeader.Append("Custom Field 1:.........................Custom Value.");

                report.Header = newHeader.ToString();
            }

            /// <summary>
            /// Customizes the Z report.
            /// </summary>
            /// <param name="report">The original Z report.</param>
            private void CustomizeZReport(Receipt report)
            {
                StringBuilder newFooter = new StringBuilder(report.Footer);

                newFooter.Append("Custom Field 1:.........................Custom Value.");

                report.Footer = newFooter.ToString();
            }
        }
    }
}