namespace Contoso.GasStationSample.CommerceRuntime.TransactionService
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;

    [DataContract]
    public class DlvModeBookSlotSearchCriteria
    {
        private DlvModeBookSlotSearchCriteria()
        {
        }

        [DataMember]
        public string DlvModeCode { get; private set; }

        [DataMember]
        public PagingInfo Paging { get; private set; }

        [DataMember]
        public SortingInfo Sorting { get; private set; }

        public DlvModeBookSlotSearchCriteria(string dlvModeCode, PagingInfo paging, SortingInfo sorting)
        {
            this.DlvModeCode = dlvModeCode;
            this.Paging = paging;
            this.Sorting = sorting;
        }
    }
}
