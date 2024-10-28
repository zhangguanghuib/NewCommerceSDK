namespace GHZ
{
    namespace HardwareStation.CoinDispenserSample
    {
        using System;
        using System.Collections;
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
            public bool isOpen = false;

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

            public Task<Response> Execute(Request request)
            {
                ThrowIf.Null(request, "request");

                Type requestType = request.GetType();

                if(requestType == typeof(OpenCoinDispenserDeviceRequest))
                {
                    var openRequest = (OpenCoinDispenserDeviceRequest)request;
                    this.Open(openRequest.DeviceName);

                }
                else if (requestType == typeof(DispenseChangeCoinDispenserDeviceRequest))
                {
                    var dispenseChangeRequest = (DispenseChangeCoinDispenserDeviceRequest)request;
                    this.DispenseChange(dispenseChangeRequest.Amount);

                }
                else if(requestType == typeof(CloseCoinDispenserDeviceRequest))
                {
                    this.Close();
                }
                else
                {
                    throw new NotSupportedException(string.Format("Request '{0}' is not supported", requestType));
                }

                return Task.FromResult((Response)NullResponse.Instance);
                
            }

            public void Dispose()
            {
                this.Close();
            }

            private void Open(string peripheralName)
            {
                this.isOpen = true;
            }

            private void  DispenseChange(int amount)
            {
                if (amount > this.coinAmount)
                {
                    throw new Exception("Not enough coins!");
                }

                this.coinAmount -= amount;
            }

            private void Close()
            {
                this.isOpen = false;
            }
        }
    }
}
