namespace Microsoft.Dynamics
{
    namespace Commerce.HardwareStation.Peripherals
    {
        using System;
        using System.Collections.Generic;
        using System.Text.RegularExpressions;
        using Interop.OposConstants;
        using Interop.OposPOSPrinter;
        using Microsoft.Dynamics.Commerce.HardwareStation;
        using Microsoft.Dynamics.Commerce.Runtime.Handlers;
        using Microsoft.Dynamics.Commerce.Runtime.Messages;
        using Microsoft.Dynamics.Retail.Diagnostics;
        using Microsoft.Dynamics.Retail.Diagnostics.Extensions;

        /// <summary>
        /// Class implements OPOS based printer driver for hardware station.
        /// </summary>
#pragma warning disable CS0618 // JUSTIFICATION: Some hardware don't work in asynchronous calls. Need to fix it before handlers will be refactored to async..
        public class OposPrinter : INamedRequestHandler, IDisposable
#pragma warning restore CS0618
        {
            private const string OPOSMarkersForSplitRegEx = @"(&#x1B;\|1C|&#x1B;\|2C|&#x1B;\|3C|&#x1B;\|4C|\r\n|\|1C)";
            private const string PrinterInstanceNameTemplate = "OposPrinter_{0}";
            private string printerInstanceName;
            private int characterSet;
            private bool binaryConversion;
            private bool usePrintBitmapMethod;

            private enum Events
            {
                InvalidImage,
                PrintBitmapConfiguration,
                PrintBitmapException,
                PrintBitmapNotSupported,
                CapRecPapercutException,
            }

            /// <summary>
            /// Gets the unique name for this request handler.
            /// </summary>
            public string HandlerName
            {
                get { return PeripheralType.Opos; }
            }

            /// <summary>
            /// Gets the collection of supported request types by this handler.
            /// </summary>
            public IEnumerable<Type> SupportedRequestTypes
            {
                get
                {
                    return new[]
                    {
                        typeof(OpenPrinterDeviceRequest),
                        typeof(PrintPrinterDeviceRequest),
                        typeof(ClosePrinterDeviceRequest),
                        typeof(HealthCheckPrinterDeviceRequest),
                    };
                }
            }

            /// <summary>
            /// Gets the configuration flag name used for determine if OPOS PrintBitmap() should be called.
            /// </summary>
            protected static string UsePrintBitmapConfigurationFlag => "HardwareStation.UsePrintBitmapMethod";

            /// <summary>
            /// Gets or sets a value indicating whether binary conversion is enabled for the printer.
            /// </summary>
            protected bool BinaryConversionEnabled { get; set; }

            /// <summary>
            /// Gets the configuration flag name used for determine if OPOS CapRecPapercut property should be ignored.
            /// </summary>
            private static string SkipOposCheckCapRecPapercut => "HardwareStation.Printer.Opos.SkipCheckCapRecPapercut";

            /// <summary>
            /// Represents the entry point for the printer device request handler.
            /// </summary>
            /// <param name="request">The incoming request message.</param>
            /// <returns>The outgoing response message.</returns>
            public Response Execute(Request request)
            {
                ThrowIf.Null(request, nameof(request));

                switch (request)
                {
                    case OpenPrinterDeviceRequest openPrinterDeviceRequest:
                        var openRequest = openPrinterDeviceRequest;
                        this.Open(openRequest.DeviceName, openRequest.CharacterSet, openRequest.BinaryConversion);
                        break;
                    case PrintPrinterDeviceRequest printPrinterDeviceRequest:
                        var printRequest = printPrinterDeviceRequest;
                        this.Print(printRequest.Header, printRequest.Lines, printRequest.Footer);
                        break;
                    case ClosePrinterDeviceRequest closePrinterDeviceRequest:
                        this.Close();
                        break;
                    case HealthCheckPrinterDeviceRequest healthCheckPrinterDeviceRequest:
                        this.HealthCheck(healthCheckPrinterDeviceRequest);
                        break;
                    default:
                        throw new NotSupportedException(string.Format("Request '{0}' is not supported.", request.GetType()));
                }

                return NullResponse.Instance;
            }

            /// <summary>
            /// Makes sure the printer OPOS object is closed.
            /// </summary>
            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Read hardware station configuration to check if PrintBitmap flag is set.
            /// </summary>
            /// <returns><c>true</c>>if flag is for PrintBitmap is set, false otherwise.</returns>
            protected static bool GetPrintBitmapConfiguration()
            {
                if (CompositionManager.HardwareStationRuntime.HardwareStationSection.Settings.TryGetValue(UsePrintBitmapConfigurationFlag, out string usePrintBitmapMethod))
                {
                    if (!string.IsNullOrWhiteSpace(usePrintBitmapMethod) && bool.TryParse(usePrintBitmapMethod, out bool result))
                    {
                        RetailLogger.Log.LogInformation(Events.PrintBitmapConfiguration, "Found configuration to use PrintBitmap OPOS method, value: '{value}'.", result.AsSystemMetadata());
                        return result;
                    }
                    else
                    {
                        RetailLogger.Log.LogInformation(Events.PrintBitmapConfiguration, "Found configuration to use PrintBitmap OPOS method, but unable to parse value, defaulting to '{value}'.", false.AsSystemMetadata());
                    }
                }

                return false;
            }

            /// <summary>
            /// Print the text to the Printer.
            /// </summary>
            /// <param name="oposPrinter">The OPOS printer.</param>
            /// <param name="textToPrint">The text to print on the receipt.</param>
            protected void PrintText(IOPOSPOSPrinter oposPrinter, string textToPrint)
            {
                int resultCode;
                int resultCodeExtended;
                string oposPrinterMethod;
                string[] textToPrintSplit = Regex.Split(textToPrint, TextParser.BarcodeRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase);

                for (int i = 0; i < textToPrintSplit.Length; i++)
                {
                    // Grab text to print from the split array and skip if null or empty.
                    string toPrint = textToPrintSplit[i];
                    if (string.IsNullOrEmpty(toPrint))
                    {
                        continue;
                    }

                    // Check if printer run out of paper after previous print call.
                    this.CheckPaperStatus(oposPrinter);

                    // Text is barcode if it doesn't contain any special escape characters found in normal receipt strings for OPS (e.g., '&#x1B;|1C' or '1C').
                    bool isBarcode = !Regex.IsMatch(toPrint, OPOSMarkersForSplitRegEx, RegexOptions.Compiled | RegexOptions.Multiline);

                    // If it is barcode, print it using special OPOS PrintBarCode method. Also print white spaces as normal text.
                    if (isBarcode && !string.IsNullOrWhiteSpace(textToPrint))
                    {
                        oposPrinterMethod = "PrintBarCode";
                        resultCode = oposPrinter.PrintBarCode(
                            (int)OPOSPOSPrinterConstants.PTR_S_RECEIPT,
                            toPrint,
                            (int)OPOSPOSPrinterConstants.PTR_BCS_Code128,
                            80,
                            80,
                            (int)OPOSPOSPrinterConstants.PTR_BC_CENTER,
                            (int)OPOSPOSPrinterConstants.PTR_BC_TEXT_BELOW);
                        resultCodeExtended = this.GetExtendedResultCodeFromPrinter(oposPrinter);
                    }
                    else
                    {
                        // Print normal text using OPOS PrintNormal method.

                        // Configure printer and convert text to binary if specified by settings.
                        if (this.BinaryConversionEnabled)
                        {
                            oposPrinter.BinaryConversion = 2; // OposBcDecimal
                            toPrint = OposHelper.ConvertToBCD(toPrint, this.characterSet);
                        }

                        oposPrinterMethod = "PrintNormal";
                        resultCode = oposPrinter.PrintNormal((int)OPOSPOSPrinterConstants.PTR_S_RECEIPT, toPrint);
                        resultCodeExtended = this.GetExtendedResultCodeFromPrinter(oposPrinter);
                        oposPrinter.BinaryConversion = 0; // OposBcNone
                    }

                    this.CheckResultCode(oposPrinterMethod, resultCode, resultCodeExtended);
                }
            }

            /// <summary>
            /// Dispose the printer objects if disposing is set to true.
            /// </summary>
            /// <param name="disposing">Disposing flag.</param>
            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.Close();
                }
            }

