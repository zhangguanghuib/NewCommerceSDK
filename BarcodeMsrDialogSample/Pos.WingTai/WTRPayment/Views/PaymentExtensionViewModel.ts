import ko from "knockout";

import { IExtensionViewControllerContext } from "PosApi/Create/Views";
import { ProxyEntities, ClientEntities } from "PosApi/Entities";
import { IPaymentExtensionViewModelOptions } from "./NavigationContracts";
import KnockoutExtensionViewModelBase from "./BaseClasses/KnockoutExtensionViewModelBase";
import { AddPreprocessedTenderLineToCartClientRequest, AddPreprocessedTenderLineToCartClientResponse } from "PosApi/Consume/Cart";
import { CurrencyFormatter } from "PosApi/Consume/Formatters";
import MessageDialog from "../Controls/DialogSample/MessageDialog";
//import CreditCardTerminalManager from "../Managers/CreditCardTerminalManager";
//import NETSTerminalManager from "../Managers/NETSTerminalManager";
import { IExtensionContext } from "PosApi/Framework/ExtensionContext";
import { WTR_PaymentTerminalEx, IWTR_PaymentTerminalEntity } from "./../Peripherals/IPaymentTerminal";
import CreditCardTerminalManager from "./../Manager/CreditCardTerminalManager";
import INETSPaymentTerminal from "./../Peripherals/HardwareStation/NETSPaymentTerminal";
import NETSPaymentTerminal from "./../Peripherals/HardwareStation/NETSPaymentTerminal";
import { LineDisplayDisplayLinesRequest, LineDisplayDisplayLinesResponse } from "PosApi/Consume/Peripherals";
import { StringExtensions} from "PosApi/TypeExtensions"
export class PaymentExtensionViewModel extends KnockoutExtensionViewModelBase {
    public title: ko.Observable<string>;                                               // The view title    
    public isBusy: ko.Observable<boolean>;                                             // Indicates there is action occurring and the page is not available for input

    public fullAmountTextCurrency: ko.Observable<string>;

    public fullAmount: ko.Observable<number>;
    public numPadAmountText: ko.Observable<string>;

    public maxAmountLabel: ko.Observable<string>;

    public maxAmountTextCurrency: ko.Observable<string>;
    public maxAmount: ko.Observable<number>;
    public totalAmount: ko.Observable<number>;
    private _context: IExtensionViewControllerContext;                              // The view controller context 
    private _cart: ko.Observable<ProxyEntities.Cart>;                                  // The cart
    private _deviceConfiguration: ko.Observable<ProxyEntities.DeviceConfiguration>;    // The device configuration
    private _tenderType: ko.Observable<ProxyEntities.TenderType>;                      // The tender type

    private _options?: IPaymentExtensionViewModelOptions;

    public _isReturned: ko.Observable<boolean>;

    public userPreDefinedValue: ko.Observable<number>;
    public simpleMessage: ko.Observable<string>;
    constructor(context: IExtensionViewControllerContext, options: IPaymentExtensionViewModelOptions) {
        super();


        this.isBusy = ko.observable(false);
        this.totalAmount = ko.observable(options.cart.AmountDue);

        this._isReturned = this.totalAmount() < 0 ? ko.observable(true) : ko.observable(false);
        this._context = context;
        this._options = options;
        this._cart = ko.observable<ProxyEntities.Cart>(this._options.cart);
        this._tenderType = ko.observable<ProxyEntities.TenderType>(this._options.tenderType);
        this._deviceConfiguration = ko.observable<ProxyEntities.DeviceConfiguration>(this._options.deviceConfiguration);

        this.userPreDefinedValue = ko.observable(0);

        this.title = ko.observable(this._tenderType().Name);

        this.fullAmount = ko.observable(this._cart().AmountDue);
        this.fullAmountTextCurrency = ko.observable(CurrencyFormatter.toCurrency(this.fullAmount()));

        this.numPadAmountText = ko.observable(CurrencyFormatter.toCurrency(0));

        this.maxAmountLabel = ko.observable("");
        this.simpleMessage = ko.observable("");
        this.updateLineDisplay(options.cart.TotalAmount, options.cart.AmountDue);
    }

    public updateLineDisplay(totalAmount: number, amountDue: number) {
        let lineDisplayString: string = "Total    " + totalAmount + "; Balance     " + amountDue;
        let req: LineDisplayDisplayLinesRequest<LineDisplayDisplayLinesResponse> = new LineDisplayDisplayLinesRequest<LineDisplayDisplayLinesResponse>(lineDisplayString.split(';'));
        this._context.runtime.executeAsync(req).then(() => {
            this._context.logger.logInformational("Custom Line display call success");
        }).catch((reason) => {
            this._context.logger.logInformational("Custom Line display call failed with error - " + reason);
        });

    }

