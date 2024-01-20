using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
using Microsoft.Dynamics.Commerce.Runtime.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.GasStationSample.CommerceRuntime
{
    public partial class NonBindableOperationController : IController
    {
        [HttpPost]
        [Authorization(CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee, CommerceRoles.Anonymous)]
        public async Task<Cart> UpdateCartLinesAsync(IEndpointContext context, string cartId, IEnumerable<CartLine> cartLines, CalculationModes? calculationModes, long? cartVersion)
        {
            var request = new UpdateCartLinesRequest(cartId, cartLines, calculationMode: calculationModes, cartVersion: cartVersion);
            var response = await context.ExecuteAsync<SaveCartResponse>(request).ConfigureAwait(false);

            return response.Cart;
        }
    }
}
