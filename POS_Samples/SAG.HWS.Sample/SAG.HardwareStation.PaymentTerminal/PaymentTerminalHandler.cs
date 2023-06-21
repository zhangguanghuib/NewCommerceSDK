using Microsoft.Dynamics.Commerce.Runtime;
using Microsoft.Dynamics.Commerce.Runtime.Messages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SAG.HardwareStation.PaymentTerminal
{
    
    public class PaymentTerminalHandler: IRequestHandlerAsync
    {
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[]
                {
                    typeof(DBS.DataContract.DBSRequest)
                };
            }
        }
        
        public async Task<Response> Execute(Request request)
        {
            ThrowIf.Null(request, "request");

            Type requestType = request.GetType();
            if (requestType == typeof(DBS.DataContract.DBSRequest))
            {
                return await this.ProcessPaymentTerminalDBS((DBS.DataContract.DBSRequest)request).ConfigureAwait(false);
            }
            else
            {
                throw new NotSupportedException(string.Format("Request '{0}' is not supported.", requestType));
            }
        }

        private async Task<PaymentTerminalResponse> ProcessPaymentTerminalDBS(DBS.DataContract.DBSRequest request)
        {
            PaymentTerminalResponse paymTerminalResponse;

            //DBS.DataContract.DBSResponse _dbsResponse = DBS.ECREngine.Process(request);
            //paymTerminalResponse = new PaymentTerminalResponse(_dbsResponse);

            paymTerminalResponse = new PaymentTerminalResponse();
            DBS.DataContract.DBSResponse dBSResponse = new DBS.DataContract.DBSResponse();
            dBSResponse.ApprovalCode = "12345";
            dBSResponse.CardNo = "1234567890";
            dBSResponse.ECRRefNo= "ECR1234567";
            dBSResponse.TraceNo="Trace123";

            paymTerminalResponse.PaymentTerminal = dBSResponse;
            return await Task.FromResult(paymTerminalResponse).ConfigureAwait(false);
        }
        
    }
    
}