    public beginProcessingManualCardPayment(tenderTypeId:string,amountTendered: number, cardType: string, approvalCode: string, last4digits: string): void {

        let preProcessedTenderLine: ProxyEntities.TenderLine = {
            Amount: amountTendered,
            TenderTypeId: "3", // Card
            Currency: this._deviceConfiguration().Currency,
            CardTypeId: cardType,
            IsPreProcessed: false,
            IsDeposit: false,
            IsVoidable: true,
            Authorization: "<![CDATA[<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfPaymentProperty xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
            "<PaymentProperty>\r\n" +
            "<Namespace>Connector</Namespace>\r\n" +
            "<Name>ConnectorName</Name>\r\n" +
            "<ValueType>String</ValueType>\r\n" +
            "<StoredStringValue>TestConnector</StoredStringValue>\r\n" +
            "<DecimalValue>" + amountTendered +
            "</DecimalValue>\r\n" +
            "<DateValue>0001-01-01T00:00:00</DateValue>\r\n" +
            "<SecurityLevel>None</SecurityLevel>\r\n " +
            "<IsEncrypted> false</IsEncrypted>\r\n " +
            "<IsPassword>false</IsPassword>\r\n " +
            "<IsReadOnly>false</IsReadOnly>\r\n " +
            "<IsHidden>false</IsHidden>\r\n " +
            "<DisplayHeight>1</DisplayHeight>\r\n " +
            "<SequenceNumber>0</SequenceNumber>\r\n " +
            "</PaymentProperty>\r\n  " +
            "\n </ArrayOfPaymentProperty>]]>",
            MaskedCardNumber: "************" + last4digits,
            TransactionStatusValue: 0,
            IncomeExpenseAccountTypeValue: -1,
            CashBackAmount: 0,
            AmountInTenderedCurrency: amountTendered,
            AmountInCompanyCurrency: amountTendered,
            ReasonCodeLines: [],
            CustomerId: "************" + last4digits,
            IsChangeLine: false,
            IsHistorical: false,
            StatusValue: 4,
            VoidStatusValue: 0,
            ExtensionProperties: []
        };

        this.addTenderToCart(preProcessedTenderLine);

        this.addPaymLineExtensionProperty(preProcessedTenderLine, "WTR_APPROVALCODE", approvalCode);
        this.addPaymLineExtensionProperty(preProcessedTenderLine, "WTR_ISMANUAL", "1");
        this.addPaymLineExtensionProperty(preProcessedTenderLine, "WTR_CARDNUMBER", "************" + last4digits);
        this.addPaymLineExtensionProperty(preProcessedTenderLine, "WTR_SELECTEDTENDERTYPE", tenderTypeId);
    }

    private addPaymLineExtensionProperty(tenderLine: ProxyEntities.TenderLine, key: string, strValue: string) {
        var propP: ProxyEntities.CommerceProperty = new ProxyEntities.CommercePropertyClass();
        propP.Key = key;
        propP.Value = new ProxyEntities.CommercePropertyValueClass({ StringValue: strValue });
        tenderLine.ExtensionProperties.push(propP);
    }

    public addTenderToCart(preProcessedTenderLine: ProxyEntities.TenderLine): Promise<ClientEntities.ICancelableDataResult<AddPreprocessedTenderLineToCartClientResponse>> {


        let addPreProcessedTenderLineToCartClientRequest: AddPreprocessedTenderLineToCartClientRequest<AddPreprocessedTenderLineToCartClientResponse> =
            new AddPreprocessedTenderLineToCartClientRequest<AddPreprocessedTenderLineToCartClientResponse>(preProcessedTenderLine);


        return this._context.runtime.executeAsync(addPreProcessedTenderLineToCartClientRequest)
            .then((result) => {

                this.navigateToPOSView(result.data.result.AmountDue);
                return result;
            }).catch((reason: any) => {
                return MessageDialog.show(this._context, "Payment", true, true, false, reason).then(() => {
                    this._context.logger.logInformational("MessageDialog closed");
                    return Promise.resolve({ canceled: true, data: null })
                });
            });
    }

    private navigateToPOSView(amountDue: number): void {
        this._context.navigator.navigateToPOSView("CartView");

        this.isBusy(true);
        
        setTimeout(() => this.documentElementClick(amountDue), 2000);
    }

