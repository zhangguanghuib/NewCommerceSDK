namespace Contoso
{
    namespace Commerce.Runtime.DataModel
    {
        using System.Runtime.Serialization;
        using Microsoft.Dynamics.Commerce.Runtime.ComponentModel.DataAnnotations;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using SystemDataAnnotations = System.ComponentModel.DataAnnotations;

        /// <summary>
        /// Defines a simple class that holds information about opening and closing times for a particular day.
        /// </summary>
        public class StoreDayHours : CommerceEntity
        {
            private const string DayColumn = "DAY";
            private const string OpenTimeColumn = "OPENTIME";
            private const string CloseTimeColumn = "CLOSINGTIME";
            private const string IdColumn = "RECID";
            private const string StoreNumberColumn = "STORENUMBER";

            /// <summary>
            /// Initializes a new instance of the <see cref="StoreDayHours"/> class.
            /// </summary>
            public StoreDayHours()
                : base("StoreDayHours")
            {
            }

            /// <summary>
            /// Gets or sets the day of the week.
            /// </summary>
            [DataMember]
            [Column(DayColumn)]
            public int DayOfWeek
            {
                get { return (int)this[DayColumn]; }
                set { this[DayColumn] = value; }
            }

            /// <summary>
            /// Gets or sets the open time.
            /// </summary>
            [DataMember]
            [Column(OpenTimeColumn)]
            public int OpenTime
            {
                get { return (int)this[OpenTimeColumn]; }
                set { this[OpenTimeColumn] = value; }
            }

            /// <summary>
            /// Gets or sets the closing time.
            /// </summary>
            [DataMember]
            [Column(CloseTimeColumn)]
            public int CloseTime
            {
                get { return (int)this[CloseTimeColumn]; }
                set { this[CloseTimeColumn] = value; }
            }

            /// <summary>
            /// Gets or sets the id.
            /// </summary>
            [SystemDataAnnotations.Key]
            [Key]
            [DataMember]
            [Column(IdColumn)]
            public long Id
            {
                get { return (long)this[IdColumn]; }
                set { this[IdColumn] = value; }
            }

            /// <summary>
            /// Gets or sets the channel id.
            /// </summary>
            [DataMember]
            [Column(StoreNumberColumn)]
            public string ChannelId
            {
                get { return (string)this[StoreNumberColumn]; }
                set { this[StoreNumberColumn] = value; }
            }
        }
    }
}