            /// <summary>
            /// Opens a peripheral.
            /// </summary>
            /// <param name="peripheralName">Name of the peripheral.</param>
            /// <param name="characterSet">The character set.</param>
            /// <param name="binaryConversion">If set to <c>true</c> [binary conversion].</param>
            private void Open(string peripheralName, int characterSet, bool binaryConversion)
            {
                this.printerInstanceName = string.Format(PrinterInstanceNameTemplate, peripheralName);

                // Open
                IOPOSPOSPrinter oposPrinter = OPOSDeviceManager<IOPOSPOSPrinter>.Instance.AcquireDeviceHandle<OPOSPOSPrinterClass>();

                if (!oposPrinter.DeviceEnabled)
                {
                    // Open the device only if the device is not claimed.
                    if (!oposPrinter.Claimed)
                    {
                        RetailLogger.Log.HardwareStationOposMethodCall(this.printerInstanceName, "Open");
                        oposPrinter.Open(peripheralName);
                        this.CheckResultCode("Open", oposPrinter.ResultCode, this.GetExtendedResultCodeFromPrinter(oposPrinter));

                        // Claim
                        RetailLogger.Log.HardwareStationOposMethodCall(this.printerInstanceName, "ClaimDevice");
                        oposPrinter.ClaimDevice(OposHelper.ClaimTimeOut);
                        this.CheckResultCode("ClaimDevice", oposPrinter.ResultCode, this.GetExtendedResultCodeFromPrinter(oposPrinter));
                    }
                    else
                    {
                        RetailLogger.Log.HardwareStationPrinterDeviceAlreadyClaimed(peripheralName);
                    }

                    // Enable
                    RetailLogger.Log.HardwareStationOposMethodCall(this.printerInstanceName, "DeviceEnabled = true");
                    oposPrinter.DeviceEnabled = true;
                    this.CheckResultCode("DeviceEnabled = true", oposPrinter.ResultCode, this.GetExtendedResultCodeFromPrinter(oposPrinter));

                    this.characterSet = characterSet;
                    this.binaryConversion = binaryConversion;
                }
                else
                {
                    RetailLogger.Log.HardwareStationPrinterDeviceAlreadyEnabled(peripheralName);
                }

                // Check if printer if run out of paper before doing any print calls.
                this.CheckPaperStatus(oposPrinter);

                oposPrinter.AsyncMode = false;
                oposPrinter.CharacterSet = this.characterSet;
                oposPrinter.RecLineChars = 56;
                oposPrinter.SlpLineChars = 60;
                this.BinaryConversionEnabled = this.binaryConversion;
                this.usePrintBitmapMethod = GetPrintBitmapConfiguration();
            }

