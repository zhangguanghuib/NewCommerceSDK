namespace Contoso.StoreCommercePackagingSample.CommerceRuntime.Messages
{
    using System;
    using System.Runtime.Serialization;
    using Contoso.CommerceRuntime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    [DataContract]
    public sealed class GetInventoryUnitOfMeasureByItemIdRequest : Request
    {
        public GetInventoryUnitOfMeasureByItemIdRequest(long productId, string itemId)
        {
            this.ProductId = productId;
            this.ItemId = itemId ?? throw new ArgumentNullException(nameof(itemId));
        }

        [DataMember]
        public long ProductId { get; private set; }

        [DataMember]
        public string ItemId { get; private set; }

    }
}
