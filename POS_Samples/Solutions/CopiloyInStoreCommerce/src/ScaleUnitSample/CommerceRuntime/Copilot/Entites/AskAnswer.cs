
namespace CommerceRuntime.Copilot.Entites
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.ComponentModel.DataAnnotations;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using SystemAnnotations = System.ComponentModel.DataAnnotations;

    [DataContract]
    public class AskAnswer : CommerceEntity
    {
        private const string UserInputColumn = "USERINPUT";
        private const string AIAnswerColumn = "AIANSWER";

        public AskAnswer(string entityName) : base("AskAnswer")
        {
        }

        [DataMember]
        [Column(UserInputColumn)]
        public string UserInput
        {
            get { return (string)this[UserInputColumn]; }
            set { this[UserInputColumn] = value; }
        }

        /// <summary>
        /// Gets or sets a property containing a string value.
        /// </summary>
        [DataMember]
        [Column(AIAnswerColumn)]
        public string AIAnswer
        {
            get { return (string)this[AIAnswerColumn]; }
            set { this[AIAnswerColumn] = value; }
        }
    }
}
