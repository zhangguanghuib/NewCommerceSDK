
namespace Contoso.CommerceRuntime.Entities.DataModel
{
    using Microsoft.Dynamics.Commerce.Runtime.ComponentModel.DataAnnotations;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using System.Runtime.Serialization;

    using SystemAnnotations = System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Defines a simple class that holds information about opening and closing times for a particular day.
    /// </summary>
    [DataContract]
    public class AVACProductTranslationEntity : CommerceEntity
    {
        private const string IdColumn = "ID";

        /// <summary>
        /// Initializes a new instance of the <see cref="AVACProductTranslationEntity"/> class.
        /// </summary>
        public AVACProductTranslationEntity()
            : base("AVACProductDetails")
        {
        }

        /// <summary>
        /// Gets or sets a property of LanguageId.
        /// </summary>
        [DataMember]
        [Column("LanguageId")]
        public string LanguageId
        {
            get { return (string)this["LanguageId"]; }
            set { this["LanguageId"] = value; }
        }

        /// <summary>
        /// Gets or sets a property of ProductId.
        /// </summary>
        [DataMember]
        [Column("ProductId")]
        public long ProductId
        {
            get { return (int)this["ProductId"]; }
            set { this["ProductId"] = value; }
        }

        /// <summary>
        /// Gets or sets a property of ProductName.
        /// </summary>
        [DataMember]
        [Column("ProductName")]
        public string ProductName
        {
            get { return (string)this["ProductName"]; }
            set { this["ProductName"] = value; }
        }

        /// <summary>
        /// Gets or sets a property of Description.
        /// </summary>
        [DataMember]
        [Column("Description")]
        public string Description
        {
            get { return (string)this["Description"]; }
            set { this["Description"] = value; }
        }

        [SystemAnnotations.Key]
        [DataMember]
        [Column(IdColumn)]
        public long Id
        {
            get { return (long)this[IdColumn]; }
            set { this[IdColumn] = value; }
        }

    }
}