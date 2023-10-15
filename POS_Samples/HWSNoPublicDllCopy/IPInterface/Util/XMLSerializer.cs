using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace PCEFTPOS.EFTClient.IPInterface
{
    public class XMLSerializer
    {
        public static string Serialize<T>(T input)
        {
            var xml = string.Empty;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StringWriter textWriter = new StringWriter())
                {
                    using (XmlWriter xmlWriter = XmlWriter.Create(textWriter))
                    {
                        serializer.Serialize(xmlWriter, input);
                    }
                    xml = textWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return xml;
        }

        public static T Deserialize<T>(string input)
        {
            T obj = default(T);
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                obj = (T)serializer.Deserialize(new System.IO.StringReader(input));
            }
            catch (Exception ex)
            {
                // Invalid XML
                throw ex;
            }

            return obj;
        }
    }
}
