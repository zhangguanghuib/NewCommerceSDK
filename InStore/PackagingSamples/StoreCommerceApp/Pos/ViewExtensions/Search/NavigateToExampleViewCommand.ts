import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as SearchView from "PosApi/Extend/Views/SearchView";

export default class NavigateToExampleViewCommand extends SearchView.ProductSearchExtensionCommandBase {
    /**
     * Creates a new instance of the NavigateToExampleViewCommand class.
     * @param {IExtensionCommandContext<ProductDetailsView.IProductSearchToExtensionCommandMessageTypeMap>} context The command context.
     * @remarks The command context contains APIs through which a command can communicate with POS.
     */
    constructor(context: IExtensionCommandContext<SearchView.IProductSearchToExtensionCommandMessageTypeMap>) {
        super(context);

        this.id = "navigateToExampleViewCommand";
        this.label = "Navigate to Full System Example View";
        this.extraClass = "iconGo";
    }

    /**
     * Initializes the command.
     * @param {ProductDetailsView.IProductDetailsExtensionCommandState} state The state used to initialize the command.
     */
    protected init(state: SearchView.IProductSearchExtensionCommandState): void {
        this.canExecute = true;
        this.isVisible = true;
    }

    /**
     * Executes the command.
     */
    protected execute(): void {
        this.context.navigator.navigate("ExampleView");
    }
}