namespace GasStationSample.CommerceRuntime
{
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using System.Runtime.InteropServices.ComTypes;
    using System.Runtime.Serialization;
    using SystemAnnotation = System.ComponentModel.DataAnnotations;

    public class GasPump : CommerceEntity
    {
        public GasPump() : base("GasPump")
        {
        }

        [SystemAnnotation.Key]
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }
    }
}
