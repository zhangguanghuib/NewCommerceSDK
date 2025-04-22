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
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
    using Microsoft.Dynamics.Retail.Diagnostics;

    public class UnitOfMeasureController: IController
    {
        [HttpPost]
        [Authorization(CommerceRoles.Employee, CommerceRoles.Application)]
        public async Task<UnitOfMeasure> GetInventoryUnitOfMeasureByItemId(IEndpointContext context, long productId, string itemId)
        {
            var getInventoryUnitsOfMeasureDataRequest = new GetItemInventoryUnitsOfMeasureDataRequest(itemId);
            PagedResult<UnitOfMeasure> result = (await context.ExecuteAsync<EntityDataServiceResponse<UnitOfMeasure>>(getInventoryUnitsOfMeasureDataRequest).ConfigureAwait(false)).PagedEntityCollection;

            int countOfInventoryUomResults = (result != null) ? result.Count() : 0;

            if (countOfInventoryUomResults != 1)
            {
                RetailLogger.Log.ProductAvailabilityServiceUnexpectedCountOfInventoryUomResults(countOfInventoryUomResults, productId);
                return null;
            }

            return result.Single();
        }

        [HttpPost]
        [Authorization(CommerceRoles.Employee, CommerceRoles.Application)]
        public async Task<decimal> ConvertUnitOfMeasure(IEndpointContext context, long productId, string itemId, string fromUOM, string toUOM, decimal fromQty)
        {
            ItemUnitConversion itemUnitConversion = new ItemUnitConversion
            {
                ItemId = itemId,
                ProductVariantId = productId,
                FromUnitOfMeasure = fromUOM,
                ToUnitOfMeasure = toUOM
            };

            var productUnitOfMeasureConversionRequest = new ProductUnitOfMeasureConversionRequest(itemUnitConversion, fromQty);

            ProductUnitOfMeasureConversionResponse response = await context.ExecuteAsync<ProductUnitOfMeasureConversionResponse>(productUnitOfMeasureConversionRequest).ConfigureAwait(false);

            // return the converted quantity
            return response.quantityInToUom;
        }

    }
}
