namespace Contoso.CommerceRuntime.Controllers
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Azure.Core;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using RSL.Commerce.Runtime.Extensions.Messages;

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
        public async Task<bool> SimplePingGet(IEndpointContext context, string  cartId)
        {
            var cartSearchCriteria = new CartSearchCriteria(cartId);

            var getCartRequest = new GetCartRequest(cartSearchCriteria, QueryResultSettings.SingleRecord);
            var getCartResponse = await context.ExecuteAsync<GetCartResponse>(getCartRequest).ConfigureAwait(false);
            SalesTransaction salesTransaction = getCartResponse.Transactions.SingleOrDefault();

            if(salesTransaction != null)
            {
                SendEmailRequest sendEmailRequest = new SendEmailRequest(salesTransaction.Id, salesTransaction.CustomerId, salesTransaction);
                SendEmailResponse sendEmailResponse = await context.ExecuteAsync<SendEmailResponse>(sendEmailRequest).ConfigureAwait(false);
            }

            return true;
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
