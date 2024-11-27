namespace Moe.RetailServer
{
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
    using Moe.Commerce.Runtime.Messages;
    using Moe.Commerce.Runtime.DataModel;

    /// <summary>
    /// The controller to retrieve a new entity.
    /// </summary>
    //[RoutePrefix("ValidCustomer")]
    //[BindEntity(typeof(ValidCustomer))]
    public class ValidateCustomerController : IController
    {
        /// <summary>
        /// Gets the default customer of online channel
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true/false based on configuration</returns>
        [HttpPost]
        [Authorization(CommerceRoles.Application, CommerceRoles.Anonymous, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee)]
        public virtual async Task<ValidCustomer> ValidateCustomerDetails(IEndpointContext context, string emailId)
        {            
            ValidateCustomerDataRequest request = new ValidateCustomerDataRequest(emailId);

            var response = await context.ExecuteAsync<ValidateCustomerDataResponse>(request).ConfigureAwait(false);

            return response.ValidCustomer;
        }
    }
}
