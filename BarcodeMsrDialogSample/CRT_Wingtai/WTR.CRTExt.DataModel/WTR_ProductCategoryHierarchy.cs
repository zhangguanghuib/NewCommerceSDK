using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTR.CRT.DataModel
{
    using Microsoft.Dynamics.Commerce.Runtime.ComponentModel.DataAnnotations;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using System.Runtime.Serialization;

    [DataContract]
    public class WTR_ProductCategoryHierarchy : CommerceEntity
    {
        private const string Lvl2NameColumn = "Lvl2Name";
        private const string Lvl3NameColumn = "Lvl3Name";
        private const string Lvl4NameColumn = "Lvl4Name";
        private const string Lvl5NameColumn = "Lvl5Name";
        private const string Lvl6NameColumn = "Lvl6Name";
        private const string ProductLevelColumn = "ProductLevel";
        public WTR_ProductCategoryHierarchy(string entityName) : base(entityName)
        {
        }


        public WTR_ProductCategoryHierarchy()
                : base("WTR_ProductCategoryHierarchy")
        {
            Initialize();
        }

        private void Initialize()
        {
        }

        [Key]
        [DataMember]
        [Column("Product")]
        public long Id
        {
            get { return (long)this["Product"]; }
            set { this["Product"] = value; }
        }


        [DataMember]
        [Column(Lvl2NameColumn)]
        public string Lvl2Name
        {
            get { return (string)this[Lvl2NameColumn]; }
            set { this[Lvl2NameColumn] = value; }
        }

        [DataMember]
        [Column(Lvl3NameColumn)]
        public string Lvl3Name
        {
            get { return (string)this[Lvl3NameColumn]; }
            set { this[Lvl3NameColumn] = value; }
        }

        [DataMember]
        [Column(Lvl4NameColumn)]
        public string Lvl4Name
        {
            get { return (string)this[Lvl4NameColumn]; }
            set { this[Lvl4NameColumn] = value; }
        }

        [DataMember]
        [Column(Lvl5NameColumn)]
        public string Lvl5Name
        {
            get { return (string)this[Lvl5NameColumn]; }
            set { this[Lvl5NameColumn] = value; }
        }

        [DataMember]
        [Column(Lvl6NameColumn)]
        public string Lvl6Name
        {
            get { return (string)this[Lvl6NameColumn]; }
            set { this[Lvl6NameColumn] = value; }
        }
        
        [DataMember]
        [Column(ProductLevelColumn)]
        public long ProductLevel
        {
            get { return (long)this[ProductLevelColumn]; }
            set { this[ProductLevelColumn] = value; }
        }
    }
}




