

namespace SAG
{
    namespace Commerce.HardwareStation.Extension.PDFPOSReportPrinter
    {
        using System;
        using System.Threading.Tasks;
        using Microsoft.Dynamics.Commerce.HardwareStation;
        using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
        using Spire.Pdf;
        using iText.Html2pdf;
        using System.IO;
        using iText.Kernel.Pdf;
        using iText.Kernel.Geom;
        using System.Text;
        using iText.IO;
        using iTextSharp.text;
        using iTextSharp.text.pdf;

        /// <summary>
        /// PDF printer web API controller class.
        /// </summary>
        [RoutePrefix("PDFPOSREPORTPRINTER")]
        public class PDFPrinterController : IController
        {
            [HttpPost]
            public async Task<bool> PrintPOSPDFReports(PrintPOSReportPDFRequest request)
            {
                ThrowIf.Null(request, "request");

                try
                {
                    if (request.HTMLString == "")
                    {
                        return await Task.FromResult(false).ConfigureAwait(false);
                    }

                    if (request.PrinterName == "")
                    {
                        request.PrinterName = "Microsoft Print to PDF";
                    }

                    if (request.FilePath == "")
                    {
                        request.FilePath = @"C:\POSReports";
                    }

                    //MemoryStream pdfstream = new MemoryStream();
                    //HtmlConverter.ConvertToPdf(request.HTMLString, pdfstream);

                    if (!Directory.Exists(request.FilePath))
                    {
                        Directory.CreateDirectory(request.FilePath);
                    }

                    string filepath = System.IO.Path.Combine(request.FilePath, request.FileName + DateTime.Now.ToString("ddMMyyyyhhmmtt") + ".pdf");

                    using (MemoryStream inputstream = new MemoryStream(ASCIIEncoding.UTF8.GetBytes(request.HTMLString)))
                    {
                        using (var pdfWriter = new iText.Kernel.Pdf.PdfWriter(filepath))
                        {

                            using (iText.Kernel.Pdf.PdfDocument pdfdoc = new iText.Kernel.Pdf.PdfDocument(pdfWriter))
                            {
                                pdfdoc.SetDefaultPageSize(iText.Kernel.Geom.PageSize.A4.Rotate());

                                HtmlConverter.ConvertToPdf(inputstream, pdfdoc);
                                using (var pdfReader = new iText.Kernel.Pdf.PdfReader(filepath))
                                {
                                    using (iText.Kernel.Pdf.PdfDocument pdfreader = new iText.Kernel.Pdf.PdfDocument(pdfReader))
                                    {

                                        int index = 0;
                                        int noofpages = pdfreader.GetNumberOfPages();
                                        pdfreader.Close();

                                        bool stoploopingCounter = false;
                                        int splitpage = 3;
                                        int loopcounter = (noofpages % splitpage == 0) ? (noofpages / splitpage) : (noofpages / splitpage) + 1;
                                        while (index <= loopcounter)
                                        {
                                            string splitfilename = System.IO.Path.Combine(request.FilePath, request.FileName + DateTime.Now.ToString("ddMMyyyyhhmmtt") + index + ".pdf");
                                            int endpage = (index + 1) * splitpage > noofpages ? noofpages : (index + 1) * splitpage;

                                            this.ExtractPages(filepath, splitfilename, (index * splitpage) + 1, endpage);
                                            if ((index + 1) * splitpage >= noofpages)
                                            {
                                                stoploopingCounter = true;
                                            }
                                            index++;
                                            //Spire.Pdf.PdfDocument pdfDocument = new Spire.Pdf.PdfDocument(splitfilename);
                                            //pdfDocument.PrintSettings.PrinterName = request.PrinterName;
                                            //pdfDocument.Print();

                                            File.Delete(splitfilename);

                                            if (stoploopingCounter)
                                            {
                                                break;
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }


                    // save File Contents
                    // File.WriteAllBytes(filepath, pdfstream.ToArray());

                    // PDF Print document
                    //Spire.Pdf.PdfDocument pdfDocument = new Spire.Pdf.PdfDocument(filepath);
                    //pdfDocument.PrintSettings.PrinterName = request.PrinterName;
                    //pdfDocument.Print();

                    return await Task.FromResult(true).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    throw new PeripheralException(PeripheralException.PrinterError, ex.Message, ex);
                }

            }

            [HttpPost]
            public async Task<bool> PrintAdditionalPOSPDFReports(PrintPOSReportPDFRequest request)
            {
                ThrowIf.Null(request, "request");

                try
                {
                    if (request.FilePath == null || request.FilePath == "")
                    {
                        request.FilePath = @"C:\POSReports";
                    }

                    byte[] receivedFile = Convert.FromBase64String(request.HTMLString);

                    //MemoryStream inputstream = new MemoryStream(receivedFile);

                    string filepath = System.IO.Path.Combine(request.FilePath, "EODReport" + DateTime.Now.ToString("ddMMyyyyhhmmtt") + ".pdf");

                    if (!Directory.Exists(request.FilePath))
                    {
                        Directory.CreateDirectory(request.FilePath);
                    }

                    File.WriteAllBytes(filepath, receivedFile);

                    //iText.Kernel.Pdf.PdfDocument pdfdoc = new iText.Kernel.Pdf.PdfDocument(new iText.Kernel.Pdf.PdfWriter(filepath));
                    //pdfdoc.SetDefaultPageSize(iText.Kernel.Geom.PageSize.A4.Rotate());

                    //HtmlConverter.ConvertToPdf(inputstream, pdfdoc);
                    using (var pdfReader = new iText.Kernel.Pdf.PdfReader(filepath))
                    {
                        using (iText.Kernel.Pdf.PdfDocument pdfDocument = new iText.Kernel.Pdf.PdfDocument(pdfReader))

                        {

                            int index = 0;
                            int noofpages = pdfDocument.GetNumberOfPages();
                            pdfDocument.Close();
                            bool stoploopingCounter = false;
                            int splitpage = 3;
                            int loopcounter = (noofpages % splitpage == 0) ? (noofpages / splitpage) : (noofpages / splitpage) + 1;
                            while (index <= loopcounter)
                            {
                                string splitfilename = System.IO.Path.Combine(request.FilePath, request.FileName + DateTime.Now.ToString("ddMMyyyyhhmmtt") + index + ".pdf");
                                int endpage = (index + 1) * splitpage > noofpages ? noofpages : (index + 1) * splitpage;

                                this.ExtractPages(filepath, splitfilename, (index * splitpage) + 1, endpage);
                                if ((index + 1) * splitpage >= noofpages)
                                {
                                    stoploopingCounter = true;
                                }
                                index++;
                                using (Spire.Pdf.PdfDocument pdfDocumentNew = new Spire.Pdf.PdfDocument(splitfilename))
                                {
                                    pdfDocumentNew.PrintSettings.PrinterName = request.PrinterName;
                                    pdfDocumentNew.Print();

                                    if (stoploopingCounter)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    // Add here the code to send the PDF file to the printer.
                    //Spire.Pdf.PdfDocument pdf = new Spire.Pdf.PdfDocument(receivedFile);
                    //pdf.PrintSettings.PrinterName = request.PrinterName;
                    //pdf.Print();


                    return await Task.FromResult(true).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    throw new PeripheralException(PeripheralException.PrinterError, ex.Message, ex);
                }

            }

            private void ExtractPages(string sourcePDFpath, string outputPDFpath, int startpage, int endpage)
            {
                iTextSharp.text.pdf.PdfReader reader = null;
                Document sourceDocument = null;
                PdfCopy pdfCopyProvider = null;
                PdfImportedPage importedPage = null;

                reader = new iTextSharp.text.pdf.PdfReader(sourcePDFpath);
                sourceDocument = new Document(reader.GetPageSizeWithRotation(startpage));
                pdfCopyProvider = new PdfCopy(sourceDocument, new System.IO.FileStream(outputPDFpath, System.IO.FileMode.Create));

                sourceDocument.Open();

                for (int i = startpage; i <= endpage; i++)
                {
                    importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                    pdfCopyProvider.AddPage(importedPage);
                }
                sourceDocument.Close();
                reader.Close();
            }

        }
    }
}
