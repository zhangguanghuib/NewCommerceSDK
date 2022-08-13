using System;
using System.Collections.Generic;
using System.Text;

namespace GHZ
{
    namespace HardwareStation.OposPrinterDeviceSample
    {
        using System;
        using System.Collections.Generic;
        using Microsoft.Dynamics.Commerce.HardwareStation;
        using Microsoft.Dynamics.Commerce.HardwareStation.Peripherals;
        using Microsoft.Dynamics.Commerce.Runtime.Handlers;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        public class OposPrinterDevice : INamedRequestHandler, IDisposable
        {

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
                        typeof(OpenPrinterDeviceRequest),
                        typeof(PrintPrinterDeviceRequest),
                        typeof(ClosePrinterDeviceRequest),
                        typeof(HealthCheckPrinterDeviceRequest),
                    };
                }
            }

            protected virtual void Dispose(bool disposing)
            {
                if(disposing)
                {
                    this.Close();
                }
            }

            public void Dispose()
            {
                this.Dispose(true);

                GC.SuppressFinalize(this);
            }

            public Response Execute(Request request)
            {
                ThrowIf.Null(request, "request");

                Type requestType = request.GetType();


                if(requestType == typeof(OpenPrinterDeviceRequest))
                {
                    this.Open();
                }
                else if (requestType == typeof(PrintPrinterDeviceRequest))
                {
                    this.Print();
                }
                else if(requestType == typeof(ClosePrinterDeviceRequest))
                {
                    this.Close();
                }
                else
                {
                    throw new NotSupportedException(string.Format("Request '{0}' is not supported", request.GetType()));
                }

                return new NullResponse();
            }

            private void Open()
            {
                this.isOpen = true;
            }

            private void Print()
            {

            }

            private void Close()
            {
                if (this.isOpen)
                {
                    this.isOpen = false;
                }
            }
        }
    }
}
