
namespace WTR.HardwareStation.Models
{
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    [DataContract]
    public class WTR_PaymentTerminalResponse : Response
    {
        //Common Fields
        [DataMember] //FieldCode 02 - Wirecard 50
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

        [DataMember] //FieldCode 40 - Wirecard 41
        public string TransactionAmount { get; set; }

        [DataMember] //FieldCode ZA
        public string TotalFee { get; set; }

        [DataMember] //FieldCode ZB
        public string FeeDueToMerchant { get; set; }

        [DataMember] //FieldCode ZC
        public string FeeDueFromMerchant { get; set; }

        [DataMember] //FieldCode 30
        public string CashCardCAN { get; set; }

        [DataMember] //FieldCode C2 - Wirecard 31
        public string ExpiryDate { get; set; }

        [DataMember] //FieldCode R0
        public string ResponseMessage1 { get; set; }

        [DataMember] //FieldCode R1
        public string ResponseMessage2 { get; set; }

        [DataMember] //FieldCode ZP
        public string PurchaseFee { get; set; }

        [DataMember] //FieldCode 01 - Wirecard 01
        public string ApprovalCode { get; set; }

        [DataMember] //FieldCode D2 - Wirecard 32
        public string CardIssuerName { get; set; }

        [DataMember] //FieldCode ZT
        public string AdditionalTransInfo { get; set; }

        [DataMember] //FieldCode D3 - Wirecard 65
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

        [DataMember] //Wirecard 30
        public string CardNumber { get; set; }

        [DataMember] //Wirecard 42
        public string DiscountAmount { get; set; }

        [DataMember] //Wirecard 43
        public string CardBalanceAmount { get; set; }

        [DataMember] //Wirecard 80
        public string SettlementTimeStamp { get; set; }

        [DataMember]
        public string ResponseCode { get; set; }

        [DataMember]
        public string CardNumberMasked { get; set; }

        [DataMember]
        public string CardType { get; set; }

        [DataMember]
        public string MerchantID { get; set; }

        [DataMember]
        public string RetrievalReferenceNumber { get; set; }

        [DataMember]
        public string EntryMode { get; set; }

        [DataMember]
        public string ApplicationLabel { get; set; }

        [DataMember]
        public string AID { get; set; }

        [DataMember]
        public string TVRTSI { get; set; }

        [DataMember]
        public string TC { get; set; }

        [DataMember]
        public string CardLabel { get; set; }

        [DataMember]
        public string CardEntryStatus { get; set; }


        [DataMember]
        public string Amount { get; set; }

        //HGM
        [DataMember]
        public string Host { get; set; }

        [DataMember]
        public string HostType { get; set; }

        [DataMember]
        public string CommandIdentifier { get; set; }

        [DataMember]
        public string CustomData2 { get; set; }

        [DataMember]
        public string CustomData3 { get; set; }

        [DataMember]
        public string UTRN { get; set; }

        [DataMember]
        public string TransactionInfo { get; set; }

        [DataMember]
        public string InvoiceNumber { get; set; }

        [DataMember]
        public string CardHolderName { get; set; }
    }

}