import { GetChannelConfigurationClientResponse, GetChannelConfigurationClientRequest } from "PosApi/Consume/Device";
import { GetOrgUnitTenderTypesClientRequest, GetOrgUnitTenderTypesClientResponse } from "PosApi/Consume/OrgUnits";
import { GetCurrenciesServiceRequest, GetCurrenciesServiceResponse, GetTenderDetailsClientRequest, GetTenderDetailsClientResponse } from "PosApi/Consume/StoreOperations";
import { IExtensionViewControllerContext } from "PosApi/Create/Views";
import {  ClientEntities, ProxyEntities } from "PosApi/Entities";
import { ArrayExtensions, ObjectExtensions, StringExtensions } from "PosApi/TypeExtensions";
import { ContosoTenderCountingLine } from "./ContosoTenderCountingLine";

import * as Messages from "../../DataService/DataServiceRequests.g";
import { Contoso } from "./Dictionary";
import { GetCurrentShiftClientRequest, GetCurrentShiftClientResponse } from "PosApi/Consume/Shifts";
import ContosoNumberExtensions from "./ContosoNumberExtensions";

declare var Commerce: any;

export default class ContosoTenderCountingViewModel {

    public tenderDetails: ContosoTenderCountingLine[];
    

    public title: string;
    public _context: IExtensionViewControllerContext;
    public _selectedItem: ProxyEntities.TenderDetail;
    public isItemSelected: () => boolean;
    public ChannelConfig: ProxyEntities.ChannelConfiguration;
    public tenderTypres: ProxyEntities.TenderType;

    // From product code
    public _currencyAmountMap: Contoso.Dictionary<ProxyEntities.CurrencyAmount>;
    public _primaryCurrencyCode: string;
    private _shiftToUse: ProxyEntities.Shift;

    constructor(context: IExtensionViewControllerContext) {
        this._context = context;
        this.title = context.resources.getString("string_0001");
        this.tenderDetails = [];
        this.isItemSelected = () => !ObjectExtensions.isNullOrUndefined(this._selectedItem);
        //this.tenderTypes:  ProxyEntities.TenderType[];

        // From product code
        this._currencyAmountMap = new Contoso.Dictionary<ProxyEntities.CurrencyAmount>();
        this._primaryCurrencyCode = Commerce.ApplicationContext.Instance.deviceConfiguration.Currency;


    }


    public _convertCashDeclarationsToDenominationDetails(cashDeclarations: ProxyEntities.CashDeclaration[]): ProxyEntities.DenominationDetail[] {
        let denominationDetails: ProxyEntities.DenominationDetail[] =
            cashDeclarations.map((cashDeclaration: ProxyEntities.CashDeclaration): ProxyEntities.DenominationDetail => {
                let denominationDetail: ProxyEntities.DenominationDetail = {};
                denominationDetail.Type = cashDeclaration.CashTypeValue;
                denominationDetail.Currency = cashDeclaration.Currency;
                denominationDetail.DenominationAmount = cashDeclaration.Amount;
                denominationDetail.QuantityDeclared = 0;
                denominationDetail.AmountDeclared = 0;
                return denominationDetail;
            });

        return denominationDetails;
    }

    // public _getCurrenciesForCurrentStoreAsync(correlationId?: string): Promise<ContosoTenderCountingLine[]> {
    public _getCurrenciesForCurrentStoreAsync(correlationId?: string): Promise<ContosoTenderCountingLine[]> {
        let primaryCurrencyMainDenominationValue: number = 1;

        return this._context.runtime.executeAsync(new Messages.StoreOperations.GetCurrenciesAmounExtRequest<Messages.StoreOperations.GetCurrenciesAmounExtResponse>(this._primaryCurrencyCode, primaryCurrencyMainDenominationValue))
            .then((response: ClientEntities.ICancelableDataResult<Messages.StoreOperations.GetCurrenciesAmounExtResponse>) => {
                console.log(response.data.result);
                let currencyAmounts: ProxyEntities.CurrencyAmount[]
                if (ArrayExtensions.hasElements(currencyAmounts)) {
                    this._currencyAmountMap = new Contoso.Dictionary<ProxyEntities.CurrencyAmount>();
                    currencyAmounts.forEach((currencyAmount: ProxyEntities.CurrencyAmount) => {
                        this._currencyAmountMap.setItem(currencyAmount.CurrencyCode, currencyAmount);
                    });
                }
                return this._getTenderLinesForCountingAsync(correlationId, currencyAmounts);
        });
    }

