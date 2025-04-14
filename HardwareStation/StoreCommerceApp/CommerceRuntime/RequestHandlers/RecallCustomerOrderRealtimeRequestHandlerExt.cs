using Microsoft.Dynamics.Commerce.Runtime;
using Microsoft.Dynamics.Commerce.Runtime.Data;
using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
using Microsoft.Dynamics.Commerce.Runtime.Messages;
using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.StoreCommercePackagingSample.CommerceRuntime.RequestHandlers
{
    public class RecallCustomerOrderRealtimeRequestHandlerExt: SingleAsyncRequestHandler<RecallCustomerOrderRealtimeRequest>
    {
        protected override async Task<Response> Process(RecallCustomerOrderRealtimeRequest request)
        {
            ThrowIf.Null(request, "request");

            using (var databaseContext = new DatabaseContext(request.RequestContext))
            {
                // Execute original functionality to save the customer.
                var response = await this.ExecuteNextAsync<RecallCustomerOrderRealtimeResponse>(request).ConfigureAwait(false);

                if (response?.SalesOrder?.ActiveSalesLines != null)
                {
                    foreach (SalesLine salesLine in response.SalesOrder.ActiveSalesLines)
                    {
                        if (string.IsNullOrEmpty(salesLine.InventorySiteId))
                        {
                            string currentDataAreaId = request.RequestContext.GetChannelConfiguration().InventLocationDataAreaId;
                            var getWarehouseDataRequest =
                                new GetWarehouseSiteByInventLocationIdDataRequest(new List<string> { salesLine.InventoryLocationId }, currentDataAreaId);

                            var getWarehouseDataResponse =
                                await request.RequestContext.ExecuteAsync<EntityDataServiceResponse<WarehouseSite>>(getWarehouseDataRequest).ConfigureAwait(false);

                            var warehouseSite = getWarehouseDataResponse.PagedEntityCollection.Results[0];
                            salesLine.InventorySiteId = warehouseSite.InventorySiteId;
                        }
                    }
                }
                return response;
            }
        }
    }
}
