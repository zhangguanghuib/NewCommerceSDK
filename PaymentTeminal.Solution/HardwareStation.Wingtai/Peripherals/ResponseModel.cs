using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace WTR.HWExt.Peripherals
{       
    [DataContract]
    public class ResponseModel
    {
        [DataMember] //FieldCode 02
        public string ResponseText { get; set; }

        [DataMember] //FieldCode D0
        public string MerchantNameAddress { get; set; }

        [DataMember] //FieldCode 03
        public string TransactionDate { get; set; }

        [DataMember] //FieldCode 04
        public string TransactionTime { get; set; }

        [DataMember] //FieldCode 65
        public string Stan { get; set; }

        [DataMember] //FieldCode 16
        public string TerminalID { get; set; }

        [DataMember] //FieldCode D1
        public string MerchantId { get; set; }

        [DataMember] //FieldCode 50
        public string BatchNumber { get; set; }

        [DataMember] //FieldCode 40
        public string TransactionAmount { get; set; }

        [DataMember] //FieldCode ZA
        public string TotalFee { get; set; }

        [DataMember] //FieldCode ZB
        public string FeeDueToMerchant { get; set; }

        [DataMember] //FieldCode ZC
        public string FeeDueFromMerchant { get; set; }

        [DataMember] //FieldCode 30
        public string CashCardCAN { get; set; }

        [DataMember] //FieldCode C2
        public string ExpiryDate { get; set; }

        [DataMember] //FieldCode R0
        public string ResponseMessage1 { get; set; }

        [DataMember] //FieldCode R1
        public string ResponseMessage2 { get; set; }

        [DataMember] //FieldCode ZP
        public string PurchaseFee { get; set; }

        [DataMember] //FieldCode 01
        public string ApprovalCode { get; set; }

        [DataMember] //FieldCode D2
        public string CardIssuerName { get; set; }

        [DataMember] //FieldCode ZT
        public string AdditionalTransInfo { get; set; }

        [DataMember] //FieldCode D3
        public string RetrievalRefNumber { get; set; }

        [DataMember] //FieldCode C1
        public string CEPASVersion { get; set; }

        [DataMember] //FieldCode C0
        public string TransData { get; set; }

        [DataMember] //FieldCode 41
        public string CashbackServiceFee { get; set; }

        [DataMember] //FieldCode FA
        public string ForeignAmount { get; set; }

        [DataMember] //FieldCode F0
        public string ForeignMID { get; set; }

        [DataMember]
        public string ResponseCode { get; set; }

        //[DataMember]
        //public string ResponseMessage { get; set; }

        //[DataMember]
        //public string ResponseCode { get; set; }        
    }
}

