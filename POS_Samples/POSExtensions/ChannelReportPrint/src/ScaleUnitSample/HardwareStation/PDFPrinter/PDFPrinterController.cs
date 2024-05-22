/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */

namespace SAG
{
    namespace Commerce.HardwareStation.PDFPrinter
    {
        using System;
        using System.IO;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.HardwareStation;
        using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
        using Spire.Pdf;

        /// <summary>
        /// PDF printer web API controller class.
        /// </summary>
        [RoutePrefix("PDFPRINTER")]
        public class PDFPrinterController : IController
        {
            /// <summary>
            /// Prints the content.
            /// </summary>
            /// <param name="printRequest">The print request.</param>
            /// <exception cref="System.Web.Http.HttpResponseException">Exception thrown when an error occurs.</exception>
            [HttpPost]
            public async Task<bool> Print(PrintPDFRequest printRequest)
            {
                ThrowIf.Null(printRequest, "printRequests");

                try
                {
                    byte[] receivedFile = Convert.FromBase64String(printRequest.EncodedBinary);
                    ///
                    string filePath = "C:\\POSReports";
                    string fileName = "";
                    if(!String.IsNullOrEmpty(printRequest.FileName))
                    {
                        fileName = printRequest.FileName;
                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }
                        string filepath = System.IO.Path.Combine(filePath, fileName + DateTime.Now.ToString(" ddMMyyyy_hhmmtt") + ".pdf");
                        File.WriteAllBytes(filepath, receivedFile);
                    }
                    else
                    {
                        // Add here the code to send the PDF file to the printer.
                        using (PdfDocument pdf = new PdfDocument(receivedFile))
                        {
                            pdf.PrintSettings.PrinterName = printRequest.DeviceName;
                            pdf.Print();
                        }
                    }

                    return await Task.FromResult(true).ConfigureAwait(false);
                }
                catch (PeripheralException ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw new PeripheralException(PeripheralException.PrinterError, ex.Message, ex);
                }
            }
        }
    }
}
