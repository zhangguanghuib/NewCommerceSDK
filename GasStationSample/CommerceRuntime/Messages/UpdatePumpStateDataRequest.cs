namespace GasStationSample.CommerceRuntime
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;


    [DataContract]
    public sealed class UpdatePumpStateDataRequest : Request
    {
        public UpdatePumpStateDataRequest(string storeNumber, long id, GasPumpState state)
        {
            this.StoreNumber = storeNumber;
            this.PumpId = id;
            this.state = state;
        }

        public string StoreNumber { get; private set; }

        public long PumpId { get; private set; }

        public GasPumpState state { get; private set; }
    }
}
