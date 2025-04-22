namespace Contoso.StoreCommercePackagingSample.CommerceRuntime.RequestHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Data;
    using Microsoft.Dynamics.Commerce.Runtime.DataAccess.SqlServer;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Contoso.CommerceRuntime.DataModel;
    using Contoso.CommerceRuntime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Entities.DataModel;
    using Contoso.StoreCommercePackagingSample.CommerceRuntime.Messages;

    public class UnitOfMeasureService : IRequestHandlerAsync
    {
        /// <summary>
        /// Gets the collection of supported request types by this handler.
        /// </summary>
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[]
                {
                    typeof(ProductUnitOfMeasureConversionRequest)
                };
            }
        }

        public Task<Response> Execute(Request request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            Type reqType = request.GetType();
            if (reqType == typeof(ProductUnitOfMeasureConversionRequest))
            {
                return this.ConvertUnitOfMeasure((ProductUnitOfMeasureConversionRequest)request);
            }
            else
            {
                string message = string.Format(CultureInfo.InvariantCulture, "Request '{0}' is not supported.", reqType);
                throw new NotSupportedException(message);
            }
        }

        public async Task<Response> ConvertUnitOfMeasure(ProductUnitOfMeasureConversionRequest productUnitOfMeasureConversionRequest)
        {
            RequestContext context = productUnitOfMeasureConversionRequest.RequestContext;
            ItemUnitConversion itemUnitConversion = productUnitOfMeasureConversionRequest.ItemUnitConversion;
            var itemUnitConversions = new List<ItemUnitConversion>();
            string dataAreaId = context.GetChannelConfiguration().InventLocationDataAreaId;
            decimal convertedItemInventoryQuantity = 0;
            itemUnitConversions.Add(itemUnitConversion);

            // If there is a need to convert.
            if (itemUnitConversions.Any())
            {
                Dictionary<ItemUnitConversion, UnitOfMeasureConversion> conversions =
                    await GetUnitOfMeasureConversions(context, itemUnitConversions).ConfigureAwait(false);

                // Apply unit conversions.

                if (IsUnitConversionNeeded(itemUnitConversion))
                {
                    convertedItemInventoryQuantity = GetConvertedQuantity(
                        conversions,
                        itemUnitConversion.FromUnitOfMeasure,
                        itemUnitConversion.ToUnitOfMeasure,
                        itemUnitConversion.ItemId,
                        itemUnitConversion.ProductVariantId,
                        productUnitOfMeasureConversionRequest.QuantityInFromUom);
                }
            }

            return new ProductUnitOfMeasureConversionResponse(itemUnitConversion, convertedItemInventoryQuantity);
        }

        public async Task<Dictionary<ItemUnitConversion, UnitOfMeasureConversion>> GetUnitOfMeasureConversions(
        RequestContext context, IEnumerable<ItemUnitConversion> itemUnitConversions)
        {
            IEnumerable<ItemUnitConversion> distinctItemUnitConversions = itemUnitConversions
                .Where(itemUnitConversion => !itemUnitConversion.IsNop)
                .Distinct();

            var getUomConvertionDataRequest = new GetUnitOfMeasureConversionDataRequest(distinctItemUnitConversions, QueryResultSettings.AllRecords);
            var getUomConvertionDataResponse =
                await context.ExecuteAsync<GetUnitOfMeasureConversionDataResponse>(getUomConvertionDataRequest).ConfigureAwait(false);
            IEnumerable<UnitOfMeasureConversion> unitOfMeasureConversions = getUomConvertionDataResponse.UnitConversions.Results;

            return unitOfMeasureConversions.ToDictionary(unitOfMeasureConversion => new ItemUnitConversion()
            {
                ItemId = unitOfMeasureConversion.ItemId,
                FromUnitOfMeasure = unitOfMeasureConversion.FromUnitOfMeasureSymbol,
                ToUnitOfMeasure = unitOfMeasureConversion.ToUnitOfMeasureSymbol,
                ProductVariantId = unitOfMeasureConversion.ProductRecordId,
            });
        }

        private bool IsUnitConversionNeeded(ItemUnitConversion itemUnitConversion)
        {
            return itemUnitConversion != null
                && !string.IsNullOrEmpty(itemUnitConversion.FromUnitOfMeasure)
                && !string.IsNullOrEmpty(itemUnitConversion.ToUnitOfMeasure)
                && !string.Equals(itemUnitConversion.FromUnitOfMeasure, itemUnitConversion.ToUnitOfMeasure, StringComparison.OrdinalIgnoreCase);
        }

        private decimal GetConvertedQuantity(
            Dictionary<ItemUnitConversion, UnitOfMeasureConversion> conversions,
            string fromUnitOfMeasure,
            string toUnitOfMeasure,
            string itemId,
            long productId,
            decimal quantityInFromUom)
        {
            ItemUnitConversion conversion = new ItemUnitConversion
            {
                FromUnitOfMeasure = fromUnitOfMeasure,
                ToUnitOfMeasure = toUnitOfMeasure,
                ItemId = itemId,
                ProductVariantId = productId,
            };

            conversions.TryGetValue(conversion, out UnitOfMeasureConversion measureConversion);

            if (measureConversion == null)
            {
                // try to get it from the item.
                conversion.ProductVariantId = 0;
                conversions.TryGetValue(conversion, out measureConversion);

                if (measureConversion == null)
                {
                    throw new DataValidationException(
                        DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_UnitOfMeasureConversionNotFound,
                        string.Format(
                            "During calculation of estimated on hand quantity, conversion from {0} to {1} for item {2} is not found.",
                            conversion.FromUnitOfMeasure, conversion.ToUnitOfMeasure, conversion.ItemId));
                }
            }

            return measureConversion.Convert(quantityInFromUom);
        }
    }
}
