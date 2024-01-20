namespace GHZ
{
    namespace HardwareStation.CoinDispenserSample
    {
        using System;
        using System.Collections.Generic;
        using System.Threading.Tasks;
        using GHZ.HardwareStation.CoinDispenserSample.Messages;
        using Microsoft.Dynamics.Commerce.HardwareStation;
        using Microsoft.Dynamics.Commerce.HardwareStation.Peripherals;
        using Microsoft.Dynamics.Commerce.Runtime.Handlers;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        public class OposCoinDispenser : INamedRequestHandlerAsync, IDisposable
        {
            private int coinAmount = 1000;
            private bool isOpen = false;

            public string HandlerName
            {
                get { return PeripheralType.Opos; }
            }

            public IEnumerable<Type> SupportedRequestTypes
            {
                get
                {
                    return new[]
                    {
                        typeof(OpenCoinDispenserDeviceRequest),
                        typeof(DispenseChangeCoinDispenserDeviceRequest),
                        typeof(CloseCoinDispenserDeviceRequest)
                    };
                }
            }
            public async Task<Response> Execute(Request request)
            {
                ThrowIf.Null(request, "request");

                Type requestType = request.GetType();

                if(requestType == typeof(OpenCoinDispenserDeviceRequest))
                {
                    var openRequest = (OpenCoinDispenserDeviceRequest)request;
                    await this.Open(openRequest.DeviceName).ConfigureAwait(false); ;

                }
                else if (requestType == typeof(DispenseChangeCoinDispenserDeviceRequest))
                {
                    var dispenseChangeRequest = (DispenseChangeCoinDispenserDeviceRequest)request;
                    await this.DispenseChange(dispenseChangeRequest.Amount).ConfigureAwait(false);

                }
                else if(requestType == typeof(CloseCoinDispenserDeviceRequest))
                {
                    await this.Close().ConfigureAwait(false);
                }
                else
                {
                    throw new NotSupportedException(string.Format("Request '{0}' is not supported", requestType));
                }

                return NullResponse.Instance;
            }

            private async Task Open(string peripheralName)
            {
                this.isOpen = true;
                await Task.CompletedTask.ConfigureAwait(false);
            }

            private async Task DispenseChange(int amount)
            {
                if (amount > this.coinAmount)
                {
                    throw new Exception("Not enough coins!");
                }

                this.coinAmount -= amount;
                await Task.CompletedTask.ConfigureAwait(false);
            }

            private async Task Close()
            {
                this.isOpen = false;
                await Task.CompletedTask.ConfigureAwait(false);
            }

            public void Dispose()
            {
                this.Close().ConfigureAwait(false); ;
            }
        }
    }
}
