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
    /// Defines a simple class that holds information about opening and closing times for a particular day.
    /// </summary>
    [DataContract]
    public class WTR_CancelDiscount : CommerceEntity
    {
        private const string IdColumn = "RECID";

        private const string ReceiptNoColumn = "ReceiptNo";
        private const string VoucherNumberColumn = "VoucherNumber";
        private const string TransactionDateColumn = "TransactionDate";
        private const string AmountColumn = "Amount";
        private const string IsRedemptionTypeColumn = "IsRedemptionTypeColumn";
        private const string IsCancelledColumn = "IsCancelled";
        private const string MessageColumn = "Message";
        /// <summary>
        /// Initializes a new instance of the <see cref="WTR_CancelDiscount"/> class.
        /// </summary>
        public WTR_CancelDiscount()
            : base("WTR_CancelDiscount")
        {
            Initialize();
        }

        public WTR_CancelDiscount(string className)
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

            this.ExtensionProperties = new List<CommerceProperty>();
            this.IsCancelled = 0;
            this.IsRedemptionType = 0;
            this.Amount = Convert.ToDecimal(0);
            this.Message = string.Empty;
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
        [Column(ReceiptNoColumn)]
        public string VoucherCode
        {
            get { return (string)this[ReceiptNoColumn]; }
            set { this[ReceiptNoColumn] = value; }
        }

        [DataMember]
        [Column(VoucherNumberColumn)]
        public string VoucherNumber
        {
            get { return (string)this[VoucherNumberColumn]; }
            set { this[VoucherNumberColumn] = value; }
        }

        [DataMember]
        [Column(TransactionDateColumn)]
        public DateTime? TransactionDate
        {
            get { return (DateTime)this[TransactionDateColumn]; }
            set { this[TransactionDateColumn] = value; }
        }


        [DataMember]
        [Column(AmountColumn)]
        public decimal Amount
        {
            get { return (decimal)this[AmountColumn]; }
            set { this[AmountColumn] = value; }
        }

        [DataMember]
        [Column(IsRedemptionTypeColumn)]
        public int IsRedemptionType
        {
            get { return (int)this[IsRedemptionTypeColumn]; }
            set { this[IsRedemptionTypeColumn] = value; }
        }


        [DataMember]
        [Column(IsCancelledColumn)]
        public int IsCancelled
        {
            get { return (int)this[IsCancelledColumn]; }
            set { this[IsCancelledColumn] = value; }
        }

        [DataMember]
        [Column(MessageColumn)]
        public string Message
        {
            get { return (string)this[MessageColumn]; }
            set { this[MessageColumn] = value; }
        }

    }
}
