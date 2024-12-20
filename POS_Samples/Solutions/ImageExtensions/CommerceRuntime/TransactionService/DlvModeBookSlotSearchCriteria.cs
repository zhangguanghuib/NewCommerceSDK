namespace Contoso.GasStationSample.CommerceRuntime.TransactionService
{
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;


    [DataContract]
    public class DlvModeBookSlotSearchCriteria
    {
        public DlvModeBookSlotSearchCriteria()
        {
            this.Initialize();
        }

        private void Initialize()
        {
            this.PagingInfo = PagingInfo.AllRecords;
           // this.Sorting = new SortingInfo();
        }

        [DataMember]
        public string DlvModeCode { get; set; }

        [DataMember]
        public PagingInfo PagingInfo { get; set; }

        //[DataMember]
        //public SortingInfo Sorting { get; set; }

        /// <summary>
        /// Gets or sets the start serialization format for the results.
        /// </summary>
        /// <remarks>0=Xml (default), 1=JSON.</remarks>
        [DataMember]
        public int SerializationFormat { get; set; }

        /// <summary>
        /// Called when deserializing.
        /// </summary>
        /// <param name="context">The context.</param>
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }

        public DlvModeBookSlotSearchCriteria(string dlvModeCode, PagingInfo paging/*, SortingInfo sorting*/)
        {
            this.DlvModeCode = dlvModeCode;
            this.PagingInfo = paging;
            // this.Sorting = sorting;
        }

        public DlvModeBookSlotSearchCriteria(string dlvModeCode)
        {
            this.DlvModeCode = dlvModeCode;
            this.Initialize();
        }
    }
}
