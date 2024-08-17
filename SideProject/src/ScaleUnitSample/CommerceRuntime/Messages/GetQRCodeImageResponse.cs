namespace  Contoso.CommerceRuntime.Messages
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
   
    [DataContract]
    public sealed class  GetQRCodeImageResponse : Response
    {
        public GetQRCodeImageResponse(string encodedQrCode)
        {
            this.EncodedQrCode = encodedQrCode;
        }

        /// <summary>
        /// Gets a value indicating whether the delete succeeded.
        /// </summary>
        public string EncodedQrCode { get; private set; }
    }
}
