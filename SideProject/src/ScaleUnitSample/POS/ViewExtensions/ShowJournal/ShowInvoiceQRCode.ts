
import * as ShowJournalView from "PosApi/Extend/Views/ShowJournalView";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import { StringExtensions } from "PosApi/TypeExtensions";
import {
    GetDeviceConfigurationClientRequest,
    GetDeviceConfigurationClientResponse,
    GetHardwareProfileClientRequest,
    GetHardwareProfileClientResponse
} from "PosApi/Consume/Device";

import {
    GetReceiptsClientRequest,
    GetReceiptsClientResponse,
    GetSalesOrderDetailsByTransactionIdClientRequest,
    GetSalesOrderDetailsByTransactionIdClientResponse
} from "PosApi/Consume/SalesOrders";

import { PrinterPrintRequest, PrinterPrintResponse } from "PosApi/Consume/Peripherals";

export default class ShowInvoiceQRCode extends ShowJournalView.ShowJournalExtensionCommandBase {
    public _selectedJournal: ProxyEntities.SalesOrder;
    public _products: ProxyEntities.SimpleProduct[];
    public _customer: ProxyEntities.Customer;
    public _mode: ClientEntities.ShowJournalMode;

    /**
     * Creates a new instance of the DownloadDocumentCommand class.
     * @param {IExtensionCommandContext<Extensibility.IShowJournalToExtensionCommandMessageTypeMap>} context The command context.
     * @remarks The command context contains APIs through which a command can communicate with POS.
     */
    constructor(context: IExtensionCommandContext<ShowJournalView.IShowJournalToExtensionCommandMessageTypeMap>) {
        super(context);

        this.id = "ShowInvoiceQRCode";
        this.label = "Show Invoice QRCode";
        this.extraClass = "iconInvoice";

        this.journalSelectionHandler = (data: ShowJournalView.ShowJournalJournalSelectedData): void => {
            this._journalChanged(data);
        };

        this.journalSelectionClearedHandler = (): void => {
            this._selectedJournal = undefined;
            this._products = [];
            this._customer = undefined;
            this.canExecute = false;
        };

        this.receiptSelectionHandler = (data: ShowJournalView.ShowJournalReceiptSelectedData): void => {
            this.isVisible = false;
        };

        this.receiptSelectionClearedHandler = (): void => {
            this.isVisible = true;
        };

        this.journalTransactionsLoadedHandler = (data: ShowJournalView.ShowJournalJournalTransactionsLoadedData): void => {
            this.isVisible = this._mode === ClientEntities.ShowJournalMode.ShowJournal;
            this.context.logger.logInformational("Executing journalTransactionsLoadedHandler for DownloadDocumentCommand: "
                + JSON.stringify(data) + ".");
        };
    }

    /**
     * Initializes the command.
     * @param {Extensibility.IShowJournalExtensionCommandState} state The state used to initialize the command.
     */
    protected init(state: ShowJournalView.IShowJournalExtensionCommandState): void {
        this._mode = state.mode;
    }

    /**
     * Executes the command.
     */
    protected execute(): void {
        this.isProcessing = true;
        this.runPingTest().then(() => {
            Promise.resolve();
            this.isProcessing = false;
        });
        
    }

    /**
     * Handles the journal changed message by sending a message by updating the command state.
     * @param {Extensibility.ShowJournalJournalSelectedData} data The information about the selected journal.
     */
    private _journalChanged(data: ShowJournalView.ShowJournalJournalSelectedData): void {
        this._selectedJournal = data.salesOrder;
        this._products = data.products;
        this._customer = data.customer;
        if (!StringExtensions.isEmptyOrWhitespace(this._selectedJournal.ReceiptId)) {
            this.canExecute = true;
        }
    }


    public runPingTest(): Promise<void> {

        //let req: GetSalesOrderDetailsByTransactionIdClientRequest<GetSalesOrderDetailsByTransactionIdClientResponse>
        //    = new GetSalesOrderDetailsByTransactionIdClientRequest<GetSalesOrderDetailsByTransactionIdClientResponse>(this._selectedJournal.SalesId, ProxyEntities.SearchLocation.Local);

        // return this.context.runtime.executeAsync(req).then((res: ClientEntities.ICancelableDataResult<GetSalesOrderDetailsByTransactionIdClientResponse>): Promise<ProxyEntities.Receipt[]> => {
   
        let salesOrder: ProxyEntities.SalesOrder = this._selectedJournal;

        return Promise.all([
            this.context.runtime.executeAsync(new GetHardwareProfileClientRequest())
                .then((response: ClientEntities.ICancelableDataResult<GetHardwareProfileClientResponse>): ProxyEntities.HardwareProfile => {
                    return response.data.result;
                }),
            this.context.runtime.executeAsync(new GetDeviceConfigurationClientRequest())
                .then((response: ClientEntities.ICancelableDataResult<GetDeviceConfigurationClientResponse>): ProxyEntities.DeviceConfiguration => {
                    return response.data.result;
                })])
            .then((results: any[]): Promise<ClientEntities.ICancelableDataResult<GetReceiptsClientResponse>> => {
                let hardwareProfile: ProxyEntities.HardwareProfile = results[0];
                let deviceConfiguration: ProxyEntities.DeviceConfiguration = results[1];

                let criteria: ProxyEntities.ReceiptRetrievalCriteria = {
                    IsCopy: true,
                    IsRemoteTransaction: salesOrder.StoreId !== deviceConfiguration.StoreNumber,
                    IsPreview: false,
                    QueryBySalesId: false,
                    ReceiptTypeValue: ProxyEntities.ReceiptType.CustomReceipt1,
                    HardwareProfileId: hardwareProfile.ProfileId
                };

                let getReceiptsRequest: GetReceiptsClientRequest<GetReceiptsClientResponse> = new GetReceiptsClientRequest(salesOrder.SalesId ? salesOrder.SalesId : salesOrder.Id, criteria);
                return this.context.runtime.executeAsync(getReceiptsRequest);
            })
            .then((response: ClientEntities.ICancelableDataResult<GetReceiptsClientResponse>): ProxyEntities.Receipt[] => {
                return response.data.result;
            }).then((receipts: ProxyEntities.Receipt[]): Promise<ClientEntities.ICancelableDataResult<PrinterPrintResponse>> => {
            let printRequest: PrinterPrintRequest<PrinterPrintResponse> = new PrinterPrintRequest(receipts);
            return this.context.runtime.executeAsync(printRequest);
        }).then((): Promise<void> => {
            return Promise.resolve();
        }).catch((reason: any) => {
            console.error(reason);
        });
    }
}