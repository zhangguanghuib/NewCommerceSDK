import { CurrencyFormatter } from "PosApi/Consume/Formatters";
import { ProxyEntities } from "PosApi/Entities";
import { ArrayExtensions } from "PosApi/TypeExtensions";

export class ContosoTenderCountingLine {
    public tenderType: ProxyEntities.TenderType;
    public tenderName: string;
    public currencyCode: string;
    public exchangeRate: number; // Exchange rate between the currency code and store currency
    public denominations: ProxyEntities.DenominationDetail[];
    public totalAmount: number; // Amount in store currency
    public totalAmountInCurrency: number; // Amount in entered currency
    public totalAmountInCurrencyToDisplay: string; // Amount in entered currency as a formatted string
    public numberOfTenderDeclarationRecount: number; // Number of tender declaration recounts performed
    public readonly ariaLabelPaymentCount: string;
    public readonly ariaLabelPaymentTotal: string;
    public readonly hasDenominations: boolean;

    constructor(
        currencyCode: string,
        tenderName: string,
        tenderType: ProxyEntities.TenderType,
        denominations: ProxyEntities.DenominationDetail[],
        exchangeRate: number = 1,
        totalAmount: number = 0,
        totalAmountInCurrency: number = 0,
        numberOfTenderDeclarationRecount: number = 0) {

        this.currencyCode = currencyCode;
        this.tenderName = tenderName;
        this.tenderType = tenderType;
        this.exchangeRate = exchangeRate;
        this.totalAmount = totalAmount;
        this.totalAmountInCurrency = totalAmountInCurrency;
        this.denominations = denominations;
        this.numberOfTenderDeclarationRecount = numberOfTenderDeclarationRecount;
        this.totalAmountInCurrencyToDisplay = CurrencyFormatter.toCurrency(this.totalAmountInCurrency);
          //  NumberExtensions.formatCurrency(this.totalAmountInCurrency, this.currencyCode);

        // Payment Method count aria label - {0} {1} count.
        this.ariaLabelPaymentCount =
            Commerce.StringExtensions.format("{0} { 1 } count", this.tenderName, this.currencyCode);
        // Payment Method total aria label - {0} total {1}.
        this.ariaLabelPaymentTotal = Commerce.StringExtensions.format(
            "{0} { 1 } count", this.tenderName, this.totalAmountInCurrencyToDisplay);
        this.hasDenominations = ArrayExtensions.hasElements(this.denominations);
    }
}