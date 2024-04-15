import { ICustomViewControllerBaseState, ICustomViewControllerContext, IExtensionViewControllerContext } from "PosApi/Create/Views";
import KnockoutExtensionViewModelBase from "../BaseClasses/KnockoutExtensionViewModelBase";
import { IContosoDenominationDetailViewModelOptions } from "../NavigationContracts";
import ko from "knockout";
import { ProxyEntities } from "PosApi/Entities";


export default class ContosoDenominationDetailViewModel extends KnockoutExtensionViewModelBase { 

    public title: string;
    private _context: ICustomViewControllerContext;
    public denominationDetailLines: ko.ObservableArray<ProxyEntities.DenominationDetail>;


    constructor(context: ICustomViewControllerContext, state: ICustomViewControllerBaseState,
        options?: IContosoDenominationDetailViewModelOptions) {

        super();
        this._context = context;
        this.title = options?.title;
        this.denominationDetailLines = ko.observableArray([].concat(options.denominations));
    }
}