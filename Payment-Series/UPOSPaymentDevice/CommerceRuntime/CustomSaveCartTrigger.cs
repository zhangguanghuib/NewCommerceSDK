using Microsoft.Dynamics.Commerce.Runtime;
using Microsoft.Dynamics.Commerce.Runtime.Messages;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace Contoso.GasStationSample.CommerceRuntime
{
    public class CustomSaveCartTrigger : IRequestTriggerAsync
    {
        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[] { typeof(SaveCartRequest) };
            }
        }

        public  Task OnExecuted(Request request, Response response)
        {
            return Task.CompletedTask;
        }

        public  Task OnExecuting(Request request)
        {
            ThrowIf.Null(request, "request");
            Type requestedType = request.GetType();
            if (requestedType == typeof(SaveCartRequest))
            {
                using (Bitmap bitmap = new Bitmap(200, 200))
                {
                    int height = bitmap.Height;
                }
            }

            return Task.CompletedTask;

        }
    }
}
