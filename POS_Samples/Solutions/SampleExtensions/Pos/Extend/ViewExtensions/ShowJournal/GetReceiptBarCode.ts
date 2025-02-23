import * as ShowJournalView from "PosApi/Extend/Views/ShowJournalView";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import { StringExtensions } from "PosApi/TypeExtensions";
import { StoreOperations } from "../../../DataService/DataServiceRequests.g";

export default class GetReceiptBarCode extends ShowJournalView.ShowJournalExtensionCommandBase {
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

        this.id = "GetReceiptBarCode";
        this.label = "Get Receipt BarCode";
        this.extraClass = "iconInvoice";
        this.isVisible = true;

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
            this.canExecute = true;
        };

        this.receiptSelectionClearedHandler = (): void => {
            this.canExecute = false;
        };

        this.journalTransactionsLoadedHandler = (data: ShowJournalView.ShowJournalJournalTransactionsLoadedData): void => {
            // this.isVisible = this._mode === ClientEntities.ShowJournalMode.ShowJournal;
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
        setTimeout((): void => {
            this.context.navigator.navigate("VoidCartLineView", this._selectedJournal);
            this.isProcessing = false;
        }, 0);

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
            const showJournalCustomerPanel: HTMLDivElement = document.getElementById('ShowJournalCustomerPanel') as HTMLDivElement;
            const showJournalTotalsPanel: HTMLDivElement = document.getElementById('showJournalTotalsPanel') as HTMLDivElement;
            this.getBarcode(showJournalCustomerPanel, showJournalTotalsPanel).then(() => { Promise.resolve() });
        }
    }

    public getBarcode(showJournalCustomerPanel: HTMLDivElement, showJournalTotalsPanel: HTMLDivElement): Promise<void> {

        this.isProcessing = true;

        let GetBarcodeRequest: StoreOperations.GetBarcodeBase64StringRequest<StoreOperations.GetBarcodeBase64StringResponse>
            = new StoreOperations.GetBarcodeBase64StringRequest(this._selectedJournal.ReceiptId);

        return this.context.runtime.executeAsync(GetBarcodeRequest)
            .then((result: ClientEntities.ICancelableDataResult<StoreOperations.GetBarcodeBase64StringResponse>): Promise<void> => {
                if (!result.canceled) {
                    //
                    const newDiv: HTMLDivElement = document.createElement("div") as HTMLDivElement;
                    newDiv.id = "showJournalCustomerPanel_ReceiptBarCode";
                    newDiv.classList.add("height60", "width280");
                    const imgElement = document.createElement("img") as HTMLImageElement;
                    imgElement.classList.add("height60", "width280");
                    imgElement.src = `data:image/png;base64,${result.data.result}`; // Set the src to base64 data
                    newDiv.appendChild(imgElement);
                    showJournalCustomerPanel.appendChild(newDiv);  

                    //
                    const newDiv02: HTMLDivElement = document.createElement("div") as HTMLDivElement;
                    newDiv02.id = "showJournalTotalsPanel_ReceiptBarCode";
                    newDiv02.classList.add("height60", "width280");
                    const imgElement02 = document.createElement("img") as HTMLImageElement;
                    imgElement02.classList.add("height60", "width280");
                    imgElement02.src = `data:image/png;base64,${result.data.result}`; // Set the src to base64 data
                    newDiv02.appendChild(imgElement02);
                    const totalField: HTMLDivElement = document.getElementById('TotalField') as HTMLDivElement;

                    if (totalField && totalField.parentNode) {
                        totalField.parentNode.insertBefore(newDiv02, totalField.nextSibling);
                    }
                    //showJournalTotalsPanel.appendChild(newDiv);  
                    console.log(showJournalTotalsPanel);
                }
                this.isProcessing = false;
                return Promise.resolve();
            }).catch((reason: any) => {
                this.isProcessing = false;
            });
    }
}