namespace Contoso.StoreCommercePackagingSample.CommerceRuntime.Messages
{
    using System;
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    
    [DataContract]
    public sealed class ProductUnitOfMeasureConversionResponse : Response
    {
        public ProductUnitOfMeasureConversionResponse(ItemUnitConversion itemUnitConversion, decimal quantityInToUom)
        {
            this.ItemUnitConversion = itemUnitConversion ?? throw new ArgumentNullException(nameof(itemUnitConversion));
            this.quantityInToUom = quantityInToUom;
        }

        [DataMember]
        public ItemUnitConversion ItemUnitConversion { get; private set; }

        [DataMember]
        public decimal quantityInToUom { get; private set; }
    }
}
