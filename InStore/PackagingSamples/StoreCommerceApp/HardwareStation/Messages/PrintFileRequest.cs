namespace Contoso
{
    namespace Commerce.HardwareStation
    {
        using System.Runtime.Serialization;

        /// <summary>
        /// Print PDF request class.
        /// </summary>
        [DataContract]
        public class PrintFileRequest
        {
            /// <summary>
            /// Gets or sets the encoded binary file.
            /// </summary>
            [DataMember]
            public string[] Lines { get; private set; }

            /// <summary>
            /// Gets or sets the device name.
            /// </summary>
            [DataMember]
            public string FileName { get; set; }
        }
    }
}