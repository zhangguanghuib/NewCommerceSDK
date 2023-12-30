import { GetDeviceConfigurationClientRequest, GetDeviceConfigurationClientResponse, GetHardwareProfileClientRequest, GetHardwareProfileClientResponse } from "PosApi/Consume/Device";
import { PrinterPrintRequest, PrinterPrintResponse } from "PosApi/Consume/Peripherals";
//import { PrinterPrintRequest, PrinterPrintResponse } from "PosApi/Consume/Peripherals";
import { GetReceiptsClientRequest, GetReceiptsClientResponse, GetSalesOrderDetailsByTransactionIdClientRequest, GetSalesOrderDetailsByTransactionIdClientResponse } from "PosApi/Consume/SalesOrders";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as ShowJournalView from "PosApi/Extend/Views/ShowJournalView";
import { StringExtensions } from "PosApi/TypeExtensions";
import { StoreOperations } from "../../DataService/DataServiceRequests.g";
export default class PrintOnlineOrderReceiptCommand extends ShowJournalView.ShowJournalExtensionCommandBase {

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

        this.id = "PrintOnlineOrderReceiptCommand";
        this.label = "Print OnlineOrder Receipt";
        this.extraClass = "iconAccept";

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

    public recreateSalesReceiptsForSalesOrder(salesOrder: ProxyEntities.SalesOrder): Promise<ProxyEntities.Receipt[]> {
        let salesOrderId: string = StringExtensions.EMPTY;
        let queryBySalesId: boolean = false;

        if (!StringExtensions.isNullOrWhitespace(salesOrder.Id)
            && StringExtensions.compare(salesOrder.Id, salesOrder.SalesId, true) !== 0) {
            salesOrderId = salesOrder.Id;
        } else if (!StringExtensions.isNullOrWhitespace(salesOrder.SalesId)) {
            salesOrderId = salesOrder.SalesId;
            queryBySalesId = true;
        }

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
                    QueryBySalesId: queryBySalesId,
                    ReceiptTypeValue: ProxyEntities.ReceiptType.SalesReceipt,
                    HardwareProfileId: hardwareProfile.ProfileId
                };

                let getReceiptsRequest: GetReceiptsClientRequest<GetReceiptsClientResponse> = new GetReceiptsClientRequest(salesOrderId, criteria);
                return this.context.runtime.executeAsync(getReceiptsRequest);
            })
            .then((response: ClientEntities.ICancelableDataResult<GetReceiptsClientResponse>): ProxyEntities.Receipt[] => {
                return response.data.result;
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
        this.canExecute = true;
    }

    /**
     * Executes the command.
     */
    protected async execute(): Promise<void> {
        // await this.processByAsyncAwait();
        this.processByPromiseInSequence();
    }

    public processByPromiseInSequence(): void {
        this.isProcessing = true;

        let searchCriteria: ProxyEntities.TransactionSearchCriteria = {
            TransactionIds: ['HOUSTON-HOUSTON-42-1702992146072', 'HOUSTON-HOUSTON-42-1703923544442']
        };

        let request: StoreOperations.SearchJournalTransactionsWithUnPrintReceiptRequest<StoreOperations.SearchJournalTransactionsWithUnPrintReceiptResponse> =
            new StoreOperations.SearchJournalTransactionsWithUnPrintReceiptRequest(searchCriteria);

        this.context.runtime.executeAsync(request).then(
            (response: ClientEntities.ICancelableDataResult<StoreOperations.SearchJournalTransactionsWithUnPrintReceiptResponse>) => {
                let transactions: ProxyEntities.Transaction[] = response.data.result;

                let arrPromise: Promise<ClientEntities.ICancelable>[] = transactions.map((trans: ProxyEntities.Transaction): Promise<ClientEntities.ICancelable> => {
                    let req: GetSalesOrderDetailsByTransactionIdClientRequest<GetSalesOrderDetailsByTransactionIdClientResponse>
                        = new GetSalesOrderDetailsByTransactionIdClientRequest<GetSalesOrderDetailsByTransactionIdClientResponse>(trans.Id, ProxyEntities.SearchLocation.Local);

                    return this.context.runtime.executeAsync(req).then((res: ClientEntities.ICancelableDataResult<GetSalesOrderDetailsByTransactionIdClientResponse>): Promise<ProxyEntities.Receipt[]> => {
                        return this.recreateSalesReceiptsForSalesOrder(res.data.result);
                    }).then((recreatedReceipts: ProxyEntities.Receipt[]): Promise<ClientEntities.ICancelableDataResult<PrinterPrintResponse>> => {
                        let printRequest: PrinterPrintRequest<PrinterPrintResponse> = new PrinterPrintRequest(recreatedReceipts);
                        return this.context.runtime.executeAsync(printRequest);
                    }).then((): Promise<ClientEntities.ICancelable> => {
                        console.log(`transaction ${trans.Id} receipt printed is done`);
                        return Promise.resolve({ canceled: true });
                    });
                });

                arrPromise.reduce((_prev: Promise<ClientEntities.ICancelable>, _cur: Promise<ClientEntities.ICancelable>) => {
                    return _prev.then(() => _cur);
                }, Promise.resolve({ canceled: true }));
            }).then(() => {
                this.isProcessing = false;
            });
    }

    public async processByAsyncAwait(): Promise<void> {
        this.isProcessing = true;

        let searchCriteria: ProxyEntities.TransactionSearchCriteria = {
            TransactionIds: ['HOUSTON-HOUSTON-42-1702992146072', 'HOUSTON-HOUSTON-42-1703923544442']
        };

        let request: StoreOperations.SearchJournalTransactionsWithUnPrintReceiptRequest<StoreOperations.SearchJournalTransactionsWithUnPrintReceiptResponse> =
            new StoreOperations.SearchJournalTransactionsWithUnPrintReceiptRequest(searchCriteria);

        let response: ClientEntities.ICancelableDataResult<StoreOperations.SearchJournalTransactionsWithUnPrintReceiptResponse> = await this.context.runtime.executeAsync(request);
        let transactions: ProxyEntities.Transaction[] = response.data.result;

        transactions.forEach(async (trans: ProxyEntities.Transaction) => {
            try {
                let req: GetSalesOrderDetailsByTransactionIdClientRequest<GetSalesOrderDetailsByTransactionIdClientResponse>
                    = new GetSalesOrderDetailsByTransactionIdClientRequest<GetSalesOrderDetailsByTransactionIdClientResponse>(trans.Id, ProxyEntities.SearchLocation.Local);

                let res: ClientEntities.ICancelableDataResult<GetSalesOrderDetailsByTransactionIdClientResponse> = await this.context.runtime.executeAsync(req);

                let recreatedReceipts: ProxyEntities.Receipt[] = await this.recreateSalesReceiptsForSalesOrder(res.data.result);

                let printRequest: PrinterPrintRequest<PrinterPrintResponse> = new PrinterPrintRequest(recreatedReceipts);
                await this.context.runtime.executeAsync(printRequest);
            }
            finally {
                console.log(`transaction ${trans.Id} receipt printed is done`);
            }
        });

        this.isProcessing = false;
    }
}