            /// <summary>
            /// Closes the peripheral.
            /// </summary>
            private void Close()
            {
                IOPOSPOSPrinter oposPrinter = OPOSDeviceManager<IOPOSPOSPrinter>.Instance.AcquireDeviceHandle<OPOSPOSPrinterClass>();

                if (oposPrinter != null)
                {
                    try
                    {
                        RetailLogger.Log.HardwareStationOposMethodCall(this.printerInstanceName, "DeviceEnabled = false");
                        oposPrinter.DeviceEnabled = false;

                        RetailLogger.Log.HardwareStationOposMethodCall(this.printerInstanceName, "ReleaseDevice");
                        oposPrinter.ReleaseDevice();

                        RetailLogger.Log.HardwareStationOposMethodCall(this.printerInstanceName, "Close");
                        oposPrinter.Close();
                    }
                    finally
                    {
                        OPOSDeviceManager<IOPOSPOSPrinter>.Instance.ReleaseDeviceHandle(oposPrinter);
                    }
                }
            }

            /// <summary>
            /// Print the data on printer.
            /// </summary>
            /// <param name="header">The header.</param>
            /// <param name="lines">The lines.</param>
            /// <param name="footer">The footer.</param>
            private void Print(string header, string lines, string footer)
            {
                IOPOSPOSPrinter oposPrinter = OPOSDeviceManager<IOPOSPOSPrinter>.Instance.AcquireDeviceHandle<OPOSPOSPrinterClass>();
                //header = header.Replace(OposHelper.EscMarker, OposHelper.EscCharacter);
                //lines = lines.Replace(OposHelper.EscMarker, OposHelper.EscCharacter);
                //footer = footer.Replace(OposHelper.EscMarker, OposHelper.EscCharacter);

                header = string.IsNullOrEmpty(header) ? string.Empty : header.Replace(OposHelper.EscMarker, OposHelper.EscCharacter);
                lines = string.IsNullOrEmpty(lines) ? string.Empty : lines.Replace(OposHelper.EscMarker, OposHelper.EscCharacter);
                footer = string.IsNullOrEmpty(footer) ? string.Empty : footer.Replace(OposHelper.EscMarker, OposHelper.EscCharacter);

                var runtime = CompositionManager.HardwareStationRuntime.GetCommerceRuntime();
                var getReceiptPartsRequest = new GetPrinterReceiptPartsRequest(this.printerInstanceName, this.characterSet, header, lines, footer);
#pragma warning disable AvoidBlockingCallsAnalyzer // Avoid blocking asynchronous execution.
                var receiptPartsResponse = runtime.ExecuteAsync<GetPrinterReceiptPartsResponse>(getReceiptPartsRequest, context: null).GetAwaiter().GetResult();
#pragma warning restore AvoidBlockingCallsAnalyzer // Avoid blocking asynchronous execution.

                this.PrintReceiptParts(oposPrinter, receiptPartsResponse.ReceiptHeaderParts);
                this.PrintReceiptParts(oposPrinter, receiptPartsResponse.ReceiptLinesParts);
                this.PrintReceiptParts(oposPrinter, receiptPartsResponse.ReceiptFooterParts);

                // Avoid paper cut when noting is printed.
                if (!string.IsNullOrEmpty(header + lines + footer) && this.ShouldCutPaper(oposPrinter))
                {
                    // The number of lines that must be advanced before the receipt paper is cut.
                    string extraLinesToBeCut = string.Empty;
                    for (int i = 0; i < oposPrinter.RecLinesToPaperCut; ++i)
                    {
                        extraLinesToBeCut += Environment.NewLine;
                    }

                    // Includes the new lines before cutting the paper.
                    this.PrintText(oposPrinter, extraLinesToBeCut);

                    // CutPaper() will be blocked if printer is run out of paper, so it's important to check paper status beforehand.
                    this.CheckPaperStatus(oposPrinter);

                    // PERCENTAGE VALUE:
                    // 0        No cut
                    // 100      Full cut
                    // 1 - 99   Partial cut (will always leave small amount of paper uncut irrespective of actual value)
                    oposPrinter.CutPaper(95);
                }
            }

