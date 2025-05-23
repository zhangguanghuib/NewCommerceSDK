using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintReport
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Prepare sample data
            DataTable dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Amount", typeof(decimal));
            dt.Rows.Add("Alice", 123.45m);
            dt.Rows.Add("Bob", 678.90m);

            // Create LocalReport and load RDLC
            LocalReport report = new LocalReport();
            report.ReportPath = @"SampleReport.rdlc"; // Path to your RDLC file

            // Add data source (name must match DataSet name in RDLC)
            report.DataSources.Add(new ReportDataSource("SampleDataSet", dt));

            // Render to PDF
            Warning[] warnings;
            string[] streamIds;
            string mimeType, encoding, extension;
            byte[] pdfBytes = report.Render(
                "PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

            // Save PDF
            File.WriteAllBytes("SampleReport.pdf", pdfBytes);

            Console.WriteLine("PDF generated: SampleReport.pdf");

        }
    }
}
