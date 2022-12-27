import ko from "knockout";

import { Entities } from "./DataService/DataServiceEntities.g";
//import { WTR_CRMResponse } from "./DataService/DataServiceRequests.g";
import { IExtensionViewControllerContext } from "PosApi/Create/Views";
import KnockoutExtensionViewModelBase from "./BaseClasses/KnockoutExtensionViewModelBase";
import { IMemberSignUpExtensionViewModelOptions } from "./NavigationContracts";
//import { ClientEntities } from "PosApi/Entities";
import { IValueCaptionPair } from "./CustomEntities/CustomEntities";
import { StringExtensions } from "PosApi/TypeExtensions";
import MessageDialog from "./../Controls/Dialog/MessageDialog";


export default class MemberSignUpViewModel extends KnockoutExtensionViewModelBase {
    public title: ko.Observable<string>;
    public memberProxy: ko.Observable<Entities.Member>;
    public mode: ko.Observable<number>;
    public canSave: ko.Computed<boolean>;
    public addressFilled: ko.Computed<boolean>;
    public allowChangeCustomerType: ko.Observable<boolean>;
    public customerIsPerson: ko.Computed<boolean>;
    public isBusy: ko.Observable<boolean>;
    //public mobile: Observable<string>;
    //public countryCode: Observable<string>;
    //public email: Observable<string>;
    //public firstName: Observable<string>;
    //public lastName: Observable<string>;
    //public gender: Observable<string>;
    //public viewTitle: Computed<string>;
    public genderList: ko.ObservableArray<IValueCaptionPair>;
    // Internal State.
    private _context: IExtensionViewControllerContext;

    constructor(context: IExtensionViewControllerContext, options?: IMemberSignUpExtensionViewModelOptions) {
        super();
        this._context = context;
        //this.title = ko.observable("Member sign up");
        this.memberProxy = ko.observable(new Entities.Member());
        this.memberProxy().CountryCode = "65";
        this.memberProxy().Mobile = "";
        this.isBusy = ko.observable(false);
        this.canSave = ko.computed(() => {
            return this._canSave();
        });

        this.genderList = ko.observableArray<IValueCaptionPair>(this.getGenderList());
    }

    private getGenderList(): IValueCaptionPair[] {
        var _list: any[] = new Array();

        _list[0] = <IValueCaptionPair>{ value: 0, caption: "Female" };
        _list[1] = <IValueCaptionPair>{ value: 1, caption: "Male" };

        return _list;
    }

    /**
     * Add new member.
     */
    public createMember(): void {
        //this.isBusy(true);
        var newMember: Entities.Member = this.memberProxy();

        if (this.validateMemberDetails(newMember)) {
            newMember.MemberID = "0";
            if (newMember.CountryCode.substr(0, 1) != "+")
                newMember.CountryCode = "+" + newMember.CountryCode;
            newMember.Gender = newMember.Gender.charAt(0);

            this.isBusy(true);
            //this._context.runtime.executeAsync(new WTR_CRMResponse.CreateMemberActionRequest(newMember))
            //    .then((CRMResponse: ClientEntities.ICancelableDataResult<WTR_CRMResponse.CreateMemberActionResponse>) => {
            //        if (CRMResponse.data.result[0].ReturnStatus === 1) {
            //            // navigate to customer search view page
            //            return MessageDialog.show(this._context, "CRM Notification", true, true, true, this._context.resources.getString("string_50270")).then(() => {
            //                let options: ClientEntities.SearchNavigationParameters = new ClientEntities.SearchNavigationParameters(
            //                    ClientEntities.SearchViewSearchEntity.Customer, newMember.Mobile.toString());
            //                this._context.navigator.navigateToPOSView("SearchView", options);
            //            }).catch((reason: any) => {
            //                return Promise.reject(reason);
            //            });
            //        }
            //        else {
            //            return MessageDialog.show(this._context, "CRM Error", true, true, true, CRMResponse.data.result[0].ReturnMessage).then(() => {

            //            });
            //        }
            //    }).catch((errors: any) => {
            //        this._context.logger.logError(JSON.stringify(errors, ['_externalLocalizedErrorMessage']));
            //        return MessageDialog.show(this._context, "CRM Error", true, true, true, JSON.stringify(errors, ['_externalLocalizedErrorMessage'])).then(() => {
            //            return Promise.reject(errors);
            //        });
            //    });

            this.isBusy(false);

        }
    }

    private validateMemberDetails(newMember: Entities.Member): boolean {
        var isValid = true;
        if ((newMember.CountryCode === "65" || newMember.CountryCode === "+65") && newMember.Mobile.length != 8) {
            isValid = false;
            MessageDialog.show(this._context, "Validation Error", true, true, true, "Mobile number should be 8 digits for country code " + newMember.CountryCode).then(() => {
                // return Commerce.AsyncResult.createRejected();
            }).catch(() => {
                // return Commerce.AsyncResult.createRejected();
            });
            return isValid;
        }

        if (StringExtensions.isNullOrWhitespace(newMember.FirstName) || StringExtensions.isNullOrWhitespace(newMember.LastName)) {
            isValid = false;
            MessageDialog.show(this._context, "Validation Error", true, true, true, "First name and Last name should not be empty").then(() => {
                // return Commerce.AsyncResult.createRejected();
            }).catch(() => {
                // return Commerce.AsyncResult.createRejected();
            });
            return isValid;
        }

        if (StringExtensions.isNullOrWhitespace(newMember.Mobile) && newMember.CountryCode != "+65") {
            isValid = false;
            MessageDialog.show(this._context, "Validation Error", true, true, true, "Mobile cannot be empty for overseas number").then(() => {
                // return Commerce.AsyncResult.createRejected();
            }).catch(() => {
                // return Commerce.AsyncResult.createRejected();
            });
            return isValid;
        }

        if (!StringExtensions.isNullOrWhitespace(newMember.Email) && !this.validateEmail(newMember.Email)) {
            isValid = false;
            MessageDialog.show(this._context, "Validation Error", true, true, true, "Please check email id format").then(() => {
                // return Commerce.AsyncResult.createRejected();
            }).catch(() => {
                // return Commerce.AsyncResult.createRejected();
            });
            return isValid;
        }

        if (StringExtensions.isNullOrWhitespace(newMember.CountryCode)) {
            isValid = false;
            MessageDialog.show(this._context, "Validation Error", true, true, true, 'Country code cannot be empty' + newMember.CountryCode).then(() => {
                // return Commerce.AsyncResult.createRejected();
            }).catch(() => {
                // return Commerce.AsyncResult.createRejected();
            });

        }

        return isValid;
    }

    private _canSave(): boolean {

        var isNameValid: boolean = false;
        isNameValid = !StringExtensions.isNullOrWhitespace(this.memberProxy().FirstName) && !StringExtensions.isNullOrWhitespace(this.memberProxy().LastName);

        var result: boolean = isNameValid;
        return result;
    }


    public doNothing(): void {
        return;
    }

    public onNavigateBack(calledFromView: boolean): boolean {
        var newMember: Entities.Member = this.memberProxy();
        if (calledFromView) {
            if (this.validateMemberDetails(newMember)) {
                return true;
            }
            else {
                return false;
            }
        }
        return false;
    }

    public validateEmail(email: string): boolean {
        var regex: RegExp = new RegExp("^.+@.+\\..+$");
        return regex.test(email);
    }
}
