/**
* SAMPLE CODE NOTICE
* 
* THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
* OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
* THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
* NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
namespace Contoso.Commerce.HardwareStation.Extension.WindowsPrinterSample
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Printing;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.Dynamics.Commerce.HardwareStation;
    using Microsoft.Dynamics.Commerce.HardwareStation.Peripherals;
    using Microsoft.Dynamics.Commerce.Runtime.Handlers;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    /// <summary>
    /// Class implements windows based printer driver for hardware station.
    /// </summary>
    public class WindowsPrinterSample : INamedRequestHandler
    {
        private const string BarCodeRegEx = "<B: (.+?)>";
        private const string NormalTextMarker = "|1C";
        private const string BoldTextMarker = "|2C";
        private const string DoubleSizeNormalTextMarker = "|3C";
        private const string DoubleSizeBoldTextMarker = "|4C";

        private const string ExceptNormalMarkersRegEx = @"\|2C|\|3C|\|4C";
        private const string AllMarkersForSplitRegEx = @"(\|1C|\|2C|\|3C|\|4C)";

        private const float defaultDpiScalingFactor = 96f;

        private List<TextPart> parts;
        private int printLine;
        private Barcode barCode = new BarcodeCode39();
        private DefaultLogo defaultLogo = new DefaultLogo();

        /// <summary>
        /// Gets the esc marker.
        /// </summary>
        public static string EscMarker
        {
            get
            {
                return "&#x1B;";
            }
        }

        /// <summary>
        /// Font used for receipt.
        /// </summary>
        public static string TextFontName
        {
            get
            {
                return "Courier New";
            }
        }

        /// <summary>
        /// Default font size for receipt.
        /// </summary>
        public static int TextFontSize
        {
            get
            {
                return 6;
            }
        }

        /// <summary>
        /// Large font size for receipt.
        /// </summary>
        public static int DoubleSizeTextFontSize
        {
            get
            {
                return TextFontSize * 2;
            }
        }

        /// <summary>
        /// Gets the unique name for this request handler.
        /// </summary>
        public string HandlerName
        {
            get { return PeripheralType.Windows; }
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
                    typeof(ClosePrinterDeviceRequest)
                };
            }
        }

        /// <summary>
        /// Gets or sets the printer name.
        /// </summary>
        protected string PrinterName { get; set; }

        /// <summary>
        /// Represents the entry point for the printer device request handler.
        /// </summary>
        /// <param name="request">The incoming request message.</param>
        /// <returns>The outgoing response message.</returns>
        public Response Execute(Request request)
        {
            ThrowIf.Null(request, nameof(request));

            Type requestType = request.GetType();

            if (requestType == typeof(OpenPrinterDeviceRequest))
            {
                var openRequest = (OpenPrinterDeviceRequest)request;

                this.Open(openRequest.DeviceName);
            }
            else if (requestType == typeof(PrintPrinterDeviceRequest))
            {
                var printRequest = (PrintPrinterDeviceRequest)request;

                this.Print(
                    printRequest.Header,
                    printRequest.Lines,
                    printRequest.Footer);
            }
            else if (requestType == typeof(ClosePrinterDeviceRequest))
            {
                // Do nothing. Just for support of the close request type.
            }
            else
            {
                throw new NotSupportedException(string.Format("Request '{0}' is not supported.", requestType));
            }

            return new NullResponse();
        }

        private static bool DrawBitmapImage(PrintPageEventArgs e, byte[] defaultLogoBytes, float contentWidth, ref float y)
        {
            using (MemoryStream ms = new MemoryStream(defaultLogoBytes))
            {
                var image = Image.FromStream(ms);

                if (y + image.Height >= e.MarginBounds.Height)
                {
                    // No more room - advance to next page
                    e.HasMorePages = true;
                    return false;
                }

                float center = ((contentWidth - image.Width) / 2.0f) + e.MarginBounds.Left;
                if (center < 0)
                {
                    center = 0;
                }

                float top = e.MarginBounds.Top + y;

                e.Graphics.DrawImage(image, center, top);

                y += image.Height;

                return true;
            }
        }

        /// <summary>
        /// Opens a peripheral.
        /// </summary>
        /// <param name="peripheralName">Name of the peripheral.</param>
        private void Open(string peripheralName)
        {
            this.PrinterName = peripheralName;
        }

        /// <summary>
        /// Print the data on printer.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="lines">The lines.</param>
        /// <param name="footer">The footer.</param>
        private void Print(string header, string lines, string footer)
        {
            string textToPrint = header + lines + footer;

            if (!string.IsNullOrWhiteSpace(textToPrint))
            {
                using (PrintDocument printDoc = new PrintDocument())
                {
                    printDoc.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
                    printDoc.PrinterSettings.PrinterName = this.PrinterName;
                    string subString = textToPrint.Replace(EscMarker, string.Empty);
                    var printText = subString.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                    this.parts = new List<TextPart>();
                    foreach (var line in printText)
                    {
                        var lineParts = TextLogoParser.Parse(line);
                        if (lineParts != null)
                        {
                            this.parts.AddRange(lineParts);
                        }
                    }

                    this.printLine = 0;

                    printDoc.PrintPage += new PrintPageEventHandler(this.PrintPageHandler);

                    try
                    {
                        printDoc.Print();
                    }
                    catch (InvalidPrinterException)
                    {
                        throw new PeripheralException(PeripheralException.PeripheralNotFound);
                    }
                }
            }
        }

        /// <summary>
        /// Prints the page.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PrintPageEventArgs" /> instance containing the event data.</param>
        private void PrintPageHandler(object sender, PrintPageEventArgs e)
        {
            const int LineHeight = 10;

            e.HasMorePages = false;
            using (Font textFont = new Font(TextFontName, TextFontSize, FontStyle.Regular))
            using (Font textFontBold = new Font(TextFontName, TextFontSize, FontStyle.Bold))
            using (Font doubleSizeTextFont = new Font(TextFontName, DoubleSizeTextFontSize, FontStyle.Regular))
            using (Font doubleSizeTextFontBold = new Font(TextFontName, DoubleSizeTextFontSize, FontStyle.Bold))
            {
                float y = 0;
                float dpiXRatio = e.Graphics.DpiX / defaultDpiScalingFactor;
                float dpiYRatio = e.Graphics.DpiY / defaultDpiScalingFactor;

                // This calculation isn't exactly the width of the rendered text.
                // All the calculations occurring in the rendering code of PrintTextLine.  It almost needs to run that code and use e.Graphics.MeasureString()
                // the first time to get the true contentWidth, then re-run the same logic using the true contentWidth for rendering.
                //
                // The rendering is close, but it's not 'exact' center due to the mismatch in estimated vs. true size
                float contentWidth = this.parts.Where(x => x.TextType == TextType.Text)
                                                .Select(p => p.Value)
                                                .Max(str => str.Replace(NormalTextMarker, string.Empty)
                                                                .Replace(BoldTextMarker, string.Empty)
                                                                .Replace(DoubleSizeNormalTextMarker, string.Empty)
                                                                .Replace(DoubleSizeBoldTextMarker, string.Empty)
                                                                .Length)
                                        * dpiXRatio; // Line with max length = content width

                for (; this.printLine < this.parts.Count; this.printLine++)
                {
                    var part = this.parts[this.printLine];

                    if (part.TextType == TextType.Text)
                    {
                        if (!this.PrintTextLine(
                                                e,
                                                LineHeight,
                                                textFont,
                                                textFontBold,
                                                doubleSizeTextFont,
                                                doubleSizeTextFontBold,
                                                dpiYRatio,
                                                contentWidth,
                                                dpiXRatio,
                                                part.Value,
                                                ref y))
                        {
                            return;
                        }
                    }
                    else if (part.TextType == TextType.LegacyLogo)
                    {
                        byte[] defaultLogoBytes = this.defaultLogo.GetBytes();
                        if (!DrawBitmapImage(e, defaultLogoBytes, contentWidth, ref y))
                        {
                            return;
                        }
                    }
                    else if (part.TextType == TextType.LogoWithBytes)
                    {
                        byte[] image = TextLogoParser.GetLogoImageBytes(part.Value);
                        if (!DrawBitmapImage(e, image, contentWidth, ref y))
                        {
                            return;
                        }
                    }
                }
            }
        }

        private bool PrintTextLine(
                        PrintPageEventArgs e,
                        int lineHeight,
                        Font textFont,
                        Font textFontBold,
                        Font doubleSizeTextFont,
                        Font doubleSizeTextFontBold,
                        float dpiYRatio,
                        float contentWidth,
                        float dpiXRatio,
                        string line,
                        ref float y)
        {
            // it always use one line height
            // because the Report designer use a col and a line to set position for all conrtols
            if (y + lineHeight >= e.MarginBounds.Height)
            {
                // No more room - advance to next page
                e.HasMorePages = true;
                return false;
            }

            bool hasFontModifier = (line.Length > 0) && Regex.IsMatch(line, ExceptNormalMarkersRegEx, RegexOptions.Compiled);
            if (hasFontModifier)
            {
                // Text line printing with bold Text in it.
                float x = 0;
                Font currentFont = textFont;
                int sizeFactor = 1;

                string[] subStrings = Regex.Split(line, AllMarkersForSplitRegEx, RegexOptions.Compiled);
                foreach (string subString in subStrings)
                {
                    switch (subString)
                    {
                        case NormalTextMarker:
                            currentFont = textFont;
                            sizeFactor = 1;
                            break;
                        case BoldTextMarker:
                            currentFont = textFontBold;
                            sizeFactor = 2;
                            break;
                        case DoubleSizeNormalTextMarker:
                            currentFont = doubleSizeTextFont;
                            sizeFactor = 1;
                            break;
                        case DoubleSizeBoldTextMarker:
                            currentFont = doubleSizeTextFontBold;
                            sizeFactor = 2;
                            break;
                        default:
                            if (!string.IsNullOrEmpty(subString))
                            {
                                e.Graphics.DrawString(subString, currentFont, Brushes.Black, x + e.MarginBounds.Left, y + e.MarginBounds.Top);
                                x = x + (subString.Length * sizeFactor * 6);
                            }

                            break;
                    }
                }
            }
            else
            {
                // Text line printing with no bold Text and no double size Text in it.
                string subString = line.Replace(NormalTextMarker, string.Empty);

                Match barCodeMarkerMatch = Regex.Match(subString, BarCodeRegEx, RegexOptions.Compiled | RegexOptions.IgnoreCase);

                if (barCodeMarkerMatch.Success)
                {
                    // Get the receiptId
                    subString = barCodeMarkerMatch.Groups[1].ToString();

                    using (Image barcodeImage = this.barCode.Create(subString, e.Graphics.DpiX, e.Graphics.DpiY))
                    {
                        if (barcodeImage != null)
                        {
                            float barcodeHeight = barcodeImage.Height / dpiYRatio;

                            if (y + barcodeHeight >= e.MarginBounds.Height)
                            {
                                // No more room - advance to next page
                                e.HasMorePages = true;
                                return true;
                            }

                            // Render barcode in the center of receipt.
                            e.Graphics.DrawImage(barcodeImage, ((contentWidth - (barcodeImage.Width / dpiXRatio)) / 2) + e.MarginBounds.Left, y + e.MarginBounds.Top);
                            y += barcodeHeight;
                        }
                    }
                }
                else
                {
                    e.Graphics.DrawString(subString, textFont, Brushes.Black, e.MarginBounds.Left, y + e.MarginBounds.Top);
                }
            }

            y = y + lineHeight;
            return true;
        }
    }
}
