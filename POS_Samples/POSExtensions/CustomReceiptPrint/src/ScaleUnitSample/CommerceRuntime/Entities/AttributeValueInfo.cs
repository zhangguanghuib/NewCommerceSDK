namespace Contoso.CommerceRuntime.Entities.DataModel
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Miscellaneous attribute value info for use in transmitting via TS call.
    /// </summary>
    [Serializable]
    [XmlType("AttributeValue")]
    public class AttributeValueInfo
    {
        /// <summary>
        /// Gets or sets the attribute value name.
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the text value.
        /// </summary>
        [XmlAttribute("TextValue")]
        public string TextValue { get; set; }

        /// <summary>
        /// Gets or sets the line number.
        /// </summary>
        [XmlAttribute("LineNumber")]
        public decimal LineNumber { get; set; }
    }
}
