

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
    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.Connectors.OpenAI;
    using Kernel = Microsoft.SemanticKernel.Kernel;
    using CommerceRuntime.Copilot.Messages;
    using CommerceRuntime.Copilot.Entites;

    public class CopilotProvider : IRequestHandlerAsync
    {
        private Kernel kernel;
        private KernelFunction kernelFunction;

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

        public CopilotProvider()
        {
            if (this.kernel == null || this.kernelFunction == null)
            {
                (this.kernel, this.kernelFunction) = chatFunction();
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

            if (this.kernel == null || this.kernelFunction == null)
            {
                (this.kernel, this.kernelFunction) = this.chatFunction();
            }

            StringBuilder sb = new StringBuilder();

            List<RoleMessage> roleMessages = CopilotProvider.ChatMessageStoreByTerminal[currentTerminalId];
            foreach (RoleMessage roleMessage in roleMessages)
            {
                sb.Append($"\n{roleMessage.Role}: {roleMessage.Message}");
            }
            sb.Append("\n");

            string history = sb.ToString();
            var arguments = new KernelArguments()
            {
                ["history"] = history
            };

            arguments["userInput"] = userInput;

            // Process the user message and get an answer
            var answer = await kernelFunction.InvokeAsync(kernel, arguments).ConfigureAwait(false);

            roleMessages.Add(new RoleMessage() { Role = "User", Message = userInput });
            roleMessages.Add(new RoleMessage() { Role = "AI", Message = answer.ToString() });

            return new GetAIAnswerResponse(roleMessages.AsPagedResult<RoleMessage>());
        }

        private (Kernel, KernelFunction) chatFunction()
        {
            var builder = Kernel.CreateBuilder();

            var (useAzureOpenAI, model, azureEndpoint, apiKey, orgId) =
                (true, "openai10", "https://openai10.openai.azure.com/", "f433557a3c284cb98a34b8aa0a164f30", "");

            if (useAzureOpenAI)
                builder.AddAzureOpenAIChatCompletion(model, azureEndpoint, apiKey);
            else
                builder.AddOpenAIChatCompletion(model, apiKey, orgId);

            var kernel = builder.Build();

            const string skPrompt = @"
                ChatBot can have a conversation with you about any topic.
                It can give explicit instructions or say 'I don't know' if it does not have an answer.

                {{$history}}
                User: {{$userInput}}
                ChatBot:";

            var executionSettings = new OpenAIPromptExecutionSettings
            {
                MaxTokens = 2000,
                Temperature = 0.7,
                TopP = 0.5
            };

            var chatFunction = kernel.CreateFunctionFromPrompt(skPrompt, executionSettings);
            return (kernel, chatFunction);
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
