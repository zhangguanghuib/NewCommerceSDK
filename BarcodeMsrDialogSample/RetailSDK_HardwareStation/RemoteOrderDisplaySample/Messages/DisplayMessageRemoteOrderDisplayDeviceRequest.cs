namespace Contoso
{
    namespace Commerce.HardwareStation.RemoteOrderDisplaySample
    {
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// Represents a remote order display display message request.
        /// </summary>
        public class DisplayMessageRemoteOrderDisplayDeviceRequest : Request
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DisplayMessageRemoteOrderDisplayDeviceRequest" /> class.
            /// </summary>
            /// <param name="data">Array of messages to display.</param>
            public DisplayMessageRemoteOrderDisplayDeviceRequest(string[] data)
            {
                this.Data = data;
            }

            /// <summary>
            /// Gets the value for array of messages to display. 
            /// </summary>
            public string[] Data { get; private set; }
        }
    }
}
