using System;
using System.Collections.Generic;
using System.Text;

namespace GHZ
{
    namespace HardwareStation.CoinDispenserSample.Messages
    {
        using Microsoft.Dynamics.Commerce.HardwareStation;
        using Microsoft.Dynamics.Commerce.HardwareStation.Peripherals;

        public class OpenCoinDispenserDeviceRequest: OpenDeviceRequestBase
        {
            public OpenCoinDispenserDeviceRequest(string deviceName, PeripheralConfiguration deviceConfig) : base(deviceName, deviceConfig) { }
        }
    }
}
