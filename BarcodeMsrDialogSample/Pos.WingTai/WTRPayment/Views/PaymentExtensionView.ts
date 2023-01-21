import ko from "knockout";
import * as NewView from "PosApi/Create/Views";
import KnockoutExtensionViewControllerBase from "./BaseClasses/KnockoutExtensionViewControllerBase";
import { PaymentExtensionViewModel } from "./PaymentExtensionViewModel";
//import { Loader, ILoaderState } from "PosUISdk/Controls/Loader";
import { IPaymentExtensionViewModelOptions } from "./NavigationContracts";
//import { HeaderSplitView, IHeaderSplitViewState } from "PosUISdk/Controls/HeaderSplitView";
import { ProxyEntities, ClientEntities } from "PosApi/Entities";
import { CurrencyFormatter } from "PosApi/Consume/Formatters";
import { ListData } from "../Controls/DialogSample/ListInputDialog";
import ListInputDialog from "../Controls/DialogSample/ListInputDialog";
import { GetPaymentCardTypeByBinRangeClientRequest, GetPaymentCardTypeByBinRangeClientResponse } from "PosApi/Consume/Payments";
import {GetHardwareProfileClientRequest, GetHardwareProfileClientResponse} from "PosApi/Consume/Device";
import { StringExtensions } from "PosApi/TypeExtensions";
import { WTR_PaymentTerminalEx, WTR_PaymentTerminalType, IWTR_PaymentTerminalEntity } from "./../Peripherals/IPaymentTerminal";
import MessageDialog from "../Controls/DialogSample/MessageDialog";


export default class PaymentExtensionView extends KnockoutExtensionViewControllerBase<PaymentExtensionViewModel> {
    public viewModel: PaymentExtensionViewModel;
    //public headerSplitView: HeaderSplitView;
    //public loader: Loader;

    public isLast4DigitsRequired: ko.Observable<boolean>;
    public isNetsPayment: boolean;
    public userEnteredQuantity: ko.Observable<string>;
    public userEnteredPreDefinedValue: ko.Observable<string>;
    public hasPreDefinedValue: ko.Observable<boolean>;
    public maxAmountTenderedCurrency: ko.Observable<string>;
    public userEnteredRemarks: ko.Observable<string>;
    public numpadHeaderLabel: ko.Observable<string>;
    public isQuantityFocus: ko.Observable<boolean>;
    public tenderedAmount: ko.Observable<number>;
    public WTR_cardTypeId: string;
    public WTR_cardType: WTR_PaymentTerminalEx;
    public WTR_paymTerminalType: WTR_PaymentTerminalType;
    public WTR_COMPort: string;
    public paymentTerminalResponse: IWTR_PaymentTerminalEntity;
    public respayTerminalResponse: any;
    public WTR_IsManualEntered: ko.Observable<boolean>;
    public WTR_switchManual: ko.Observable<boolean>;
    public WTR_ManualEntCardNumber: ko.Observable<string>;
    public WTR_ManualEntLast4Digits: ko.Observable<string>;
    public WTR_ManualEntApprovalCode: ko.Observable<string>;
    public WTR_ManualCardTypeInfo: ProxyEntities.CardTypeInfo;
    public tenderType: string;
    constructor(context: NewView.IExtensionViewControllerContext, options?: IPaymentExtensionViewModelOptions) {
        // Do not save in history
        super(context, false);
        this.tenderedAmount = ko.observable(0);
        this.numpadHeaderLabel = ko.observable(context.resources.getString("necstring_0"));
        this.WTR_ManualEntApprovalCode = ko.observable("");
        this.WTR_ManualEntLast4Digits = ko.observable("");
        this.userEnteredPreDefinedValue = ko.observable("");
        this.maxAmountTenderedCurrency = ko.observable(CurrencyFormatter.toCurrency(0));
        this.WTR_switchManual = ko.observable(false);
        // Initialize the view model.
        this.viewModel = new PaymentExtensionViewModel(context, options);

        //// Set the header
        //let loaderState: ILoaderState = {
        //    visible: this.viewModel.isBusy
        //};

        //this.loader = new Loader(loaderState);
        this.tenderType = options.tenderType.TenderTypeId;
        this.isNetsPayment = this.tenderType == WTR_PaymentTerminalEx.NETS.toString() ? true : false;
        this.isLast4DigitsRequired = ko.observable(this.isNetsPayment);
        // Initialize the POS SDK Controls.
        //let headerSplitViewState: IHeaderSplitViewState = {
        //    title: this.viewModel.title
        //};

        //this.headerSplitView = new HeaderSplitView(headerSplitViewState);
        this.setFullAmountDue();

        this.WTR_IsManualEntered = ko.observable(false);

    }


