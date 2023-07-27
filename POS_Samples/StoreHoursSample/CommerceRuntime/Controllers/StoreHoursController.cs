namespace Contoso.RetailServer.StoreHoursSample
{
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
    using Contoso.Commerce.Runtime.StoreHoursSample.Messages;
    using SampleDataModel = Contoso.Commerce.Runtime.DataModel;

    /// <summary>
    /// The controller to retrieve a new entity.
    /// </summary>
    [RoutePrefix("StoreHours")]
    [BindEntity(typeof(SampleDataModel.StoreDayHours))]
    public class StoreHoursController : IController
    {
        /// <summary>
        /// Gets the store hours for a given store.
        /// </summary>
        /// <param name="parameters">The parameters to this action.</param>
        /// <returns>The list of store hours.</returns>
        [HttpPost]
        [Authorization(CommerceRoles.Anonymous, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee)]
        public async Task<PagedResult<SampleDataModel.StoreDayHours>> GetStoreDaysByStore(IEndpointContext context, string StoreNumber, QueryResultSettings queryResultSettings)
        {
            var request = new GetStoreHoursDataRequest(StoreNumber) { QueryResultSettings = queryResultSettings };
            var hoursResponse = await context.ExecuteAsync<GetStoreHoursDataResponse>(request).ConfigureAwait(false);
            return hoursResponse.DayHours;
        }

        /// <summary>
        /// Updates the store day hours for a given store.
        /// </summary>
        /// <param name="key">The OData key.</param>
        /// <param name="parameters">The OData parameters.</param>
        /// <returns>The updated store hours.</returns>
        [HttpPost]
        [Authorization(CommerceRoles.Anonymous, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee)]
        public async Task<SampleDataModel.StoreDayHours> UpdateStoreDayHours(IEndpointContext context, [EntityKey] long key, SampleDataModel.StoreDayHours storeDayHours)
        {
            storeDayHours.Id = key;
            var request = new UpdateStoreDayHoursDataRequest(storeDayHours);
            var response = await context.ExecuteAsync<UpdateStoreDayHoursDataResponse>(request).ConfigureAwait(false);
            return response.StoreDayHours;
        }
    }
}