    private documentElementClick(amountDue: number) : void {
        if (amountDue == 0) {
            var amountDueElement: HTMLElement = this.getAttribute("data-ax-bubble", "cartView_amountDueLink");
            amountDueElement.click();
        }

        // show the payment tab
        var paymentsElement: HTMLElement = this.getAttribute("data-ax-bubble", "grid_payments");
        paymentsElement.click();

        this.isBusy(false);
    }

    private getAttribute(attribute: string, value: string): HTMLElement {
        var element: HTMLElement = document.createElement('div');
        var all = document.getElementsByTagName("*");
        for (var i = 0; i < all.length; i++) {
            if (all[i].getAttribute(attribute) == value) {
                element = <HTMLElement>all[i];
                break;
            }
        }

        return element;
    }

    public validateApprovalCode(approvalCode: string): boolean {
        if (approvalCode.length < 6) {
            MessageDialog.show(this._context, "Payment", true, true, false, "Approval code should be 6 digits").then(() => {
                this._context.logger.logInformational("validate approvalCode MessageDialog closed");
            });
            return false
        }
        else {
            if (this.isNumeric(approvalCode))
                return true;
        }

        return false;
    }


    public validateLast4Digits(last4digits: string): boolean {
        if (last4digits.length < 4) {
            MessageDialog.show(this._context, "Payment", true, true, false, "Enter last 4 digits of card no.").then(() => {
                this._context.logger.logInformational("validate last4digits MessageDialog closed");
            });
            return false
        }
        else {
            if (this.isNumeric(last4digits))
                return true;
        }

        return false;
    }

    private isNumeric(value: string): boolean {
        return /^\d+$/.test(value);
    }

    public creditCardPayment(comPort: string, tenderedAmount: number, tenderTypeId: number, _context: IExtensionContext): Promise<IWTR_PaymentTerminalEntity> {
        let creditCardPaymentTerminal: CreditCardTerminalManager = new CreditCardTerminalManager(_context);
        return creditCardPaymentTerminal.makePayment(comPort, tenderedAmount, tenderTypeId, _context).then((result: IWTR_PaymentTerminalEntity) => {
            if (StringExtensions.isNullOrWhitespace(result.ApprovalCode) && result.ResponseMessage != null) {
                return MessageDialog.show(this._context, "Payment", true, true, false, "Error - " + result.ResponseMessage).then(() => {
                    this._context.logger.logInformational("Credit Card Payment Terminal Error - " + result.ResponseMessage);
                    return result;
                });
                
            }
            else {
                return result;
            }
        }).catch((reason) => {
            MessageDialog.show(this._context, "Payment", true, true, false, "Error - " + this._context.resources.getString(reason[0]._errorCode)).then(() => {
                this._context.logger.logInformational("Credit Card Payment Terminal Error - " + reason);
            });
            return Promise.reject("Closed");
        });
        //return creditCardPaymTerminal.makePayment(comPort, tenderedAmount, tenderTypeId, _context).then((result: IWTR_PaymentTerminalEntity) => {
        //    if (result.ApprovalCode != null)
        //        this.beginProcessingAutoCardPayment(tenderedAmount, result.CardType, result.ApprovalCode, result.CardNumberMasked, result);
        //    else {
        //        MessageDialog.show(this._context, "Payment", true, true, false, "Error - " + result.ResponseText).then(() => {
        //            this._context.logger.logInformational("Credit Card Payment Terminal Error - " + result.ResponseText);
        //        });
        //        return Promise.reject("Closed");
        //    }
        //});
    }

    public NETSPayment(comPort: string, tenderedAmount: number, tenderTypeId: number, _context: IExtensionContext): Promise<IWTR_PaymentTerminalEntity> {
        let NETSPaymTerminal: INETSPaymentTerminal = new NETSPaymentTerminal(this._context.runtime);
        return NETSPaymTerminal.makePayment(comPort, tenderedAmount, tenderTypeId, _context).then((result: IWTR_PaymentTerminalEntity) => {
            if (StringExtensions.isNullOrWhitespace(result.ApprovalCode) && result.ResponseMessage != null) {
                return MessageDialog.show(this._context, "Payment", true, true, false, "Error - " + result.ResponseMessage).then(() => {
                    this._context.logger.logInformational("NETS Card Payment Terminal Error - " + result.ResponseMessage);
                    return result;
                });

            }
            else {
                return result;
            }
        }).catch((reason) => {
            MessageDialog.show(this._context, "Payment", true, true, false, "Error - " + this._context.resources.getString(reason[0]._errorCode)).then(() => {
                this._context.logger.logInformational("NETS Card Payment Terminal Error - " + reason);
            });
            return Promise.reject("Closed");
        });
    }


