namespace Contoso.GasStationSample.CommerceRuntime.RequestHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using System.Collections.Concurrent;
    using Microsoft.Extensions.Logging;
    using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages.ProductAvailability;
    using System.Collections.ObjectModel;
    using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages.Inventory;
    using Microsoft.Dynamics.Commerce.Runtime.TransactionService;
    using Microsoft.Dynamics.Commerce.Runtime.TransactionService.Serialization;
    using Microsoft.Dynamics.Retail.Diagnostics;
    using Microsoft.Dynamics.Retail.Diagnostics.Extensions;
    using System.Linq;
    using System.Globalization;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

    public class SearchOrgUnitAvailabilitiesServiceRequestHandler : SingleAsyncRequestHandler<SearchOrgUnitAvailabilitiesServiceRequest>
    {
        private const int MaxNumberOfStoresToReturn = 10000;

        private enum Events
        {
            InventoryLookupSortOrderFallback,
            OrgUnitNameFallbackToInventLocationId,
        }

        protected override async Task<Response> Process(SearchOrgUnitAvailabilitiesServiceRequest request)
        {
            GetProductsDataRequest productsDataRequest = new GetProductsDataRequest(new long[] { request.ProductId }, QueryResultSettings.SingleRecord);
            IList<SimpleProduct> products = (await request.RequestContext.ExecuteAsync<EntityDataServiceResponse<SimpleProduct>>(productsDataRequest).ConfigureAwait(false)).PagedEntityCollection.Results;

            if (products == null || products.Count == 0)
            {
                throw new DataValidationException(
                    DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_ObjectNotFound,
                    string.Format("No products were found for the specified product identifier ({0}).", request.ProductId));
            }

            // Convert the product identifier to item and retail variant identifiers.
            var dataRequest = new ConvertProductIdsToProductLookupClausesDataRequest(new long[] { request.ProductId }, QueryResultSettings.SingleRecord);
            ProductLookupClause productLookupClause = (await request.RequestContext.Runtime.ExecuteAsync<EntityDataServiceResponse<ProductLookupClause>>(dataRequest, request.RequestContext).ConfigureAwait(false)).PagedEntityCollection.FirstOrDefault();

            UnitOfMeasure inventoryUnitOfMeasure = await GetInventoryUnitOfMeasureByItemId(request, productLookupClause.ItemId).ConfigureAwait(false);
            string inventoryUnitOfMeasureSymbol = products[0].DefaultUnitOfMeasure;
            string inventoryUnitOfMeasureDescription = string.Empty;

            if (inventoryUnitOfMeasure != null)
            {
                inventoryUnitOfMeasureSymbol = inventoryUnitOfMeasure.Symbol;
                inventoryUnitOfMeasureDescription = inventoryUnitOfMeasure.Description;
            }

            GetStoreAvailabilityRtsVersion rtsMethodVersionToCall = await GetRtsVersionForSearchOrgUnitsRequestAsync(request).ConfigureAwait(false);

            // Get the product item's inventory information by a real time AX call.
            string itemId = productLookupClause.ItemId;
            string variantId = productLookupClause.RetailVariantId;
            long currentStoreId = request.RequestContext.GetPrincipal().ChannelId;
            bool includeNonStoreWarehouses = true;
            var getStoreAvailabilityRealtimeRequest = new GetStoreAvailabilityRealtimeRequest(
                itemId, variantId, currentStoreId, includeNonStoreWarehouses,
                request.QueryResultSettings.Paging, rtsMethodVersionToCall, request.QueryResultSettings.Sorting, request.OrgUnitAvailabilitySearchCriteria);

            StoreAvailabilityRealtimeResponse getStoreAvailabilityRealtimeResponse = await request.RequestContext.ExecuteAsync<StoreAvailabilityRealtimeResponse>(getStoreAvailabilityRealtimeRequest).ConfigureAwait(false);
            ReadOnlyCollection<ItemAvailabilityStore> availabilities = getStoreAvailabilityRealtimeResponse.StoreAvailabilities.Results;

            GetStoreAvailabilityRtsVersion rtsMethodVersionCalled = getStoreAvailabilityRealtimeResponse.RtsMethodVersionCalled;
            List<OrgUnitAvailability> storeAvailabilities = new List<OrgUnitAvailability>();

            if (rtsMethodVersionCalled != GetStoreAvailabilityRtsVersion.V1)
            {
                Dictionary<string, OrgUnitLocation> storeOrgUnitLocations = await GetStoresInFullfilmentGroup(request).ConfigureAwait(false);

                IList<ItemAvailabilityStore> inventoryInfo = availabilities;
                OrgUnitLocation location;

                for (int i = 0; i < inventoryInfo.Count; i++)
                {
                    ItemAvailability itemAvailibility = new ItemAvailability
                    {
                        ItemId = inventoryInfo[i].ItemId,
                        ProductId = request.ProductId,
                        InventoryLocationId = inventoryInfo[i].InventoryLocationId,
                        AvailableQuantity = inventoryInfo[i].AvailableQuantity,
                        OrderedSum = inventoryInfo[i].OrderedSum,
                        PhysicalReserved = inventoryInfo[i].PhysicalReserved,
                        UnitOfMeasure = products[0].DefaultUnitOfMeasure,
                        InventoryUnitOfMeasure = inventoryUnitOfMeasureSymbol,
                        InventoryUnitOfMeasureDescription = inventoryUnitOfMeasureDescription,
                        ExtensionProperties = inventoryInfo[i].ExtensionProperties
                    };

                    OrgUnitLocation currentLocation;
                    string key = (inventoryInfo[i].OrgUnitName + "-" + inventoryInfo[i].InventoryLocationId).ToLowerInvariant();
                    if (storeOrgUnitLocations.TryGetValue(key, out currentLocation))
                    {
                        // If value is found in dictionary, we return the OrgUnitLocation object.
                        location = currentLocation;
                    }
                    else
                    {
                        // This is a case where a store was returned from the RTS method as in the current stores fulfillment group, but not the Channel Side.
                        // This case should should be uncommon, if so, the inventory information will return and look identical to a non-store Warehouse.
                        location = CreateWarehouseOrgUnitLocation(inventoryInfo[i]);
                    }

                    OrgUnitAvailability storeAvailability = new OrgUnitAvailability(location, new List<ItemAvailability> { itemAvailibility });
                    storeAvailabilities.Add(storeAvailability);
                }
            }
            else
            {
                var queryResultSettingsWithoutSorting = new QueryResultSettings(
                    request.QueryResultSettings.ColumnSet, request.QueryResultSettings.Paging,
                    new SortingInfo(), request.QueryResultSettings.ChangeTracking);
                var requestWithoutSorting = new SearchOrgUnitAvailabilitiesServiceRequest(
                    request.ProductId, request.OrgUnitAvailabilitySearchCriteria,
                    queryResultSettingsWithoutSorting);
                requestWithoutSorting.RequestContext = request.RequestContext;

                storeAvailabilities = await FilterOrgUnitAvailabilitiesByStore(requestWithoutSorting, availabilities, products, inventoryUnitOfMeasureSymbol, inventoryUnitOfMeasureDescription).ConfigureAwait(false);
                storeAvailabilities = SortOrgUnitAvailabilitiesWithPagination(storeAvailabilities, request.QueryResultSettings, currentStoreId);
            }

            return new EntityDataServiceResponse<OrgUnitAvailability>(storeAvailabilities.AsPagedResult());
        }

        private static async Task<UnitOfMeasure> GetInventoryUnitOfMeasureByItemId(SearchOrgUnitAvailabilitiesServiceRequest request, string itemId)
        {
            var getInventoryUnitsOfMeasureDataRequest = new GetItemInventoryUnitsOfMeasureDataRequest(itemId);
            PagedResult<UnitOfMeasure> result = (await request.RequestContext.ExecuteAsync<EntityDataServiceResponse<UnitOfMeasure>>(getInventoryUnitsOfMeasureDataRequest).ConfigureAwait(false)).PagedEntityCollection;

            int countOfInventoryUomResults = (result != null) ? result.Count() : 0;

            if (countOfInventoryUomResults != 1)
            {
                // the result may be null or have more than one record due to misconfiguration or customization
                // for backward compatibility, logging this instead of error.
                RetailLogger.Log.ProductAvailabilityServiceUnexpectedCountOfInventoryUomResults(countOfInventoryUomResults, request.ProductId);
                return null;
            }

            return result.Single();
        }

        internal static async Task<GetStoreAvailabilityRtsVersion> GetRtsVersionForSearchOrgUnitsRequestAsync(SearchOrgUnitAvailabilitiesServiceRequest request)
        {
            GetStoreAvailabilityRtsVersion rtsMethodVersionToCall;
            var searchCriteria = request.OrgUnitAvailabilitySearchCriteria;
            bool isSearchCriteriaNull = searchCriteria == null || (searchCriteria.OrgUnitName == null && searchCriteria.OrgUnitNumber == null);
            bool isLatestVersionForSearchCriteriaFeatureEnabled = await request.RequestContext.IsFeatureEnabledAsync(InventoryFeatureNames.InventoryLookupByStoreUsesLatestVersionForSearchCriteriaFeatureName, defaultValue: true).ConfigureAwait(false);

            // If filter is included we revert to legacy logic, as it is not currently supported for paginating RTS call.
            if (isSearchCriteriaNull || isLatestVersionForSearchCriteriaFeatureEnabled)
            {
                // This line finds the maximum enum value (The most up to date version available).
                rtsMethodVersionToCall = Enum.GetValues(typeof(GetStoreAvailabilityRtsVersion)).Cast<GetStoreAvailabilityRtsVersion>().Max();
            }
            else
            {
                rtsMethodVersionToCall = GetStoreAvailabilityRtsVersion.V1;
            }

            return rtsMethodVersionToCall;
        }

        private static async Task<Dictionary<string, OrgUnitLocation>> GetStoresInFullfilmentGroup(SearchOrgUnitAvailabilitiesServiceRequest request)
        {
            // Grab the OrgUnitLocation objects for the stores in the fullfillment group.
            // We will grab all stores in the current stores fulfillment group (max = MaxNumberOfStoresToReturn).
            PagingInfo pagingInfoAllRecords = new PagingInfo(MaxNumberOfStoresToReturn);

            // Make a QueryResultSettings object that will instead query all stores.
            QueryResultSettings queryResultSettings = new QueryResultSettings(new ColumnSet(), pagingInfoAllRecords, new SortingInfo(), new ChangeQueryInfo());

            // Create a list of OrgUnitLocation objects for each store in the fulfillment group.
            var getStoresRequest = new SearchStoresDataRequest(request.RequestContext.GetPrincipal().ChannelId, filterCriteria: null, queryResultSettings);
            ReadOnlyCollection<OrgUnitLocation> storeLocations = (await request.RequestContext.ExecuteAsync<EntityDataServiceResponse<OrgUnitLocation>>(getStoresRequest).ConfigureAwait(false)).PagedEntityCollection.Results;
            List<OrgUnitLocation> stores = storeLocations.ToList();

            // Convert to dictionary for constant access time.
            Dictionary<string, OrgUnitLocation> storeOrgUnitLocations = new Dictionary<string, OrgUnitLocation>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < stores.Count; i++)
            {
                OrgUnitLocation orgUnitLocation = stores[i];
                string orgUnitName = orgUnitLocation.OrgUnitName;
                string inventLocationId = orgUnitLocation.InventoryLocationId;
                if (string.IsNullOrEmpty(orgUnitName))
                {
                    // Org unit name is required as part of the inventory display in POS, so use InventLocationId (which is mandatory field) as fallback when it is absent.
                    RetailLogger.Log.LogInformation(
                        Events.OrgUnitNameFallbackToInventLocationId,
                        "Org unit name is not found, fallback to use invent location ID {inventLocatonId} as name.",
                        inventLocationId.AsCustomerContent());
                    stores[i].OrgUnitName = inventLocationId;
                }

                string key = (orgUnitName + "-" + inventLocationId).ToLowerInvariant();

                if (!storeOrgUnitLocations.ContainsKey(key))
                {
                    storeOrgUnitLocations.Add(key, stores[i]);
                }
            }

            return storeOrgUnitLocations;
        }

        private static OrgUnitLocation CreateWarehouseOrgUnitLocation(ItemAvailabilityStore itemAvailabilityStore)
        {
            var location = new OrgUnitLocation()
            {
                OrgUnitName = itemAvailabilityStore.OrgUnitName,
                InventoryLocationId = itemAvailabilityStore.InventoryLocationId,
            };

            return location;
        }

        private static async Task<List<OrgUnitAvailability>> FilterOrgUnitAvailabilitiesByStore(SearchOrgUnitAvailabilitiesServiceRequest request, ReadOnlyCollection<ItemAvailabilityStore> availabilities,
                                                                                 IList<SimpleProduct> products, string inventoryUnitOfMeasureSymbol, string inventoryUnitOfMeasureDescription)
        {
            List<OrgUnitAvailability> storeAvailabilities;

            // Get a list of valid store locations.
            OrgUnitAvailabilitySearchCriteria filter = request.OrgUnitAvailabilitySearchCriteria;
            OrgUnitLocationSearchCriteria orgUnitLocationSearchCriteria = (filter == null) ? null : new OrgUnitLocationSearchCriteria()
            {
                OrgUnitName = filter.OrgUnitName,
                OrgUnitNumber = filter.OrgUnitNumber,
            };
            var getStoresRequest = new SearchStoresDataRequest(request.RequestContext.GetPrincipal().ChannelId, orgUnitLocationSearchCriteria, request.QueryResultSettings);
            ReadOnlyCollection<OrgUnitLocation> storeLocations = (await request.RequestContext.ExecuteAsync<EntityDataServiceResponse<OrgUnitLocation>>(getStoresRequest).ConfigureAwait(false)).PagedEntityCollection.Results;

            // Get a list of valid warehouses.
            ReadOnlyCollection<Warehouse> warehouses;
            string warehouseSearchText = (filter == null) ? null : filter.OrgUnitName;

            if ((filter != null) && !string.IsNullOrWhiteSpace(filter.OrgUnitNumber))
            {
                // OrgUnitNumber in OrgUnitAvailability is used for store numbers, therefore warehouses should be all fitered out.
                warehouses = PagedResult<Warehouse>.Empty().Results;
            }
            else
            {
                var getWarehousesRequest = new SearchWarehousesDataRequest(request.QueryResultSettings, warehouseSearchText);
                warehouses = (await request.RequestContext.ExecuteAsync<EntityDataServiceResponse<Warehouse>>(getWarehousesRequest).ConfigureAwait(false)).PagedEntityCollection.Results;
            }

            // Construct OrgUnitAvailability list with item inventory information and valid stores/warehouses list
            storeAvailabilities = ConstructOrgUnitAvailabilities(request.ProductId, availabilities, storeLocations, warehouses, products[0].DefaultUnitOfMeasure, inventoryUnitOfMeasureSymbol, inventoryUnitOfMeasureDescription);
            return storeAvailabilities;
        }

        private static List<OrgUnitAvailability> ConstructOrgUnitAvailabilities(
        long productId, IList<ItemAvailabilityStore> inventoryInfo, ReadOnlyCollection<OrgUnitLocation> readOnlyStoreCollection, ReadOnlyCollection<Warehouse> readOnlyWarehouseCollection,
        string unitOfMeasure, string inventoryUnitOfMeasureSymbol, string inventoryUnitOfMeasureDescription)
        {
            List<OrgUnitLocation> stores = readOnlyStoreCollection.ToList();
            List<Warehouse> warehouses = (readOnlyWarehouseCollection == null) ? null : readOnlyWarehouseCollection.ToList();

            List<OrgUnitAvailability> storeAvailabilities = new List<OrgUnitAvailability>();
            OrgUnitLocation location;

            for (int i = 0; i < inventoryInfo.Count; i++)
            {
                if (inventoryInfo[i].IsWarehouse)
                {
                    if (warehouses != null)
                    {
                        // Fulfillment group warehouses list is provided, so check the record and see if it is a valid warehouse in the group.
                        // Reduce the warehouses list for performance.
                        var warehousesFound = new List<Warehouse>();
                        warehouses.RemoveAll(c =>
                        {
                            if (c.Name.Equals(inventoryInfo[i].OrgUnitName, StringComparison.OrdinalIgnoreCase))
                            {
                                warehousesFound.Add(c);
                                return true;
                            }

                            return false;
                        });

                        if (!warehousesFound.Any())
                        {
                            // This warehouse in inventoryInfo[i] is not part of the fulfillment group, skip it.
                            continue;
                        }
                    }

                    // If a record is found as warehouse under fulfillment group, generate an OrgUnitLocation entity for this non-store location.
                    // For backward compatibility, when fulfillment group warehouses list is not be provided, do the same for the warehouse record as well.
                    location = new OrgUnitLocation()
                    {
                        OrgUnitName = inventoryInfo[i].OrgUnitName,
                        InventoryLocationId = inventoryInfo[i].InventoryLocationId,
                    };
                }
                else
                {
                    // Reduce the store list for performance.
                    // If no store found, nothing would be removed from stores.
                    var storesFound = new List<OrgUnitLocation>();
                    stores.RemoveAll(c =>
                    {
                        if (c.OrgUnitName.Equals(inventoryInfo[i].OrgUnitName, StringComparison.OrdinalIgnoreCase))
                        {
                            storesFound.Add(c);
                            return true;
                        }

                        return false;
                    });

                    if (storesFound.Any())
                    {
                        // Use the store's OrgUnitLocation.
                        // storesFound may include multiple result!
                        location = storesFound.First();
                    }
                    else
                    {
                        // Skip the store if it is not a warehouse and not found in the store locator.
                        // This is for backward compatibility.
                        continue;
                    }
                }

                ItemAvailability itemAvailibility = new ItemAvailability
                {
                    ItemId = inventoryInfo[i].ItemId,
                    ProductId = productId,
                    InventoryLocationId = inventoryInfo[i].InventoryLocationId,
                    AvailableQuantity = inventoryInfo[i].AvailableQuantity,
                    OrderedSum = inventoryInfo[i].OrderedSum,
                    PhysicalReserved = inventoryInfo[i].PhysicalReserved,
                    UnitOfMeasure = unitOfMeasure,
                    InventoryUnitOfMeasure = inventoryUnitOfMeasureSymbol ?? string.Empty,
                    InventoryUnitOfMeasureDescription = inventoryUnitOfMeasureDescription ?? string.Empty,
                    ExtensionProperties = inventoryInfo[i].ExtensionProperties
                };

                OrgUnitAvailability storeAvailability = new OrgUnitAvailability(location, new List<ItemAvailability> { itemAvailibility });
                storeAvailabilities.Add(storeAvailability);
            }

            return storeAvailabilities;
        }

        private static List<OrgUnitAvailability> SortOrgUnitAvailabilitiesWithPagination(List<OrgUnitAvailability> orgUnitAvailabilities, QueryResultSettings queryResultSettings, long channelId)
        {
            SortingInfo sortingInfo = queryResultSettings.Sorting;
            PagingInfo pagingInfo = queryResultSettings.Paging;
            SortColumn sortColumn = sortingInfo.Columns.SingleOrDefault();

            switch (sortColumn?.ColumnName?.ToUpperInvariant())
            {
                case ItemAvailability.AvailableQuantityColumn:
                    orgUnitAvailabilities = sortColumn.IsDescending
                        ? orgUnitAvailabilities.OrderByDescending(availability => availability.ItemAvailabilities.Single().AvailableQuantity).ToList()
                        : orgUnitAvailabilities.OrderBy(availability => availability.ItemAvailabilities.Single().AvailableQuantity).ToList();
                    break;

                case ItemAvailability.OrderedSumColumn:
                    orgUnitAvailabilities = sortColumn.IsDescending
                        ? orgUnitAvailabilities.OrderByDescending(availability => availability.ItemAvailabilities.Single().OrderedSum).ToList()
                        : orgUnitAvailabilities.OrderBy(availability => availability.ItemAvailabilities.Single().OrderedSum).ToList();
                    break;

                case ItemAvailability.PhysicalReservedColumn:
                    orgUnitAvailabilities = sortColumn.IsDescending
                        ? orgUnitAvailabilities.OrderByDescending(availability => availability.ItemAvailabilities.Single().PhysicalReserved).ToList()
                        : orgUnitAvailabilities.OrderBy(availability => availability.ItemAvailabilities.Single().PhysicalReserved).ToList();
                    break;

                case OrgUnitLocation.OrgUnitNameColumn:
                    orgUnitAvailabilities = sortColumn.IsDescending
                        ? orgUnitAvailabilities.OrderByDescending(availability => availability.OrgUnitLocation.OrgUnitName).ToList()
                        : orgUnitAvailabilities.OrderBy(availability => availability.OrgUnitLocation.OrgUnitName).ToList();
                    break;

                case OrgUnitLocation.OrgUnitNumberColumn:
                    orgUnitAvailabilities = sortColumn.IsDescending
                        ? orgUnitAvailabilities.OrderByDescending(availability => availability.OrgUnitLocation.OrgUnitNumber).ToList()
                        : orgUnitAvailabilities.OrderBy(availability => availability.OrgUnitLocation.OrgUnitNumber).ToList();
                    break;

                case OrgUnitLocation.DistanceColumn:
                    if (orgUnitAvailabilities.All(availability => availability.OrgUnitLocation.Distance == 0))
                    {
                        // Use fallback order when sorting by distance is not a valid scenario, i.e. when the current org unit does not have geo location configured.
                        orgUnitAvailabilities = SortOrgUnitAvailabilitiesInFallbackOrder(orgUnitAvailabilities, channelId);
                    }
                    else
                    {
                        orgUnitAvailabilities = sortColumn.IsDescending
                            ? orgUnitAvailabilities.OrderByDescending(availability => availability.OrgUnitLocation.Distance).ToList()
                            : orgUnitAvailabilities.OrderBy(availability => availability.OrgUnitLocation.Distance).ToList();
                    }

                    break;

                default:
                    if (orgUnitAvailabilities.All(availability => availability.OrgUnitLocation.Distance == 0))
                    {
                        // Use fallback order when sorting by distance is not a valid scenario, i.e. when the current org unit does not have geo location configured.
                        orgUnitAvailabilities = SortOrgUnitAvailabilitiesInFallbackOrder(orgUnitAvailabilities, channelId);
                    }
                    else
                    {
                        orgUnitAvailabilities = orgUnitAvailabilities.OrderBy(availability => availability.OrgUnitLocation.Distance).ToList();
                    }

                    break;
            }

            return orgUnitAvailabilities.AsPagedResult(queryResultSettings).ToList();
        }

        private static List<OrgUnitAvailability> SortOrgUnitAvailabilitiesInFallbackOrder(List<OrgUnitAvailability> orgUnitAvailabilities, long channelId)
        {
            RetailLogger.Log.LogInformation(
                Events.InventoryLookupSortOrderFallback,
                "Inventory lookup sorting in fallback order - put the current org unit at the top and sort the rest by name asc. ChannelId: {channelId}",
                channelId.AsSystemMetadata());

            return orgUnitAvailabilities.OrderByDescending(availability => availability.OrgUnitLocation.ChannelId == channelId)
                .ThenBy(availability => availability.OrgUnitLocation.OrgUnitName)
                .ToList();
        }
    }
}
