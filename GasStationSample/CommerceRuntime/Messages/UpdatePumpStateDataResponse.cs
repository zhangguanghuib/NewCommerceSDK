namespace GasStationSample.CommerceRuntime
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    [DataContract]
    public sealed class UpdatePumpStateDataResponse:Response
    {
        public UpdatePumpStateDataResponse(GasPump pump)
        {
            this.Pump = pump;
        }

        public GasPump Pump { get; set; }
    }
}
