import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as ShowJournalView from "PosApi/Extend/Views/ShowJournalView";

export default class CoinDispenserCommand extends ShowJournalView.ShowJournalExtensionCommandBase {

    constructor(context: IExtensionCommandContext<ShowJournalView.IShowJournalToExtensionCommandMessageTypeMap>) {
        super(context);

        this.id = "coinDispenserCommand";
        this.label = "Coin Dispenser";
        this.extraClass = "iconInvoice";
        this.isVisible = true;
        this.canExecute = true;
    }

    protected init(state: ShowJournalView.IShowJournalExtensionCommandState): void {
    }

    protected execute(): void {
        this.isProcessing = true;
        this.context.navigator.navigate("ExampleView");
        this.isProcessing = false;
    }

}