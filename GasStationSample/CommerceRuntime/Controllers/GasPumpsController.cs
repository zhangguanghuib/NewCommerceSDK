namespace GasStationSample.CommerceRuntime
{
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
    using System.Threading.Tasks;

    [RoutePrefix("GasPums")]
    [BindEntity(typeof(GasPump))]
    public class GasPumpsController : IController
    {
        [HttpPost]
        [Authorization(CommerceRoles.Device, CommerceRoles.Employee)]
        public async Task<PagedResult<GasPump>> GetGasPumpsByStore(IEndpointContext context, string storeNumber, QueryResultSettings queryResultSettings)
        {
            var request = new GetGasPumpsDataRequest(storeNumber);
            var response = await context.ExecuteAsync<GetGasPumpsDataResonse>(request).ConfigureAwait(false);
            return new PagedResult<GasPump>(response.GasPumps.AsReadOnly());
        }

        [HttpPost]
        [Authorization(CommerceRoles.Device, CommerceRoles.Employee)]
        public async Task<GasStationDetails> GetGasStationDetailsByStore(IEndpointContext context, string storeNumber)
        {
            var request = new GetGasStationDetailsDataRequest(storeNumber);
            var response = await context.ExecuteAsync<GetGasStationDetailsDataResponse>(request).ConfigureAwait(false);
            return response.Details;
        }

        [HttpPost]
        [Authorization(CommerceRoles.Device, CommerceRoles.Employee)]
        public async Task<PagedResult<GasPump>> StopAllPumps(IEndpointContext context, string storeNumber)
        {
            var request = new StopAllPumpsDataRequest(storeNumber);
            var response = await context.ExecuteAsync<StopAllPumpsDataResponse>(request).ConfigureAwait(false);
            return new PagedResult<GasPump>(response.Pumps.AsReadOnly());
        }

        [HttpPost]
        [Authorization(CommerceRoles.Device, CommerceRoles.Employee)]
        public async Task<PagedResult<GasPump>>  StartAllPumps(IEndpointContext context, string storeNumber)
        {
            var request = new StartAllPumpsDataRequest(storeNumber);
            var response = await context.ExecuteAsync<StartAllPumpsDataResponse>(request).ConfigureAwait(false);
            return new PagedResult<GasPump>(response.Pumps.AsReadOnly());
        }

        [HttpPost]
        [Authorization(CommerceRoles.Device, CommerceRoles.Employee)]
        public async Task<GasPump> UpdatePumpState(IEndpointContext context, string storeNumber, long id, GasPumpState state)
        {
            var request = new UpdatePumpStateDataRequest(storeNumber, id, state);
            var response = await context.ExecuteAsync<UpdatePumpStateDataResponse>(request).ConfigureAwait(false);
            return response.Pump;
        }
    }
}
