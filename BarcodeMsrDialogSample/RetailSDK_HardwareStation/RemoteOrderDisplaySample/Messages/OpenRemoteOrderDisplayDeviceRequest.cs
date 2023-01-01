namespace Contoso
{
    namespace Commerce.HardwareStation.RemoteOrderDisplaySample
    {
        using Microsoft.Dynamics.Commerce.HardwareStation;
        using Microsoft.Dynamics.Commerce.HardwareStation.Peripherals;

        /// <summary>
        /// Represents a remote order display open request.
        /// </summary>
        public class OpenRemoteOrderDisplayDeviceRequest : OpenDeviceRequestBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="OpenRemoteOrderDisplayDeviceRequest"/> class.
            /// </summary>
            /// <param name="deviceName">Specify the device name.</param>
            /// <param name="deviceConfig">Specify the device configuration.</param>
            public OpenRemoteOrderDisplayDeviceRequest(string deviceName, PeripheralConfiguration deviceConfig) : base(deviceName, deviceConfig)
            {
            }
        }
    }
}
