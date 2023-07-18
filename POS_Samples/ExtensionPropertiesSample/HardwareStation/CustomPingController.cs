using System;
using System.Collections.Generic;
using System.Text;

namespace GHZ
{
    namespace HardwareStation.CoinDispenserSample
    {
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
        using GHZ.HardwareStation.CoinDispenserSample.Messages;

        [RoutePrefix("CUSTOMPING")]
        public class CustomPingController: IController
        {
            [HttpPost]
            public Task<string> CustomPing(PingRequest pingRequest)
            {
                ThrowIf.Null(pingRequest, "pingRequest");

                return Task.FromResult(string.Format("Your message is successfully received: {0}", pingRequest.Message));
            }
        }
    }
}
