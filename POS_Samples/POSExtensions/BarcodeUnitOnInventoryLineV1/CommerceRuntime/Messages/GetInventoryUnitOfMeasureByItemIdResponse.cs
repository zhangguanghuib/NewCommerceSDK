namespace Contoso.StoreCommercePackagingSample.CommerceRuntime.Messages
{
    using System;
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    [DataContract]
    public sealed class GetInventoryUnitOfMeasureByItemIdResponse : Response
    {
        public GetInventoryUnitOfMeasureByItemIdResponse(UnitOfMeasure unitOfMeasure)
        {
            this.UnitOfMeasure = unitOfMeasure ?? throw new ArgumentNullException(nameof(UnitOfMeasure));
        }
        [DataMember]
        public UnitOfMeasure UnitOfMeasure { get; private set; }
    }
}
