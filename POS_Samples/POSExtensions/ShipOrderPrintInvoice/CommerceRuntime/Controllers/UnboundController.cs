namespace Contoso.CommerceRuntime.Controllers
{
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Retail.Diagnostics;

    /// <summary>
    /// An extension controller to handle requests to the StoreHours entity set.
    /// </summary>
    public class UnboundController : IController
    {
        [HttpPost]
        [Authorization(CommerceRoles.Anonymous, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee)]
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
            var overrideCartLinePriceRequest = new OverrideCartLinePriceRequest(cart, lineId, newPrice, CalculationModes.All);
            var saveCartResponse = await context.ExecuteAsync<SaveCartResponse>(request).ConfigureAwait(false);

            return saveCartResponse.Cart;
        }
    }
}