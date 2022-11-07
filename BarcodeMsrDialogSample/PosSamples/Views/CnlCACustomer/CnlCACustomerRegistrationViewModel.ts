import { CustomViewControllerBase, ICustomViewControllerContext } from "PosApi/Create/Views";
import KnockoutExtensionViewModelBase from "../BaseClasses/KnockoutExtensionViewModelBase";
import ko from "knockout";


export class CnlCACustomerRegistrationViewModel extends KnockoutExtensionViewModelBase {

    //private context: ICustomViewControllerContext;

    public firstName: ko.Observable<string>;
    public lastName: ko.Observable<string>;

    constructor(context: ICustomViewControllerContext, state: CustomViewControllerBase) {
        super();

       // this.context = context;

        this.firstName = ko.observable("");
        this.lastName = ko.observable("");
    }

    public confirmAndAddCustomerRegistration() {
        var isNameValid = false;

        console.log(this.firstName(), this.lastName());

        isNameValid = !Commerce.StringExtensions.isNullOrWhitespace(this.firstName()) &&
            !Commerce.StringExtensions.isNullOrWhitespace(this.lastName());
        if (!isNameValid) {
            return;
        }
    }

    public isPerson(): boolean {
        return true;
    }

}