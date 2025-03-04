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
        using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages;
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
                //var getCustomReceiptsRequest = new GetSalesOrderDetailsByTransactionIdServiceRequest(request.TransactionId, SearchLocation.All);
                //var getSalesOrderDetailsServiceResponse = await request.RequestContext.ExecuteAsync<GetSalesOrderDetailsServiceResponse>(getCustomReceiptsRequest).ConfigureAwait(false);

                GetSalesOrderDetailsBySalesIdServiceRequest getSalesOrderDetailsBySalesIdServiceRequest = new GetSalesOrderDetailsBySalesIdServiceRequest(request.TransactionId, SearchLocation.All);
                GetSalesOrderDetailsServiceResponse getSalesOrderDetailsServiceResponse = await request.RequestContext.ExecuteAsync<GetSalesOrderDetailsServiceResponse>(getSalesOrderDetailsBySalesIdServiceRequest).ConfigureAwait(false);

                if (getSalesOrderDetailsServiceResponse == null ||
                    getSalesOrderDetailsServiceResponse.SalesOrder == null)
                {
                    throw new DataValidationException(
                        DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_ObjectNotFound,
                        string.Format("Unable to get the sales order created. ID: {0}", request.TransactionId));
                }

                SalesOrder salesOrder = getSalesOrderDetailsServiceResponse.SalesOrder;

                if(request.ReceiptRetrievalCriteria.ReceiptType == ReceiptType.CustomReceipt6)
                {
                    InvokeExtensionMethodRealtimeRequest extensionRequest = new InvokeExtensionMethodRealtimeRequest("PrintSalesInvoice", salesOrder.SalesId);
                    InvokeExtensionMethodRealtimeResponse response = await request.RequestContext.ExecuteAsync<InvokeExtensionMethodRealtimeResponse>(extensionRequest).ConfigureAwait(false);
                    ReadOnlyCollection<object> results = response.Result;

                    string resValue = (string)results[0];
                }


                Collection<Receipt> result = new Collection<Receipt>();

                return new GetReceiptResponse(new ReadOnlyCollection<Receipt>(result));
            }
        }
    }
}