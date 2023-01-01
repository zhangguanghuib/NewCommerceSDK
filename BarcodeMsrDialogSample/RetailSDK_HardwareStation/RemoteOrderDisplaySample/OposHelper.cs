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
        using System.Threading;
        using Interop.OposConstants;

        /// <summary>
        /// Implements OPOS related helper functions.
        /// </summary>
        public static class OposHelper
        {
            /// <summary>
            /// Border types supported by the device.
            /// </summary>
            public enum BorderType
            {
                /// <summary>
                /// No border line required.
                /// </summary>
                None = 0,

                /// <summary>
                /// Single border in the remote order display.
                /// </summary>
                SingleBorder = (int)OPOSRemoteOrderDisplayConstants.ROD_BDR_SINGLE,

                /// <summary>
                /// Double border in the remote order display.
                /// </summary>
                DoubleBorder = (int)OPOSRemoteOrderDisplayConstants.ROD_BDR_DOUBLE,

                /// <summary>
                /// Solid border in the remote order display.
                /// </summary>
                SolidBorder = (int)OPOSRemoteOrderDisplayConstants.ROD_BDR_SOLID
            }

            /// <summary>
            /// Gets the claim time out.
            /// </summary>
            public static int ClaimTimeOut
            {
                get
                {
                    return 10000;
                }
            }

            /// <summary>
            /// Gets the units field for the Clear Video Operation.
            /// </summary>
            public static int VideoUnitsForClearVideoOperation
            {
                get
                {
                    // Sample value for test purposes.
                    return 0;
                }
            }

            /// <summary>
            /// Gets the video attributes for the device.
            /// </summary>
            public static int DeviceVideoAttribute
            {
                get
                {
                    // Sample value for test purposes.
                    return 0;
                }
            }

            /// <summary>
            /// Gets the distance (in rows) between two boxes holding two messages.
            /// </summary>
            public static int InterMessageDistance
            {
                get
                {
                    // Sample value for test purposes.
                    return 3;
                }
            }

            /// <summary>
            /// Gets the number of columns supported in a box.
            /// </summary>
            public static int ColumnsInBox
            {
                get
                {
                    // Sample value for test purposes.
                    return 100;
                }
            }

            /// <summary>
            /// Gets the units field for the Draw Box and Display Data Operations.
            /// </summary>
            public static int VideoUnitsForDisplayDataAndDrawBoxOperations
            {
                get
                {
                    // Sample value for test purposes.
                    return 0;
                }
            }
            
            /// <summary>
            /// Check the OPOS result code from last operation.
            /// </summary>
            /// <param name="source">Source of result code.</param>
            /// <param name="resultCode">Result code returned from last operation.</param>
            /// <exception cref="IOException">Device IO error.</exception>
            public static void CheckResultCode(object source, int resultCode)
            {
                if (source == null)
                {
                    throw new ArgumentNullException("source");
                }

                OPOS_Constants result = (OPOS_Constants)resultCode;

                if (result != OPOS_Constants.OPOS_SUCCESS)
                {
                    string message = string.Format(CultureInfo.InvariantCulture, "{0} device failed with error '{1}'.", source.GetType().Name, result);
                    throw new OposPeripheralException(result, message);
                }
            }

            /// <summary>
            /// Gets ID of the current thread.
            /// </summary>
            public static string CurrentThreadId
            {
                get
                {
                    return Thread.CurrentThread.GetHashCode().ToString();
                }
            }
        }
    }
}