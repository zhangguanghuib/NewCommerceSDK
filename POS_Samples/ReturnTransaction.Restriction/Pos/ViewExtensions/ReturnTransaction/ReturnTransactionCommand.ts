import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as ReturnTransactionView from "PosApi/Extend/Views/ReturnTransactionView";
import { ProxyEntities } from "PosApi/Entities";
import { StringExtensions, ObjectExtensions } from "PosApi/TypeExtensions";
import MessageDialog from "../../Dialogs/MessageDialog";

export default class ReturnTransactionCommand extends ReturnTransactionView.ReturnTransactionExtensionCommandBase {

    private _salesLines: ProxyEntities.SalesLine[] = [];
    private _receiptNumber: string = StringExtensions.EMPTY;
    private _salesOrder: ProxyEntities.SalesOrder = null;

    private readonly receiveNow: string = "RETURNING NOW ";
    private multiSelected: boolean = false;
    private returnBtn: HTMLButtonElement; 

    constructor(context: IExtensionCommandContext<ReturnTransactionView.IReturnTransactionToExtensionCommandMessageTypeMap>) {
        super(context);
        this.id = "sampleReturnTransactionCommand";
        this.label = "Sample return transaction command";
        this.extraClass = "iconLightningBolt";
    }

    protected init(state: ReturnTransactionView.IReturnTransactionExtensionCommandState): void {
        this.isVisible = false;
        this.canExecute = true;

        this._receiptNumber = state.receiptNumber;
        this._salesOrder = state.salesOrder;

        this.returnBtn = document.getElementById("return") as HTMLButtonElement;

        this.transactionLineSelectedHandler = (data: ReturnTransactionView.ReturnTransactionTransactionLineSelectedData): void => {
            this._salesLines = data.salesLines;

            const receiveNowCtrls = document.querySelectorAll('#salesOrderLinesView div[aria-label*="RETURNING NOW"]');
            
            let newArray = Array.from(receiveNowCtrls).filter(item => {
                let ariaLabel = item["ariaLabel"].substring(this.receiveNow.length);
                let returnQty = parseFloat(ariaLabel);
                return returnQty > 0;
            });

            this.multiSelected = newArray.length >= 1 || this._salesLines.length > 1;
            this.returnBtn.style.visibility = this.multiSelected ? "hidden" : "visible";

            if (this.multiSelected) {
                MessageDialog.show(this.context,
                    "There are multiple lines selected, this is not allowed to be returned, please clear and only select one single line");
            }

            //if (newArray.length >= 1) {
            //    this.multiSelected = true;
            //    MessageDialog.show(this.context, "There are multiple lines selected");
            //    this.returnBtn.style.visibility = "hidden";
            //} else {
            //    this.multiSelected = false;
            //    this.returnBtn.style.visibility = "visible";
            //}
            //console.log(this.multiSelected);
        };

        this.transactionLineSelectionClearedHandler = () => {
            this._salesLines = [];
            this.returnBtn.style.visibility = "visible";
        };

        this.transactionSelectedHandler = (data: ReturnTransactionView.ReturnTransactionTransactionSelectedData): void => {
            this._salesOrder = data.salesOrder;
        };

        this.transactionSelectionClearedHandler = () => {
            this._salesOrder = null;
        };
    }

    protected execute(): void {
        let salesOrderId: string = !ObjectExtensions.isNullOrUndefined(this._salesOrder) ? this._salesOrder.Id : StringExtensions.EMPTY;
        let message: string = `Receipt number: ${this._receiptNumber}; Selected sales order: ${salesOrderId}; Return transaction selected data:`
            + this._salesLines.map((value: ProxyEntities.SalesLine) => { return ` ItemId: ${value.ItemId}`; });
        console.log(message);
    }
}