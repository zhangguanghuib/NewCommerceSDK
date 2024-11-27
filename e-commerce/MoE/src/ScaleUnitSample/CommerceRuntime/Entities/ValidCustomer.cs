namespace Moe
{
    namespace Commerce.Runtime.DataModel
    {
        using System.Runtime.Serialization;
        //using Microsoft.Dynamics.Commerce.Runtime.ComponentModel.DataAnnotations;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using System.ComponentModel.DataAnnotations;
        using System;
        using System.ComponentModel.DataAnnotations.Schema;

        /// <summary>
        /// Defines a simple class that holds information about artifacts detail.
        /// </summary>
        [DataContract]
        public class ValidCustomer : CommerceEntity
        {
            private const string ValidCustomerColumn = "VALIDCUSTOMER";
            private const string PartyIdColumn = "ID";

            /// <summary>
            /// Initializes a new instance of the <see cref="DefaultAddressEmail"/> class.
            /// </summary>
            public ValidCustomer()
                : base("ValidCustomer")
            {
            }
            
            [Column(ValidCustomerColumn)]
            [DataMember]
            public bool isValid 
            {
                get { return (bool)this[ValidCustomerColumn]; }
                set { this[ValidCustomerColumn] = value; }
            }

            /// <summary>
            /// Gets or sets the id.
            /// </summary>
            [Key]
            [DataMember]
            [Column(PartyIdColumn)]
            public long Id
            {
                get { return (long)this[PartyIdColumn]; }
                set { this[PartyIdColumn] = value; }
            }

        }
    }
}
