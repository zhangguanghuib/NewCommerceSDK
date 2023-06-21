using Microsoft.Dynamics.Commerce.Runtime.Messages;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SAG.HardwareStation.PaymentTerminal.DBS.DataContract
{
    [DataContract]
    public class DBSResponse :Response
    {
        public DBSResponse()
        {
            this.Initialize();
        }

        [DataMember(Name = "ErrorMessage")]
        public string ErrorMessage;
        [DataMember(Name = "TransactionType")]
        public string TransactionType { get; set; }

        [DataMember(Name = "ECRRefNo")]
        public string ECRRefNo { get; set; }

        [DataMember(Name = "AmountInCents")]
        public string AmountInCents { get; set; }

        [DataMember(Name = "TipsInCents")]
        public string TipsInCents { get; set; }

        [DataMember(Name = "ResponseCode")]
        public string ResponseCode { get; set; }

        [DataMember(Name = "ResponseText")]
        public string ResponseText { get; set; }

        [DataMember(Name = "TransactionDateTime")]
        public string TransactionDateTime { get; set; }

        [DataMember(Name = "CardType")]
        public string CardType { get; set; }

        [DataMember(Name = "CardNo")]
        public string CardNo { get; set; }

        [DataMember(Name = "ExpirationDate")]
        public string ExpirationDate { get; set; }

        [DataMember(Name = "CardHolderName")]
        public string CardHolderName { get; set; }

        [DataMember(Name = "TerminalNo")]
        public string TerminalNo { get; set; }

        [DataMember(Name = "MerchantNo")]
        public string MerchantNo { get; set; }

        [DataMember(Name = "TraceNo")]
        public string TraceNo { get; set; }

        [DataMember(Name = "BatchNo")]
        public string BatchNo { get; set; }

        [DataMember(Name = "ApprovalCode")]
        public string ApprovalCode { get; set; }

        [DataMember(Name = "RetrievalRefNo")]
        public string RetrievalRefNo { get; set; }
        [DataMember(Name = "EntryMode")]
        public string EntryMode { get; set; }
        [DataMember(Name = "EMVApplicationId")]
        public string EMVApplicationId { get; set; }
        [DataMember(Name = "EMVTRansactionCryptogram")]
        public string EMVTRansactionCryptogram { get; set; }
        [DataMember(Name = "EMVApplicationName")]
        public string EMVApplicationName { get; set; }
        [DataMember(Name = "EMVNoSignatureRequired")]
        public string EMVNoSignatureRequired { get; set; }
        [DataMember(Name = "InstallmentPlan")]
        public string InstallmentPlan { get; set; }
        [DataMember(Name = "MonthlyAmountDue")]
        public string MonthlyAmountDue { get; set; }
        [DataMember(Name = "Reserved")]
        public string Reserved { get; set; }
        [DataMember(Name = "DCCAmountInCents")]
        public string DCCAmountInCents { get; set; }
        [DataMember(Name = "DCCTipInCents")]
        public string DCCTipInCents { get; set; }
        [DataMember(Name = "ExchangeRate")]
        public string ExchangeRate { get; set; }
        [DataMember(Name = "LocalCurrencyName")]
        public string LocalCurrencyName { get; set; }
        [DataMember(Name = "ForeignCurrencyName")]
        public string ForeignCurrencyName { get; set; }
        [DataMember(Name = "DCCPrintText")]
        public string DCCPrintText { get; set; }
        [DataMember(Name = "ExchangeRateFormat")]
        public string ExchangeRateFormat { get; set; }
        [DataMember(Name = "DCCMarkUpRateText")]
        public string DCCMarkUpRateText { get; set; }

        private void Initialize()
        {
            this.ErrorMessage = "";
            this.TransactionType = "";
            this.ECRRefNo = "";
            this.AmountInCents = "";
            this.TipsInCents = "";
            this.ResponseCode = "";
            this.ResponseText = "";
            this.TransactionDateTime = "";
            this.CardType = "";
            this.CardNo = "";
            this.ExpirationDate = "";
            this.CardHolderName = "";
            this.TerminalNo = "";
            this.MerchantNo = "";
            this.TraceNo = "";
            this.BatchNo = "";
            this.ApprovalCode = "";
            this.RetrievalRefNo = "";
            this.EntryMode = "";
            this.EMVApplicationId = "";
            this.EMVTRansactionCryptogram = "";
            this.EMVApplicationName = "";
            this.EMVNoSignatureRequired = "";
            this.InstallmentPlan = "";
            this.MonthlyAmountDue = "";
            this.Reserved = "";
            this.DCCAmountInCents = "";
            this.DCCTipInCents = "";
            this.ExchangeRate = "";
            this.LocalCurrencyName = "";
            this.ForeignCurrencyName = "";
            this.DCCPrintText = "";
            this.ExchangeRateFormat = "";
            this.DCCMarkUpRateText = "";

        }
    }
}
