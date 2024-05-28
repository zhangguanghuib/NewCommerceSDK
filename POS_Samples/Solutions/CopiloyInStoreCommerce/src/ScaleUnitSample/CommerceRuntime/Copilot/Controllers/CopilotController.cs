

namespace CommerceRuntime.Copilot.Controllers
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using Azure.Core;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages;
    using CommerceRuntime.Copilot.Entites;
    using CommerceRuntime.Copilot.Messages;

    public class CopilotController: IController
    {
        [HttpPost]
        [Authorization(CommerceRoles.Anonymous, CommerceRoles.Application, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee, CommerceRoles.Storefront)]
        public async Task<PagedResult<RoleMessage>> GetAIAnswers(IEndpointContext context,  string userInput, string terminalId, QueryResultSettings queryResultSettings)
        {
            queryResultSettings = QueryResultSettings.SingleRecord;
            queryResultSettings.Paging = new PagingInfo(50);

            GetAIAnswerRequest getAIAnswerRequest = new GetAIAnswerRequest(userInput, terminalId);
            GetAIAnswerResponse getAIAnswerResponse = await context.ExecuteAsync<GetAIAnswerResponse>(getAIAnswerRequest).ConfigureAwait(false);

            return getAIAnswerResponse.RoleMessages;
        }
    }
}