    public onReady(element: HTMLElement): void {
        super.onReady(element);
    }

    public setFullAmountDue(): void {
        let fullAmount: string = this.viewModel.fullAmount().toString();
        this.viewModel.numPadAmountText(fullAmount);
        this.tenderedAmount(this.viewModel.fullAmount());
        this.maxAmountTenderedCurrency(CurrencyFormatter.toCurrency(this.viewModel.fullAmount()));
    }

    public setMaxAmountDue(): void {
        this.viewModel.numPadAmountText(this.viewModel.maxAmount().toString());
    }

    public beginProcessingPayment(tenderType:string,cardType:string): void {

        this.viewModel.beginProcessingManualCardPayment(tenderType,Number(this.tenderedAmount().toString().replace(",", "")),
            cardType, this.WTR_ManualEntApprovalCode(), this.WTR_ManualEntLast4Digits());
    }

    public doAmountCalculation(resultValue: any): void {

        this.tenderedAmount(resultValue);

        this.maxAmountTenderedCurrency(CurrencyFormatter.toCurrency(resultValue));
        this.viewModel.numPadAmountText("");
    }

    public onNumPadEnterEventHandler(result: any): void {
        this.doAmountCalculation(result.value);
    }

    public tenderClick(): void {
        if (this.WTR_IsManualEntered())
            this.WTR_ManualEnter();
        else
            this.WTR_AutoEnter();
    }

    // Process card payments on the payment terminal
    public WTR_AutoEnter(): void {
        //HGM Move to viewmodel
        this.WTR_cardType = WTR_PaymentTerminalEx[this.tenderType];
        let tenderAmount: number = Number(this.tenderedAmount().toString().replace(",", ""));
        var paymTerminalDetails: string[] = this.getPaymentTerminalCOMPORT(this.WTR_cardType.toString());
        this.WTR_COMPort = paymTerminalDetails[0];
        if (this.tenderType == WTR_PaymentTerminalEx.CREDITCARD.toString())
        { //Credit Card
            this.viewModel.isBusy(true);
            this.viewModel.creditCardPayment(this.WTR_COMPort, tenderAmount, Number(this.tenderType), this.context).then((result) => {
                if (result.ResponseCode != "00") //Not successful
                {
                    if (StringExtensions.isNullOrWhitespace(result.ResponseMessage)) {
                        MessageDialog.show(this.context, "Auto Credit Card Payment", true, true, false, StringExtensions.format("Error- {0} ", result.ResponseText)).then(() => {
                        });
                    }
                    else {
                        MessageDialog.show(this.context, "Auto Credit Card Payment", true, true, false, StringExtensions.format("The payment failed due to {0}", result.ResponseMessage)).then(() => {
                        });
                    }
                }
                else {
                    this.viewModel.beginProcessingAutoCardPayment(this.tenderType, tenderAmount, result);
                }
                this.viewModel.isBusy(false);
            }).catch((reason) => {
                this.viewModel.isBusy(false);
            });
        }
        else
        {
            this.viewModel.isBusy(true);
            this.viewModel.NETSPayment(this.WTR_COMPort, this.tenderedAmount(), Number(this.tenderType), this.context).then((result: IWTR_PaymentTerminalEntity) => {
                if (result.ResponseCode != "00") //Not successful
                {
                    if (StringExtensions.isNullOrWhitespace(result.ResponseMessage)) {
                        MessageDialog.show(this.context, "Auto NETS Payment", true, true, false, StringExtensions.format("Error- {0} ", result.ResponseText)).then(() => {
                        });
                    }
                    else {
                        MessageDialog.show(this.context, "Auto NETS Payment", true, true, false, StringExtensions.format("The payment failed due to {0}", result.ResponseMessage)).then(() => {
                        });
                    }
                }
                else {
                    this.viewModel.beginProcessingAutoCardPayment(this.tenderType, tenderAmount, result);
                }
                this.viewModel.isBusy(false);
            }).catch((reason) => {
                this.viewModel.isBusy(false);
            });
        }
    }

