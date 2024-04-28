import { ICustomViewControllerBaseState, ICustomViewControllerContext, IExtensionViewControllerContext } from "PosApi/Create/Views";
import KnockoutExtensionViewModelBase from "../BaseClasses/KnockoutExtensionViewModelBase";
import { IContosoDenominationDetailViewModelOptions } from "../NavigationContracts";
import ko from "knockout";
import { ProxyEntities } from "PosApi/Entities";
import NumericInputDialog from "../../Create/Dialogs/NumericInputDialog";
import ContosoDenominationInputDialog from "./ContosoDenominationNumericDialog";
import { IContosoDenominationInputDialogResult, ContosoDenominationInputType } from "./ContosoDenominationNumericDialogTypes";
import ContosoDenominationAmountDialog from "./ContosoDenominationAmountDialog";


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

        let keyQuantityDeclared: string = 'QuantityDeclared_DenominationDetailView';
        let keyAmountDeclared: string = 'AmountDeclared_DenominationDetailView';

        if (localStorage.getItem(keyQuantityDeclared) !== null) {
            // alert(`QUANTITY column cliked, the clicked row ${item.QuantityDeclared}- ${item.DenominationAmount}`);
            localStorage.removeItem(keyQuantityDeclared);
            let contosoDenominationInputDialog: ContosoDenominationInputDialog = new ContosoDenominationInputDialog();
            let inputs: IContosoDenominationInputDialogResult = {
                item: item,
                inputType: "Quantity" 
            }
            return contosoDenominationInputDialog.open(inputs).then((result: IContosoDenominationInputDialogResult) => {
                if (!result.canceled) {
                    item.QuantityDeclared = result.item.QuantityDeclared;
                    item.AmountDeclared = result.item.AmountDeclared;
                }
            });
        } else if (localStorage.getItem(keyAmountDeclared) !== null) {
            // alert(`ToTAL column cliked, the clicked row ${item.AmountDeclared}- ${item.DenominationAmount}`);
            localStorage.removeItem(keyAmountDeclared);
            let contosoDenominationAmountDialog: ContosoDenominationAmountDialog = new ContosoDenominationAmountDialog();
            let inputs: IContosoDenominationInputDialogResult = {
                item: item,
                inputType: "Amount"
            }
            return contosoDenominationAmountDialog.open(inputs).then((result: IContosoDenominationInputDialogResult) => {
                if (!result.canceled) {
                    item.QuantityDeclared = result.item.QuantityDeclared;
                    item.AmountDeclared = result.item.AmountDeclared;
                }
            });
        }

        return Promise.resolve();

        //let numericInputDialog: NumericInputDialog = new NumericInputDialog();
        //return numericInputDialog.show(this.context, this.title)
        //    .then((result: string) => {
        //        this.dialogResult(result);
        //        item.AmountDeclared = parseFloat(result);;
        //        return Promise.resolve();
        //    }).catch((reason: any) => {
        //        this.context.logger.logError("NumericInputDialog: " + JSON.stringify(reason));
        //        return Promise.reject(reason);
        //    });
    }
}