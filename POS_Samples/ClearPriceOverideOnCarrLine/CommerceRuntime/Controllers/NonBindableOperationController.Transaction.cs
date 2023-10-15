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

    public partial class NonBindableOperationController : IController
    {
        [HttpPost]
        [Authorization(CommerceRoles.Anonymous, CommerceRoles.Employee, CommerceRoles.Customer)]
        public async Task<Cart> ClearIsPriceOverrideOnCart(IEndpointContext context, string cartId)
        {
            GetCartServiceRequest getCartServiceRequest = new GetCartServiceRequest(new CartSearchCriteria(cartId), QueryResultSettings.SingleRecord);
            GetCartServiceResponse getCartServiceResponse = await context.ExecuteAsync<GetCartServiceResponse>(getCartServiceRequest).ConfigureAwait(false);
            SalesTransaction transaction = getCartServiceResponse.Transactions.SingleOrDefault();

            if (transaction != null && transaction.SalesLines.Count > 0)
            {
                foreach(var line in transaction.SalesLines)
                {
                    if (line.IsPriceOverridden) 
                        line.IsPriceOverridden = false;
                }

                transaction.LongVersion = (await context.ExecuteAsync<SingleEntityDataServiceResponse<long>>(new SaveCartVersionedDataRequest(transaction)).ConfigureAwait(false)).Entity;

                ConvertSalesTransactionToCartServiceRequest convertRequest = new ConvertSalesTransactionToCartServiceRequest(transaction);
                Cart cart = (await context.ExecuteAsync<UpdateCartServiceResponse>(convertRequest).ConfigureAwait(false)).Cart;

                return cart;
            }
            else
            {
                return getCartServiceResponse.Carts.SingleOrDefault(); ;
            }
        }

    }
}