    public getPaymentTerminalCOMPORT(cardType: string): string[] {
        let correlationId = this.context.logger.getNewCorrelationId();
        var paymTerminalDetails: string[] = [];
        var comPort: string = StringExtensions.EMPTY;
        //var terminal1Key: string = 'WTR_PAYMENTTERMINAL1TYPE';
        //var terminal2Key: string = 'WTR_PAYMENTTERMINAL2TYPE';
        var WTR_paymTerminalType: WTR_PaymentTerminalType = WTR_PaymentTerminalType.None;
        switch (cardType) {
            case "NETS":
            case "FLASHPAY":
                WTR_paymTerminalType = WTR_PaymentTerminalType.Nets;
                this.context.runtime.executeAsync(new GetHardwareProfileClientRequest<GetHardwareProfileClientResponse>(correlationId)).then((res: ClientEntities.ICancelableDataResult<GetHardwareProfileClientResponse>) => {
                    let terminalType1ExtensionProperty: ProxyEntities.CommerceProperty =
                        Commerce.ArrayExtensions.firstOrUndefined(res.data.result.ExtensionProperties, (property: ProxyEntities.CommerceProperty) => {
                            return property.Key === "WTR_PAYMENTTERMINAL1TYPE";
                        });

                    let comPort1ExtensionProperty: ProxyEntities.CommerceProperty =
                        Commerce.ArrayExtensions.firstOrUndefined(res.data.result.ExtensionProperties, (property: ProxyEntities.CommerceProperty) => {
                            return property.Key === "WTR_PAYMENTTERMINAL1COMPORT";
                        });

                    let terminalType2ExtensionProperty: ProxyEntities.CommerceProperty =
                        Commerce.ArrayExtensions.firstOrUndefined(res.data.result.ExtensionProperties, (property: ProxyEntities.CommerceProperty) => {
                            return property.Key === "WTR_PAYMENTTERMINAL2TYPE";
                        });

                    let comPort2ExtensionProperty: ProxyEntities.CommerceProperty =
                        Commerce.ArrayExtensions.firstOrUndefined(res.data.result.ExtensionProperties, (property: ProxyEntities.CommerceProperty) => {
                            return property.Key === "WTR_PAYMENTTERMINAL2COMPORT";
                        });

                    let paymentTerminal1Type: number = terminalType1ExtensionProperty.Value.IntegerValue;
                    if (paymentTerminal1Type == WTR_PaymentTerminalType.Nets)
                        comPort = comPort1ExtensionProperty.Value.StringValue;

                    let paymentTerminal2Type: number = terminalType2ExtensionProperty.Value.IntegerValue;
                    if (paymentTerminal2Type == WTR_PaymentTerminalType.Nets)
                        comPort = comPort2ExtensionProperty.Value.StringValue;

                });
                if(StringExtensions.isNullOrWhitespace(comPort))
                    comPort = "COM2";  //default to COM2 for NETS
                break;
            default: //Ingenico
                WTR_paymTerminalType = WTR_PaymentTerminalType.CreditCard;
                this.context.runtime.executeAsync(new GetHardwareProfileClientRequest<GetHardwareProfileClientResponse>(correlationId)).then((res: ClientEntities.ICancelableDataResult<GetHardwareProfileClientResponse>) => {

                    let terminalType1ExtensionProperty: ProxyEntities.CommerceProperty =
                        Commerce.ArrayExtensions.firstOrUndefined(res.data.result.ExtensionProperties, (property: ProxyEntities.CommerceProperty) => {
                            return property.Key === "WTR_PAYMENTTERMINAL1TYPE";
                        });

                    let comPort1ExtensionProperty: ProxyEntities.CommerceProperty =
                        Commerce.ArrayExtensions.firstOrUndefined(res.data.result.ExtensionProperties, (property: ProxyEntities.CommerceProperty) => {
                            return property.Key === "WTR_PAYMENTTERMINAL1COMPORT";
                        });

                    let terminalType2ExtensionProperty: ProxyEntities.CommerceProperty =
                        Commerce.ArrayExtensions.firstOrUndefined(res.data.result.ExtensionProperties, (property: ProxyEntities.CommerceProperty) => {
                            return property.Key === "WTR_PAYMENTTERMINAL2TYPE";
                        });

                    let comPort2ExtensionProperty: ProxyEntities.CommerceProperty =
                        Commerce.ArrayExtensions.firstOrUndefined(res.data.result.ExtensionProperties, (property: ProxyEntities.CommerceProperty) => {
                            return property.Key === "WTR_PAYMENTTERMINAL2COMPORT";
                        });

                    let paymentTerminal1Type: number = terminalType1ExtensionProperty.Value.IntegerValue;
                    if (paymentTerminal1Type == WTR_PaymentTerminalType.CreditCard)
                        comPort = comPort1ExtensionProperty.Value.StringValue;

                    let paymentTerminal2Type: number = terminalType2ExtensionProperty.Value.IntegerValue;
                    if (paymentTerminal2Type == WTR_PaymentTerminalType.CreditCard)
                        comPort = comPort2ExtensionProperty.Value.StringValue;

                });

                if (StringExtensions.isNullOrWhitespace(comPort))
                    comPort = "COM1"; //default to COM1 for Credit Card
                break;
        }
        paymTerminalDetails[0] = comPort;
        paymTerminalDetails[1] = WTR_paymTerminalType.toString();
        return paymTerminalDetails;
    }

