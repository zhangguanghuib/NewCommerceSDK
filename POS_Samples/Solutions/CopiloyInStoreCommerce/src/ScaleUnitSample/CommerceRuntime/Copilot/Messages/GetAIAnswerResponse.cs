namespace CommerceRuntime.Copilot.Messages
{
    using System.Runtime.Serialization;
    using Contoso.CommerceRuntime.Entities.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using CommerceRuntime.Copilot.Entites;

    public class GetAIAnswerResponse : Response
    {
        public GetAIAnswerResponse(PagedResult<RoleMessage> roleMessages)
        {
            this.RoleMessages = roleMessages;
        }

        /// <summary>
        /// Gets the retrieved Example Entities as a paged result.
        /// </summary>
        [DataMember]
        public PagedResult<RoleMessage> RoleMessages { get; private set; }
    }
}
