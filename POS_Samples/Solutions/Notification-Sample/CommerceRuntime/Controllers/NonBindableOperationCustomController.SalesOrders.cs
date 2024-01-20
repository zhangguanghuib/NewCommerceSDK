namespace Contoso.GasStationSample.CommerceRuntime.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
    using Microsoft.Dynamics.Commerce.Runtime;
    using System.Collections.Generic;
    using System.Linq;
    using System;

    public partial class NonBindableOperationCustomController : IController
    {
        [HttpPost]
        [Authorization(CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee, CommerceRoles.Anonymous)]
        public async Task<PagedResult<SalesOrder>> GetPickupOrdersCreatedFromOtherStore(IEndpointContext context, long currentChannelId, QueryResultSettings settings)
        {
            ThrowIf.Null(settings, nameof(settings));

            // Only get the orders created last 10 minutes
            OrderSearchCriteria criteria = new OrderSearchCriteria();
            criteria.FulfillmentTypes.Add(FulfillmentOperationType.Pickup);
            criteria.StartDateTime = DateTimeOffset.UtcNow.AddDays(-1);
            criteria.EndDateTime = DateTimeOffset.UtcNow;

            var request = new SearchOrdersServiceRequest(criteria, settings);
            var response = await context.ExecuteAsync<SearchOrdersServiceResponse>(request).ConfigureAwait(false);

            var pagedOrders = response.Orders;
            //Only take the orders created from other store:
            //var pagedOrders = response.Orders.Where(o => o.ChannelId != currentChannelId).AsPagedResult<SalesOrder>();

            return pagedOrders;
        }
    }
}
