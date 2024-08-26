namespace Contoso.CommerceRuntime.Messages
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    [DataContract]
    public sealed class SetBankTransferCommentToTenderLineResponse : Response
    {
        public SetBankTransferCommentToTenderLineResponse(Cart cart)
        {
            this.cart = cart;
        }

        public Cart cart { get; private set; }
    }
}
