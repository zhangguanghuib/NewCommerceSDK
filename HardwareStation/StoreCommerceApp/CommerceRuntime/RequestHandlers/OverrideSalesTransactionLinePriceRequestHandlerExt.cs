
namespace Contoso.StoreCommercePackagingSample.CommerceRuntime.RequestHandlers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Contoso.StoreCommercePackagingSample.CommerceRuntime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

    public class OverrideSalesTransactionLinePriceRequestHandlerExt : SingleAsyncRequestHandler<OverrideSalesTransactionLinePriceRequest>
    {
        protected async override Task<Response> Process(OverrideSalesTransactionLinePriceRequest overrideSalesTransactionLinePriceRequest)
        {
            ThrowIf.Null(overrideSalesTransactionLinePriceRequest, nameof(overrideSalesTransactionLinePriceRequest));

            string cartId = overrideSalesTransactionLinePriceRequest.Cart.Id;
            string lineId = overrideSalesTransactionLinePriceRequest.LineId;
            decimal newPrice = overrideSalesTransactionLinePriceRequest.NewPrice;

            // Workaround
            var getCartServiceRequest = new GetCartServiceRequest(new CartSearchCriteria(cartId), QueryResultSettings.SingleRecord);
            var getCartServiceResponse = await overrideSalesTransactionLinePriceRequest.RequestContext.ExecuteAsync<GetCartServiceResponse>(getCartServiceRequest).ConfigureAwait(false);
            SalesTransaction salesTransaction = getCartServiceResponse.Transactions.SingleOrDefault();

            SalesLine salesLine = salesTransaction.ActiveSalesLines.Single(l => l.LineId.Equals(lineId, StringComparison.OrdinalIgnoreCase));
            salesLine.IsPriceOverridden = true;
            salesLine.Price = newPrice;

            CalculateSalesTransactionServiceRequest calculateServiceRequest = new CalculateSalesTransactionServiceRequest(salesTransaction, CalculationModes.All);
            salesTransaction = (await overrideSalesTransactionLinePriceRequest.RequestContext.ExecuteAsync<CalculateSalesTransactionServiceResponse>(calculateServiceRequest).ConfigureAwait(false)).Transaction;

            salesTransaction.LongVersion = (await overrideSalesTransactionLinePriceRequest.RequestContext.ExecuteAsync<SingleEntityDataServiceResponse<long>>(new SaveCartVersionedDataRequest(salesTransaction)).ConfigureAwait(false)).Entity;

            ConvertSalesTransactionToCartServiceRequest convertRequest = new ConvertSalesTransactionToCartServiceRequest(salesTransaction);
            var cart = (await overrideSalesTransactionLinePriceRequest.RequestContext.ExecuteAsync<UpdateCartServiceResponse>(convertRequest).ConfigureAwait(false)).Cart;

            return new SaveCartResponse(cart);
        }
    }
}
