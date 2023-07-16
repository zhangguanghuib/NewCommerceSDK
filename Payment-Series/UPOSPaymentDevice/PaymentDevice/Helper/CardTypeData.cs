/// <summary>
///   class is used to deserialize xml card type
/// </summary>
namespace Contoso
{
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Xml.Serialization;

    namespace Commerce.HardwareStation.Extension.UPOS_HardwareStation.DataModel
    {
        [XmlRoot("UPOS_CardTypeInfoHeader")]
        public class UPOS_CardTypeInfoHeader
        {
            public Collection<UPOS_CardTypeInfoDetails> CardTypeInfoDetails;

            public string ToXml()
            {
                string xmlString = string.Empty;
                XmlSerializer serializer = new XmlSerializer(typeof(UPOS_CardTypeInfoHeader));
                using (StringWriter writer = new StringWriter())
                {
                    serializer.Serialize(writer, this);
                    writer.Flush();
                    xmlString = writer.ToString();
                }
                return xmlString;
            }
            public static UPOS_CardTypeInfoHeader FromXml(string orderXml)
            {
                UPOS_CardTypeInfoHeader orderHeaderInfo;
                XmlSerializer serializer = new XmlSerializer(typeof(UPOS_CardTypeInfoHeader));
                using (StringReader reader = new StringReader(orderXml))
                {
                    serializer = new XmlSerializer(typeof(UPOS_CardTypeInfoHeader));
#pragma warning disable CA5369 // Use XmlReader For Deserialize
                    orderHeaderInfo = serializer.Deserialize(reader) as UPOS_CardTypeInfoHeader;
#pragma warning restore CA5369 // Use XmlReader For Deserialize
                    
                    return orderHeaderInfo;
                }
            }
        }

        [XmlType("UPOS_CardTypeInfoDetails")]
        public class UPOS_CardTypeInfoDetails
        {
            [XmlAttribute("NAME")]
            public string NAME;

            [XmlAttribute("NUMBERFROM")]
            public string NUMBERFROM;

            [XmlAttribute("NUMBERTO")]
            public string NUMBERTO;
        }
    }
}

