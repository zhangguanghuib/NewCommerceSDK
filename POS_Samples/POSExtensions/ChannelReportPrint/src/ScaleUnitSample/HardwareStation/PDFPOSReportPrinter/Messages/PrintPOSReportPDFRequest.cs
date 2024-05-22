
namespace SAG
{
    namespace Commerce.HardwareStation.Extension.PDFPOSReportPrinter
    {
        using System.Runtime.Serialization;

        [DataContract]
        public class PrintPOSReportPDFRequest
        {
            [DataMember]
            public string HTMLString { get; set; }

            [DataMember]
            public string PrinterName { get; set; }

            [DataMember]
            public string FilePath { get; set; }

            [DataMember]
            public string FileName { get; set; }

        }
    }
}