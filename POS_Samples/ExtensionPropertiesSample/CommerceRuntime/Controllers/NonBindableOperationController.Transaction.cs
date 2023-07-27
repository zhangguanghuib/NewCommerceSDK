namespace Contoso.GasStationSample.CommerceRuntime
{
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;

    public partial class NonBindableOperationController : IController
    {
        [HttpPost]
        [Authorization(CommerceRoles.Anonymous, CommerceRoles.Employee, CommerceRoles.Customer)]
        public async Task<string> ReturnTransactionByApiAsync(IEndpointContext context, string origTransId, string cartId, string newItemDescription)
        {     
            var request = new GetSalesOrderDetailsByTransactionIdServiceRequest(origTransId, SearchLocation.All);
            var response = await context.ExecuteAsync<GetSalesOrderDetailsServiceResponse>(request).ConfigureAwait(false);
            SalesOrder originalTransaction = response.SalesOrder;

            // Create ReasonCodeLine
            ReasonCodeLine reasonCodeLine = new ReasonCodeLine();
            reasonCodeLine.LineId = Guid.NewGuid().ToString("N");
            reasonCodeLine.ReasonCodeId = "Return";
            reasonCodeLine.Information = "Defect";
            reasonCodeLine.TransactionId = originalTransaction.Id;
            reasonCodeLine.LineType = ReasonCodeLineType.Sales;
            reasonCodeLine.CreatedDateTime = DateTimeOffset.UtcNow;

            CartSearchCriteria cartSearchCriteria = new CartSearchCriteria(cartId);
            GetCartServiceRequest getCartServiceRequest = new GetCartServiceRequest(cartSearchCriteria, QueryResultSettings.SingleRecord);
            GetCartServiceResponse getCartServiceResponse = await context.ExecuteAsync<GetCartServiceResponse>(getCartServiceRequest).ConfigureAwait(false);
            var returnCart = getCartServiceResponse.Carts.SingleOrDefault<Cart>();

            //var returnCart = new Cart
            //{
            //    Id = cartId,
            //    CartLines = new List<CartLine>(),
            //    TenderLines = new List<TenderLine>()
            //};

            //SaveCartRequest createReturnCartRequest = new SaveCartRequest(returnCart, null, false);
            //var createReturnCartResponse = await context.ExecuteAsync<SaveCartResponse>(createReturnCartRequest).ConfigureAwait(false); ;
            //returnCart = createReturnCartResponse.Cart;

            var cartLines = new List<CartLine>();
            foreach (SalesLine origCartLine in originalTransaction.SalesLines.Where(i => !i.IsVoided && i.Quantity > i.ReturnQuantity))
            {
                cartLines.Add(new CartLine
                {
                    ItemId = origCartLine.ItemId,
                    ProductId = origCartLine.ProductId,
                    Quantity = -1 * Math.Abs(origCartLine.Quantity - origCartLine.ReturnQuantity),
                    ReturnLineNumber = origCartLine.LineNumber,
                    ReturnTransactionId = originalTransaction.Id,
                    Description = !string.IsNullOrEmpty(newItemDescription) ? newItemDescription : origCartLine.Description,
                    ReasonCodeLines = new List<ReasonCodeLine>() { reasonCodeLine },
                });
            }

            returnCart.CartLines.AddRange(cartLines);
            returnCart.DiscountAmount = originalTransaction.DiscountAmount;
            returnCart.DiscountAmountWithoutTax = originalTransaction.DiscountAmountWithoutTax;
            returnCart.TotalManualDiscountAmount = originalTransaction.TotalManualDiscountAmount;
            returnCart.TotalManualDiscountPercentage = originalTransaction.TotalManualDiscountPercentage;
            returnCart.ReasonCodeLines = originalTransaction.ReasonCodeLines;
            returnCart.DiscountCodes = originalTransaction.DiscountCodes;

            var addCartLinesRequest = new SaveCartRequest(returnCart, null, false);
            var addCartLinesResponse = await context.ExecuteAsync<SaveCartResponse>(addCartLinesRequest).ConfigureAwait(false); ;
            returnCart = addCartLinesResponse.Cart;

            getCartServiceRequest = new GetCartServiceRequest(new CartSearchCriteria(cartId), QueryResultSettings.SingleRecord);
            getCartServiceResponse = await context.ExecuteAsync<GetCartServiceResponse>(getCartServiceRequest).ConfigureAwait(false);
            SalesTransaction transaction = getCartServiceResponse.Transactions.SingleOrDefault();
            foreach (SalesLine salesLine in transaction.SalesLines)
            {
                salesLine.Description = newItemDescription;
            }

            transaction.LongVersion = (await context.ExecuteAsync<SingleEntityDataServiceResponse<long>>(new SaveCartVersionedDataRequest(transaction)).ConfigureAwait(false)).Entity;

            ConvertSalesTransactionToCartServiceRequest convertRequest = new ConvertSalesTransactionToCartServiceRequest(transaction);
            returnCart = (await context.ExecuteAsync<UpdateCartServiceResponse>(convertRequest).ConfigureAwait(false)).Cart;

            var tenderLine = new CartTenderLine
            {
                TenderLineId = string.Empty,
                Amount = returnCart.AmountDue,
                Currency =  originalTransaction.CurrencyCode,
                TenderTypeId = "1"
            };
            var saveTenderLineRequest = new SaveTenderLineRequest()
            {
                CartId = returnCart.Id,
                CartVersion = new long?(),
                TenderLine = tenderLine,
                OperationType = TenderLineOperationType.Create
            };

            SaveTenderLineResponse saveTenderLineResponse = await context.ExecuteAsync<SaveTenderLineResponse>(saveTenderLineRequest).ConfigureAwait(false);
            returnCart = saveTenderLineResponse.Cart;

            var saveCartRequest = new SaveCartRequest(returnCart, null, false);
            var saveCartResponse = await context.ExecuteAsync<SaveCartResponse>(saveCartRequest).ConfigureAwait(false);
            returnCart = saveCartResponse.Cart;

            //CheckoutCartRequest checkoutCartRequest = new CheckoutCartRequest(returnCart.Id, string.Empty, null, null, "2", returnCart.Version);
            //var salesOrder = (await context.ExecuteAsync<CheckoutCartResponse>(request).ConfigureAwait(continueOnCapturedContext: false)).SalesOrder;

            return cartId;
        }

    }
}