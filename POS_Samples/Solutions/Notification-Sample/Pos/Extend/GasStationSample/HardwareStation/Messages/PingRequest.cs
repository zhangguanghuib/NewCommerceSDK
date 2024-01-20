
namespace GHZ.HardwareStation.CoinDispenserSample.Messages
{
    using System.Runtime.Serialization;

    [DataContract]
    public class PingRequest
    {
        [DataMember]
        public string Message { get; set; }
    }
}
