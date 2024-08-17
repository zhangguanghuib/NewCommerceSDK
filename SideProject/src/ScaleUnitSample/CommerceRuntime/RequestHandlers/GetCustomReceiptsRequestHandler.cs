namespace Contoso
{
    namespace Commerce.Runtime.ReceiptsSample
    {
        using System;
        using System.Collections.Generic;
        using System.Collections.ObjectModel;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

        public class GetCustomReceiptsRequestHandler : SingleAsyncRequestHandler<GetCustomReceiptsRequest>
        {
            /// <summary>
            /// Processes the GetCustomReceiptsRequest to return the set of receipts. The request should not be null.
            /// </summary>
            /// <param name="request">The request parameter.</param>
            /// <returns>The GetReceiptResponse.</returns>
            protected override async Task<Response> Process(GetCustomReceiptsRequest request)
            {
                ThrowIf.Null(request, "request");
                ThrowIf.Null(request.ReceiptRetrievalCriteria, "request.ReceiptRetrievalCriteria");

                // 1. We need to get the sales order that we are print receipts for.
                var getCustomReceiptsRequest = new GetSalesOrderDetailsByTransactionIdServiceRequest(request.TransactionId, SearchLocation.Local);
                var getSalesOrderDetailsServiceResponse = await request.RequestContext.ExecuteAsync<GetSalesOrderDetailsServiceResponse>(getCustomReceiptsRequest).ConfigureAwait(false);

                if (getSalesOrderDetailsServiceResponse == null ||
                    getSalesOrderDetailsServiceResponse.SalesOrder == null)
                {
                    throw new DataValidationException(
                        DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_ObjectNotFound,
                        string.Format("Unable to get the sales order created. ID: {0}", request.TransactionId));
                }

                SalesOrder salesOrder = getSalesOrderDetailsServiceResponse.SalesOrder;

                Collection<Receipt> result = new Collection<Receipt>();

                // 2. Now we can handle any additional receipt here.
                switch (request.ReceiptRetrievalCriteria.ReceiptType)
                {
                    // An example of getting custom receipts. 
                    case ReceiptType.CustomReceipt1:
                        {
                            IEnumerable<Receipt> customReceipts = await this.GetCustomReceiptsAsync(salesOrder, request.ReceiptRetrievalCriteria, request.RequestContext).ConfigureAwait(false);

                            result.AddRange(customReceipts);
                        }

                        break;

                    default:
                        // Add more logic to handle more types of custom receipt types.
                        break;
                }

                return new GetReceiptResponse(new ReadOnlyCollection<Receipt>(result));
            }

            /// <summary>
            /// An example to print all special custom receipts. If there are multiple special items
            /// were sold in a transaction, then we print a custom receipt for each of these items.
            /// </summary>
            /// <param name="salesOrder">The sales order that we are printing receipts for.</param>
            /// <param name="criteria">The receipt retrieval criteria.</param>
            /// <param name="context">The request context.</param>
            /// <returns>A collection of receipts.</returns>
            private async Task<Collection<Receipt>> GetCustomReceiptsAsync(SalesOrder salesOrder, ReceiptRetrievalCriteria criteria, RequestContext context)
            {
                // Back up and clear existing sales lines because we want to print a custom receipt for each one of the sales lines.
                Collection<SalesLine> originalSalesLines = salesOrder.SalesLines;
                salesOrder.SalesLines = new Collection<SalesLine>();

                Collection<Receipt> result = new Collection<Receipt>();

                foreach (SalesLine salesLine in originalSalesLines)
                {
                    // Check if the item is a special item.
                    if (this.IsSpecialItem(salesLine))
                    {
                        // Add this special item back to the sales order so that we can print the custom receipt for this sales line.
                        salesOrder.SalesLines.Add(salesLine);

                        // Call receipt service to get the custom receipt.
                        var getReceiptServiceRequest = new GetReceiptServiceRequest(
                                salesOrder,
                                new Collection<ReceiptType> { criteria.ReceiptType },
                                salesOrder.TenderLines,
                                criteria.IsCopy,
                                criteria.IsPreview,
                                criteria.HardwareProfileId,
                                includeExternalReceipt: false,
                                requestedReceiptType: criteria.ReceiptType);
                        var customReceiptsResponse = await context.ExecuteAsync<GetReceiptServiceResponse>(getReceiptServiceRequest).ConfigureAwait(false);
                        ReadOnlyCollection<Receipt> customReceipts = customReceiptsResponse.Receipts;

                        // Add the custom receipt to the result collection.
                        result.AddRange(customReceipts);

                        // Clean the sales lines.
                        salesOrder.SalesLines.Clear();
                    }
                }

                return result;
            }

            /// <summary>
            /// A fake logic of determining if a sales line is special or not.
            /// </summary>
            /// <param name="salesLine">The sales line.</param>
            /// <returns>True if the item is special, false otherwise.</returns>
            private bool IsSpecialItem(SalesLine salesLine)
            {
                if (salesLine.ItemId.StartsWith("0", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}