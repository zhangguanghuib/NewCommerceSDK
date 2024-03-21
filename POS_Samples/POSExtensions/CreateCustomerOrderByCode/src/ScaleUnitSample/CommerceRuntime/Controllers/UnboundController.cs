namespace Contoso.CommerceRuntime.Controllers
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Common;
    using System.Globalization;
    using System.Linq;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using System.Transactions;
    using Azure.Core;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;


    /// <summary>
    /// An extension controller to handle requests to the StoreHours entity set.
    /// </summary>
    public class UnboundController : IController
    {
        /// <summary>
        /// A simple GET endpoint to demonstrate GET endpoints on an unbound controller.
        /// </summary>
        /// <returns>A simple true value to indicate the endpoint was reached.</returns>
        [HttpGet]
        [Authorization(CommerceRoles.Anonymous, CommerceRoles.Application, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee, CommerceRoles.Storefront)]
        public async Task<Cart> SimplePingGet(IEndpointContext context, string cartId)
        {
            var getCartServiceRequest = new GetCartServiceRequest(new CartSearchCriteria(cartId), QueryResultSettings.SingleRecord);
            GetCartServiceResponse getCartServiceResponse = await context.ExecuteAsync<GetCartServiceResponse>(getCartServiceRequest).ConfigureAwait(false);
            SalesTransaction transaction = getCartServiceResponse.Transactions.SingleOrDefault();

            if(transaction.CartType == CartType.CustomerOrder && 
                (string.IsNullOrEmpty(transaction.DeliveryMode) || transaction.SalesLines.Any(sl => string.IsNullOrEmpty(sl.DeliveryMode))))
            {
                GetCustomersServiceRequest getCustomersServiceRequest = new GetCustomersServiceRequest(QueryResultSettings.SingleRecord, transaction.CustomerId, SearchLocation.Local);
                GetCustomersServiceResponse getCustomersServiceResponse = await context.ExecuteAsync<GetCustomersServiceResponse>(getCustomersServiceRequest).ConfigureAwait(false);
                Address shippingAddress = getCustomersServiceResponse.Customers.FirstOrDefault().GetPrimaryAddress();

                (Cart cart, transaction) = await this.updateOrderDeliverySpecifications(context, transaction, shippingAddress).ConfigureAwait(false);

                cart = await this.updateLinesDeliverySpecifications(context, cartId, transaction, shippingAddress).ConfigureAwait(false);
                return cart;
            }
            else
            {
               return getCartServiceResponse.Carts.FirstOrDefault();
            }
        }

        public async Task<(Cart, SalesTransaction)> updateOrderDeliverySpecifications(IEndpointContext context, SalesTransaction salesTransaction, Address shippingAddress)
        {
            var newDeliveryInformation = new DeliverySpecification()
            {
                DeliveryModeId = "99",
                DeliveryAddress = shippingAddress,
                DeliveryPreferenceType = DeliveryPreferenceType.ShipToAddress,
                RequestedDeliveryDate = System.DateTimeOffset.Now.AddDays(2),
            };

            var updateDeliveryRequest = new UpdateDeliverySpecificationsRequest(salesTransaction, newDeliveryInformation);

            salesTransaction = (await context.ExecuteAsync<UpdateDeliverySpecificationsResponse>(updateDeliveryRequest).ConfigureAwait(false)).ExistingCart;
            salesTransaction.LongVersion = (await context.ExecuteAsync<SingleEntityDataServiceResponse<long>>(new SaveCartVersionedDataRequest(salesTransaction)).ConfigureAwait(false)).Entity;

            ConvertSalesTransactionToCartServiceRequest convertRequest = new ConvertSalesTransactionToCartServiceRequest(salesTransaction);
            Cart cart = (await context.ExecuteAsync<UpdateCartServiceResponse>(convertRequest).ConfigureAwait(false)).Cart;
            return (cart, salesTransaction);
        }
    
        public async Task<Cart> updateLinesDeliverySpecifications(IEndpointContext context, string cartId, SalesTransaction transaction, Address shippingAddress)
        {
            var lineDeliverySpecs = this.CreateLineDeliverySpecifications(transaction, shippingAddress);
            var updateDeliveryRequest = new UpdateDeliverySpecificationsRequest(cartId, lineDeliverySpecs);

            Cart cart = (await context.ExecuteAsync<UpdateDeliverySpecificationsResponse>(updateDeliveryRequest).ConfigureAwait(false)).Cart;

            return cart;
        }

        public  IEnumerable<LineDeliverySpecification> CreateLineDeliverySpecifications(SalesTransaction transaction, Address shipAddress)
        {
            var lineDeliverySpecifications = new Collection<LineDeliverySpecification>();
            foreach (var salesLine in transaction.SalesLines)
            {
                var deliverySpec = new LineDeliverySpecification()
                {
                    LineId = salesLine.LineId,
                    DeliverySpecification = new DeliverySpecification()
                    {
                        DeliveryModeId = "99",
                        DeliveryAddress = shipAddress,
                        DeliveryPreferenceType = DeliveryPreferenceType.ShipToAddress,
                        RequestedDeliveryDate = System.DateTimeOffset.Now.AddDays(2),
                    },
                };
                lineDeliverySpecifications.Add(deliverySpec);
            }

            return lineDeliverySpecifications;
        }

        /// <summary>
        /// A simple POST endpoint to demonstrate POST endpoints on an unbound controller.
        /// </summary>
        /// <returns>A simple true value to indicate the endpoint was reached.</returns>
        [HttpPost]
        [Authorization(CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee)]
        public Task<bool> SimplePingPost(IEndpointContext context)
        {
            return Task.FromResult(true);
        }
    }
}
