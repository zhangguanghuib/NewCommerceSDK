namespace Contoso.StoreCommercePackagingSample.CommerceRuntime.Messages
{
    using System;
    using System.Runtime.Serialization;
    using Contoso.CommerceRuntime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    [DataContract]
    public sealed class ProductUnitOfMeasureConversionRequest : Request
    {
        public ProductUnitOfMeasureConversionRequest(ItemUnitConversion itemUnitConversion, decimal quantityInFromUom)
        {
            this.ItemUnitConversion = itemUnitConversion ?? throw new ArgumentNullException(nameof(itemUnitConversion));
            this.QuantityInFromUom = quantityInFromUom;
        }

        [DataMember]
        public ItemUnitConversion ItemUnitConversion { get; private set; }

        [DataMember]
        public decimal QuantityInFromUom { get; private set; }
    }
}
