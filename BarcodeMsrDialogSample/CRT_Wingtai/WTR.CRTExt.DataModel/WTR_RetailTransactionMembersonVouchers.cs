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
    public class WTR_RetailTransactionCRMVouchers : CommerceEntity
    {

        private const string IdColumn = "RECID";
        private const string TRANSACTIONIDColumn = "TRANSACTIONID";
        private const string MEMVOUCHERCODEColumn = "MEMVOUCHERCODE";


        public WTR_RetailTransactionCRMVouchers()
            : base("WTR_RetailTransactionCRMVouchers")
        {
            Initialize();
        }

        public WTR_RetailTransactionCRMVouchers(string className)
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

        [Key]
        [DataMember]
        [Column(IdColumn)]
        public long Id
        {
            get { return (long)this[IdColumn]; }
            set { this[IdColumn] = value; }
        }


        [DataMember]
        [Column(TRANSACTIONIDColumn)]
        public string TRANSACTIONID
        {
            get { return (string)this[TRANSACTIONIDColumn]; }
            set { this[TRANSACTIONIDColumn] = value; }
        }



        [DataMember]
        [Column(MEMVOUCHERCODEColumn)]
        public string MEMVOUCHERCODE
        {
            get { return (string)this[MEMVOUCHERCODEColumn]; }
            set { this[MEMVOUCHERCODEColumn] = value; }
        }

    }
}

