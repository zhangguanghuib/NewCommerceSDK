
namespace PricingEngine.ContosoTradeAgreementCalculator
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Contoso.CommerceRuntime.PricingEngine;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Services.PricingEngine;
    using Microsoft.Dynamics.Retail.Diagnostics;
    using Microsoft.Dynamics.Retail.Diagnostics.Extensions;

    public class ContosoTradeAgreementCalculator : IPricingCalculator, IPriceTradeAgreementCalculator
    {

        private const string ItemUnitKeyDelimiter = "|";
        private static readonly DateTime NoDate = new DateTime(1900, 1, 1);

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeAgreementCalculator" /> class.
        /// </summary>
        public ContosoTradeAgreementCalculator()
        {
        }

        private enum Events
        {
            GetMatchedChannel,
        }

        IDictionary<string, IEnumerable<PriceLine>> IPricingCalculator.CalculatePriceLines(
                SalesTransaction transaction,
                PriceContext priceContext,
                IPricingDataAccessor pricingDataManager)
        {
            ThrowIf.Null(transaction, nameof(transaction));
            ThrowIf.Null(priceContext, nameof(priceContext));
            ThrowIf.Null(pricingDataManager, nameof(pricingDataManager));

            IEnumerable<SalesLine> salesLines = transaction.PriceCalculableSalesLines;

            if (priceContext.CalculateForNewSalesLinesOnly)
            {
                salesLines = transaction.PriceCalculableSalesLines.Where(p => priceContext.NewSalesLineIdSet.Contains(p.LineId)).ToList();
            }

            CalculatedPrices result = PricingEngineExtensionRepository.GetPriceTradeAgreementCalculator().CalculatePrices(salesLines, priceContext, pricingDataManager);

            Dictionary<string, IEnumerable<PriceLine>> itemPriceLines = new Dictionary<string, IEnumerable<PriceLine>>(StringComparer.OrdinalIgnoreCase);

            foreach (SalesLine salesLine in salesLines)
            {
                PriceLine price;
                if (result.TryFindPrice(salesLine.LineId, out price))
                {
                    itemPriceLines.Add(salesLine.LineId, new List<PriceLine>(1) { price });
                }
            }

            return itemPriceLines;
        }

        public CalculatedPrices CalculatePrices(
                IEnumerable<SalesLine> salesLines,
                PriceContext priceContext,
                IPricingDataAccessor pricingDataManager)
        {
            ThrowIf.Null(salesLines, nameof(salesLines));
            ThrowIf.Null(priceContext, nameof(priceContext));
            ThrowIf.Null(pricingDataManager, nameof(pricingDataManager));

            Tuple<DateTimeOffset, DateTimeOffset> dateRange = PricingEngine.GetMinAndMaxActiveDates(salesLines, priceContext.ActiveDate);

            IEnumerable<OrgUnitLocation> allStores = ((IPricingDataAccessorV5)pricingDataManager).GetAllStores() as IEnumerable<OrgUnitLocation>;

            ChannelPriceConfiguration channelPriceConfig = pricingDataManager.GetChannelPriceConfiguration();

            //bool disableGetPriceTradeAgreementsBySearchCriteriaKillSwitch = priceContext.KillSwitchContext.GetKillSwitchValue(InternalPriceContextHelper.PricingGetPriceTradeAgreementsBySearchCriteriaKillSwitch, defaultValue: false);
            bool disableGetPriceTradeAgreementsBySearchCriteriaKillSwitch = false;

            ReadOnlyCollection<TradeAgreement> tradeAgreements = null;

            if (!disableGetPriceTradeAgreementsBySearchCriteriaKillSwitch)
            {
                // look up all trade agreements for given context, items and search criteria
                 #pragma warning disable CA1851
                // The code that's violating the rule is on this line.
                HashSet<TradeAgreementSearchCriteria> tradeAgreementSearchCriteria =
                    new HashSet<TradeAgreementSearchCriteria>(
                        salesLines.Select(sl => new TradeAgreementSearchCriteria(sl.ItemId, GetMatchedChannel(sl, allStores, channelPriceConfig).InventoryLocationId ?? string.Empty, sl.UnitOfMeasureSymbol ?? string.Empty)));
                #pragma warning restore CA1851


                //using (PricingDiagnostics.PricingDatabaseInstrument instrument = new PricingDiagnostics.PricingDatabaseInstrument("ReadPriceTradeAgreementsBySearchCriteria", tradeAgreementSearchCriteria.Count))
                //{
                tradeAgreements = pricingDataManager.ReadPriceTradeAgreements(
                tradeAgreementSearchCriteria,
                InternalPriceContextHelper.GetAllPriceGroupsForPrice(priceContext),
                priceContext.CustomerAccount,
                dateRange.Item1,
                dateRange.Item2,
                priceContext.CurrencyCode,
                QueryResultSettings.AllRecords) as ReadOnlyCollection<TradeAgreement>;

                //    instrument.ResultCount = tradeAgreements.Count;
                //}
            }
            else
            {
                // look up all trade agreements for given items and context
                #pragma warning disable CA1851
                HashSet<string> itemIds = new HashSet<string>(salesLines.Select(s => s.ItemId).Distinct(), StringComparer.OrdinalIgnoreCase);
                #pragma warning disable CA1851
                //using (PricingDiagnostics.PricingDatabaseInstrument instrument = new PricingDiagnostics.PricingDatabaseInstrument("ReadPriceTradeAgreements", itemIds.Count))
                //{
                tradeAgreements = pricingDataManager.ReadPriceTradeAgreements(
                itemIds,
                InternalPriceContextHelper.GetAllPriceGroupsForPrice(priceContext),
                priceContext.CustomerAccount,
                dateRange.Item1,
                dateRange.Item2,
                priceContext.CurrencyCode,
                QueryResultSettings.AllRecords) as ReadOnlyCollection<TradeAgreement>;

                //    instrument.ResultCount = tradeAgreements.Count;
                //}
            }

            // Return directly if there is no trade agreement line loaded.
            if (!tradeAgreements.Any())
            {
                return new CalculatedPrices();
            }

            //priceContext.PricingEngineDiagnosticsObject.AddTradeAgreementsConsidered(tradeAgreements);

            var discountParameters = DiscountParameters.CreateAndInitialize(priceContext.PriceParameters);

            Dictionary<string, decimal> itemUnitQuantities = null;

            if (priceContext.PriceCalculationMode == PricingCalculationMode.Transaction)
            {
                itemUnitQuantities = GetItemUnitQuantities(salesLines);
            }

            CalculatedPrices result = new CalculatedPrices();

            Dictionary<string, List<SalesLine>> variantsByItemId = salesLines.GroupBy(o => o.ItemId).ToDictionary(g => g.Key, g => g.ToList());

            List<TradeAgreement> candidateTradeAgreements = new List<TradeAgreement>();

            foreach (SalesLine salesLine in salesLines)
            {
                candidateTradeAgreements = FilterTradeAgreementsByInventoryDimensions(tradeAgreements, salesLine, priceContext);
                candidateTradeAgreements = FilterTradeAgreementsByTrackingDimensions(candidateTradeAgreements, salesLine);

                TradeAgreementPriceLine price = CalculateAgreementPriceLine(
                    salesLine,
                    priceContext,
                    candidateTradeAgreements,
                    discountParameters,
                    itemUnitQuantities,
                    variantsByItemId[salesLine.ItemId]);

                if (price != null)
                {
                    result.AddPriceLine(salesLine.LineId, price);
                }
            }

            return result;
        }

        internal static Dictionary<string, IList<TradeAgreement>> IndexAgreementsByItemId(ReadOnlyCollection<TradeAgreement> tradeAgreements)
        {
            var agreementsByItemId = new Dictionary<string, IList<TradeAgreement>>(StringComparer.OrdinalIgnoreCase);
            foreach (var ta in tradeAgreements)
            {
                var itemId = ta.ItemRelation;

                if (!agreementsByItemId.ContainsKey(itemId))
                {
                    agreementsByItemId.Add(itemId, new List<TradeAgreement>());
                }

                IList<TradeAgreement> agreements;
                if (!agreementsByItemId.TryGetValue(itemId, out agreements))
                {
                    agreements = Array.Empty<TradeAgreement>();
                }

                agreements.Add(ta);
            }

            return agreementsByItemId;
        }

        private static OrgUnitLocation GetMatchedChannel(SalesLine salesLine, IEnumerable<OrgUnitLocation> allStores, ChannelPriceConfiguration channelPriceConfig)
        {
            {
                // If salesLine has site specified, use the site and warehouse on salesLine to match trade agreements.
                if (!string.IsNullOrEmpty(salesLine.InventorySiteId))
                {
                    return new OrgUnitLocation()
                    {
                        InventorySiteId = salesLine.InventorySiteId,
                        InventoryLocationId = salesLine.InventoryLocationId,
                    };
                }

                // If not given, the site should be inferred. The first choice is to use fulfillment store when provided.
                if (!string.IsNullOrEmpty(salesLine.FulfillmentStoreId) && allStores != null)
                {
                    OrgUnitLocation fulfilmentStore = allStores.FirstOrDefault(x => string.Equals(x.OrgUnitNumber, salesLine.FulfillmentStoreId, StringComparison.OrdinalIgnoreCase));

                    if (fulfilmentStore != null)
                    {
                        return fulfilmentStore;
                    }
                    else
                    {
                        RetailLogger.Log.LogInformation(
                            Events.GetMatchedChannel,
                            format: "Fulfillment store provided, but not found in the store list. SalesLineId = { LineId }.",
                            salesLine.LineId.AsSystemMetadata());
                    }
                }

                // Else, fall back to use channel store.

                // Else, fall back to use channel store.
                string channelWarehouse = channelPriceConfig?.InventLocationId;

                OrgUnitLocation channelStore = allStores?.FirstOrDefault(x => string.Equals(x.InventoryLocationId, channelWarehouse, StringComparison.OrdinalIgnoreCase));

                if (channelStore != null)
                {
                    return channelStore;
                }

                // Otherwise, at least return current channel store.
                return new OrgUnitLocation()
                {
                    InventoryLocationId = channelPriceConfig.InventLocationId,
                };
            }
        }

        private static Dictionary<string, decimal> GetItemUnitQuantities(IEnumerable<SalesLine> salesLines)
        {
            // ItemId|UOM => Quantity lookup
            Dictionary<string, decimal> itemUnitQuantites = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            foreach (SalesLine saleLine in salesLines)
            {
                decimal existingQuantity;
                if (!saleLine.IsVoided)
                {
                    string key = ConstructItemUnitGroupKey(saleLine);
                    itemUnitQuantites.TryGetValue(key, out existingQuantity);

                    itemUnitQuantites[key] = existingQuantity + saleLine.Quantity;
                }
            }

            return itemUnitQuantites;
        }

        private static string ConstructItemUnitGroupKey(SalesLine salesLine)
        {
            ThrowIf.Null(salesLine, nameof(salesLine));

            return string.Concat(salesLine.ItemId, ItemUnitKeyDelimiter,
                (salesLine.UnitOfMeasureSymbol == null) ? string.Empty : salesLine.UnitOfMeasureSymbol);
        }

        /// <summary>
        /// Filters the trade agreements by matched inventory dimensions of the salesLine.
        /// </summary>
        /// <param name="agreements">The price trade agreements.</param>
        /// <param name="salesLine">Current sales line.</param>
        /// <param name="priceContext">Current price context instance.</param>
        /// <returns>The trade agreements filtered by inventory dimensions.</returns>
        internal static List<TradeAgreement> FilterTradeAgreementsByInventoryDimensions(IList<TradeAgreement> agreements, SalesLine salesLine, PriceContext priceContext, IEnumerable<OrgUnitLocation> allStores = null, ChannelPriceConfiguration channelPriceConfig = null)
        {
            List<TradeAgreement> candidateTradeAgreements = new List<TradeAgreement>();

            if (salesLine != null)
            {
                OrgUnitLocation matchedChannel = GetMatchedChannel(salesLine, allStores, channelPriceConfig);
                candidateTradeAgreements = FilterTradeAgreementsByMatchedChannel(agreements, salesLine, matchedChannel);
            }

            return candidateTradeAgreements;
        }

        private static List<TradeAgreement> FilterTradeAgreementsByTrackingDimensions(IList<TradeAgreement> agreements, SalesLine salesLine)
        {
            IEnumerable<TradeAgreement> candidateAgreements = agreements.Where(ta =>
                ta.SerialNumber.IsNullOrEmpty() || string.Equals(ta.SerialNumber, salesLine.SerialNumber, StringComparison.Ordinal));

            return candidateAgreements.ToList();
        }

        private static TradeAgreementPriceLine CalculateAgreementPriceLine(
               SalesLine salesLine,
               PriceContext priceContext,
               IList<TradeAgreement> tradeAgreementRules,
               DiscountParameters discountParameters,
               Dictionary<string, decimal> itemUnitQuantities,
               List<SalesLine> variants)
        {
            var quantity = salesLine.Quantity;

            // count all occurrences for this item if this is a transaction
            if (priceContext.PriceCalculationMode == PricingCalculationMode.Transaction)
            {
                string key = ConstructItemUnitGroupKey(salesLine);
                itemUnitQuantities.TryGetValue(key, out quantity);

                if (quantity == decimal.Zero)
                {
                    quantity = 1m;
                }
            }

            var activeDate = (salesLine.SalesDate != null) ? salesLine.SalesDate.Value : priceContext.ActiveDate;

            return GetActiveTradeAgreementPriceAndGroup(
                tradeAgreementRules,
                discountParameters,
                priceContext.CurrencyCode,
                salesLine.ItemId,
                salesLine.OriginalSalesOrderUnitOfMeasure,
                Discount.GetUnitOfMeasure(salesLine),
                salesLine.Variant,
                salesLine.UnitOfMeasureConversion,
                quantity,
                priceContext.CustomerAccount,
                priceContext.CustomerPriceGroup,
                InternalPriceContextHelper.GetApplicablePriceGroupsForPrice(priceContext, salesLine.CatalogIds),
                priceContext,
                activeDate,
                variants);
        }

        private static TradeAgreementPriceLine GetActiveTradeAgreementPriceAndGroup(
               IList<TradeAgreement> tradeAgreementRules,
               DiscountParameters priceParameters,
               string currencyCode,
               string itemId,
               string defaultSalesUnit,
               string salesUnit,
               ProductVariant variantLine,
               UnitOfMeasureConversion unitOfMeasureConversion,
               decimal quantity,
               string customerId,
               string customerPriceGroup,
               IEnumerable<string> channelPriceGroupIds,
               PriceContext priceContext,
               DateTimeOffset activeDate,
               List<SalesLine> variants)
        {
            PriceResult result = GetPriceResultOfActiveTradeAgreement(
                tradeAgreementRules,
                priceParameters,
                currencyCode,
                itemId,
                defaultSalesUnit,
                salesUnit,
                variantLine,
                unitOfMeasureConversion,
                quantity,
                customerId,
                customerPriceGroup,
                channelPriceGroupIds,
                priceContext,
                activeDate,
                variants);

            TradeAgreementPriceLine price = null;

            if (result.TradeAgreementRecordId != 0 || result.Price != decimal.Zero)
            {
                price = new TradeAgreementPriceLine();
                price.RecordId = result.TradeAgreementRecordId;
                price.Value = result.Price;
                price.PriceMethod = PriceMethod.Fixed;
                price.CustPriceGroup = result.CustPriceGroup;
                price.OriginId = price.RecordId.ToString();
            }

            return price;
        }

        /// <summary>
        /// Get struct PriceResult from active trade agreement.
        /// Struct PriceResult contains India MaxRetailPrice. Currently there is a field �Max. retail price� in the form price/discount agreement journal
        /// (Navigation path: Main Menu > Sales and marketing > Journal > price/discount agreement journal).
        /// The field will be visible only when the logged on company is an India company. And it is optional.
        /// User can use this field to specify different MRP values on different sites and warehouses for the same item. And when the trade agreement applies to a transaction,
        /// the MRP value should flow to the MRP field of the transaction as the default value.
        /// So current change is when fetching the superset of trade agreements which could apply to all of these items and customer for the given date,
        /// also takes field MAXIMUMRETAILPRICE_IN through the stored procedures GETALLDISCOUNTTRADEAGREEMENTS/ GETALLTRADEAGREEMENTS/ GETTRADEAGREEMENTS.
        /// Then return the whole struct PriceResult  rather than PriceResult.Price.
        /// </summary>
        /// <param name="tradeAgreementRules">The trade agreement rules.</param>
        /// <param name="priceParameters">The price parameters.</param>
        /// <param name="currencyCode">The currency code.</param>
        /// <param name="itemId">The item Id.</param>
        /// <param name="defaultSalesUnit">The default sales unit.</param>
        /// <param name="salesUnit">The sales unit.</param>
        /// <param name="variantLine">The variant line.</param>
        /// <param name="unitOfMeasureConversion">The UnitOfMeasure Conversion.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="customerId">The customer Id.</param>
        /// <param name="customerPriceGroup">The customer price group.</param>
        /// <param name="channelPriceGroupIds">The channel price group Ids.</param>
        /// <param name="priceContext">Price context.</param>
        /// <param name="activeDate">The active date.</param>
        /// <param name="variants">The variants.</param>
        /// <returns>The PriceResult of active trade agreement.</returns>
        private static PriceResult GetPriceResultOfActiveTradeAgreement(
            IList<TradeAgreement> tradeAgreementRules,
            DiscountParameters priceParameters,
            string currencyCode,
            string itemId,
            string defaultSalesUnit,
            string salesUnit,
            ProductVariant variantLine,
            UnitOfMeasureConversion unitOfMeasureConversion,
            decimal quantity,
            string customerId,
            string customerPriceGroup,
            IEnumerable<string> channelPriceGroupIds,
            PriceContext priceContext,
            DateTimeOffset activeDate,
            List<SalesLine> variants)
        {
            PriceResult result;
            variantLine = variantLine ?? new ProductVariant();

            decimal quantityOnDefaultSOM = UnitOfMeasureOperations.Convert(unitOfMeasureConversion, quantity);

            // Get basic arguments for Price evaluation
            RetailPriceArgs args = new RetailPriceArgs()
            {
                Barcode = string.Empty,
                CurrencyCode = currencyCode,
                CustomerId = customerId,
                Dimensions = variantLine,
                DefaultSalesUnitOfMeasure = defaultSalesUnit,
                ItemId = itemId,
                PriceGroups = channelPriceGroupIds.AsReadOnly(),
                Quantity = quantity,
                QuantityOnDefaultSOM = quantityOnDefaultSOM,
                SalesUOM = salesUnit,
                UnitOfMeasureConversion = unitOfMeasureConversion,
            };

            // Get the active retail price - checks following prices brackets in order: Customer TAs, Store price group TAs, 'All' TAs.
            // First bracket to return a price 'wins'. Each bracket returns the lowest price it can find.
            result = FindPriceAgreement(tradeAgreementRules, priceParameters, args, priceContext, activeDate, variants);

            // Direct customer TA price would have been caught above.
            // Compare against customer price group TAs now and override if lower than previously found price (or if previously found price was 0).
            if (!string.IsNullOrEmpty(customerId)
                && !string.IsNullOrEmpty(customerPriceGroup)
                && !channelPriceGroupIds.Contains(customerPriceGroup))
            {
                // Customer price group
                args.PriceGroups = new ReadOnlyCollection<string>(new[] { customerPriceGroup });
                PriceResult customerResult = FindPriceAgreement(tradeAgreementRules, priceParameters, args, priceContext, activeDate, variants);

                // Pick the Customer price if either the Retail price is ZERO, or the Customer Price is non-zero AND lower
                if ((result.Price == decimal.Zero)
                    || ((customerResult.Price > decimal.Zero) && (customerResult.Price <= result.Price)))
                {
                    result = customerResult;
                }
            }

            return result;
        }


        private static PriceResult FindPriceAgreement(
               IList<TradeAgreement> tradeAgreementRules,
               DiscountParameters priceParameters,
               RetailPriceArgs args,
               PriceContext priceContext,
               DateTimeOffset activeDate,
               List<SalesLine> variants)
        {
            // First we get the price according to the base UOM
            PriceAgreementArgs p = args.AgreementArgsForDefaultSales();
            PriceResult result = ApplyPriceTradeAgreements(tradeAgreementRules, p, priceParameters, priceContext, activeDate, variants);

            // Is the current UOM something different than the base UOM?
            if (args.SalesUOM != args.DefaultSalesUnitOfMeasure)
            {
                // Then let's see if we find some price agreement for that UOM
                p = args.AgreementArgsForSale();
                PriceResult salesUOMResult = ApplyPriceTradeAgreements(tradeAgreementRules, p, priceParameters, priceContext, activeDate, variants);

                // If there is a price found then we return that as the price
                if (salesUOMResult.Price > decimal.Zero)
                {
                    return salesUOMResult;
                }
                else
                {
                    return new PriceResult(0m, 0);
                }
            }

            // else we return baseUOM price multiplied with the unit qty factor.
            return result;
        }


        private static PriceResult ApplyPriceTradeAgreements(
                IList<TradeAgreement> tradeAgreementRules,
                PriceAgreementArgs args,
                DiscountParameters priceParameters,
                PriceContext priceContext,
                DateTimeOffset activeDate,
                List<SalesLine> variants)
        {
            bool searchAgain = true;
            PriceResult priceResult = new PriceResult(0M, 0);

            // For price trade agreements, only Item Code = Item or AllItems are supported.
            var itemCodes = new PriceDiscountItemCode[] { PriceDiscountItemCode.Item, PriceDiscountItemCode.AllItems };
            var accountCodes = new PriceDiscountAccountCode[] { PriceDiscountAccountCode.Customer, PriceDiscountAccountCode.CustomerGroup, PriceDiscountAccountCode.AllCustomers };

            // Search through combinations of item/account codes from most to least specific.
            // This needs to match the behavior of AX code PriceDisc.findPriceAgreement().
            foreach (var accountCode in accountCodes)
            {
                foreach (var itemCode in itemCodes)
                {
                    if (priceParameters.Activation(PriceDiscountType.PriceSales, accountCode, itemCode))
                    {
                        IList<string> accountRelations = args.GetAccountRelations(accountCode);
                        string itemRelation = args.GetItemRelation(itemCode);

                        if (accountRelations.All(a => ValidRelation(accountCode, a)) &&
                            ValidRelation(itemCode, itemRelation))
                        {
                            IEnumerable<TradeAgreement> tradeAgreements = FindPriceAgreements(tradeAgreementRules, args, itemCode, accountCode, priceContext, activeDate, variants);
                            PriceResult currentPriceResult = GetBestPriceAgreement(tradeAgreements, out searchAgain);

                            if (priceResult.Price == 0M ||
                                (currentPriceResult.Price > 0M && currentPriceResult.Price < priceResult.Price))
                            {
                                priceResult = currentPriceResult;
                            }

                            if (!searchAgain)
                            {
                                break;
                            }
                        }
                    }
                }

                if (!searchAgain)
                {
                    break;
                }
            }

            return priceResult;
        }

        private static bool ValidRelation(PriceDiscountItemCode itemCode, string relation)
        {
            bool ok = true;

            if (!string.IsNullOrEmpty(relation) && (itemCode == PriceDiscountItemCode.AllItems))
            {
                ok = false;
            }

            if (string.IsNullOrEmpty(relation) && (itemCode != PriceDiscountItemCode.AllItems))
            {
                ok = false;
            }

            return ok;
        }

        internal static bool ValidRelation(PriceDiscountAccountCode accountCode, string relation)
        {
            bool ok = true;

            if (!string.IsNullOrEmpty(relation) && (accountCode == PriceDiscountAccountCode.AllCustomers))
            {
                ok = false;
            }

            if (string.IsNullOrEmpty(relation) && (accountCode != PriceDiscountAccountCode.AllCustomers))
            {
                ok = false;
            }

            return ok;
        }

        private static void RetainTopPriorityTradeAgreements(
            List<TradeAgreement> agreements,
            PriceContext priceContext)
        {
            if (agreements != null && agreements.Count > 1)
            {
                // Makes sure it's all customer group trade agreements.
                if (!agreements.Where(p => p.AccountCode != PriceDiscountAccountCode.CustomerGroup || string.IsNullOrEmpty(p.AccountRelation)).Any())
                {
                    int priority = 0;

                    int highestPriority = agreements.Max(p => priceContext.PriceGroupIdToPriorityDictionary.TryGetValue(p.AccountRelation, out priority) ? priority : 0);

                    agreements.RemoveAll(p => priceContext.PriceGroupIdToPriorityDictionary.TryGetValue(p.AccountRelation, out priority) && priority < highestPriority);
                }
            }
        }

        internal static IEnumerable<TradeAgreement> FindPriceAgreements(
               IList<TradeAgreement> tradeAgreementRules,
               PriceAgreementArgs args,
               PriceDiscountItemCode itemCode,
               PriceDiscountAccountCode accountCode,
               PriceContext priceContext,
               DateTimeOffset activeDate,
               List<SalesLine> variants)
        {
            string itemRelation = args.GetItemRelation(itemCode);
            IList<string> accountRelations = args.GetAccountRelations(accountCode);
            string unitId = args.GetUnitId(itemCode);

            if (!tradeAgreementRules.Any())
            {
                return new List<TradeAgreement>(0);
            }

            List<TradeAgreement> tradeAgreementsOfVariantUnFilteredByQuantity = new List<TradeAgreement>();
            List<TradeAgreement> tradeAgreementsOfMasterOrProduct = new List<TradeAgreement>();

            ITradeAgreementSorter sorter = PricingEngineExtensionRepository.GetTradeAgreementSorter();
            sorter.SetPriceContext(priceContext);

            // Get the initial filtered trade agreements, not checking quantity.
            foreach (TradeAgreement t in tradeAgreementRules)
            {
                if (t.ItemRelation.Equals(itemRelation, StringComparison.OrdinalIgnoreCase)
                    && t.ItemCode == itemCode
                    && t.AccountCode == accountCode
                    && accountRelations.Contains(t.AccountRelation)
                    && t.Currency.Equals(args.CurrencyCode, StringComparison.OrdinalIgnoreCase)
                    && (t.FromDate.DateTime <= activeDate.Date || t.FromDate.DateTime <= NoDate)
                    && (t.ToDate.DateTime >= activeDate.Date || t.ToDate.DateTime <= NoDate)
                    && (string.IsNullOrWhiteSpace(t.UnitOfMeasureSymbol)
                        || t.UnitOfMeasureSymbol.Equals(unitId, StringComparison.OrdinalIgnoreCase))
                    && t.IsVariantMatch(args.Dimensions))
                {
                    if (t.IsVariant)
                    {
                        tradeAgreementsOfVariantUnFilteredByQuantity.Add(t);
                    }
                    else
                    {
                        if (t.IsQuantityMatch(args.Quantity))
                        {
                            tradeAgreementsOfMasterOrProduct.Add(t);
                        }
                    }
                }
            }

            // For variants
            if (args.IsVariant)
            {
                List<TradeAgreement> tradeAgreementsOfVariant = new List<TradeAgreement>();
                List<TradeAgreement> tradeAgreementsOfVariantExactMatch = new List<TradeAgreement>();

                foreach (TradeAgreement t in tradeAgreementsOfVariantUnFilteredByQuantity)
                {
                    if (t.IsVariant)
                    {
                        decimal aggregatedQuantityByAgreementVariant = decimal.Zero;
                        foreach (SalesLine salesLine in variants)
                        {
                            if (string.Equals(args.ItemId, salesLine.ItemId, StringComparison.OrdinalIgnoreCase) && t.IsVariantMatch(salesLine.Variant))
                            {
                                decimal quantityOnDefaultSOM = UnitOfMeasureOperations.Convert(salesLine.UnitOfMeasureConversion, salesLine.Quantity);

                                aggregatedQuantityByAgreementVariant += quantityOnDefaultSOM;
                            }
                        }

                        if (aggregatedQuantityByAgreementVariant == decimal.Zero)
                        {
                            aggregatedQuantityByAgreementVariant = 1m;
                        }

                        if (t.IsQuantityMatch(aggregatedQuantityByAgreementVariant))
                        {
                            if (t.IsVariantExactMatch(args.Dimensions))
                            {
                                tradeAgreementsOfVariantExactMatch.Add(t);
                            }

                            tradeAgreementsOfVariant.Add(t);
                        }
                    }
                }

                // 1. Return exact matches if any
                if (tradeAgreementsOfVariantExactMatch != null && tradeAgreementsOfVariantExactMatch.Any())
                {
                    if (accountCode == PriceDiscountAccountCode.CustomerGroup)
                    {
                        RetainTopPriorityTradeAgreements(tradeAgreementsOfVariantExactMatch, priceContext);
                    }

                    SortTradeAgreementForExactMatch(tradeAgreementsOfVariantExactMatch, sorter);

                    return tradeAgreementsOfVariantExactMatch;
                }

                // 2. Return (partial) variant matches if any.
                if (tradeAgreementsOfVariant.Count > 0)
                {
                    if (accountCode == PriceDiscountAccountCode.CustomerGroup)
                    {
                        RetainTopPriorityTradeAgreements(tradeAgreementsOfVariant, priceContext);
                    }

                    sorter.SpecifyProductVariantForOrderComparisonInPartialMatch(args.Dimensions);
                    tradeAgreementsOfVariant.Sort(sorter.CompareTradeAgreementOrderForPartialVariantMatchConsiderSiteWarehouse);
                    return tradeAgreementsOfVariant;
                }
            }

            // 3. Return non-variant matches.
            if (accountCode == PriceDiscountAccountCode.CustomerGroup)
            {
                RetainTopPriorityTradeAgreements(tradeAgreementsOfMasterOrProduct, priceContext);
            }

            SortTradeAgreementForExactMatch(tradeAgreementsOfMasterOrProduct, sorter);

            return tradeAgreementsOfMasterOrProduct;
        }



        /// <summary>
        /// Find the line discount trade agreement for given criteria.
        /// For line discount trade agreement, the trade agreements are filtered in such an order:
        /// - For a product master,
        /// 1. Find the trade agreement which is an exact match to a product variant. And if any, return the collection of all exact matched trade agreements.
        /// 2. Find the trade agreement which is defined on the product master. And if any, return the collection of all matched trade agreements.
        /// - For a distinct product, return the collection of all matched trade agreements.
        /// </summary>
        /// <param name="tradeAgreementRules">Target line discount trade agreement to search.</param>
        /// <param name="args">An instance of PriceAgreementArgs.</param>
        /// <param name="itemCode">Target item code.</param>
        /// <param name="itemRelation">Target item relation.</param>
        /// <param name="accountCode">Target account code.</param>
        /// <param name="accountRelation">Target account relation.</param>
        /// <param name="priceContext">Current price context instance.</param>
        /// <param name="activeDate">Target date to find the trade agreement.</param>
        /// <param name="variants">Target product variant to find the trade agreement.</param>
        /// <returns>Returns the filtered line discount trade agreements.</returns>
        internal static IEnumerable<TradeAgreement> FindLineDiscountAgreements(
            IDictionary<string, IList<TradeAgreement>> tradeAgreementRules,
            PriceAgreementArgs args,
            PriceDiscountItemCode itemCode,
            string itemRelation,
            PriceDiscountAccountCode accountCode,
            string accountRelation,
            PriceContext priceContext,
            DateTimeOffset activeDate,
            List<SalesLine> variants)
        {
            string unitId = args.GetUnitId(itemCode);

            // price trade agreements are always item-specific, so first filter by itemId.
            IList<TradeAgreement> rulesForItem;
            if (!tradeAgreementRules.TryGetValue(itemRelation, out rulesForItem))
            {
                return new List<TradeAgreement>(0);
            }

            SalesLine currentSalesLine = variants.FirstOrDefault();

            List<TradeAgreement> candidateTradeAgreements = FilterTradeAgreementsByInventoryDimensions(rulesForItem, currentSalesLine, priceContext);
            candidateTradeAgreements = FilterTradeAgreementsByTrackingDimensions(candidateTradeAgreements, currentSalesLine);

            List<TradeAgreement> tradeAgreementsOfVariantUnFilteredByQuantity = new List<TradeAgreement>();
            List<TradeAgreement> tradeAgreementsOfMasterOrProduct = new List<TradeAgreement>();

            ITradeAgreementSorter sorter = PricingEngineExtensionRepository.GetTradeAgreementSorter();
            sorter.SetPriceContext(priceContext);

            // Get the initial filtered trade agreements, not checking quantity.
            foreach (TradeAgreement t in candidateTradeAgreements)
            {
                if (t.ItemRelation.Equals(itemRelation, StringComparison.OrdinalIgnoreCase)
                    && t.ItemCode == itemCode
                    && t.AccountRelation.Equals(accountRelation, StringComparison.OrdinalIgnoreCase)
                    && t.AccountCode == accountCode
                    && t.Currency.Equals(args.CurrencyCode, StringComparison.OrdinalIgnoreCase)
                    && (t.FromDate.DateTime <= activeDate.Date || t.FromDate.DateTime <= NoDate)
                    && (t.ToDate.DateTime >= activeDate.Date || t.ToDate.DateTime <= NoDate)
                    && (string.IsNullOrWhiteSpace(t.UnitOfMeasureSymbol)
                        || t.UnitOfMeasureSymbol.Equals(unitId, StringComparison.OrdinalIgnoreCase))
                    && t.IsVariantMatch(args.Dimensions))
                {
                    if (t.IsVariant)
                    {
                        tradeAgreementsOfVariantUnFilteredByQuantity.Add(t);
                    }
                    else
                    {
                        if (t.IsQuantityMatch(args.Quantity))
                        {
                            tradeAgreementsOfMasterOrProduct.Add(t);
                        }
                    }
                }
            }

            // For variants
            if (args.IsVariant)
            {
                List<TradeAgreement> tradeAgreementsOfVariantExactMatch = new List<TradeAgreement>();

                // Check quantity limits for variants.
                foreach (TradeAgreement t in tradeAgreementsOfVariantUnFilteredByQuantity)
                {
                    if (t.IsVariant)
                    {
                        decimal aggregatedQuantityByAgreementVariant = decimal.Zero;
                        foreach (SalesLine salesLine in variants)
                        {
                            if (string.Equals(args.ItemId, salesLine.ItemId, StringComparison.OrdinalIgnoreCase) && t.IsVariantMatch(salesLine.Variant))
                            {
                                decimal quantityOnDefaultSOM = UnitOfMeasureOperations.Convert(salesLine.UnitOfMeasureConversion, salesLine.Quantity);

                                aggregatedQuantityByAgreementVariant += quantityOnDefaultSOM;
                            }
                        }

                        if (aggregatedQuantityByAgreementVariant == decimal.Zero)
                        {
                            aggregatedQuantityByAgreementVariant = 1m;
                        }

                        if (t.IsQuantityMatch(aggregatedQuantityByAgreementVariant))
                        {
                            if (t.IsVariantExactMatch(args.Dimensions))
                            {
                                tradeAgreementsOfVariantExactMatch.Add(t);
                            }
                        }
                    }
                }

                // 1. Return exact matches if any
                if (tradeAgreementsOfVariantExactMatch != null && tradeAgreementsOfVariantExactMatch.Any())
                {
                    if (accountCode == PriceDiscountAccountCode.CustomerGroup)
                    {
                        RetainTopPriorityTradeAgreements(tradeAgreementsOfVariantExactMatch, priceContext);
                    }

                    SortTradeAgreementForExactMatch(tradeAgreementsOfVariantExactMatch, sorter);

                    return tradeAgreementsOfVariantExactMatch;
                }
            }

            // 2. Return non-variant matches.
            if (accountCode == PriceDiscountAccountCode.CustomerGroup)
            {
                RetainTopPriorityTradeAgreements(tradeAgreementsOfMasterOrProduct, priceContext);
            }

            SortTradeAgreementForExactMatch(tradeAgreementsOfMasterOrProduct, sorter);

            return tradeAgreementsOfMasterOrProduct;
        }


        private static void SortTradeAgreementForExactMatch(List<TradeAgreement> agreements, ITradeAgreementSorter sorter)
        {
            agreements.Sort(sorter.CompareTradeAgreementOrderForExactMatchConsiderSiteWarehouse);
        }


        /// <summary>
        /// This function searches a list of trade agreements (assumed to be sorted with lowest prices first).
        ///   It calculates the price for each trade agreement, returning the lowest amount, and optionally stopping
        ///   early if it encounters a trade agreement with SearchAgain=False.
        /// </summary>
        /// <param name="tradeAgreements">List of price agreements, sorted by Amount ascending.</param>
        /// <param name="searchAgain">Out parameter indicating whether SearchAgain=False was hit.</param>
        /// <returns>Best price agreement price for the given list of trade agreements.</returns>
        private static PriceResult GetBestPriceAgreement(IEnumerable<TradeAgreement> tradeAgreements, out bool searchAgain)
        {
            decimal price = 0;
            decimal maxRetailPrice = 0M;
            string custPriceGroup = null;
            searchAgain = true;
            long tradeAgreementRecordId = 0;
            foreach (var ta in tradeAgreements)
            {
                decimal priceUnit = (ta.PriceUnit != 0) ? ta.PriceUnit : 1;
                decimal markup = ta.ShouldIncludeMarkup ? ta.MarkupAmount : 0;
                decimal currentPrice = (ta.Amount / priceUnit) + markup;

                if ((price == 0M) || (currentPrice != 0M && price > currentPrice))
                {
                    price = currentPrice;
                    maxRetailPrice = ta.MaximumRetailPriceIndia;
                    tradeAgreementRecordId = ta.RecordId;
                    if (ta.AccountCode == PriceDiscountAccountCode.CustomerGroup)
                    {
                        custPriceGroup = ta.AccountRelation;
                    }
                }

                if (!ta.ShouldSearchAgain)
                {
                    searchAgain = false;
                    break;
                }
            }

            return new PriceResult(price, tradeAgreementRecordId, maxRetailPrice, custPriceGroup);
        }

        private static List<TradeAgreement> FilterTradeAgreementsByMatchedChannel(IList<TradeAgreement> agreements, SalesLine salesLine, OrgUnitLocation matchedChannel)
        {
            string itemId = salesLine.ItemId;

            IEnumerable<TradeAgreement> candidateAgreements = agreements.Where(ta =>
                (ta.ItemCode != PriceDiscountItemCode.Item) ||
                (string.Equals(ta.ItemRelation, itemId, StringComparison.OrdinalIgnoreCase) && IsInventoryDimensionMatched(ta, matchedChannel)));

            return candidateAgreements.ToList();
        }

        private static bool IsInventoryDimensionMatched(TradeAgreement tradeAgreement, OrgUnitLocation fulfillmentStore)
        {
            if (!string.IsNullOrEmpty(tradeAgreement.WarehouseId))
            {
                return string.Equals(tradeAgreement.WarehouseId, fulfillmentStore.InventoryLocationId, StringComparison.OrdinalIgnoreCase);
            }

            return string.IsNullOrEmpty(tradeAgreement.SiteId) || string.Equals(tradeAgreement.SiteId, fulfillmentStore.InventorySiteId, StringComparison.OrdinalIgnoreCase);
        }
    }
}
