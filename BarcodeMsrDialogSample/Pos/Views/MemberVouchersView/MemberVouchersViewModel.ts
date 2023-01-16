import ko from "knockout";

import { Entities } from "./DataService/DataServiceEntities.g";
import { ICustomViewControllerBaseState, ICustomViewControllerContext } from "PosApi/Create/Views";
import KnockoutExtensionViewModelBase from "./BaseClasses/KnockoutExtensionViewModelBase";
import { IMemberVouchersExtensionViewModelOptions } from "./NavigationContracts";
//import { ProxyEntities, ClientEntities } from "PosApi/Entities";
//import { MemberVoucher } from "../DataService/DataServiceRequests.g";
//import { StringExtensions } from "PosApi/TypeExtensions";
//import {
//    GetCurrentCartClientRequest, GetCurrentCartClientResponse,
//    RefreshCartClientRequest, RefreshCartClientResponse
//    //SetCustomerOnCartOperationRequest, SetCustomerOnCartOperationResponse,
//    //SaveAttributesOnCartClientRequest, SaveAttributesOnCartClientResponse,
//    //SaveExtensionPropertiesOnCartClientRequest, SaveExtensionPropertiesOnCartClientResponse
//} from "PosApi/Consume/Cart";
//import MessageDialog from "../Controls/Dialog/MessageDialog";
//import CRMDataHelper from "../Helper/CRMDataHelper";
//import { Entities } from "../DataService/DataServiceEntities.g";
//import ConfirmDialogModule from "../Controls/Dialog/ConfirmDialogModule";
//import { IConfirmDialogResult } from "../Controls/Dialog/IConfirmDialogResult";

export default class MemberVouchersViewModel extends KnockoutExtensionViewModelBase {
    public title: ko.Observable<string>;
    public isBusy: ko.Observable<boolean>;
    public currentMemberVouchers: ko.ObservableArray<Entities.MemberVoucher>;
    public selectedMemberVouchers: ko.ObservableArray<Entities.MemberVoucher>;
    //private _context: IExtensionViewControllerContext;

    private _context: ICustomViewControllerContext;
    private _customViewControllerBaseState: ICustomViewControllerBaseState;

    //constructor(context: IExtensionViewControllerContext, options?: IMemberVouchersExtensionViewModelOptions) {
    constructor(context: ICustomViewControllerContext, state: ICustomViewControllerBaseState, options?: IMemberVouchersExtensionViewModelOptions) {
        super();

        this._context = context;
        this.title = ko.observable(context.resources.getString("string_50005") + " Membership No: " + options.MemberNumber);
        this.isBusy = ko.observable(true);

        this._customViewControllerBaseState = state;
        this._customViewControllerBaseState.isProcessing = true;

        this.currentMemberVouchers = ko.observableArray([]);
        this.selectedMemberVouchers = ko.observableArray([]);

        // get member vouchers
        //Remove After Test
        //StoreId = "LHQS";

        //this._context.runtime.executeAsync(
        //    new MemberVoucher.GetMemberVouchersActionRequest(options.CardNo, 0, "", "", "", options.TransactionId)).then((result) => {
        //        this.currentMemberVouchers(result.data.result);
        //    });

        let memberVoucher: Entities.MemberVoucher = new Entities.MemberVoucher();
        memberVoucher.Id = 111;
        memberVoucher.VoucherNumber = "VoucherNumber";
        memberVoucher.VoucherCode = "Voucher code";
        memberVoucher.VoucherName = "Voucher Name";
        memberVoucher.ValidFrom = new Date();
        memberVoucher.ExpiryDate = new Date();

        this.currentMemberVouchers().push(memberVoucher);

       // this.isBusy(false);
        this._customViewControllerBaseState.isProcessing = false;
    }

