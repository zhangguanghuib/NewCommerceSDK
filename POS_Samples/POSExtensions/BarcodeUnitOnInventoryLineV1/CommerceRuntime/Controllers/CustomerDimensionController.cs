namespace Contoso.StoreCommercePackagingSample.CommerceRuntime.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Contoso.StoreCommercePackagingSample.CommerceRuntime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
    using Microsoft.Dynamics.Retail.Diagnostics;

    public class CustomerDimensionController : IController
    {
        [HttpPost]
        [Authorization(CommerceRoles.Employee, CommerceRoles.Application)]
        public async Task<String> GetCustomerDimensionByDimensionAttribute(IEndpointContext context, string accountNum, string dimensionAttribute)
        {
            var getCustomerDimensionRequest = new GetCustomerDimensionRequest(accountNum, dimensionAttribute);
            var response = await context.ExecuteAsync<GetCustomerDimensionResponse>(getCustomerDimensionRequest).ConfigureAwait(false);
            return response.DimensionAttributeValue;
        }
    }
}
