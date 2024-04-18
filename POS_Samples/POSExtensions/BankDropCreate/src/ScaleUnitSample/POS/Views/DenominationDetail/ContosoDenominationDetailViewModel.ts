import { ICustomViewControllerBaseState, ICustomViewControllerContext, IExtensionViewControllerContext } from "PosApi/Create/Views";
import KnockoutExtensionViewModelBase from "../BaseClasses/KnockoutExtensionViewModelBase";
import { IContosoDenominationDetailViewModelOptions } from "../NavigationContracts";
import ko from "knockout";
import { ProxyEntities } from "PosApi/Entities";
import NumericInputDialog from "../../Create/Dialogs/NumericInputDialog";


export default class ContosoDenominationDetailViewModel extends KnockoutExtensionViewModelBase { 

    public title: string;
    private context: ICustomViewControllerContext;
    public denominationDetailLines: ko.ObservableArray<ProxyEntities.DenominationDetail>;
    public dialogResult: ko.Observable<string>;



    constructor(context: ICustomViewControllerContext, state: ICustomViewControllerBaseState,
        options?: IContosoDenominationDetailViewModelOptions) {

        super();
        this.context = context;
        this.title = options?.title;
        this.denominationDetailLines = ko.observableArray([].concat(options.denominations));
        this.dialogResult = ko.observable("");

    }

    public listItemSelected(item: ProxyEntities.DenominationDetail): Promise<void> {

        let numericInputDialog: NumericInputDialog = new NumericInputDialog();
        return numericInputDialog.show(this.context, this.title)
            .then((result: string) => {
                this.dialogResult(result);
                item.AmountDeclared = parseFloat(result);;
                return Promise.resolve();
            }).catch((reason: any) => {
                this.context.logger.logError("NumericInputDialog: " + JSON.stringify(reason));
                return Promise.reject(reason);
            });
    }
}