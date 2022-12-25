using System;
using System.Collections.Generic;
using System.Text;

namespace GHZ.BarcodeMsrDialogSample.CommerceRuntime.Controllers
{
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
    using GHZ.BarcodeMsrDialogSample.CommerceRuntime.Models;
    using System.Threading.Tasks;
    using System.Collections.ObjectModel;

    [RoutePrefix("ModelA1")]
    [BindEntity(typeof(ModelA))]
    public class ModelAController : IController
    {
        [HttpPost]
        [Authorization(CommerceRoles.Anonymous, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee)]
        public async Task<PagedResult<ModelA>> GetModelById(IEndpointContext context, string id)
        {
            IList<ModelA> list = new List<ModelA>() { new ModelA()};
            var result = new PagedResult<ModelA>(new ReadOnlyCollection<ModelA>(list));
            return await Task.FromResult(result).ConfigureAwait(false); 
        }
    }
   
}
