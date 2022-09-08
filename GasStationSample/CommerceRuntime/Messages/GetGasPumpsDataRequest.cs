namespace GasStationSample.CommerceRuntime
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    [DataContract]
    public sealed class GetGasPumpsDataRequest :Request
    {
        public  GetGasPumpsDataRequest(string storeNumber)
        {
            this.StoreNumber = storeNumber;
        }

        public string StoreNumber { get; private set; }
    }
}
