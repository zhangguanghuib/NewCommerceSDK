namespace Contoso.PricingEngine.TradeAgreementCalculator
{
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Services.PricingEngine;
    using System;
    using System.Collections.Generic;

    public static class InternalPriceContextHelper
    {
        internal static ISet<string> GetAllPriceGroupsForPrice(PriceContext priceContext)
        {
            ThrowIf.Null(priceContext, nameof(priceContext));

            return GetAllPriceGroups(priceContext, GetAllPriceGroupsExceptCatalogsForPrice(priceContext));
        }

        private static HashSet<string> GetAllPriceGroups(PriceContext priceContext, ISet<string> allPriceGroupsExceptCatalogs)
        {
            HashSet<string> allPriceGroups = new HashSet<string>(allPriceGroupsExceptCatalogs, StringComparer.OrdinalIgnoreCase);

            foreach (KeyValuePair<long, ISet<string>> priceGroups in priceContext.CatalogPriceGroups)
            {
                allPriceGroups.UnionWith(priceGroups.Value);
            }

            return allPriceGroups;
        }

        private static ISet<string> GetAllPriceGroupsExceptCatalogsForPrice(PriceContext priceContext)
        {
            return priceContext.AllPriceGroupsExceptCatalogsForPrice;
        }

        internal static ISet<string> GetApplicablePriceGroupsForPrice(PriceContext priceContext, ISet<long> itemCatalogIds)
        {
            ThrowIf.Null(priceContext, nameof(priceContext));

            return GetApplicablePriceGroups(priceContext, GetAllPriceGroupsExceptCatalogsForPrice(priceContext), itemCatalogIds);
        }

        private static ISet<string> GetApplicablePriceGroups(PriceContext priceContext, ISet<string> allPriceGroupsExceptCatalogs, ISet<long> itemCatalogIds)
        {
            ThrowIf.Null(priceContext, nameof(priceContext));

            HashSet<string> applicablePriceGroups = new HashSet<string>(allPriceGroupsExceptCatalogs, StringComparer.OrdinalIgnoreCase);

            if (itemCatalogIds != null)
            {
                foreach (var itemCatalogId in itemCatalogIds)
                {
                    ISet<string> catalogPriceGroups;
                    if (priceContext.CatalogPriceGroups.TryGetValue(itemCatalogId, out catalogPriceGroups))
                    {
                        if (catalogPriceGroups != null)
                        {
                            applicablePriceGroups.UnionWith(catalogPriceGroups);
                        }
                    }
                }
            }

            return applicablePriceGroups;
        }


    }
}

   
