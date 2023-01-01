/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

namespace Contoso
{
    namespace Commerce.HardwareStation.RemoteOrderDisplaySample
    {
        using System;
        using System.Globalization;
        using System.IO;
        using System.Runtime.Serialization;
        using Interop.OposConstants;

        /// <summary>
        /// Represents the exception class originating from the peripheral devices.
        /// </summary>
        [Serializable]
        public class OposPeripheralException : IOException
        {
            /// <summary>
            /// The name of the error code property for serialization purposes.
            /// </summary>
            private const string ErrorResourcePropertyName = "ErrorResourceId";

            /// <summary>
            /// Initializes a new instance of the <see cref="OposPeripheralException" /> class.
            /// </summary>
            /// <param name="oposErrorCode">The error code.</param>
            /// <param name="message">The message.</param>
            /// <param name="args">The optional format arguments.</param>
            public OposPeripheralException(OPOS_Constants oposErrorCode, string message, params object[] args)
                : base(string.Format(CultureInfo.InvariantCulture, message, args))
            {
                this.ErrorCode = oposErrorCode;
            }

            /// <summary>
            /// Gets or sets the error resource code associated with this exception.
            /// </summary>
            /// <value>
            /// The error code.
            /// </value>
            public OPOS_Constants ErrorCode { get; set; }

            /// <summary>
            /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with information about the exception.
            /// </summary>
            /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
            /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                if (info == null)
                {
                    throw new ArgumentNullException("info");
                }

                info.AddValue(OposPeripheralException.ErrorResourcePropertyName, this.ErrorCode);
                base.GetObjectData(info, context);
            }
        }
    }
}