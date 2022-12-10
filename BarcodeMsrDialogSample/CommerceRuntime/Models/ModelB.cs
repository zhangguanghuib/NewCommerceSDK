using System;
using System.Collections.Generic;
using System.Text;

namespace GHZ.BarcodeMsrDialogSample.CommerceRuntime.Models
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.ComponentModel.DataAnnotations;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using SystemDataAnnotations = System.ComponentModel.DataAnnotations;

    public class ModelB:  CommerceEntity
    {
        private const string ColumnB = "ColumnA";

        public ModelB() : base("ModelB")
        {
        }

        [SystemDataAnnotations.Key]
        [Key]
        [DataMember]
        [Column(ColumnB)]
        public int fieldA
        {
            get { return (int)this[ColumnB]; }
            set { this[ColumnB] = value; }
        }
    }
}
