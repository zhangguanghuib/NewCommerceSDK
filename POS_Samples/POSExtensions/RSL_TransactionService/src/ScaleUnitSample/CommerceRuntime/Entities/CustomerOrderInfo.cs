namespace Contoso.CommerceRuntime.Entities.DataModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;

    /// <summary>
    /// Represents the customer order information. Used in serializing and transmitting the order to AX.
    /// </summary>
    [Serializable]
    [XmlRoot("CustomerOrder")]
    public class CustomerOrderInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerOrderInfo"/> class.
        /// </summary>
        public CustomerOrderInfo()
        {
            this.Items = new ItemInfoCollection();
            this.AttributeValues = new AttributeValueInfoCollection();
            this.ExtensionProperties = new Collection<CommerceProperty>();
        }

        /// <summary>
        /// Gets or sets the transaction id.
        /// </summary>
        [XmlElement("TransactionId")]
        public string TransactionId { get; set; }

        /// <summary>
        /// Gets or sets the customer account number.
        /// </summary>
        [XmlElement("CustomerAccount")]
        public string CustomerAccount { get; set; }

        /// <summary>
        /// Gets or sets the creation date in string format.
        /// </summary>
        [XmlElement("CreationDate")]
        public string CreationDateString { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the order/quote used during edition flow.
        /// </summary>
        [XmlElement("Id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets the collection of item information.
        /// </summary>
        public ItemInfoCollection Items { get; private set; }

        /// <summary>
        /// Gets the collection of attribute values.
        /// </summary>
        [XmlArray("AttributeValues")]
        public AttributeValueInfoCollection AttributeValues { get; private set; }

        /// <summary>
        /// Gets or sets the extension properties collection.
        /// </summary>
        [XmlArray("ExtensionProperties")]
        public Collection<CommerceProperty> ExtensionProperties { get; set; }

        /// <summary>
        /// Serializes the current customer order information to XML.
        /// </summary>
        /// <returns>The object in XML format.</returns>
        public string ToXml()
        {
            var xmlWriterSettings = new XmlWriterSettings { CheckCharacters = false, CloseOutput = true };
            using (var strWriter = new StringWriter(CultureInfo.CurrentCulture))
            using (var xmlWriter = XmlWriter.Create(strWriter, xmlWriterSettings))
            {
                new XmlSerializer(typeof(CustomerOrderInfo)).Serialize(xmlWriter, this);
                xmlWriter.Flush();
                return strWriter.ToString();
            }
        }
    }
}
