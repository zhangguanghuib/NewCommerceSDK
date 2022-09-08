namespace GasStationSample.CommerceRuntime
{
    using System.Runtime.Serialization;

    [DataContract]
    public enum GasPumpStatus
    {
        [EnumMember]
        Unknown,

        [EnumMember]
        Idle,

        [EnumMember]
        Pumping,

        [EnumMember]
        PumpingComplete,

        [EnumMember]
        Stopped,

        [EnumMember]
        Emergency
    }
}
