

namespace Contoso.GasStationSample.CommerceRuntime.Messages
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    [DataContract]
    public sealed class GetBarCodeImageResponse : Response
    {
        public GetBarCodeImageResponse(string encodedBarCode)
        {
            this.EncodedBarCode = encodedBarCode;
        }

        /// <summary>
        /// Gets a value indicating whether the delete succeeded.
        /// </summary>
        [DataMember]
        public string EncodedBarCode { get; private set; }
    }
}