    public beginProcessingAutoCardPayment(tenderTypeId: string, amountTendered: number, result: IWTR_PaymentTerminalEntity): void {
        let cardType:string = StringExtensions.EMPTY;
        if (tenderTypeId == WTR_PaymentTerminalEx.CREDITCARD.toString() || tenderTypeId == WTR_PaymentTerminalEx.CONTACTLESS.toString())
        {
            cardType = this.resolveCardType(result.CardType)
        }
        else
        {
            cardType = tenderTypeId; //NETS, FLASHPAY
        }
        let preProcessedTenderLine: ProxyEntities.TenderLine = {
                Amount: amountTendered,
                TenderTypeId: "3", // Card
                Currency: this._deviceConfiguration().Currency,
                CardTypeId: cardType,
                IsPreProcessed: false,
                IsDeposit: false,
                IsVoidable: true,
                Authorization: "<![CDATA[<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfPaymentProperty xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
                "<PaymentProperty>\r\n" +
                "<Namespace>Connector</Namespace>\r\n" +
                "<Name>ConnectorName</Name>\r\n" +
                "<ValueType>String</ValueType>\r\n" +
                "<StoredStringValue>TestConnector</StoredStringValue>\r\n" +
                "<DecimalValue>" + amountTendered +
                "</DecimalValue>\r\n" +
                "<DateValue>0001-01-01T00:00:00</DateValue>\r\n" +
                "<SecurityLevel>None</SecurityLevel>\r\n " +
                "<IsEncrypted> false</IsEncrypted>\r\n " +
                "<IsPassword>false</IsPassword>\r\n " +
                "<IsReadOnly>false</IsReadOnly>\r\n " +
                "<IsHidden>false</IsHidden>\r\n " +
                "<DisplayHeight>1</DisplayHeight>\r\n " +
                "<SequenceNumber>0</SequenceNumber>\r\n " +
                "</PaymentProperty>\r\n  " +
                "\n </ArrayOfPaymentProperty>]]>",
                MaskedCardNumber: "*****" + result.CardNumberMasked,
                TransactionStatusValue: 0,
                IncomeExpenseAccountTypeValue: -1,
                CashBackAmount: 0,
                AmountInTenderedCurrency: amountTendered,
                AmountInCompanyCurrency: amountTendered,
                ReasonCodeLines: [],
                CustomerId: result.CardNumberMasked, //"************" + result.CardNumberMasked,
                IsChangeLine: false,    
                IsHistorical: false,
                StatusValue: 4,
                VoidStatusValue: 0,
                ExtensionProperties: []
            };

        this.addTenderToCart(preProcessedTenderLine);

        this.addPaymLineExtensionProperty(preProcessedTenderLine, "WTR_APPROVALCODE", result.ApprovalCode);
        this.addPaymLineExtensionProperty(preProcessedTenderLine, "WTR_ISMANUAL", "0");
        this.addPaymLineExtensionProperty(preProcessedTenderLine, "WTR_INVOICENUMBER", result.InvoiceNumber);
        this.addPaymLineExtensionProperty(preProcessedTenderLine, "WTR_MERCHANTID", result.MerchantId);
        this.addPaymLineExtensionProperty(preProcessedTenderLine, "WTR_CARDNUMBER", result.CardNumberMasked);
        this.addPaymLineExtensionProperty(preProcessedTenderLine, "WTR_EXPDATE", result.ExpiryDate);
        this.addPaymLineExtensionProperty(preProcessedTenderLine, "WTR_SELECTEDTENDERTYPE", tenderTypeId);
    
    }

    private resolveCardType(cardTypeIn: string): string {
        // RAD 180129
        this._context.logger.logInformational("method:resolveCardType:cardTypeIn" + cardTypeIn);
        //END
        var cardTypeOut: string = StringExtensions.EMPTY;
        switch (cardTypeIn) {
            case "V":
                cardTypeOut = "VISA";
                break;
            case 'M':
                cardTypeOut = "MASTER"
                break;
            case 'A':
                cardTypeOut = "AMEX"
                break;
            case 'J':
                cardTypeOut = "JCB"
                break;

            // RAD 180129
            case 'C':
                cardTypeOut = "CUP"
                break;
            //

            case 'D':
                cardTypeOut = "DINERS"
                break;
            default:
                cardTypeOut = "INVALID";
                break;
        }

        this._context.logger.logInformational("method:resolveCardType:cardTypeOut" + cardTypeOut);
        // For remaining cards the Host would resolve to the setup

        return cardTypeOut;
    }
}

