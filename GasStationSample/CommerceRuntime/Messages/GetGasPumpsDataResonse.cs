namespace GasStationSample.CommerceRuntime
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    [DataContract]
    public sealed class GetGasPumpsDataResonse : Response
    {
        public GetGasPumpsDataResonse(IEnumerable<GasPump> pumps)
        {
            this.GasPumps = pumps;
        }

        public IEnumerable<GasPump> GasPumps { get; private set; }
    }

}
