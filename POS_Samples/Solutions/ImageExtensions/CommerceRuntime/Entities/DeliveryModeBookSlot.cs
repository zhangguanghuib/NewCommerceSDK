namespace Contoso.GasStationSample.CommerceRuntime.Entities
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.ComponentModel.DataAnnotations;
    using SystemDataAnnotations = System.ComponentModel.DataAnnotations;
    using System;

    public class DlvModeBookSlot : CommerceEntity
    {
        private const string DlvModeCodeColumn = "DLVMODECODE";
        private const string DlvModeTxtColumn = "DLVMODETXT";
        private const string ShippingDateColumn = "SHIPPINGDATE";
        private const string MaxSlotColumn = "MAXSLOT";
        private const string FreeSlotColumn = "FREESLOT";

        public DlvModeBookSlot()
            : base("DlvModeBookSlot")
        {

        }

        [DataMember]
        [SystemDataAnnotations.Key]
        [Column(DlvModeCodeColumn)]
        public string DlvModeCode
        {
            get { return (string)this[DlvModeCodeColumn]; }
            set { this[DlvModeCodeColumn] = value; }
        }

        [DataMember]
        [Column(DlvModeTxtColumn)]
        public string DlvModeTxt
        {
            get { return (string)this[DlvModeTxtColumn]; }
            set { this[DlvModeTxtColumn] = value; }
        }

        [DataMember]
        [Column(ShippingDateColumn)]
        public DateTimeOffset ShippingDate
        {
            get { return (DateTimeOffset)this[ShippingDateColumn]; }
            set { this[ShippingDateColumn] = value; }
        }

        [DataMember]
        [Column(MaxSlotColumn)]
        public int MaxSlot
        {
            get { return (int)this[MaxSlotColumn]; }
            set { this[MaxSlotColumn] = value; }
        }

        [DataMember]
        [Column(FreeSlotColumn)]
        public int FreeSlot
        {
            get { return (int)this[FreeSlotColumn]; }
            set { this[FreeSlotColumn] = value; }
        }
    }
}
    