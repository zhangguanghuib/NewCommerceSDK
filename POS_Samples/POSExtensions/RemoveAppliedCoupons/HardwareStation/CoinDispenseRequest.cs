using System;
using System.Collections.Generic;
using System.Text;

namespace GHZ
{
    namespace HardwareStation.CoinDispenserSample
    {
        using System.Runtime.Serialization;
        using Microsoft.Dynamics.Commerce.HardwareStation;


        [DataContract]
        public class CoinDispenseRequest
        {
            [DataMember]
            public string DeviceName { get; set; }

            [DataMember]
            public string DeviceType { get; set; }

            [DataMember]
            public int Amount { get; set; }
        }
    }
}
