namespace GHZ
{
    namespace HardwareStation.CoinDispenserSample
    {
        using System;
        using System.Collections.Generic;
        using GHZ.HardwareStation.CoinDispenserSample.Messages;
        using Microsoft.Dynamics.Commerce.HardwareStation;
        using Microsoft.Dynamics.Commerce.HardwareStation.Peripherals;
        using Microsoft.Dynamics.Commerce.Runtime.Handlers;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;


        public class OposCoinDispenser : INamedRequestHandler, IDisposable
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

            public Response Execute(Request request)
            {
                ThrowIf.Null(request, "request");

                Type requestType = request.GetType();

                if(requestType == typeof(OpenCoinDispenserDeviceRequest))
                {
                    var openReques = (OpenCoinDispenserDeviceRequest)request;

                }
                else if (requestType == typeof(DispenseChangeCoinDispenserDeviceRequest))
                {
                    var dispenseChangeRequest = (DispenseChangeCoinDispenserDeviceRequest)request;

                }
                else if(requestType == typeof(CloseCoinDispenserDeviceRequest))
                {

                }
                else
                {
                    throw new NotSupportedException(string.Format("Request '{0}' is not supported", requestType));
                }

                return new NullResponse();
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
