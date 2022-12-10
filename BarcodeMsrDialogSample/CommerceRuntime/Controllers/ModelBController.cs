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

    [RoutePrefix("ModelB")]
    [BindEntity(typeof(ModelB))]
    public class ModelBController : IController
    {
        [HttpPost]
        [Authorization(CommerceRoles.Anonymous, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee)]
        public async Task<PagedResult<ModelB>> GetModelById(IEndpointContext context, string id)
        {
            IList<ModelB> list = new List<ModelB>() { new ModelB()};
            var result = new PagedResult<ModelB>(new ReadOnlyCollection<ModelB>(list));
            return await Task.FromResult(result); 
        }
    }
   
}
