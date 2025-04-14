namespace Contoso
{
    namespace Commerce.HardwareStation
    {
        using System;
        using System.IO;
        using System.Threading.Tasks;
        using Contoso.StoreCommercePackagingSample.HardwareStation.Messages;
        using Microsoft.Dynamics.Commerce.HardwareStation;
        using Microsoft.Dynamics.Commerce.HardwareStation.Models;
        using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;

        [RoutePrefix("DATAEXPORT")]
        public class DataExportController: IController
        {
            [HttpPost]
            public async Task<string> ExportToExcel(ExportInventoryDocumentLinesRequest exportInventoryDocumentLinesRequest)
            {
                ThrowIf.Null(exportInventoryDocumentLinesRequest, "exportInventoryDocumentLinesRequest");
                string filePath = string.Empty;
                try
                {
                    var generator = new ExcelGenerator();
                    filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ExportedExcel", $"{exportInventoryDocumentLinesRequest.FileName}");
                    generator.ExportToExcelFile(filePath, exportInventoryDocumentLinesRequest.Lines);
                    return await Task.FromResult<string>(filePath).ConfigureAwait(false);
                }
                catch (PeripheralException ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw new PeripheralException(PeripheralException.PeripheralEventError, ex.Message, ex);
                }
            }
        }
    }
}