            /// <summary>
            /// Print the receipt parts.
            /// </summary>
            /// <param name="oposPrinter">The opos printer instance.</param>
            /// <param name="partsToPrint">The receipt parts to print.</param>
            private void PrintReceiptParts(IOPOSPOSPrinter oposPrinter, IReadOnlyCollection<ReceiptPart> partsToPrint)
            {
                foreach (var part in partsToPrint)
                {
                    switch (part.Type)
                    {
                        case ReceiptPartType.LegacyLogo:
                        case ReceiptPartType.LogoWithBytes:
                        case ReceiptPartType.Image:
                            var data = TextParser.GetImageBytes(part);
                            this.PrintImage(oposPrinter, data);
                            break;
                        case ReceiptPartType.Text:
                            if (!string.IsNullOrWhiteSpace(part.Value))
                            {
                                this.PrintText(oposPrinter, part.Value);
                            }

                            break;
                        default:
                            break;
                    }
                }
            }

            /// <summary>
            /// Runs health check.
            /// </summary>
            /// <param name="request">The health check request.</param>
            private void HealthCheck(HealthCheckPrinterDeviceRequest request)
            {
                try
                {
                    // Try to open OPOS device.
                    this.Open(request.DeviceName, request.CharacterSet, request.BinaryConversion);
                }
                finally
                {
                    // Close/clean device state even if it failed to open.
                    this.Close();
                }
            }

