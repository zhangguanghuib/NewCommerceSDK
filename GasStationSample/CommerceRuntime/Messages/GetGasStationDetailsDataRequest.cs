namespace GasStationSample.CommerceRuntime
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    [DataContract]
    public sealed class GetGasStationDetailsDataRequest:Request
    {
        public GetGasStationDetailsDataRequest(string storeNumber)
        {
            this.StoreNumber = storeNumber;
        }

        public string StoreNumber { get; private set; }
    }
}
