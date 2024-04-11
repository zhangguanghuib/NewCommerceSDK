import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as JournalView  from "PosApi/Extend/Views/ShowJournalView";
//import { ICustomColumnsContext } from "PosApi/Extend/Views/CustomListColumns";
//import { DateFormatter, TransactionTypeFormatter, CurrencyFormatter } from "PosApi/Consume/Formatters";
//import { ProxyEntities } from "PosApi/Entities";

export default class DisableEmailReceiptCommand extends JournalView.ShowJournalExtensionCommandBase {

    public constructor(context: IExtensionCommandContext<JournalView.IShowJournalToExtensionCommandMessageTypeMap>) {
        super(context);
        this.id = "navigateToSimpleExtensionViewCommand";
        this.label = "Navigate to Samples View";
        this.extraClass = "iconGo";
        this.isVisible = false;
    }

    protected init(state: JournalView.IShowJournalExtensionCommandState): void {
        const buttons = document.querySelectorAll('#HomeView_showEmailReceiptsMenuCommand');

        buttons.forEach((button: Element) => {
            let btnHtml: HTMLButtonElement = button as HTMLButtonElement;
            btnHtml.style.display = 'none';
        });
    }

    protected execute(): void {
    }

}