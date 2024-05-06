namespace Contoso.GasStationSample.CommerceRuntime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
    using Microsoft.Dynamics.Commerce.Runtime.Services.Messages;

    public class HtmlController : IController
    {
        [HttpGet]
        [Authorization(CommerceRoles.Anonymous, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee)]
        public async Task<string> GetHtmlContent(IEndpointContext context, string url, QueryResultSettings queryResultSettings)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(url).ConfigureAwait(false); ;

                    if (!response.IsSuccessStatusCode)
                    {
                        return "Failed to fetch website content";
                    }

                    var htmlContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return htmlContent;
                }
                catch (Exception ex)
                {
                    throw new DataValidationException(DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_InvalidRequest, $"Internal server error: {ex.Message}");
                }
                finally
                {
                    client.Dispose();
                }
            }
        }  
    }
}