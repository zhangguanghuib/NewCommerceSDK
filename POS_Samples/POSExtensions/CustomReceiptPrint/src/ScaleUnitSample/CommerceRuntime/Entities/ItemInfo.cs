namespace Contoso.CommerceRuntime.Entities.DataModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml.Serialization;
    using global::Microsoft.Dynamics.Commerce.Runtime.DataModel;

    /// <summary>
    /// Item info, for use in transmitting via TS call.
    /// </summary>
    [Serializable]
    [XmlType("Item")]
    public class ItemInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemInfo"/> class.
        /// </summary>
        public ItemInfo()
        {
            this.AttributeValues = new AttributeValueInfoCollection();
        }
        /// <summary>
        /// Gets or sets the item identifier.
        /// </summary>
        [XmlAttribute]
        public string ItemId { get; set; }

        /// <summary>
        /// Gets or sets the line number.
        /// </summary>
        [XmlAttribute("LineNumber")]
        public decimal LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        [XmlAttribute]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Gets or sets the net amount including taxes.
        /// </summary>
        [XmlAttribute]
        public decimal NetAmount { get; set; }

        /// <summary>
        /// Gets the collection of attribute values.
        /// </summary>
        [XmlArray("AttributeValues")]
        public AttributeValueInfoCollection AttributeValues { get; private set; }


    }
}

