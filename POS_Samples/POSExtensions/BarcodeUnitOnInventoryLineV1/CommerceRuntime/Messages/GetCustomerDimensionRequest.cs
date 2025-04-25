namespace Contoso.StoreCommercePackagingSample.CommerceRuntime.Messages
{
    using System;
    using System.Runtime.Serialization;
    using Contoso.CommerceRuntime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    [DataContract]
    public sealed class GetCustomerDimensionRequest: Request
    {
        public GetCustomerDimensionRequest(string accountNum, string dimensionAttributeName)
        {
            this.AccountNum = accountNum;
            this.DimensionAttributeName = dimensionAttributeName ?? throw new ArgumentNullException(nameof(DimensionAttributeName));
        }
        [DataMember]
        public string AccountNum { get; private set; }
        [DataMember]
        public string DimensionAttributeName { get; private set; }
    }
}
