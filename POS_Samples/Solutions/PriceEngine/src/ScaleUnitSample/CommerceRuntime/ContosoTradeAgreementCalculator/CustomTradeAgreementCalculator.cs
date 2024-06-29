namespace Contoso.CommerceRuntime.PricingEngine
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Services.PricingEngine;
    using Microsoft.Dynamics.Retail.Diagnostics;
    using Microsoft.Dynamics.Retail.Diagnostics.Extensions;

    /// <summary>
    /// This class finds all possible price trade agreement lines for items and creates a set
    ///  of price trade agreement price lines for each item line, keyed by item line line Id.
    /// </summary>
    public class CustomTradeAgreementCalculator : IPriceTradeAgreementCalculator
    {
        private const string ItemUnitKeyDelimiter = "|";
        private static readonly DateTime NoDate = new DateTime(1900, 1, 1);

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeAgreementCalculator" /> class.
        /// </summary>
        public CustomTradeAgreementCalculator()
        {
        }

        private enum Events
        {
            GetMatchedChannel,
        }

        /// <summary>
        /// Gets minimum and maximum date time from set of sales lines or default date/time.
        /// </summary>
        /// <param name="salesLines">Lines to read date range from.</param>
        /// <param name="defaultDate">Date to fall back to if lines are missing dates.</param>
        /// <returns>Truncated min and max date suitable for querying price rules.</returns>
        internal static Tuple<DateTimeOffset, DateTimeOffset> GetMinAndMaxActiveDates(IEnumerable<SalesLine> salesLines, DateTimeOffset defaultDate)
        {
            DateTimeOffset? minDateTime = null;
            DateTimeOffset? maxDateTime = null;

            // if we have sales lines, find any min/max if any dates are specified
            if (salesLines != null)
            {
                foreach (var line in salesLines)
                {
                    if (line.SalesDate != null)
                    {
                        if (minDateTime == null || line.SalesDate < minDateTime)
                        {
                            minDateTime = line.SalesDate;
                        }

                        if (maxDateTime == null || line.SalesDate > maxDateTime)
                        {
                            maxDateTime = line.SalesDate;
                        }
                    }
                }
            }

            // default dates if none found
            minDateTime = minDateTime ?? defaultDate;
            maxDateTime = maxDateTime ?? defaultDate;

            // extend range to contain default date if necessary
            minDateTime = (minDateTime.Value < defaultDate) ? minDateTime : defaultDate;
            maxDateTime = (maxDateTime.Value > defaultDate) ? maxDateTime : defaultDate;

            // return discovered date range, truncated to midnight
            return new Tuple<DateTimeOffset, DateTimeOffset>(minDateTime.Value, maxDateTime.Value);
        }

        /// <summary>
        /// Get all price groups for price from price context.
        /// </summary>
        /// <param name="priceContext">Price context.</param>
        /// <returns>All price groups for price.</returns>
        /// <remarks>We could have made it an C# extension. Leave it here for all price context logic.</remarks>
        internal static ISet<string> GetAllPriceGroupsForPrice(PriceContext priceContext)
        {
            ThrowIf.Null(priceContext, nameof(priceContext));

            HashSet<string> allPriceGroups = new HashSet<string>(priceContext.AllPriceGroupsExceptCatalogsForPrice, StringComparer.OrdinalIgnoreCase);

            foreach (KeyValuePair<long, ISet<string>> priceGroups in priceContext.CatalogPriceGroups)
            {
                allPriceGroups.UnionWith(priceGroups.Value);
            }

            return allPriceGroups;
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


        /// <inheritdoc/>
        public CalculatedPrices CalculatePrices(
            IEnumerable<SalesLine> salesLines,
            PriceContext priceContext,
            IPricingDataAccessor pricingDataManager)
        {
            ThrowIf.Null(salesLines, nameof(salesLines));
            ThrowIf.Null(priceContext, nameof(priceContext));
            ThrowIf.Null(pricingDataManager, nameof(pricingDataManager));

            Tuple<DateTimeOffset, DateTimeOffset> dateRange = GetMinAndMaxActiveDates(salesLines, priceContext.ActiveDate);
            ReadOnlyCollection<TradeAgreement> tradeAgreements = null;

            IEnumerable<OrgUnitLocation> allStores = ((IPricingDataAccessorV5)pricingDataManager).GetAllStores() as IEnumerable<OrgUnitLocation>;

            ChannelPriceConfiguration channelPriceConfig = pricingDataManager.GetChannelPriceConfiguration();

            // look up all trade agreements for given context, items and search criteria
            #pragma warning disable CA1851
            HashSet<TradeAgreementSearchCriteria> tradeAgreementSearchCriteria =
                new HashSet<TradeAgreementSearchCriteria>(
                    salesLines.Select(sl => new TradeAgreementSearchCriteria(sl.ItemId, GetMatchedChannel(sl, priceContext, allStores, channelPriceConfig).InventoryLocationId ?? string.Empty, sl.UnitOfMeasureSymbol ?? string.Empty)));
            #pragma warning disable CA1851

            tradeAgreements = pricingDataManager.ReadPriceTradeAgreements(
                tradeAgreementSearchCriteria,
                GetAllPriceGroupsForPrice(priceContext),
                priceContext.CustomerAccount,
                dateRange.Item1,
                dateRange.Item2,
                priceContext.CurrencyCode,
                QueryResultSettings.AllRecords) as ReadOnlyCollection<TradeAgreement>;

            // Return directly if there is no trade agreement line loaded.
            if (!tradeAgreements.Any())
            {
                return new CalculatedPrices();
            }

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
                candidateTradeAgreements = FilterTradeAgreementsByInventoryDimensions(tradeAgreements, salesLine, priceContext, allStores, channelPriceConfig);
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

        /// <summary>
        /// True if there is a valid relation between the account code and relation.
        /// </summary>
        /// <param name="accountCode">The customer account code to validate against (customer/customer group/all).</param>
        /// <param name="relation">The relation to validate.</param>
        /// <returns>True if the relation is compatible with the account code.</returns>
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

        /// <summary>
        /// Convert trade agreement collection to a dictionary with item id as key.
        /// </summary>
        /// <param name="tradeAgreements">A read-only collection or target trade agreements.</param>
        /// <returns>A dictionary with item id as key.</returns>
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

        /// <summary>
        /// Find the best matched price trade agreement for given criteria.
        /// For price trade agreement, the trade agreements are filtered in such an order:
        /// - For a product master,
        /// 1. Find the trade agreement which is an exact match to a product variant. And if any, return the collection of all exact matched trade agreements.
        /// 2. Find the trade agreement which is a partial match to a product variant. And if any, return the collection of all partial matched trade agreements.
        /// 3. Find the trade agreement which is defined on the product master. And if any, return the collection of all matched trade agreements.
        /// - For a distinct product, return the collection of all matched trade agreements.
        /// </summary>
        /// <param name="tradeAgreementRules">Target trade agreement to search.</param>
        /// <param name="args">An instance of PriceAgreementArgs.</param>
        /// <param name="itemCode">Target item code.</param>
        /// <param name="accountCode">Target account code.</param>
        /// <param name="priceContext">Current price context instance.</param>
        /// <param name="activeDate">Target date to find the trade agreement.</param>
        /// <param name="variants">Target product variant to find the trade agreement.</param>
        /// <returns>Returns the best matched price trade agreement.</returns>
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
                                decimal quantityOnDefaultSOM = salesLine.UnitOfMeasureConversion.Convert(salesLine.Quantity);

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
        /// Filters the trade agreements by matched inventory dimensions of the salesLine.
        /// </summary>
        /// <param name="agreements">The price trade agreements.</param>
        /// <param name="salesLine">Current sales line.</param>
        /// <param name="priceContext">Current price context instance.</param>
        /// <returns>The trade agreements filtered by inventory dimensions.</returns>
        internal static List<TradeAgreement> FilterTradeAgreementsByInventoryDimensions(IList<TradeAgreement> agreements, SalesLine salesLine, PriceContext priceContext, IEnumerable<OrgUnitLocation> allStores, ChannelPriceConfiguration channelPriceConfig)
        {
            List<TradeAgreement> candidateTradeAgreements = new List<TradeAgreement>();

            if (salesLine != null)
            {
                OrgUnitLocation matchedChannel = GetMatchedChannel(salesLine, priceContext, allStores, channelPriceConfig);
                candidateTradeAgreements = FilterTradeAgreementsByMatchedChannel(agreements, salesLine, matchedChannel);
            }

            return candidateTradeAgreements;
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

            decimal quantityOnDefaultSOM = unitOfMeasureConversion.Convert(quantity);

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
        /// This function takes arguments (customer, item, currency, etc.) related to price (trade) agreement
        /// as well as the set of currently enabled trade agreement types. It returns the best trade agreement
        /// price for the given constraints.
        /// As in AX, the method searches for a price on the given item which has been given to a
        /// customer, price group, or anyone (in given precedence order). If a price is found and marked as
        /// SearchAgain=False, the search will terminate. Otherwise, search for lowest price will continue.
        /// To recap, the logic is that three searches are done for customer, price group, and all, each bracket
        /// will return the lowest price it has for the constraints. If it has SearchAgain=True, then the search
        /// for lowest price continues to the next bracket.
        /// </summary>
        /// <param name="tradeAgreementRules">Trade agreements applicable to item Id.</param>
        /// <param name="args">Arguments for price agreement search.</param>
        /// <param name="priceParameters">Set of enabled price agreement types.</param>
        /// <param name="priceContext">Price context.</param>
        /// <param name="activeDate">Date to use for querying trade agreement rules.</param>
        /// <param name="variants">The variants.</param>
        /// <returns>
        /// Most applicable price for the given price agreement constraints.
        /// </returns>
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

        /// <summary>
        /// True if there is a valid relation between the item code and relation.
        /// </summary>
        /// <param name="itemCode">The item code to validate against (item/item group/all).</param>
        /// <param name="relation">The relation to validate.</param>
        /// <returns>True if the relation is compatible with the item code.</returns>
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
                GetUnitOfMeasure(salesLine),
                salesLine.Variant,
                salesLine.UnitOfMeasureConversion,
                quantity,
                priceContext.CustomerAccount,
                priceContext.CustomerPriceGroup,
                GetApplicablePriceGroups(priceContext, GetAllPriceGroupsForPrice(priceContext), salesLine.CatalogIds),
                priceContext,
                activeDate,
                variants);
        }

        /// <summary>
        /// Gets the unit of measure.
        /// </summary>
        /// <param name="salesLine">The sales line.</param>
        /// <returns>Unit of measure.</returns>
        internal static string GetUnitOfMeasure(SalesLine salesLine)
        {
            string unitOfMeasure = salesLine.SalesOrderUnitOfMeasure;
            if (string.IsNullOrWhiteSpace(unitOfMeasure))
            {
                unitOfMeasure = salesLine.UnitOfMeasureSymbol ?? string.Empty;
            }

            return unitOfMeasure;
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
                    return new PriceResult(result.Price * UnitOfMeasureOperations.GetFactorForQuantity(args.UnitOfMeasureConversion, args.Quantity), result.TradeAgreementRecordId, custPriceGroup: result.CustPriceGroup);
                }
            }

            // else we return baseUOM price multiplied with the unit qty factor.
            return result;
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

        /// <summary>
        /// Constructs the item unit group key from a sales line.
        /// </summary>
        /// <param name="salesLine">salesLine.</param>
        /// <returns>Group key.</returns>
        private static string ConstructItemUnitGroupKey(SalesLine salesLine)
        {
            ThrowIf.Null(salesLine, nameof(salesLine));

            return string.Concat(salesLine.ItemId, ItemUnitKeyDelimiter,
                (salesLine.UnitOfMeasureSymbol == null) ? string.Empty : salesLine.UnitOfMeasureSymbol);
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

        /// <summary>
        /// Filters the trade agreements by given matched channel. Only when ItemCode = 'Item', can user specify Site and Warehouse on trade agreement.
        /// </summary>
        /// <param name="agreements">The price trade agreements.</param>
        /// <param name="salesLine">Current sales line.</param>
        /// <param name="matchedChannel">Matched channel.</param>
        /// <returns>The trade agreements filtered by matched channel.</returns>
        private static List<TradeAgreement> FilterTradeAgreementsByMatchedChannel(IList<TradeAgreement> agreements, SalesLine salesLine, OrgUnitLocation matchedChannel)
        {
            string itemId = salesLine.ItemId;

            IEnumerable<TradeAgreement> candidateAgreements = agreements.Where(ta =>
                (ta.ItemCode != PriceDiscountItemCode.Item) ||
                (string.Equals(ta.ItemRelation, itemId, StringComparison.OrdinalIgnoreCase) && IsInventoryDimensionMatched(ta, matchedChannel)));

            return candidateAgreements.ToList();
        }

        /// <summary>
        /// Check if the site warehouse matches between the price trade agreement and the sales line's fulfillment store.
        /// Warehouse has higher priority than Site.
        /// - If ta.Warehouse is specified, compare it with the fulfillmentStore.
        /// - Else, compare ta.Site with the fulfillmentStore.
        /// - Always return true if no Site or Warehouse is specified on TA.
        /// </summary>
        /// <param name="tradeAgreement">The price trade agreement.</param>
        /// <param name="fulfillmentStore">The fulfillment store.</param>
        /// <returns>True if inventory dimensions match. Otherwise, false.</returns>
        private static bool IsInventoryDimensionMatched(TradeAgreement tradeAgreement, OrgUnitLocation fulfillmentStore)
        {
            if (!string.IsNullOrEmpty(tradeAgreement.WarehouseId))
            {
                return string.Equals(tradeAgreement.WarehouseId, fulfillmentStore.InventoryLocationId, StringComparison.OrdinalIgnoreCase);
            }

            return string.IsNullOrEmpty(tradeAgreement.SiteId) || string.Equals(tradeAgreement.SiteId, fulfillmentStore.InventorySiteId, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Filters the trade agreements with tracking dimensions of a SalesLine.
        /// </summary>
        /// <param name="agreements">The price trade agreements.</param>
        /// <param name="salesLine">The sales line.</param>
        /// <returns>The price trade agreements with matching tracking dimensions.</returns>
        private static List<TradeAgreement> FilterTradeAgreementsByTrackingDimensions(IList<TradeAgreement> agreements, SalesLine salesLine)
        {
            IEnumerable<TradeAgreement> candidateAgreements = agreements.Where(ta =>
                ta.SerialNumber.IsNullOrEmpty() || string.Equals(ta.SerialNumber, salesLine.SerialNumber, StringComparison.Ordinal));

            return candidateAgreements.ToList();
        }

        /// <summary>
        /// Gets the matched channel of the sales line, might be its site/warehouse, or derived from its fulfillment store.
        /// </summary>
        /// <param name="salesLine">The sales line.</param>
        /// <param name="priceContext">Price Context.</param>
        /// <returns>The matched channel for the given sales line.</returns>
        private static OrgUnitLocation GetMatchedChannel(SalesLine salesLine, PriceContext priceContext, IEnumerable<OrgUnitLocation> allStores, ChannelPriceConfiguration channelPriceConfig)
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
            if (!string.IsNullOrEmpty(salesLine.FulfillmentStoreId))
            {
                OrgUnitLocation fulfilmentStore = allStores?.FirstOrDefault(x => string.Equals(x.OrgUnitNumber, salesLine.FulfillmentStoreId, StringComparison.OrdinalIgnoreCase));

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
            string channelWarehouse = channelPriceConfig.InventLocationId;

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
}