            /// <summary>
            /// Method to print the raw bytes of a BMP-formatted image.
            /// </summary>
            /// <param name="oposPrinter">The OPOS printer.</param>
            /// <param name="image">Image bytes to print.</param>
            private void PrintImage(IOPOSPOSPrinter oposPrinter, byte[] image)
            {
                int resultCode = (int)OPOS_Constants.OPOS_SUCCESS;

                // Check if printer run out of paper after previous print call.
                this.CheckPaperStatus(oposPrinter);

                if (oposPrinter.CapRecBitmap)
                {
                    if (image != null && image.Length > 0)
                    {
                        int conversion = oposPrinter.BinaryConversion; // save current conversion mode
                        oposPrinter.BinaryConversion = 2; // OposBcDecimal

                        if (this.usePrintBitmapMethod)
                        {
                            try
                            {
                                string imageFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "logo.bmp");
                                System.IO.File.WriteAllBytes(imageFilePath, image);

                                resultCode = oposPrinter.PrintBitmap(
                                    (int)OPOSPOSPrinterConstants.PTR_S_RECEIPT,
                                    imageFilePath,
                                    (int)OPOSPOSPrinterConstants.PTR_BM_ASIS,
                                    (int)OPOSPOSPrinterConstants.PTR_BM_CENTER);
                            }
                            catch (Exception ex)
                            {
                                RetailLogger.Log.LogError(Events.PrintBitmapException, "PrintBitmap() failed due to following exception: {ex}", ex.ToString().AsSystemMetadata());
                            }
                        }
                        else
                        {
                            resultCode = oposPrinter.PrintMemoryBitmap(
                                (int)OPOSPOSPrinterConstants.PTR_S_RECEIPT,
                                OposHelper.ConvertToBCD(image),
                                (int)OPOSPOSPrinterConstants.PTR_BMT_BMP,
                                (int)OPOSPOSPrinterConstants.PTR_BM_ASIS,
                                (int)OPOSPOSPrinterConstants.PTR_BM_CENTER);
                        }

                        oposPrinter.BinaryConversion = conversion; // restore previous conversion mode
                        this.CheckResultCode(
                            this.usePrintBitmapMethod ? nameof(oposPrinter.PrintBitmap) : nameof(oposPrinter.PrintMemoryBitmap),
                            resultCode,
                            this.GetExtendedResultCodeFromPrinter(oposPrinter));
                    }
                    else
                    {
                        RetailLogger.Log.LogWarning(Events.InvalidImage, "Logo image is invalid or null.");
                    }
                }
                else
                {
                    RetailLogger.Log.LogWarning(Events.PrintBitmapNotSupported, "The printer '{printerName}' can't print bitmaps.", this.printerInstanceName.AsObjectMetadata());
                }
            }

            /// <summary>
            /// Check the OPOS printer result code.
            /// </summary>
            /// <param name="oposMethod">The OPOS method called.</param>
            /// <param name="resultCode">The result code from OPOS method.</param>
            /// <param name="resultCodeExtended">The extended result code from OPOS method.</param>
            private void CheckResultCode(string oposMethod, int resultCode, int resultCodeExtended)
            {
                if (string.IsNullOrEmpty(oposMethod))
                {
                    throw new ArgumentException(string.Format("'Argument '{0}' can't be null or empty", nameof(oposMethod)));
                }

                if (resultCode != (int)OPOS_Constants.OPOS_SUCCESS)
                {
                    // Following error codes indicates 'out of paper' error.
                    if (resultCode == (int)OPOS_Constants.OPOS_E_EXTENDED && resultCodeExtended == (int)OPOSPOSPrinterConstants.OPOS_EPTR_REC_EMPTY)
                    {
                        RetailLogger.Log.HardwareStationPrinterOutOfPaper(this.printerInstanceName, PeripheralType.Opos);
                        throw new PeripheralException(PeripheralException.PrinterOutOfPaperError);
                    }
                    else
                    {
                        string message = string.Format(
                                                "OPOS printer failed to execute '{0}' with error '{1}', extended error code - {2}",
                                                oposMethod, resultCode, resultCodeExtended);
                        throw new PeripheralException(PeripheralException.PrinterError, message);
                    }
                }
            }

            /// <summary>
            /// Check the OPOS printer paper status.
            /// </summary>
            /// <param name="oposPrinter">The OPOS printer.</param>
            private void CheckPaperStatus(IOPOSPOSPrinter oposPrinter)
            {
                try
                {
                    if (oposPrinter.CapRecEmptySensor && oposPrinter.RecEmpty)
                    {
                        RetailLogger.Log.HardwareStationPrinterOutOfPaper(this.printerInstanceName, PeripheralType.Opos);
                        throw new PeripheralException(PeripheralException.PrinterOutOfPaperError);
                    }

                    // For diagnostic purposes log event when printer near out of paper. That should help to correlate driver's freeze/crash
                    // which customer facing now with HP printers.
                    if (oposPrinter.CapRecNearEndSensor && oposPrinter.RecNearEnd)
                    {
                        RetailLogger.Log.HardwareStationPrinterAlmostOutOfPaper(this.printerInstanceName, PeripheralType.Opos);
                    }
                }
                catch (Exception)
                {
                    // Ignoring exception here since all devices are supporting those basic properties and reading them causing issues only
                    // for older version of Peripheral Simulator, so this needed only for backward compatibility.
                }
            }

            /// <summary>
            /// Determines if the printer should cut the receipt paper based on the hardware station configuration and if it is supported on the OPOS printer.
            /// </summary>
            /// <param name="oposPrinter">The OPOS printer.</param>
            /// <returns>True if the receipt paper should be cut. False otherwise.</returns>
            private bool ShouldCutPaper(IOPOSPOSPrinter oposPrinter)
            {
                if (CompositionManager.HardwareStationRuntime.HardwareStationSection.Settings.TryGetValue(SkipOposCheckCapRecPapercut, out string skipOposCheckCapRecPapercut))
                {
                    if (!string.IsNullOrWhiteSpace(skipOposCheckCapRecPapercut) && bool.TryParse(skipOposCheckCapRecPapercut, out bool result) && result)
                    {
                        return true;
                    }
                }

                try
                {
                    return oposPrinter.CapRecPapercut;
                }
                catch (Exception ex)
                {
                    // If there is an exception thrown when checking the RecPapercut property log an error and return false to skip paper cutting.
                    RetailLogger.Log.LogError(Events.CapRecPapercutException, "oposPrinter.CapRecPapercut failed due to following exception: {ex}", ex.AsSystemMetadata());
                    return false;
                }
            }

            /// <summary>
            /// Trying to get <c>ResultCodeExtended</c> value from OPOS printer and returns default if failed.
            /// </summary>
            /// <remarks>This is needed to support backward compatibility with older version of Peripheral Simulator.</remarks>
            /// <param name="oposPrinter">The OPOS printer.</param>
            /// <returns>The value of ResultCodeExtended printer's property.</returns>
            private int GetExtendedResultCodeFromPrinter(IOPOSPOSPrinter oposPrinter)
            {
                var result = (int)OPOS_Constants.OPOS_SUCCESS;
                try
                {
                    result = oposPrinter.ResultCodeExtended;
                }
                catch (Exception)
                {
                    // Ignoring exception here since all devices are supporting those basic properties and reading them causing issues only
                    // for older version of Peripheral Simulator, so this needed only for backward compatibility.
                }

                return result;
            }
        }
    }
}