

namespace CommerceRuntime.Copilot.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Data;
    using Microsoft.Dynamics.Commerce.Runtime.DataAccess.SqlServer;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Contoso.CommerceRuntime.Entities.DataModel;
    using Contoso.CommerceRuntime.Messages;
    using System.IO;
    using Newtonsoft.Json;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.RealtimeServices.Messages;
    using CommerceRuntime.Copilot.Messages;
    using CommerceRuntime.Copilot.Entites;
    using System.Net.Http;
    using System.Text.Json;

    public class CopilotProvider : IRequestHandlerAsync
    {
        private static Dictionary<string, List<RoleMessage>> ChatMessageStoreByTerminal;
        private static List<RoleMessage> RoleMessages = new List<RoleMessage>();

        public IEnumerable<Type> SupportedRequestTypes
        {
            get
            {
                return new[]
                {
                    typeof(GetAIAnswerRequest),
                };
            }
        }

        /// <summary>
        /// Entry point to StoreHoursDataService service.
        /// </summary>
        /// <param name="request">The request to execute.</param>
        /// <returns>Result of executing request, or null object for void operations.</returns>
        public async Task<Response> Execute(Request request)
        {
            ThrowIf.Null(request, nameof(request));

            this.InitializeGasPumps(request);

            switch (request)
            {
                case GetAIAnswerRequest getAIAnswerRequest:
                    return await this.GetAIAnswer(getAIAnswerRequest).ConfigureAwait(false);
                default:
                    throw new NotSupportedException($"Request '{request.GetType()}' is not supported.");
            }
        }

        private async Task<Response> GetAIAnswer(GetAIAnswerRequest getAIAnswerRequest)
        {
            string userInput = getAIAnswerRequest.UserInput;
            string currentTerminalId = getAIAnswerRequest.RequestContext.GetTerminalId();

            StringBuilder sb = new StringBuilder();

            List<RoleMessage> roleMessages = CopilotProvider.ChatMessageStoreByTerminal[currentTerminalId];

            if (roleMessages == null)
            {
               roleMessages = new List<RoleMessage>();
               CopilotProvider.ChatMessageStoreByTerminal[currentTerminalId] = roleMessages;
            }

            foreach (RoleMessage roleMessage in roleMessages)
            {
                sb.Append($"\n{roleMessage.Role}: {roleMessage.Message}");
            }
            sb.Append("\n");

            string history = sb.ToString();
            using (var client = new HttpClient())
            {
                ChatInputRequest chatInputRequest = new ChatInputRequest { UserInput = userInput, SecondString = "" };
                using (var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(chatInputRequest), Encoding.UTF8, "application/json"))
                {
                    var response = await client.PostAsync("https://****.microsoft.com:80*2/api/Chat", content).ConfigureAwait(false);

                    string answer = "";
                    if (response.IsSuccessStatusCode)
                    {
                        answer = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        answer = $"Error: {response.StatusCode}";
                    }

                    // Process the user message and get an answer
                    roleMessages.Add(new RoleMessage() { Role = "User", Message = userInput });
                    roleMessages.Add(new RoleMessage() { Role = "AI", Message = answer });

                    return new GetAIAnswerResponse(roleMessages.AsPagedResult<RoleMessage>());
                }
            }
        }
        private void InitializeGasPumps(Request request)
        {
            string currentTerminalId = request.RequestContext.GetTerminalId();
            if (CopilotProvider.ChatMessageStoreByTerminal != null && CopilotProvider.ChatMessageStoreByTerminal[currentTerminalId] != null)
            {
                return;
            }

            if (CopilotProvider.ChatMessageStoreByTerminal == null)
            {
                CopilotProvider.ChatMessageStoreByTerminal = new Dictionary<string, List<RoleMessage>>();
            }

            List<RoleMessage> roleMessages;
            if (!CopilotProvider.ChatMessageStoreByTerminal.TryGetValue(currentTerminalId, out roleMessages))
            {
                List<RoleMessage> roleMessageList = new List<RoleMessage>();
                CopilotProvider.ChatMessageStoreByTerminal[currentTerminalId] = roleMessages;
            }
        }
    }
}
