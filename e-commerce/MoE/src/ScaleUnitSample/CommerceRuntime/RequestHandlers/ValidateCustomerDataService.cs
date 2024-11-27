namespace Moe
{
    namespace Commerce.Runtime
    {
        using System;
        using System.Linq;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.Runtime;
        using Microsoft.Dynamics.Commerce.Runtime.Data;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Messages;
        using Moe.Commerce.Runtime.DataModel;
        using Microsoft.Dynamics.Commerce.Runtime.DataAccess.SqlServer;

        /// <summary>
        /// Sample service to demonstrate returning an array of a new entity.
        /// </summary>
        public class ValidateCustomerDataService : SingleAsyncRequestHandler<ValidateCustomerDataRequest>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="request"></param>
            /// <returns></returns>
            protected override async Task<Response> Process(ValidateCustomerDataRequest request)
            {               
                bool validCustomer = false;

                using (SqlServerDatabaseContext databaseContext = new SqlServerDatabaseContext(request.RequestContext))
                {
                    ParameterSet parameters = new ParameterSet();
                    parameters["@nvc_EmailAddress"] = request.EmailId;

                    var result = await databaseContext.ExecuteNonPagedStoredProcedureAsync<ExtensionsEntity>("[ext].[VALIDATECUSTOMEREMAIL]", parameters, request.QueryResultSettings).ConfigureAwait(false);
                    validCustomer = Convert.ToBoolean(Convert.ToInt16(result.First().GetProperty("VALIDCUSTOMER")));
                }
                ValidCustomer isValidCust = new ValidCustomer();
                isValidCust.Id = 1;
                isValidCust.isValid = validCustomer;
                
                return new ValidateCustomerDataResponse(isValidCust);
            }
        }
    }
}
