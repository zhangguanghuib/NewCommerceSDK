
using System;
using System.Collections.Generic;
using System.Text;

namespace Contoso.GasStationSample.CommerceRuntime
{
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;

    public class NonBindableOperationController:  IController
    {
        [HttpPost]
        [Authorization(CommerceRoles.Device, CommerceRoles.Employee, CommerceRoles.Anonymous, CommerceRoles.Application)]
        public async Task<PagedResult<Cart>> SearchCartsAsync(IEndpointContext context, 
            DateTimeOffset lastModifiedDateTimeFrom,                                                          
            DateTimeOffset lastModifiedDateTimeTo,
            QueryResultSettings queryResultSettings)
        {
            var cartSearchCriteria = new CartSearchCriteria
            {
                CartTypeValue = (int)CartType.Shopping,
                LastModifiedDateTimeFrom = lastModifiedDateTimeFrom,
                LastModifiedDateTimeTo = lastModifiedDateTimeTo,
                IncludeAnonymous = true,
                SuspendedOnly = false
            };

            queryResultSettings.Paging = PagingInfo.AllRecords;

            var sortColumns = new ObservableCollection<SortColumn>();
            sortColumns.Add(new SortColumn(columnName: "ModifiedDateTime", isDescending: false ));
            queryResultSettings.Sorting = new SortingInfo()
            {
                Columns = sortColumns
            };

            var getCartRequest = new GetCartRequest(cartSearchCriteria, queryResultSettings);
            var response = await context.ExecuteAsync<GetCartResponse>(getCartRequest).ConfigureAwait(false);

            return new PagedResult<Cart>(response.Carts.AsReadOnly());
        }

        [HttpPost]
        [Authorization(CommerceRoles.Device, CommerceRoles.Employee, CommerceRoles.Anonymous, CommerceRoles.Application)]
        public async Task<PagedResult<Cart>> DeleteCartsAsync(IEndpointContext context,
            DateTimeOffset lastModifiedDateTimeFrom,
            DateTimeOffset lastModifiedDateTimeTo, 
            QueryResultSettings queryResultSettings)
        {
            var cartSearchCriteria = new CartSearchCriteria
            {
                CartTypeValue = (int)CartType.Shopping,
                LastModifiedDateTimeFrom = lastModifiedDateTimeFrom,
                LastModifiedDateTimeTo = lastModifiedDateTimeTo,
                IncludeAnonymous = true,
                SuspendedOnly = false
            };

            queryResultSettings.Paging = PagingInfo.AllRecords;

            var sortColumns = new ObservableCollection<SortColumn>();
            sortColumns.Add(new SortColumn(columnName: "ModifiedDateTime", isDescending: false));
            queryResultSettings.Sorting = new SortingInfo()
            {
                Columns = sortColumns
            };

            var getCartRequest = new GetCartRequest(cartSearchCriteria, queryResultSettings);
            var response = await context.ExecuteAsync<GetCartResponse>(getCartRequest).ConfigureAwait(false);

            IEnumerable<string> cartIds = response.Carts.Select(cart => cart.Id);

            DeleteCartDataRequest request = new DeleteCartDataRequest(cartIds.Where(id => !string.IsNullOrWhiteSpace(id)));
            await context.ExecuteAsync<NullResponse>(request).ConfigureAwait(false);

            return new PagedResult<Cart>(response.Carts.AsReadOnly());
        }
    }
}
