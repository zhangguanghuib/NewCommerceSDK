//THM_WTR

namespace WTR.HWExt.Peripherals
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    public class CCSettlementResponse
    {
        public string ResponseCode { get; set; }

        public string TerminalID { get; set; }

        public string MerchantID { get; set; }

        public string Host { get; set; }

        public string CardLabel { get; set; }

        public string CardType { get; set; }

        public string HostType { get; set; }

        public string BatchNumber { get; set; }

        public string BatchInformation { get; set; }
    }
}