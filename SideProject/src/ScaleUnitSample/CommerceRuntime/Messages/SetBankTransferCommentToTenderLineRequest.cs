

namespace Contoso.CommerceRuntime.Messages
{
    using System.Runtime.Serialization;
    using Contoso.CommerceRuntime.Entities.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    [DataContract]
    public sealed class SetBankTransferCommentToTenderLineRequest : Request
    {
        public SetBankTransferCommentToTenderLineRequest(SaveTenderLineRequest saveTenderLineRequest, string bankTransferComment)
        {
            this.SaveTenderLineRequest = saveTenderLineRequest;
            this.BankTransferComment = bankTransferComment;
        }

        [DataMember]
        public string BankTransferComment { get; private set; }

        [DataMember]
        public SaveTenderLineRequest SaveTenderLineRequest { get; private set; }
    }
}
