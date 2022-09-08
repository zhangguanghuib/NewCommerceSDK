namespace GasStationSample.CommerceRuntime
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    [DataContract]
    public sealed class GetGasStationDetailsDataResponse: Response
    {
        public GetGasStationDetailsDataResponse(GasStationDetails details)
        {
            this.Details = details;
        }

        public GasStationDetails Details { get; private set; }
    }
}
