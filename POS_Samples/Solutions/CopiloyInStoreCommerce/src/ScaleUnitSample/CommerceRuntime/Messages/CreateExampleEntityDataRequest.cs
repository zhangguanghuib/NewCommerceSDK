namespace Contoso.CommerceRuntime.Messages
{
    using System.Runtime.Serialization;
    using Contoso.CommerceRuntime.Entities.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    /// <summary>
    /// A simple request class to create an Example Entity in the database.
    /// </summary>
    [DataContract]
    public sealed class CreateExampleEntityDataRequest : Request
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateExampleEntityDataRequest"/> class.
        /// </summary>
        /// <param name="entityData">An example entity with its fields populated with the values to be stored.</param>
        public CreateExampleEntityDataRequest(ExampleEntity entityData)
        {
            this.EntityData = entityData;
        }

        /// <summary>
        /// Gets an Example Entity instance with its fields set with the values to be stored.
        /// </summary>
        [DataMember]
        public ExampleEntity EntityData { get; private set; }
    }
}