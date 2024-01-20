import {
    ConcludeTransactionClientRequest,
    ConcludeTransactionClientResponse,
    CreateEmptyCartServiceRequest,
    CreateEmptyCartServiceResponse,
    RefreshCartClientRequest,
    RefreshCartClientResponse,
    VoidTransactionOperationRequest,
    VoidTransactionOperationResponse
} from "PosApi/Consume/Cart";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as ShowJournalView from "PosApi/Extend/Views/ShowJournalView";
import { StoreOperations } from "../../DataService/DataServiceRequests.g";


export default class ReturnTransByCodeCommand extends ShowJournalView.ShowJournalExtensionCommandBase {

    private _selectedJournal: ProxyEntities.SalesOrder;
    private _products: ProxyEntities.SimpleProduct[];
    private _customer: ProxyEntities.Customer;
    private _mode: ClientEntities.ShowJournalMode;

    /**
     * Creates a new instance of the DownloadDocumentCommand class.
     * @param {IExtensionCommandContext<Extensibility.IShowJournalToExtensionCommandMessageTypeMap>} context The command context.
     * @remarks The command context contains APIs through which a command can communicate with POS.
     */
    constructor(context: IExtensionCommandContext<ShowJournalView.IShowJournalToExtensionCommandMessageTypeMap>) {
        super(context);

        this.id = "downloadDocumentCommand";
        this.label = "Download document";
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
        console.log(this._customer);
        console.log(this._products);
        let correlationId: string = this.context.logger.getNewCorrelationId();

        if (this._selectedJournal.SalesLines.filter(i => !i.IsVoided && i.Quantity > i.ReturnQuantity).length > 0) {
            let createEmptyCartServiceRequest: CreateEmptyCartServiceRequest = new CreateEmptyCartServiceRequest(correlationId);
            this.context.runtime.executeAsync(createEmptyCartServiceRequest)
                .then((response: ClientEntities.ICancelableDataResult<CreateEmptyCartServiceResponse>): Promise<string> => {
                    return Promise.resolve(response.data.cart.Id);
                }).then((cartId: string): Promise<ClientEntities.ICancelableDataResult<StoreOperations.ReturnTransactionByApiAsyncResponse>> => {
                    let req: StoreOperations.ReturnTransactionByApiAsyncRequest<StoreOperations.ReturnTransactionByApiAsyncResponse> =
                        new StoreOperations.ReturnTransactionByApiAsyncRequest(this._selectedJournal.Id, cartId, "new_Unified" + this._selectedJournal.ReceiptId);
                    return this.context.runtime.executeAsync(req);
                }).then((response: ClientEntities.ICancelableDataResult<StoreOperations.ReturnTransactionByApiAsyncResponse>)
                    : Promise<ClientEntities.ICancelableDataResult<RefreshCartClientResponse>> => {
                    let request: RefreshCartClientRequest<RefreshCartClientResponse> = new RefreshCartClientRequest<RefreshCartClientResponse>();
                    return this.context.runtime.executeAsync(request);
                }).then((result: ClientEntities.ICancelableDataResult<RefreshCartClientResponse>)
                    : Promise<ClientEntities.ICancelableDataResult<ConcludeTransactionClientResponse>> => {
                    let req: ConcludeTransactionClientRequest<ConcludeTransactionClientResponse> = new ConcludeTransactionClientRequest(correlationId);
                    return this.context.runtime.executeAsync(req);
                }).then((response: ClientEntities.ICancelableDataResult<ConcludeTransactionClientResponse>): Promise<void> => {
                    this.isProcessing = false;
                    this.context.logger.logError(response.data.salesOrder.Id + "Checkout successfully");
                    return Promise.resolve();
                }).catch((reason: any) => {
                    let forceVoidTransactionRequest: VoidTransactionOperationRequest<VoidTransactionOperationResponse> =
                        new VoidTransactionOperationRequest<VoidTransactionOperationResponse>(false, correlationId);
                    this.context.runtime.executeAsync(forceVoidTransactionRequest);
                    //.then((value: ClientEntities.ICancelableDataResult<VoidTransactionOperationResponse>) => {     
                }).catch((err: any) => {
                    this.context.logger.logError(JSON.stringify(err));
                    console.log(err);
                    this.isProcessing = false;
                });
        } else {
            this.isProcessing = false;
        }
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
}