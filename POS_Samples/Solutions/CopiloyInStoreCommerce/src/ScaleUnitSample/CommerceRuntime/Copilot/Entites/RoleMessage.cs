namespace CommerceRuntime.Copilot.Entites
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.ComponentModel.DataAnnotations;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using SystemAnnotations = System.ComponentModel.DataAnnotations;

    [DataContract]
    public class RoleMessage:CommerceEntity
    {
        private const string RoleColumn = "ROLE";
        private const string MessageColumn = "MESSAGE";
        private const string TerminalIdColumn = "TERMINALID";

 
        public RoleMessage(string entityName = "RoleMessage") : base("RoleMessage")
        {

        }

        [DataMember]
        [Column(RoleColumn)]
        public string Role
        {
            get { return (string)this[RoleColumn]; }
            set { this[RoleColumn] = value; }
        }

        /// <summary>
        /// Gets or sets a property containing a string value.
        /// </summary>
        [DataMember]
        [Column(MessageColumn)]
        public string Message
        {
            get { return (string)this[MessageColumn]; }
            set { this[MessageColumn] = value; }
        }

        [DataMember]
        [Column(TerminalIdColumn)]
        public string TerminalId
        {
            get { return (string)this[TerminalIdColumn]; }
            set { this[TerminalIdColumn] = value; }
        }
    }
}
