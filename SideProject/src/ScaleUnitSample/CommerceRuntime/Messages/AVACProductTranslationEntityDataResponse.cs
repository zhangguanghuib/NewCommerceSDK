namespace Contoso.CommerceRuntime.Messages
{
    using System.Runtime.Serialization;
    using Contoso.CommerceRuntime.Entities.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    /// <summary>
    /// Defines a simple response class that holds a collection of Example Entities.
    /// </summary>
    [DataContract]
    public sealed class AVACProductTranslationEntityDataResponse : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExampleEntityDataResponse"/> class.
        /// </summary>
        /// <param name="exampleEntities">The collection of Example Entities.</param>
        public AVACProductTranslationEntityDataResponse(PagedResult<AVACProductTranslationEntity> exampleEntities)
        {
            this.ExampleEntities = exampleEntities;
        }

        /// <summary>
        /// Gets the retrieved Example Entities as a paged result.
        /// </summary>
        [DataMember]
        public PagedResult<AVACProductTranslationEntity> ExampleEntities { get; private set; }
    }

}