    private _getTenderLinesForCountingAsync(correlationId: string, currencies?: ProxyEntities.CurrencyAmount[]): Promise<ContosoTenderCountingLine[]> {

        let cashDeclarationsMap: Contoso.Dictionary<ProxyEntities.CashDeclaration[]>;
        let tenderList: ContosoTenderCountingLine[] = [];
        let tenderTypesMap = Commerce.ApplicationContext.Instance.tenderTypesMap;
        let tenderTypesForSalesTransaction: ProxyEntities.TenderType[] = tenderTypesMap.getTenderTypesForSalesTransactions();
        let tenderDetails: ProxyEntities.TenderDetail[] = [];

        return Promise.resolve(Commerce.ApplicationContext.Instance.cashDeclarationsMapAsync.value)
            .then((result: Contoso.Dictionary<ProxyEntities.CashDeclaration[]>) => {
                // Stores the populated map of cash declarations to use later.
                if (!ObjectExtensions.isNullOrUndefined(result)) {
                    cashDeclarationsMap = result;
                }
                let getCurrentShiftClientRequest: GetCurrentShiftClientRequest<GetCurrentShiftClientResponse> =
                    new GetCurrentShiftClientRequest(correlationId);
                return this._context.runtime.executeAsync(getCurrentShiftClientRequest);
            }).then((response: ClientEntities.ICancelableDataResult<GetCurrentShiftClientResponse>) => {
                this._shiftToUse = response.data.result;
                let request: GetTenderDetailsClientRequest<GetTenderDetailsClientResponse> =
                    new GetTenderDetailsClientRequest<GetTenderDetailsClientResponse>(correlationId,
                        this._shiftToUse.ShiftId,
                        this._shiftToUse.TerminalId,
                        ClientEntities.ExtensibleTransactionType.BankDrop);
                return this._context.runtime.executeAsync(request);
            }).then((response: ClientEntities.ICancelableDataResult<GetTenderDetailsClientResponse>) => {
                if (!response.canceled) {
                    tenderDetails = response.data.result;
                }
            }).then(() => {
                if (ArrayExtensions.hasElements(tenderDetails)) {
                    tenderDetails.forEach((tenderDetail: ProxyEntities.TenderDetail) => {
                        tenderList.push(this._convertDetailLinesToCountLines(tenderDetail, cashDeclarationsMap));
                    });
                } else {
                    tenderTypesForSalesTransaction.forEach((tenderType: ProxyEntities.TenderType) => {
                        if (ArrayExtensions.hasElements(currencies) && tenderType.OperationId === ProxyEntities.RetailOperation.PayCurrency) {
                            currencies.forEach((currency: ProxyEntities.CurrencyAmount) => {
                                if (currency.CurrencyCode !== this._primaryCurrencyCode) {
                                    let denominationsForCurrency: ProxyEntities.DenominationDetail[] = this._getDenominationsFromCashDeclarationsMap(
                                        currency.CurrencyCode, tenderType, cashDeclarationsMap);
                                    let tenderLineName: string = StringExtensions.format("{0} - {1}", tenderType.Name, currency.CurrencyCode);
                                    let newCurrencyTenderLine: ContosoTenderCountingLine = new ContosoTenderCountingLine(
                                        currency.CurrencyCode, tenderLineName, tenderType, denominationsForCurrency, currency.ExchangeRate);
                                    tenderList.push(newCurrencyTenderLine);
                                }
                            });
                        } else {
                            let denominations: ProxyEntities.DenominationDetail[]
                                = this._getDenominationsFromCashDeclarationsMap(this._primaryCurrencyCode, tenderType, cashDeclarationsMap);
                            let newTenderLine: ContosoTenderCountingLine
                                = new ContosoTenderCountingLine(this._primaryCurrencyCode, tenderType.Name, tenderType, denominations);
                            tenderList.push(newTenderLine);
                        }
                    });
                }
                return Promise.resolve(tenderList);
            });
    }



