using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
using Microsoft.Dynamics.Commerce.Runtime.Messages;
using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.GasStationSample.CommerceRuntime
{
    public partial class NonBindableOperationController : IController
    {
        [HttpPost]
        [Authorization(CommerceRoles.Anonymous, CommerceRoles.Employee, CommerceRoles.Application, CommerceRoles.Customer)]
        public async Task<SalesOrder> GetSalesOrderDetailsByTransactionId(IEndpointContext context, string transactionId)
        {
            var getCustomReceiptsRequest = new GetSalesOrderDetailsByTransactionIdServiceRequest(transactionId, SearchLocation.All);
            var getSalesOrderDetailsServiceResponse = await  context.ExecuteAsync<GetSalesOrderDetailsServiceResponse>(getCustomReceiptsRequest).ConfigureAwait(false);
            return getSalesOrderDetailsServiceResponse.SalesOrder;
        }

    }
}
