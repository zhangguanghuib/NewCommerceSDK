namespace GHZ.BarcodeMsrDialogSample.CommerceRuntime
{
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
    using Microsoft.Dynamics.Commerce.Runtime;
    using System.Collections.Generic;

    public class Jewelry2Controller : IController
    {
        [HttpPost]
        [Authorization(CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee, CommerceRoles.Anonymous)]

        public async Task<PagedResult<ProductDimensionValue>> JewelryGetDimensionValues(IEndpointContext context, long channelId, long masterProductId, ProductDimensionType requestedDimension,  QueryResultSettings settings)
        {
            ThrowIf.Null(settings, nameof(settings));

            ICollection<ProductDimension> matchingDimensionValues = new List<ProductDimension>();
            ProductVariantResolutionContext variantResolutionContext = null;
            var dataRequest = new GetProductDimensionValuesDataRequest(channelId, masterProductId, requestedDimension, matchingDimensionValues, variantResolutionContext, settings);
            var dataResponse = await context.ExecuteAsync<EntityDataServiceResponse<ProductDimensionValue>>(dataRequest).ConfigureAwait(false);

            return dataResponse.PagedEntityCollection;

        }
    }
}
