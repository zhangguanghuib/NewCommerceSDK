using System;
using System.Collections.Generic;

namespace WTR.CRT.DataModel
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.ComponentModel.DataAnnotations;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;

    [DataContract]
    public class PromotionCode : CommerceEntity
    {
        [Key]
        [DataMember]
        [Column("RECID")]
        public long Id
        {
            get { return (long)this["RECID"]; }
            set { this["RECID"] = value; }
        }

        [DataMember(Name = "Code")]
        public string Code { get; set; }

        public PromotionCode()
            : base("Wtr_PromotionCode")
        {
            Initialize();
        }

        public PromotionCode(string className)
        : base(className)
        {
            Initialize();
        }

        [OnDeserializing]
        public new void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }
        private void Initialize()
        {
            this.Id = 0;
            this.ExtensionProperties = new List<CommerceProperty>();
        }
    }

    [DataContract]
    public class Item : CommerceEntity
    {
        [Key]
        [DataMember]
        [Column("RECID")]
        public long Id
        {
            get { return (long)this["RECID"]; }
            set { this["RECID"] = value; }
        }

        [DataMember(Name = "Currency")]
        public string Currency { get; set; }

        [DataMember(Name = "ItemCode")]
        public string ItemCode { get; set; }

        [DataMember(Name = "Amount")]
        public double Amount { get; set; }

        public Item()
            : base("Wtr_Item")
        {
            Initialize();
        }

        public Item(string className)
        : base(className)
        {
            Initialize();
        }

        [OnDeserializing]
        public new void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }
        private void Initialize()
        {
            this.Id = 0;
            this.Amount = 0;
            this.ExtensionProperties = new List<CommerceProperty>();
        }


    }

    [DataContract]
    public class Payment : CommerceEntity
    {

        [Key]
        [DataMember]
        [Column("RECID")]
        public long Id
        {
            get { return (long)this["RECID"]; }
            set { this["RECID"] = value; }
        }

        [DataMember(Name = "PaymentType")]
        public string PaymentType { get; set; }

        [DataMember(Name = "Currency")]
        public string Currency { get; set; }

        [DataMember(Name = "Amount")]
        public double Amount { get; set; }

        public Payment()
            : base("Payment")
        {
            Initialize();
        }

        public Payment(string className)
        : base(className)
        {
            Initialize();
        }

        [OnDeserializing]
        public new void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }
        private void Initialize()
        {
            this.Id = 0;
            this.Amount = 0;
            this.ExtensionProperties = new List<CommerceProperty>();
        }

    }


    [DataContract]
    public class Wtr_Transaction : CommerceEntity
    {
        [Key]
        [DataMember]
        [Column("RECID")]
        public long Id
        {
            get { return (long)this["RECID"]; }
            set { this["RECID"] = value; }
        }
        #region ReturnTransactionFields
        [DataMember(Name = "OriginalReceiptNo")]
        public string OriginalReceiptNo { get; set; }

        [DataMember(Name = "OriginalOutletCode")]
        public string OriginalOutletCode { get; set; }

        [DataMember(Name = "OriginalTransactDate")]
        public string OriginalTransactDate { get; set; }

        [DataMember(Name = "CorrectionReceiptNo")]
        public string CorrectionReceiptNo { get; set; }

        [DataMember(Name = "Reason")]
        public string Reason { get; set; }
        [DataMember(Name = "AmtTobeAdjusted")]
        public double AmtTobeAdjusted { get; set; }
        [DataMember(Name = "AmtTobeAdjusted2")]
        public double AmtTobeAdjusted2 { get; set; }
        [DataMember(Name = "RefundSaleItemsList")]
        public List<RefundSaleItemsList> RefundSaleItemsList { get; set; }
        [DataMember(Name = "RefundPaymentList")]
        public List<RefundPaymentList> RefundPaymentList { get; set; }
        #endregion
        [DataMember(Name = "CardNo")]
        public string CardNo { get; set; }

        [DataMember(Name = "TRANSACTIONID")]
        public string TransactionId { get; set; }

        [DataMember(Name = "MEMBERID")]
        public string MemberId { get; set; }

        [DataMember(Name = "TransactDate")]
        public string TransactDate { get; set; }

        [DataMember(Name = "TransactTime")]
        public string TransactTime { get; set; }

        [DataMember(Name = "SalesAmt")]
        public double? SalesAmt { get; set; }

        [DataMember(Name = "SalesAmtToCalculatePoints")]
        public double? SalesAmtToCalculatePoints { get; set; }

        [DataMember(Name = "PointsCalculationType")]
        public double? PointsCalculationType { get; set; }

        [DataMember(Name = "RedemptionVoucherLists")]
        public List<RedemptionVoucherList> RedemptionVoucherLists { get; set; }

        [DataMember(Name = "TransactDetailLists")]
        public List<TransactDetailList> TransactDetailLists { get; set; }

        [DataMember(Name = "PaymentList")]
        public List<PaymentList> PaymentList { get; set; }

        [DataMember(Name = "PointsRedemption")]
        public List<PointsRedemption> PointsRedemption { get; set; }

        [DataMember(Name = "StoreCode")]
        public string StoreCode { get; set; }

        [DataMember(Name = "POSId")]
        public string POSId { get; set; }

        [DataMember(Name = "OperatorId")]
        public string OperatorId { get; set; }

        [DataMember(Name = "Description")]
        public string Description { get; set; }

        [DataMember(Name = "ReceiptNumber")]
        public string ReceiptNumber { get; set; }

        [DataMember(Name = "CampaignCode")]
        public string CampaignCode { get; set; }

        [DataMember(Name = "PromotionCodes")]
        public List<PromotionCode> PromotionCodes { get; set; }

        [DataMember(Name = "Items")]
        public List<Item> Items { get; set; }

        [DataMember(Name = "Payments")]
        public List<Payment> Payments { get; set; }
        [DataMember(Name = "Ref1")]
        public string Ref1 { get; set; }
        [DataMember(Name = "Ref2")]
        public string Ref2 { get; set; }
        [DataMember(Name = "Ref3")]
        public string Ref3 { get; set; }
        [DataMember(Name = "Ref4")]
        public string Ref4 { get; set; }
        [DataMember(Name = "Ref5")]
        public string Ref5 { get; set; }
        [DataMember(Name = "Ref6")]
        public string Ref6 { get; set; }
        [DataMember(Name = "Ref7")]
        public string Ref7 { get; set; }

        [DataMember(Name = "CRMErrorMessage")]
        public string CRMErrorMessage { get; set; }
        public Wtr_Transaction()
            : base("Wtr_Transaction")
        {
            Initialize();
        }

        public Wtr_Transaction(string className)
        : base(className)
        {
            Initialize();
        }

        [OnDeserializing]
        public new void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }

        private void Initialize()
        {
            this.Id = 0;
            this.ExtensionProperties = new List<CommerceProperty>();
            this.PromotionCodes = new List<PromotionCode>();
            this.Items = new List<Item>();
            this.Payments = new List<Payment>();
            this.RedemptionVoucherLists = new List<RedemptionVoucherList>();
            this.PointsRedemption = new List<DataModel.PointsRedemption>();
            this.PaymentList = new List<PaymentList>();
            this.RefundPaymentList = new List<RefundPaymentList>();
            this.RefundSaleItemsList = new List<RefundSaleItemsList>();
            this.TransactDetailLists = new List<TransactDetailList>();
            this.Ref1 = string.Empty;
            this.Ref2 = string.Empty;
            this.Ref3 = string.Empty;
            this.Ref4 = string.Empty;
            this.Ref5 = string.Empty;
            this.Ref6 = string.Empty;
            this.Ref7 = string.Empty;
            this.ReceiptNumber = string.Empty;
            this.CorrectionReceiptNo = string.Empty;
            this.StoreCode = string.Empty;
            this.OriginalOutletCode = string.Empty;
            this.OriginalReceiptNo = string.Empty;
            this.Description = string.Empty;
            this.CardNo = string.Empty;
            this.CampaignCode = string.Empty;
        }
    }


    public class TransactDetailList
    {
        public string Department_Code { get; set; }
        public string SubDepartment_Code { get; set; }
        public string Category_Code { get; set; }
        public string SubCategory_Code { get; set; }
        public string Brand_Code { get; set; }
        public string Level_Code { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal Nett { get; set; }
        public int LineNo { get; set; }
        public string Ref1 { get; set; }
        public string Ref2 { get; set; }
        public string Ref3 { get; set; }
        public string Ref4 { get; set; }
        public string Ref5 { get; set; }
        public string Ref6 { get; set; }
        public string Ref7 { get; set; }
    }

    public class PaymentList
    {
        public string Type { get; set; }
        public string Mode { get; set; }
        public decimal Value { get; set; }
        public string Currency { get; set; }
        public string Ref1 { get; set; }
        public string Ref2 { get; set; }
        public string Ref3 { get; set; }
        public string Ref4 { get; set; }
        public string Ref5 { get; set; }
        public string Ref6 { get; set; }
        public string Ref7 { get; set; }
        public int LineNo { get; set; }
        public int CurRate { get; set; }
        public string ForeignCurrency { get; set; }
        public int ForeignCurrencyValue { get; set; }
        public string CardName { get; set; }
    }
    public class RedemptionVoucherList
    {
        public string VoucherNo { get; set; }

        public RedemptionVoucherList(string voucherNo)
        {
            this.VoucherNo = voucherNo;
        }
    }
    public class PointsRedemption
    {
        public string DollarToPointsRatio { get; set; }
        public int PointsToBeDeducted { get; set; }
        public int PointsUsage { get; set; }
        public int GivenRebateValue { get; set; }

        public PointsRedemption()
        {
            this.DollarToPointsRatio = string.Empty;
            this.PointsToBeDeducted = 0;
            this.PointsUsage = 0;
            this.GivenRebateValue = 0;
        }

        public PointsRedemption(string ratio, int pointsDeducted, int pointsUsage, int givenRebateValue)
        {
            this.DollarToPointsRatio = ratio;
            this.PointsToBeDeducted = pointsDeducted;
            this.PointsUsage = pointsUsage;
            this.GivenRebateValue = givenRebateValue;
        }
    }

    public class RefundSaleItemsList
    {
        public int LineNo { get; set; }
        public string ItemCode { get; set; }
        public decimal Price { get; set; }
        public decimal NettAmount { get; set; }
        public int ItemQty { get; set; }
        public string Ref1 { get; set; }
        public string Ref2 { get; set; }
        public string Ref3 { get; set; }
        public string Ref4 { get; set; }
        public string Ref5 { get; set; }
        public string Ref6 { get; set; }
        public string Ref7 { get; set; }
        public string Department_Code { get; set; }
        public string SubDepartment_Code { get; set; }
        public string Category_Code { get; set; }
        public string SubCategory_Code { get; set; }
        public string Brand_Code { get; set; }
        public string Level_Code { get; set; }
        public string Description { get; set; }
        public object DiscountPer { get; set; }
        public object Points { get; set; }
    }

    public class RefundPaymentList
    {
        public string Type { get; set; }
        public string Mode { get; set; }
        public double Value { get; set; }
        public string Currency { get; set; }
        public string Ref1 { get; set; }
        public string Ref2 { get; set; }
        public string Ref3 { get; set; }
        public string Ref4 { get; set; }
        public string Ref5 { get; set; }
        public string Ref6 { get; set; }
        public string Ref7 { get; set; }
        public int LineNo { get; set; }
        public int CurRate { get; set; }
        public string ForeignCurrency { get; set; }
        public int ForeignCurrencyValue { get; set; }
        public string CardName { get; set; }
    }

}