    /**
     * Handler for list item selection.
     * @param {any} item
     */
    public applyVoucher(): void {
        //var MemberVouchersResult: Entities.MemberVoucher[] = [];

        //let confirmMessage: string = StringExtensions.format("Do you want to apply selected vouchers? {0} ", this.selectedMemberVouchers().map(v => v.VoucherNumber).join(","));
        //let dialog: ConfirmDialogModule = new ConfirmDialogModule(confirmMessage);

        //dialog.open(confirmMessage, "Apply Voucher").then((result: IConfirmDialogResult) => {

        //    if (result.selectedValue === "Yes") {
        //        //var ValidVouchers: Commerce.Proxy.Entities.MemberVoucher[] = [];
        //        //var cartRefreshNeeded: boolean = false;
        //        let cardNo: string = StringExtensions.EMPTY;
        //        let memberId: string = StringExtensions.EMPTY;
        //        let correlationId = this._context.logger.getNewCorrelationId();
        //        let helper: CRMDataHelper = new CRMDataHelper();
        //        let getCurrentCartClientRequest: GetCurrentCartClientRequest<GetCurrentCartClientResponse> = new GetCurrentCartClientRequest(correlationId);
        //        this._context.runtime.executeAsync(getCurrentCartClientRequest)
        //            .then((getCurrentCartClientResponse: ClientEntities.ICancelableDataResult<GetCurrentCartClientResponse>) => {

        //                if (getCurrentCartClientResponse.canceled) {
        //                    return Promise.resolve(<ClientEntities.ICancelableDataResult<GetCurrentCartClientResponse>>{ canceled: true, data: null });
        //                }
        //                let cart: ProxyEntities.Cart = getCurrentCartClientResponse.data.result;
        //                let desiredProperties: ProxyEntities.CommerceProperty[] = cart.ExtensionProperties.filter(
        //                    (value: ProxyEntities.CommerceProperty): boolean => {
        //                        return value.Key === "CRMCardNumber";
        //                    }
        //                );
        //                cardNo = desiredProperties.length > 0 ? desiredProperties[0].Value.StringValue : StringExtensions.EMPTY;

        //                let desiredProperties1: ProxyEntities.CommerceProperty[] = cart.ExtensionProperties.filter(
        //                    (value: ProxyEntities.CommerceProperty): boolean => {
        //                        return value.Key === "CRMMemberNumber";
        //                    }
        //                );
        //                memberId = desiredProperties1.length > 0 ? desiredProperties1[0].Value.StringValue : StringExtensions.EMPTY;


        //                var ChannelId: number = 0;
        //                var DataAreaId: string = "";
        //                var StoreId: string = "";
        //                var TerminalId: string = "";
        //                var TransactionId: string = cart.Id;

        //                var utilizeVoucherArray: Entities.UtilizeVoucherCriteria[] = [];
        //                utilizeVoucherArray = [];

        //                for (var i = 0; i < this.selectedMemberVouchers().length; i++) {
        //                    let mv: Entities.MemberVoucher = this.selectedMemberVouchers()[i];
        //                    var date = new Date();
        //                    var memberIdentifier = memberId + ":" + cardNo;
        //                    var utilizeVoucherOpt: Entities.UtilizeVoucherCriteria = {
        //                        Id: 0,
        //                        TransactDate: "\/Date(" + date.getTime() + ")\/",
        //                        TransactTime: date.getHours() + ":" + date.getMinutes() + ":" + date.getSeconds(),
        //                        MemberNumber: memberIdentifier,
        //                        VoucherCode: mv.VoucherCode,
        //                        VoucherNumber: mv.VoucherNumber,
        //                        VoucherName: mv.VoucherName,
        //                        DataLevelValue: mv.DenominationValue
        //                    };
        //                    utilizeVoucherArray.push(utilizeVoucherOpt);
        //                }


        //                var voucherCoupons: string[] = [];
        //                let voucherNumbers: string = StringExtensions.EMPTY;
        //                var i: number = 0;
        //                this._context.runtime.executeAsync(new MemberVoucher.UtilizeVoucherActionRequest(utilizeVoucherArray, ChannelId, DataAreaId, StoreId, TerminalId, TransactionId))
        //                    .then((result1) => {
        //                        let res: MemberVoucher.UtilizeVoucherActionResponse = result1.data;
        //                        MemberVouchersResult = res.result; //Filter vouchers with only utilized vouchers
        //                        utilizeVoucherArray.filter(r => MemberVouchersResult.some(({ VoucherNumber }) => r.VoucherNumber === VoucherNumber)).forEach((uv: Entities.UtilizeVoucherCriteria) => {
        //                            voucherCoupons[i] = uv.VoucherCode;

        //                            i = i + 1;
        //                            voucherNumbers = voucherNumbers + uv.VoucherNumber + ",";
        //                            //return this.cartManager.addCouponsToCartAsync(voucherCoupons, false);
        //                        });

        //                        //key1 = "UTILIZEVOUCHER";
        //                        //let utilizeVoucherProperty: ProxyEntities.CommerceProperty = {
        //                        //    Key: key1,
        //                        //    Value: <ProxyEntities.CommercePropertyValue>{ StringValue: StringExtensions.EMPTY }
        //                        //};
        //                        return helper.saveDataToCart(this._context, "UTILIZEVOUCHER", voucherNumbers, correlationId, cart).then(() => {

        //                            let refreshCartClientRequest: RefreshCartClientRequest<RefreshCartClientResponse> =
        //                                new RefreshCartClientRequest<RefreshCartClientResponse>(correlationId);
        //                            this._context.runtime.executeAsync(refreshCartClientRequest)
        //                                .then((refreshCartClientResponse: ClientEntities.ICancelableDataResult<RefreshCartClientResponse>) => {
        //                                    return MessageDialog.show(this._context, "Apply Member Vouchers", true, true, false, "Vouchers have been processed").then(() => {
        //                                        this._context.navigator.navigateToPOSView("CartView");
        //                                    });
        //                                });
        //                        });

        //                    }).catch((reason: any) => {
        //                        // Error will be thrown if total cart amount is negative already.
        //                        return MessageDialog.show(this._context, "Apply Voucher Error", true, true, false, reason).then(() => {
        //                            this._context.logger.logInformational("Apply Voucher Error Dialog closed");
        //                        });
        //                    });

        //                return Promise.resolve(getCurrentCartClientResponse);
        //            });
        //    }
        //});
    }

    public cancelVoucher(): void {
        this._context.navigator.navigateToPOSView("CartView");
    }


}