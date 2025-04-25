namespace Contoso.StoreCommercePackagingSample.CommerceRuntime.Messages
{
    using System;
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    [DataContract]
    public sealed class GetCustomerDimensionResponse : Response
    {
        public GetCustomerDimensionResponse(string dimensionAttributeValue)
        {
            this.DimensionAttributeValue = dimensionAttributeValue ?? throw new ArgumentNullException(nameof(dimensionAttributeValue));
        }
        [DataMember]
        public string DimensionAttributeValue { get; private set; }
    }
}