    private _convertDetailLinesToCountLines(
        tenderDetail: ProxyEntities.TenderDetail,
        cashDeclarationMap?: Contoso.Dictionary<ProxyEntities.CashDeclaration[]>): ContosoTenderCountingLine {

        let tenderType: ProxyEntities.TenderType
            = Commerce.ApplicationContext.Instance.tenderTypesMap.getTenderByTypeId(tenderDetail.TenderTypeId);
        let tenderTypeName: string = tenderType.Name;
        let currencyCode: string = this._primaryCurrencyCode;
        let exchangeRate: number = 1;
        let amount: number = tenderDetail.Amount;
        let amountInForeignCurrency: number = amount;
        let denominations: ProxyEntities.DenominationDetail[] = tenderDetail.DenominationDetails;

        // We assume that the tender detail is for a foreign currency based on its type.
        if (tenderType.OperationId === ProxyEntities.RetailOperation.PayCurrency) {
            currencyCode = tenderDetail.ForeignCurrency;
            exchangeRate =  ContosoNumberExtensions.isNullOrZero(tenderDetail.ForeignCurrencyExchangeRate)
                ? this._getCurrencyExchangeRate(currencyCode)
                : tenderDetail.ForeignCurrencyExchangeRate;
            amount = this._convertToStoreCurrency(tenderDetail.AmountInForeignCurrency, currencyCode, exchangeRate);
            amountInForeignCurrency = tenderDetail.AmountInForeignCurrency;
            tenderTypeName = StringExtensions.format("{0} - {1}", tenderType.Name, currencyCode);
        }

        // If the tenderDetail doesn't define its own denominations, we fetch them from the Cash Declaration Map.
        if (!ObjectExtensions.isNullOrUndefined(cashDeclarationMap) && !ArrayExtensions.hasElements(denominations)) {
            denominations = this._getDenominationsFromCashDeclarationsMap(currencyCode, tenderType, cashDeclarationMap);
        }

        return new ContosoTenderCountingLine(
            currencyCode,
            tenderTypeName,
            tenderType,
            denominations,
            exchangeRate,
            amount,
            amountInForeignCurrency,
            tenderDetail.TenderRecount
        );
    }


    private _getCurrencyExchangeRate(currencyCode: string): number {
        // Get the currency code
        if (StringExtensions.isNullOrWhitespace(currencyCode)) {
            currencyCode = Commerce.ApplicationContext.Instance.deviceConfiguration.Currency;
        }

        // Get the currency information
        let exchangeRate: number = 0;

        // Set exchange rate to 1 if store currency is same as the amount's currency
        if (currencyCode === this._primaryCurrencyCode) {
            exchangeRate = 1;
        } else if (this._currencyAmountMap.hasItem(currencyCode)) {
            let currency: ProxyEntities.CurrencyAmount = this._currencyAmountMap.getItem(currencyCode);

            // Compute the exchange rate between store currency and specified currency
            exchangeRate = currency.ExchangeRate === 0 ? 0 : (1 / currency.ExchangeRate);
        }

        return exchangeRate;
    }

    private _convertToStoreCurrency(amount: number, fromCurrencyCode: string, fromCurrencyExchangeRate: number): number {
        // Check the parameters
        if (ObjectExtensions.isNullOrUndefined(amount)) {
            return amount;
        }

        if (StringExtensions.isNullOrWhitespace(fromCurrencyCode)) {
            fromCurrencyCode = Commerce.ApplicationContext.Instance.deviceConfiguration.Currency;
        }

        if (!this._currencyAmountMap.hasItem(fromCurrencyCode)) {
            return amount;
        }

        // Convert the currency amount to the store currency amount
        let storeAmount: number = amount * fromCurrencyExchangeRate;
        return ContosoNumberExtensions.roundToNDigits(storeAmount, ContosoNumberExtensions.getDecimalPrecision());
    }


