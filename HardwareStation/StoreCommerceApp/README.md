# Call Hardware Station API to export data to Excel or PDF
## 1. The HardWare Station API is like:
```csharp
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
```
## 2. The POS  API to call Hardware Station API is:
```TS
protected execute(): void {
    this.isProcessing = true;
    let sourceDocumentLines: ProxyEntities.InventoryInboundOutboundSourceDocumentLine[] = [];
    const searchCriteria: ProxyEntities.InventoryDocumentLineSearchCriteria =
    {
        SourceDocumentId: this.SourceDocumentId,
        SourceDocumentTypeValue: this.SourceDocumentTypeValue
    }
    const request: Proxy.StoreOperations.SearchInventoryDocumentLineRequest<Proxy.StoreOperations.SearchInventoryDocumentLineResponse>
        = new Proxy.StoreOperations.SearchInventoryDocumentLineRequest(searchCriteria);
    this.context.runtime.executeAsync(request)
        .then((response: ClientEntities.ICancelableDataResult<Proxy.StoreOperations.SearchInventoryDocumentLineResponse>) => {
            if (response.data.result.length > 0) {
                const inventoryInboundOutboundDocumentLines: ProxyEntities.InventoryInboundOutboundDocumentLine[] = response.data.result;
                inventoryInboundOutboundDocumentLines.forEach((inventoryInboundOutboundDocumentLine: ProxyEntities.InventoryInboundOutboundDocumentLine) => {
                    sourceDocumentLines.push(inventoryInboundOutboundDocumentLine.SourceDocumentLine);
                });
                console.table(sourceDocumentLines);
            }               
        }).then(() => {
            let exportInventoryDocumentLinesRequest: { Lines: ProxyEntities.InventoryInboundOutboundSourceDocumentLine[], FileName: string } = {
                Lines: sourceDocumentLines,
                FileName: this.getUniqueExcelName()
            };
            let hardwareStatationDeviceActionRequest: HardwareStationDeviceActionRequest<HardwareStationDeviceActionResponse> =
                new HardwareStationDeviceActionRequest("DATAEXPORT",
                    "ExportToExcel",
                    exportInventoryDocumentLinesRequest
                );
            this.context.runtime.executeAsync(hardwareStatationDeviceActionRequest).then((response: ClientEntities.ICancelableDataResult<HardwareStationDeviceActionResponse>) => {
                console.log(response.data.response);
                this.context.logger.logInformational("Hardware Station request executed successfully");
                const exportedFolder: string = response.data.response;
                const message: string = `Export the transfer order lines successfully, the exported file is in this folder: ${exportedFolder}`;
                MessageDialog.show(this.context, message).then(() => {
                    this.isProcessing = false;
                });
            }).catch((err) => {
                this.context.logger.logInformational("Failure in executing Hardware Station request");
                const message: string = "Something wrong happened when export the transder order lines via Hardware Station";
                MessageDialog.show(this.context, message).then(() => {
                    this.isProcessing = false;
                    throw err;
                });
            });
        });    
}
```
## 3. C# Nuget package for Excel Export:<br\>
![image](https://github.com/user-attachments/assets/ed9bb45d-6bac-4a22-9232-f02ef10f915d)
## 4. Add the command button to view:<br\>
![image](https://github.com/user-attachments/assets/67f2d3cd-56eb-4a49-b1b4-98363b17136a)


