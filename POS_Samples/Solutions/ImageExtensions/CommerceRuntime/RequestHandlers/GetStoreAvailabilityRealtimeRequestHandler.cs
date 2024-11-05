
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

namespace Contoso.GasStationSample.CommerceRuntime.RequestHandlers
{
    public class GetStoreAvailabilityRealtimeRequestHandler : SingleAsyncRequestHandler<GetStoreAvailabilityRealtimeRequest>
    {
        private static GetStoreAvailabilityRtsVersion defaultVersion = GetStoreAvailabilityRtsVersion.V3;

        private static TransactionServiceClient transactionServiceClient;
        private static int expirationMinutes = 30;
        private static TimeSpan expirationPeriod = TimeSpan.FromMinutes(expirationMinutes);
        private static string getStoreAvailabilityRtsCacheName = "GetStoreAvailability";
        private static RtsMethodAvailableVersionCache<GetStoreAvailabilityRtsVersion> getStoreAvailabilityRtsMethodVersion =
            new RtsMethodAvailableVersionCache<GetStoreAvailabilityRtsVersion>(getStoreAvailabilityRtsCacheName, defaultVersion);
        private static string customColumn1Name = "customCol1";
        private static string customColumn2Name = "customCol2";

        protected override async Task<Response> Process(GetStoreAvailabilityRealtimeRequest request)
        {
            GetStoreAvailabilityRtsVersion rtsMethodVersionCalled = 0;
            GetStoreAvailabilityRtsVersion rtsMethodVersionToCall = 0;

            // The highest version is the most up to date
            GetStoreAvailabilityRtsVersion highestAvailableVersion = GetStoreAvailabilityRtsVersion.V3;

            // The version that our request is attempting to call.
            GetStoreAvailabilityRtsVersion rtsMethodVersionRequestedToCall = request.VersionToCall;

            if (rtsMethodVersionRequestedToCall >= highestAvailableVersion)
            {
                // If we are requesting to call a higher version than is available, then we revert down to the highest available version
                rtsMethodVersionToCall = highestAvailableVersion;
            }
            else
            {
                // Otherwise if our requested version to call is available, then we call that version.
                rtsMethodVersionToCall = rtsMethodVersionRequestedToCall;
            }

            TransactionServiceClient transactionClient = new TransactionServiceClient(request.RequestContext);
            var inventoryLookupSearchCriteria = new TransactionServiceInventoryLookupSearchCriteria(
                request.ItemId, request.VariantId, request.CurrentStoreId,
                request.IncludeNonStoreWarehouses, request.Paging, request.Sorting, request.OrgUnitAvailabilitySearchCriteria);
            ReadOnlyCollection<InventoryInfo> inventoryInfo;
            (inventoryInfo, rtsMethodVersionCalled) = await transactionClient.InventoryLookup(
                rtsMethodVersionToCall, inventoryLookupSearchCriteria).ConfigureAwait(false);

            PagedResult<ItemAvailabilityStore> storeAvailabilities = inventoryInfo.Select(
                info => new ItemAvailabilityStore
                {
                    AvailableQuantity = decimal.Parse(string.IsNullOrWhiteSpace(info.InventoryAvailable) ? "0.0" : info.InventoryAvailable, CultureInfo.InvariantCulture),
                    OrderedSum = decimal.Parse(string.IsNullOrWhiteSpace(info.OrderedSum) ? "0.0" : info.OrderedSum, CultureInfo.InvariantCulture),
                    PhysicalReserved = decimal.Parse(string.IsNullOrWhiteSpace(info.PhysicalReserved) ? "0.0" : info.PhysicalReserved, CultureInfo.InvariantCulture),
                    ItemId = info.ItemId,
                    InventoryLocationId = info.InventoryLocationId,
                    OrgUnitName = info.StoreName,
                    IsWarehouse = info.IsWarehouse,
                    ExtensionProperties = new List<CommerceProperty>
                    {
                         new CommerceProperty
                         {
                             Key = customColumn1Name,
                             Value = new CommercePropertyValue()
                             {
                                 IntegerValue = info[customColumn1Name] as int? ?? 0
                             }
                         },
                         new CommerceProperty
                         {
                             Key = customColumn2Name,
                             Value = new CommercePropertyValue()
                             {
                                 IntegerValue = info[customColumn2Name] as int? ?? 0
                             }
                         },
                    }
                }).AsPagedResult();

            if (rtsMethodVersionCalled != rtsMethodVersionToCall)
            {
                getStoreAvailabilityRtsMethodVersion.Version = rtsMethodVersionCalled;
            }

            var response = new StoreAvailabilityRealtimeResponse(storeAvailabilities, rtsMethodVersionCalled);
            return response;
        }
    }
}
