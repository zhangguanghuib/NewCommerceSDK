import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import { HardwareStationDeviceActionRequest, HardwareStationDeviceActionResponse } from "PosApi/Consume/Peripherals";
import *  as InventoryDocument from "PosApi/Extend/Views/InventoryDocumentShippingAndReceivingView";
import * as Proxy from "../../DataService/DataServiceRequests.g";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import MessageDialog from "../../Controls/Dialogs/Create/MessageDialog";
export default class ExportFullOrderListCommand extends InventoryDocument.InventoryDocumentShippingAndReceivingExtensionCommandBase {

    private SourceDocumentId: string;
    private SourceDocumentTypeValue: number;

    public constructor(context: IExtensionCommandContext<InventoryDocument.IInventoryDocumentShippingAndReceivingToExtensionCommandMessageTypeMap>) {
        super(context);

        this.label = "Export";
        this.id = "ExportInventoryOrderList";
        this.extraClass = "iconLightningBolt";
        this.SourceDocumentId = "";
        this.SourceDocumentTypeValue = 0;
    }

    protected init(state: InventoryDocument.IInventoryDocumentShippingAndReceivingExtensionCommandState): void {
        this.isVisible = true;
        this.canExecute = true;
        this.SourceDocumentId = state.document.SourceDocument.DocumentId;
        this.SourceDocumentTypeValue = state.document.SourceDocument.DocumentTypeValue;
        console.log("ExportFullOrderListCommand - init");
    }

    private getUniqueExcelName(): string {
        const timestamp = new Date().getTime();
        const timestampString = timestamp.toString();
        return `ExportedData_${timestampString}.xlsx`;
    }

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

}