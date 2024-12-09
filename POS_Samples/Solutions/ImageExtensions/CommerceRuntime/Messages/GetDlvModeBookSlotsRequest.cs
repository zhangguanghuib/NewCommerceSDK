namespace Contoso.GasStationSample.CommerceRuntime.Messages
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    [DataContract]
    public sealed class GetDlvModeBookSlotsRequest:Request
    {
        public GetDlvModeBookSlotsRequest(string dlvModeCode)
        {
            this.DlvModeCode = dlvModeCode;
        }

        [DataMember]
        public string DlvModeCode { get; private set; }
    }
}