    private _getDenominationsFromCashDeclarationsMap(
        currencyCode: string,
        tenderType: ProxyEntities.TenderType,
        cashMap: Contoso.Dictionary<ProxyEntities.CashDeclaration[]>): ProxyEntities.DenominationDetail[] {

        if (StringExtensions.isNullOrWhitespace(currencyCode)
            || !cashMap.hasItem(currencyCode)
            || ObjectExtensions.isNullOrUndefined(tenderType)
            || (tenderType.OperationId !== ProxyEntities.RetailOperation.PayCash
            && tenderType.OperationId !== ProxyEntities.RetailOperation.PayCurrency)) {
            return [];
        }

        let cashDeclarations: ProxyEntities.CashDeclaration[] = cashMap.getItem(currencyCode);

        return this._convertCashDeclarationsToDenominationDetails(cashDeclarations);
    }

    public loadAsync(): Promise<void> {

        //let tenderTypesMap: TenderTypeMap = ApplicationContext.Instance.tenderTypesMap;

        let correlationId = this._context.logger.getNewCorrelationId();

        let accountNumber: string = Commerce.ApplicationContext.Instance.storeInformation.DefaultCustomerAccount;

        let storeTenderTypeMap = Commerce.ApplicationContext.Instance.tenderTypesMap;

        let tenderTypes: ProxyEntities.TenderType[] = storeTenderTypeMap.getItems();
        console.log(tenderTypes);

        let operationIds: ProxyEntities.RetailOperation[] = storeTenderTypeMap.getItems();

        console.log(operationIds);

        tenderTypes = storeTenderTypeMap.getTenderTypesForSalesTransactions();
        console.log(tenderTypes);

        let cashDeclarations: ProxyEntities.CashDeclaration[] = Commerce.ApplicationContext.Instance.cashDeclarationsMapAsync.value;

        console.log(cashDeclarations);

        let dictCashDeclaration: Contoso.Dictionary<ProxyEntities.CashDeclaration> = new Contoso.Dictionary();
        console.log(dictCashDeclaration);

        //operationIds.forEach((operationId: ProxyEntities.RetailOperation) => {
        //    let tenderTypes: ProxyEntities.TenderType[] = storeTenderTypeMap.getItem(operationId);
        //    console.log(tenderTypes);
        //});

        console.log(storeTenderTypeMap);

        console.log(accountNumber);

        return Promise.all([
            this._context.runtime.executeAsync(new GetOrgUnitTenderTypesClientRequest<GetOrgUnitTenderTypesClientResponse>(correlationId))
                .then((response: ClientEntities.ICancelableDataResult<GetOrgUnitTenderTypesClientResponse>): ProxyEntities.TenderType[] => {
                    return response.data.result;
                }),

            this._context.runtime.executeAsync(new GetCurrenciesServiceRequest(correlationId))
                .then((response: ClientEntities.ICancelableDataResult<GetCurrenciesServiceResponse>): ProxyEntities.Currency[] => {
                    return response.data.currencies;
                }),

            this._context.runtime.executeAsync(new GetChannelConfigurationClientRequest<GetChannelConfigurationClientResponse>(correlationId))
                .then((response: ClientEntities.ICancelableDataResult<GetChannelConfigurationClientResponse>): ProxyEntities.ChannelConfiguration => {
                    return response.data.result;
                })])
            .then((results: any[]) => {
               // this.TenderTypres = results[0];
                this.ChannelConfig = results[2]
                return this._context.runtime.executeAsync(new Messages.StoreOperations.GetCurrenciesAmounExtRequest<Messages.StoreOperations.GetCurrenciesAmounExtResponse>(this.ChannelConfig.Currency, 1));
            }).then((response: ClientEntities.ICancelableDataResult<Messages.StoreOperations.GetCurrenciesAmounExtResponse>) => {
                console.log(response.data.result);
            }).catch((reason: any) => {
                console.log(reason);
            });
    }
}