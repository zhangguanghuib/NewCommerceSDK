namespace Contoso
{
    namespace Commerce.Runtime.StoreHoursSample.Messages
    {
        using System.Collections.ObjectModel;
        using System.Runtime.Serialization;
        using Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;

        /// <summary>
        /// Defines a simple response class that holds a list of StoreHour instances.
        /// </summary>
        [DataContract]
        public sealed class UpdateStoreDayHoursDataResponse : Response
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="UpdateStoreDayHoursDataResponse"/> class.
            /// </summary>
            /// <param name="storeDayHours">The store day hours.</param>
            public UpdateStoreDayHoursDataResponse(StoreDayHours storeDayHours)
            {
                this.StoreDayHours = storeDayHours;
            }

            /// <summary>
            /// Gets the store day hours.
            /// </summary>
            [DataMember]
            public StoreDayHours StoreDayHours { get; private set; }
        }
    }
}