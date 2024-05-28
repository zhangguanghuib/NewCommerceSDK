namespace CommerceRuntime.Copilot.Messages
{
    using System.Runtime.Serialization;
    using CommerceRuntime.Copilot.Entites;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    [DataContract]
    public class GetAIAnswerRequest: Request
    {
        [DataMember]
        public string UserInput { get; private set; }


        [DataMember]
        public string TerminalId { get; private set; }

        public GetAIAnswerRequest(string UserInput, string terminalId)
        {
            this.UserInput = UserInput; 
            this.TerminalId = terminalId;
        }
    }
}
