namespace Contoso.CommerceRuntime.Hosting
{
    using System.Threading.Tasks;
    using Contoso.StoreCommercePackagingSample.CommerceRuntime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

    /// <summary>
    /// An extension controller to handle ping requests to CommerceRuntime.
    /// </summary>
    public class UnboundController : IController
    {
        /// <summary>
        /// A simple GET endpoint to demonstrate GET endpoints on an unbound controller.
        /// </summary>
        /// <returns>A simple true value to indicate the endpoint was reached.</returns>
        [HttpGet]
        [Authorization(CommerceRoles.Anonymous, CommerceRoles.Application, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee, CommerceRoles.Storefront)]
        public Task<bool> SimplePingGet()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// A simple POST endpoint to demonstrate POST endpoints on an unbound controller.
        /// </summary>
        /// <returns>A simple true value to indicate the endpoint was reached.</returns>
        [HttpPost]
        [Authorization(CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee)]
        public Task<bool> SimplePingPost()
        {
            return Task.FromResult(true);
        }

        [HttpPost]
        [Authorization(CommerceRoles.Employee, CommerceRoles.Application)]
        public async Task<Cart> OverrideCartLinePrice(IEndpointContext context, string cartId, string lineId, decimal newPrice)
        {
            if (string.IsNullOrWhiteSpace(cartId))
            {
                throw new DataValidationException(DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_MissingParameter, "The cart identifier are missing.");
            }
            // Get the cart
            CartSearchCriteria cartSearchCriteria = new CartSearchCriteria(cartId);
            var request = new GetCartRequest(cartSearchCriteria, QueryResultSettings.SingleRecord);
            var response = await context.ExecuteAsync<GetCartResponse>(request).ConfigureAwait(false);
            Cart cart = response.Carts.SingleOrDefault();
            if (cart == null)
            {
                throw new DataValidationException(DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_ObjectNotFound, "The cart is not found.");
            }

            // Override the price
            var overrideSalesTransactionLinePriceRequest = new OverrideSalesTransactionLinePriceRequest(cart, lineId, newPrice, CalculationModes.All);
            var saveCartResponse = await context.ExecuteAsync<SaveCartResponse>(overrideSalesTransactionLinePriceRequest).ConfigureAwait(false);

            return saveCartResponse.Cart;
        }

        [HttpPost]
        [Authorization(CommerceRoles.Employee, CommerceRoles.Application)]
        public async Task<PagedResult<Cart>> GetOnlineShoppingCartList(IEndpointContext context, QueryResultSettings queryResultSettings)
        {
            CartSearchCriteria cartSearchCriteria = new CartSearchCriteria();
            cartSearchCriteria.CartType = CartType.Checkout;
            cartSearchCriteria.StaffId = "";
            cartSearchCriteria.IncludeAnonymous = true;
            cartSearchCriteria.LastModifiedDateTimeFrom = System.DateTime.Now.AddDays(-10);
            cartSearchCriteria.LastModifiedDateTimeTo = System.DateTime.Now;

            if (cartSearchCriteria == null)
            {
                throw new DataValidationException(DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_MissingParameter, "The cart identifier are missing.");
            }

            // Get the cart
            var request = new GetCartRequest(cartSearchCriteria, queryResultSettings);
            var response = await context.ExecuteAsync<GetCartResponse>(request).ConfigureAwait(false);

            return response.Carts;
        }
        [HttpPost]
        [Authorization(CommerceRoles.Employee, CommerceRoles.Application)]
        public async Task<Cart> GetCartById(IEndpointContext context, string id)
        {
            CartSearchCriteria cartSearchCriteria = new CartSearchCriteria(id);
            var request = new GetCartRequest(cartSearchCriteria, QueryResultSettings.SingleRecord);
            var response = await context.ExecuteAsync<GetCartResponse>(request).ConfigureAwait(false);
            Cart cart = response.Carts.SingleOrDefault();
            if (cart == null)
            {
                throw new DataValidationException(DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_ObjectNotFound, "The cart is not found.");
            }
            return cart;
        }

        [HttpPost]
        [Authorization(CommerceRoles.Employee)]

        public async Task<PagedResult<InventoryInboundOutboundDocumentLine>> SearchInventoryDocumentLine(IEndpointContext context, InventoryDocumentLineSearchCriteria searchCriteria, QueryResultSettings settings)
        {
            var request = new SearchInventoryDocumentLinesRequest(searchCriteria, settings);

            var response = await context.ExecuteAsync<SearchInventoryDocumentLinesResponse>(request).ConfigureAwait(false);

            return response.Lines;
        }
    }
}
