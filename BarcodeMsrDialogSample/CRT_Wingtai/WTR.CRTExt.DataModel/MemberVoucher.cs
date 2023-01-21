/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

namespace WTR.CRT.DataModel
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.ComponentModel.DataAnnotations;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a simple class that holds information about CRM voucher 
    /// </summary>
    [DataContract]
    public class MemberVoucher : CommerceEntity
    {
        private const string IdColumn = "RECID";

        private const string VoucherCodeColumn = "VoucherCode";
        private const string VoucherNumberColumn = "VoucherNumber";
        private const string VoucherDescriptionColumn = "VoucherDescription";
        private const string VoucherNameColumn = "VoucherName";
        private const string IssueDateColumn = "IssueDate";
        private const string CollectDateColumn = "CollectDate";
        private const string PrintDateColumn = "PrintDate";
        private const string UtilizeDateColumn = "UtilizeDate";
        private const string SalePersonColumn = "SalePerson";
        private const string RegistratorColumn = "Registrator";
        private const string MemberNumberColumn = "MemberNumber";
        private const string IssueReceiptColumn = "IssueReceipt";
        private const string CollectReceiptColumn = "CollectReceipt";
        private const string PrintReceiptColumn = "PrintReceipt";
        private const string UtilizeReceiptColumn = "UtilizeReceipt";
        private const string IssueLocationCodeColumn = "IssueLocationCode";
        private const string CollectLocationCodeColumn = "CollectLocationCode";
        private const string PrintLocationCodeColumn = "PrintLocationCode";
        private const string UtilizeLocationCodeColumn = "UtilizeLocationCode";
        private const string StatusColumn = "Status";
        private const string ReferenceNumberColumn = "ReferenceNumber";
        private const string VoucherTextColumn = "VoucherText";
        private const string SourceColumn = "Source";
        private const string ValidFromColumn = "ValidFrom";
        private const string ExpiryDateColumn = "ExpiryDate";
        private const string IssueLocationColumn = "IssueLocation";
        private const string CollectLocationColumn = "CollectLocation";
        private const string PrintLocationColumn = "PrintLocation";
        private const string UtilizeLocationColumn = "UtilizeLocation";
        private const string DenominationValueColumn = "DenominationValue";
        private const string DenominationCurrencyColumn = "DenominationCurrency";
        private const string CostValueColumn = "CostValue";
        private const string CostCurrencyColumn = "CostCurrency";
        private const string CanPrintColumn = "CanPrint";
        private const string NeedApprovalColumn = "NeedApproval";
        private const string VoucherTypeColumn = "VoucherType";
        private const string VoucherTypeCodeColumn = "VoucherTypeCode";
        private const string VoucherSubTypeColumn = "VoucherSubType";
        private const string VoucherSubTypeCodeColumn = "VoucherSubTypeCode";
        private const string IssuerColumn = "Issuer";
        private const string DescriptionColumn = "Description";
        private const string CanUtilizeInLocationColumn = "CanUtilizeInLocation";
        private const string ShortDescriptionColumn = "ShortDescription";
        private const string CustomPropertiesColumn = "CustomProperties";
        private const string IsUtilizeColumn = "IsUtilize";
        private const string MessageColumn = "Message";
        private const string DollarValueColumn = "DollarValue";
        private const string DiscountPercentageColumn = "DiscountPercentage";


        /// <summary>
        /// Initializes a new instance of the <see cref="MemberVoucher"/> class.
        /// </summary>
        public MemberVoucher()
            : base("MemberVoucher")
        {
            Initialize();
        }

        public MemberVoucher(string className)
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
            this.DenominationValue = 0;
            this.CostValue = 0;
            this.CanPrint = false;
            this.NeedApproval = false;
            this.ExtensionProperties = new List<CommerceProperty>();
            this.IsUtilize = 0;
            this.Message = string.Empty;
            this.DiscountPercentage = 0;
            this.DollarValue = 0;
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Key]
        [DataMember]
        [Column(IdColumn)]
        public long Id
        {
            get { return (long)this[IdColumn]; }
            set { this[IdColumn] = value; }
        }


        [DataMember]
        [Column(VoucherCodeColumn)]
        public string VoucherCode
        {
            get { return (string)this[VoucherCodeColumn]; }
            set { this[VoucherCodeColumn] = value; }
        }

        [DataMember]
        [Column(VoucherNumberColumn)]
        public string VoucherNumber
        {
            get { return (string)this[VoucherNumberColumn]; }
            set { this[VoucherNumberColumn] = value; }
        }

        [DataMember]
        [Column(VoucherDescriptionColumn)]
        public string VoucherDescription
        {
            get { return (string)this[VoucherDescriptionColumn]; }
            set { this[VoucherDescriptionColumn] = value; }
        }

        [DataMember]
        [Column(VoucherNameColumn)]
        public string VoucherName
        {
            get { return (string)this[VoucherNameColumn]; }
            set { this[VoucherNameColumn] = value; }
        }

        [DataMember]
        [Column(IssueDateColumn)]
        public DateTime? IssueDate
        {
            get { return (DateTime?)this[IssueDateColumn]; }
            set { this[IssueDateColumn] = value; }
        }

        [DataMember]
        [Column(CollectDateColumn)]
        public DateTime? CollectDate
        {
            get { return (DateTime?)this[CollectDateColumn]; }
            set { this[CollectDateColumn] = value; }
        }

        [DataMember]
        [Column(PrintDateColumn)]
        public DateTime? PrintDate
        {
            get { return (DateTime?)this[PrintDateColumn]; }
            set { this[PrintDateColumn] = value; }
        }


        [DataMember]
        [Column(UtilizeDateColumn)]
        public DateTime? UtilizeDate
        {
            get { return (DateTime?)this[UtilizeDateColumn]; }
            set { this[UtilizeDateColumn] = value; }
        }

        [DataMember]
        [Column(SalePersonColumn)]
        public string SalePerson
        {
            get { return (string)this[SalePersonColumn]; }
            set { this[SalePersonColumn] = value; }
        }

        [DataMember]
        [Column(RegistratorColumn)]
        public string Registrator
        {
            get { return (string)this[RegistratorColumn]; }
            set { this[RegistratorColumn] = value; }
        }

        [DataMember]
        [Column(MemberNumberColumn)]
        public string MemberNumber
        {
            get { return (string)this[MemberNumberColumn]; }
            set { this[MemberNumberColumn] = value; }
        }

        [DataMember]
        [Column(IssueReceiptColumn)]
        public string IssueReceipt
        {
            get { return (string)this[IssueReceiptColumn]; }
            set { this[IssueReceiptColumn] = value; }
        }

        [DataMember]
        [Column(CollectReceiptColumn)]
        public string CollectReceipt
        {
            get { return (string)this[CollectReceiptColumn]; }
            set { this[CollectReceiptColumn] = value; }
        }

        [DataMember]
        [Column(PrintReceiptColumn)]
        public string PrintReceipt
        {
            get { return (string)this[PrintReceiptColumn]; }
            set { this[PrintReceiptColumn] = value; }
        }


        [DataMember]
        [Column(UtilizeReceiptColumn)]
        public string UtilizeReceipt
        {
            get { return (string)this[UtilizeReceiptColumn]; }
            set { this[UtilizeReceiptColumn] = value; }
        }


        [DataMember]
        [Column(IssueLocationCodeColumn)]
        public string IssueLocationCode
        {
            get { return (string)this[IssueLocationCodeColumn]; }
            set { this[IssueLocationCodeColumn] = value; }
        }

        [DataMember]
        [Column(CollectLocationCodeColumn)]
        public string CollectLocationCode
        {
            get { return (string)this[CollectLocationCodeColumn]; }
            set { this[CollectLocationCodeColumn] = value; }
        }

        [DataMember]
        [Column(PrintLocationCodeColumn)]
        public string PrintLocationCode
        {
            get { return (string)this[PrintLocationCodeColumn]; }
            set { this[PrintLocationCodeColumn] = value; }
        }

        [DataMember]
        [Column(UtilizeLocationCodeColumn)]
        public string UtilizeLocationCode
        {
            get { return (string)this[UtilizeLocationCodeColumn]; }
            set { this[UtilizeLocationCodeColumn] = value; }
        }


        [DataMember]
        [Column(StatusColumn)]
        public string Status
        {
            get { return (string)this[StatusColumn]; }
            set { this[StatusColumn] = value; }
        }

        [DataMember]
        [Column(ReferenceNumberColumn)]
        public string ReferenceNumber
        {
            get { return (string)this[ReferenceNumberColumn]; }
            set { this[ReferenceNumberColumn] = value; }
        }

        [DataMember]
        [Column(VoucherTextColumn)]
        public string VoucherText
        {
            get { return (string)this[VoucherTextColumn]; }
            set { this[VoucherTextColumn] = value; }
        }

        [DataMember]
        [Column(SourceColumn)]
        public string Source
        {
            get { return (string)this[SourceColumn]; }
            set { this[SourceColumn] = value; }
        }

        [DataMember]
        [Column(ValidFromColumn)]
        public DateTime? ValidFrom
        {
            get { return (DateTime?)this[ValidFromColumn]; }
            set { this[ValidFromColumn] = value; }
        }

        [DataMember]
        [Column(ExpiryDateColumn)]
        public DateTime? ExpiryDate
        {
            get { return (DateTime?)this[ExpiryDateColumn]; }
            set { this[ExpiryDateColumn] = value; }
        }


        [DataMember]
        [Column(IssueLocationColumn)]
        public string IssueLocation
        {
            get { return (string)this[IssueLocationColumn]; }
            set { this[IssueLocationColumn] = value; }
        }

        [DataMember]
        [Column(CollectLocationColumn)]
        public string CollectLocation
        {
            get { return (string)this[CollectLocationColumn]; }
            set { this[CollectLocationColumn] = value; }
        }

        [DataMember]
        [Column(PrintLocationColumn)]
        public string PrintLocation
        {
            get { return (string)this[PrintLocationColumn]; }
            set { this[PrintLocationColumn] = value; }
        }


        [DataMember]
        [Column(UtilizeLocationColumn)]
        public string UtilizeLocation
        {
            get { return (string)this[UtilizeLocationColumn]; }
            set { this[UtilizeLocationColumn] = value; }
        }


        [DataMember]
        [Column(DenominationValueColumn)]
        public decimal? DenominationValue
        {
            get { return (decimal?)this[DenominationValueColumn]; }
            set { this[DenominationValueColumn] = value; }
        }

        [DataMember]
        [Column(DenominationCurrencyColumn)]
        public string DenominationCurrency
        {
            get { return (string)this[DenominationCurrencyColumn]; }
            set { this[DenominationCurrencyColumn] = value; }
        }


        [DataMember]
        [Column(CostValueColumn)]
        public decimal? CostValue
        {
            get { return (decimal?)this[CostValueColumn]; }
            set { this[CostValueColumn] = value; }
        }


        [DataMember]
        [Column(CostCurrencyColumn)]
        public string CostCurrency
        {
            get { return (string)this[CostCurrencyColumn]; }
            set { this[CostCurrencyColumn] = value; }
        }

        [DataMember]
        [Column(CanPrintColumn)]
        public bool? CanPrint
        {
            get { return (bool?)this[CanPrintColumn]; }
            set { this[CanPrintColumn] = value; }
        }

        [DataMember]
        [Column(NeedApprovalColumn)]
        public bool? NeedApproval
        {
            get { return (bool?)this[NeedApprovalColumn]; }
            set { this[NeedApprovalColumn] = value; }
        }


        [DataMember]
        [Column(VoucherTypeColumn)]
        public string VoucherType
        {
            get { return (string)this[VoucherTypeColumn]; }
            set { this[VoucherTypeColumn] = value; }
        }

        [DataMember]
        [Column(VoucherTypeCodeColumn)]
        public string VoucherTypeCode
        {
            get { return (string)this[VoucherTypeCodeColumn]; }
            set { this[VoucherTypeCodeColumn] = value; }
        }

        [DataMember]
        [Column(VoucherSubTypeColumn)]
        public string VoucherSubType
        {
            get { return (string)this[VoucherSubTypeColumn]; }
            set { this[VoucherSubTypeColumn] = value; }
        }


        [DataMember]
        [Column(VoucherSubTypeCodeColumn)]
        public string VoucherSubTypeCode
        {
            get { return (string)this[VoucherSubTypeCodeColumn]; }
            set { this[VoucherSubTypeCodeColumn] = value; }
        }


        [DataMember]
        [Column(IssuerColumn)]
        public string Issuer
        {
            get { return (string)this[IssuerColumn]; }
            set { this[IssuerColumn] = value; }
        }


        [DataMember]
        [Column(DescriptionColumn)]
        public string Description
        {
            get { return (string)this[DescriptionColumn]; }
            set { this[DescriptionColumn] = value; }
        }

        [DataMember]
        [Column(CanUtilizeInLocationColumn)]
        public string CanUtilizeInLocation
        {
            get { return (string)this[CanUtilizeInLocationColumn]; }
            set { this[CanUtilizeInLocationColumn] = value; }
        }



        [DataMember]
        [Column(ShortDescriptionColumn)]
        public string ShortDescription
        {
            get { return (string)this[ShortDescriptionColumn]; }
            set { this[ShortDescriptionColumn] = value; }
        }


        //[DataMember]
        //[Column(CustomPropertiesColumn)]
        //public object CustomProperties
        //{
        //    get { return (object)this[CustomPropertiesColumn]; }
        //    set { this[CustomPropertiesColumn] = value; }
        //}

        [DataMember]
        [Column(IsUtilizeColumn)]
        public int IsUtilize
        {
            get { return (int)this[IsUtilizeColumn]; }
            set { this[IsUtilizeColumn] = value; }
        }

        [DataMember]
        [Column(MessageColumn)]
        public string Message
        {
            get { return (string)this[MessageColumn]; }
            set { this[MessageColumn] = value; }
        }

        [DataMember]
        [Column(DollarValueColumn)]
        public decimal DollarValue
        {
            get { return (decimal)this[DollarValueColumn]; }
            set { this[DollarValueColumn] = value; }
        }

        [DataMember]
        [Column(DiscountPercentageColumn)]
        public decimal DiscountPercentage
        {
            get { return (decimal)this[DiscountPercentageColumn]; }
            set { this[DiscountPercentageColumn] = value; }
        }
    }

}