    public WTR_ManualEnter(): void {
        if (this.tenderType == "10002") { //Credit Card
            // if (this.WTR_paymTerminalType === WTR_PaymentTerminalType.CreditCard) {
            if (!this.viewModel.validateApprovalCode(this.WTR_ManualEntApprovalCode()) || !this.viewModel.validateLast4Digits(this.WTR_ManualEntLast4Digits())) {
                return;
            }
            this.getCardTypeAsync().then((result) => {
                //// Get preprocessed tender line.
                let array: ListData[] = [];
                let cardTypeArray: string[] = [];
                let listItem: ListData = new ListData("", "");
                result.data.result.forEach((cardType) => {
                    if (!(cardTypeArray.indexOf(cardType.TypeId) > 0)) {
                        cardTypeArray.push(cardType.TypeId);

                        if (cardType.Issuer.toUpperCase().indexOf("CREDIT") != -1) { //if the card is credit card
                            listItem = new ListData("", "");
                            listItem.label = cardType.Name;
                            listItem.value = cardType.TypeId;
                            array.push(listItem);
                        }
                    }
                });

                let listInputDialog: ListInputDialog = new ListInputDialog();
                listInputDialog.show(this.context, this.context.resources.getString("string_14"), array)
                    .then((result: string) => {
                        if (result != null) {
                            this.beginProcessingPayment(this.tenderType,result);
                        }
                    }).catch((reason: any) => {
                        this.context.logger.logError("ListInputDialog: " + JSON.stringify(reason));
                    });


            }).catch((reason: any) => {
                // Handle set card type failure.
                //voidPayment = true;
                //voidPaymentMessageId = (paymentAmount >= 0) ? ErrorTypeEnum.PAYMENT_AUTHORIZED_VOID_FAILED : ErrorTypeEnum.PAYMENT_CAPTURED_VOID_FAILED;
                //asyncResult.reject(errors);
            });
        }
        else {
            if (this.tenderType == "10003") {
                if (!this.viewModel.validateApprovalCode(this.WTR_ManualEntApprovalCode())) {
                    return;
                }
                this.beginProcessingPayment(this.tenderType,"NETS"); //Debit card
            }
        }
    }

    /**
        * Sets the payment card type based on the payment card number.
        *
        * @return {IVoidAsyncResult} The void async result.
        */
    public getCardTypeAsync(): Promise<ClientEntities.ICancelableDataResult<GetPaymentCardTypeByBinRangeClientResponse>> {
        let c: ClientEntities.ICardInfo;
        let req: GetPaymentCardTypeByBinRangeClientRequest<GetPaymentCardTypeByBinRangeClientResponse> = new GetPaymentCardTypeByBinRangeClientRequest("", false, c);
        return this.context.runtime.executeAsync(req)
            .then((result) => {
                return result;
            });

    }


}