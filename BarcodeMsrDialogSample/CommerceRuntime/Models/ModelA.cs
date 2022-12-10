using System;
using System.Collections.Generic;
using System.Text;

namespace GHZ.BarcodeMsrDialogSample.CommerceRuntime.Models
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.ComponentModel.DataAnnotations;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using SystemDataAnnotations = System.ComponentModel.DataAnnotations;

    public class ModelA:  CommerceEntity
    {
        private const string ColumnA = "ColumnA";

        public ModelA() : base("ModelA")
        {
        }

        [SystemDataAnnotations.Key]
        [Key]
        [DataMember]
        [Column(ColumnA)]
        public int fieldA
        {
            get { return (int)this[ColumnA]; }
            set { this[ColumnA] = value; }
        }
    }
}
