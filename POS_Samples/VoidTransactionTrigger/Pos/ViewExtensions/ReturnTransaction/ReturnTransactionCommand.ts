import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as ReturnTransactionView from "PosApi/Extend/Views/ReturnTransactionView";
import { ProxyEntities } from "PosApi/Entities";
import { StringExtensions, ObjectExtensions } from "PosApi/TypeExtensions";

export default class ReturnTransactionCommand extends ReturnTransactionView.ReturnTransactionExtensionCommandBase {

    private _salesLines: ProxyEntities.SalesLine[] = [];
    private _receiptNumber: string = StringExtensions.EMPTY;
    private _salesOrder: ProxyEntities.SalesOrder = null;

    /**
     * Creates a new instance of the ReturnTransactionCommand class.
     * @param {IExtensionCommandContext<ReturnTransactionView.IReturnTransactionToExtensionCommandMessageTypeMap>} context The context.
     * @remarks The command context contains APIs through which a command can communicate with POS.
     */
    constructor(context: IExtensionCommandContext<ReturnTransactionView.IReturnTransactionToExtensionCommandMessageTypeMap>) {
        super(context);

        this.id = "sampleReturnTransactionCommand";
        this.label = "Sample return transaction command";
        this.extraClass = "iconLightningBolt";
    }

    /**
     * Initializes the command.
     * @param {ReturnTransactionView.IReturnTransactionExtensionCommandState} state The state used to initialize the command.
     */
    protected init(state: ReturnTransactionView.IReturnTransactionExtensionCommandState): void {
        this.isVisible = true;
        this.canExecute = true;

        this._receiptNumber = state.receiptNumber;
        this._salesOrder = state.salesOrder;

        this.transactionLineSelectedHandler = (data: ReturnTransactionView.ReturnTransactionTransactionLineSelectedData): void => {
            this._salesLines = data.salesLines;
            console.log(`there are ${this._salesLines.length} lines got selected`);
            this.toggleReturnBtn();

        };

        this.transactionLineSelectionClearedHandler = () => {
            this._salesLines = [];
            this.toggleReturnBtn();
        };

        this.transactionSelectedHandler = (data: ReturnTransactionView.ReturnTransactionTransactionSelectedData): void => {
            this._salesOrder = data.salesOrder;
        };

        this.transactionSelectionClearedHandler = () => {
            this._salesOrder = null;
        };
    }

    private toggleReturnBtn(): void {
        let returnBtn: HTMLButtonElement = document.getElementById('return') as HTMLButtonElement;
        if (this._salesLines.length > 1) {
            // returnBtn.setAttribute("disabled", "disabled");
            returnBtn.style.visibility = "hidden";
        } else {
            //returnBtn.removeAttribute("disabled");
            returnBtn.style.visibility = "visible";
        }
    }


    /**
     * Executes the command.
     */
    protected execute(): void {
        let salesOrderId: string = !ObjectExtensions.isNullOrUndefined(this._salesOrder) ? this._salesOrder.Id : StringExtensions.EMPTY;
        let message: string = `Receipt number: ${this._receiptNumber}; Selected sales order: ${salesOrderId}; Return transaction selected data:`
            + this._salesLines.map((value: ProxyEntities.SalesLine) => { return ` ItemId: ${value.ItemId}`; });
        console.log(message);
    }
}