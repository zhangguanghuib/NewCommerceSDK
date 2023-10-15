namespace GHZ
{
    namespace HardwareStation.CoinDispenserSample
    {
        using System;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.HardwareStation;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
        using GHZ.HardwareStation.CoinDispenserSample.Messages;

        [RoutePrefix("COINDISPENSER")]
        public class CoinDispenserController : IController
        {
            private const string CoinDispenserTestName = "MockOPOSCoinDispenser";

            [HttpPost]
            public async Task<bool> DispenseChange(CoinDispenseRequest request, IEndpointContext context)
            {
                ThrowIf.Null(request, "request");

                string deviceName = request.DeviceName;

                if (string.IsNullOrWhiteSpace(deviceName))
                {
                    deviceName = CoinDispenserController.CoinDispenserTestName;
                }

                try
                {
                    var openCoinDispenserDeviceRequest = new OpenCoinDispenserDeviceRequest(deviceName, null);
                    await context.ExecuteAsync<NullResponse>(openCoinDispenserDeviceRequest).ConfigureAwait(false);

                    var dispenseChangeCoinDispenserDeviceRequest = new DispenseChangeCoinDispenserDeviceRequest(request.Amount);
                    await context.ExecuteAsync<NullResponse>(dispenseChangeCoinDispenserDeviceRequest).ConfigureAwait(false);

                    return true;
                }
                catch (Exception ex)
                {
                    throw new PeripheralException("Microsoft_Dynamics_Commerce_HardwareStation_CoinDispenser_Error", ex.Message, ex);
                }
                finally
                {
                    var closeCoinDispenserDeviceRequest = new CloseCoinDispenserDeviceRequest();
                    await context.ExecuteAsync<NullResponse>(closeCoinDispenserDeviceRequest).ConfigureAwait(false);
                }
            }

        }
    }
}
