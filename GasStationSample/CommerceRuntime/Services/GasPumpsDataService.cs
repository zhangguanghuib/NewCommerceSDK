namespace GasStationSample.CommerceRuntime
{
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GasPumpsDataService : IRequestHandlerAsync
    {
        private const string DEFAULT_GASOLINE_ITEM_ID = "gasoline";
        private static decimal COST_PER_UNIT = 3.599m;
        private static Dictionary<string, IEnumerable<GasPump>> GasPumpsByStore;
        private static List<GasStationDetails> GasStations = new List<GasStationDetails>();

        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[] { 
                typeof(GetGasPumpsDataRequest),
                typeof(UpdatePumpStateDataRequest),
                typeof(StopAllPumpsDataRequest),
                typeof(StartAllPumpsDataRequest),
                typeof(GetGasStationDetailsDataRequest)
                };
            }
 

        }

        public async Task<Response> Execute(Request request)
        {
            if(request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            Type reqType = request.GetType();
            if(reqType == typeof(GetGasPumpsDataRequest))
            {

            }
            else if(reqType == typeof(UpdatePumpStateDataRequest))
            {

            }
            else if(reqType == typeof(StopAllPumpsDataRequest))
            {

            }
            else if(reqType == typeof(StartAllPumpsDataRequest))
            {

            }
            else if (reqType == typeof(GetGasStationDetailsDataRequest))
            {

            }
            else
            {
                string message = string.Format(CultureInfo.InvariantCulture, "Request '{0}' is not supported", request);
                throw new NotSupportedException(message);
            }
        }

        private Response GetGasStationDetails(GetGasStationDetailsDataRequest request)
        {
            var station = GasStations.First(s=>s.StoreNumber == request.StoreNumber);
            return new GetGasStationDetailsDataResponse(station);
        }

        private Response startAllPumps(StartAllPumpsDataRequest request)
        {
            var pums = GasPumpsDataService.GasPumpsByStore[request.StoreNumber];
        }

        private async Task InitializeGasPumps(Request request)
        {
            if(GasPumpsDataService.GasPumpsByStore != null)
            {
                return;
            }

            var gasScanInfo = new ScanInfo();
            gasScanInfo.ScannedText = GetGasolineItemId(request);
            var getScanResultRequest = new GetScanResultRequest(gasScanInfo);
            var response = await request.RequestContext.Runtime.ExecuteAsync<GetScanResultResponse>(getScanResultRequest, request.RequestContext);

            if(response.Result.MaskType == BarcodeMaskType.Item)
            {
                GasPumpsDataService.COST_PER_UNIT = response.Result.Product.BasePrice;
            }



        }

        private string GetGasolineItemId(Request request)
        {

            return DEFAULT_GASOLINE_ITEM_ID;
        }


    }
}
