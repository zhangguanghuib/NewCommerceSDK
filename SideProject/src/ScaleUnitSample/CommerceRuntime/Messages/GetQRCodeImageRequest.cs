
namespace Contoso.CommerceRuntime.Messages
{
    using System.Runtime.Serialization;
    using Contoso.CommerceRuntime.Entities.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    /// <summary>
    /// A simple request class to create an Example Entity in the database.
    /// </summary>
    [DataContract]
    public sealed class GetQRCodeImageRequest : Request
    {
        public GetQRCodeImageRequest(string qrcodeurl)
        {
            this.QRCodeurl = qrcodeurl;
        }

        /// <summary>
        /// Gets an Example Entity instance with its fields set with the values to be stored.
        /// </summary>
        [DataMember]
        public string QRCodeurl { get; private set; }
    }
}
