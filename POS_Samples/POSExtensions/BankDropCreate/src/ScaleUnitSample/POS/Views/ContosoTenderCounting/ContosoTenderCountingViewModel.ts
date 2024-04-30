import { GetChannelConfigurationClientResponse, GetChannelConfigurationClientRequest } from "PosApi/Consume/Device";
import { GetOrgUnitTenderTypesClientRequest, GetOrgUnitTenderTypesClientResponse } from "PosApi/Consume/OrgUnits";
import { CreateBankDropTransactionClientRequest, CreateBankDropTransactionClientResponse, GetBankBagNumberClientRequest, GetBankBagNumberClientResponse, GetCurrenciesServiceRequest, GetCurrenciesServiceResponse, GetTenderDetailsClientRequest, GetTenderDetailsClientResponse } from "PosApi/Consume/StoreOperations";
import { IExtensionViewControllerContext } from "PosApi/Create/Views";
import {  ClientEntities, ProxyEntities } from "PosApi/Entities";
import { ArrayExtensions, ObjectExtensions, StringExtensions } from "PosApi/TypeExtensions";
import { ContosoTenderCountingLine } from "./ContosoTenderCountingLine";

import * as Messages from "../../DataService/DataServiceRequests.g";
import { Contoso } from "./Dictionary";
import { GetCurrentShiftClientRequest, GetCurrentShiftClientResponse } from "PosApi/Consume/Shifts";
import ContosoNumberExtensions from "./ContosoNumberExtensions";
import NumericInputDialog from "../../Create/Dialogs/NumericInputDialog";

import ko from "knockout";

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
    public _tenderCountingType: ClientEntities.ExtensibleTransactionType;
    public tenderCountingLines: ko.ObservableArray<ContosoTenderCountingLine>;
    //private _tenderCountingLines: ContosoTenderCountingLine[];

    constructor(context: IExtensionViewControllerContext) {
        this._context = context;
        this.title = "Contoso Bank Drop";
        this.tenderDetails = [];
        this.isItemSelected = () => !ObjectExtensions.isNullOrUndefined(this._selectedItem);
        //this.tenderTypes:  ProxyEntities.TenderType[];

        // From product code
        this._currencyAmountMap = new Contoso.Dictionary<ProxyEntities.CurrencyAmount>();
        this._primaryCurrencyCode = Commerce.ApplicationContext.Instance.deviceConfiguration.Currency;
        this._tenderCountingType = ClientEntities.ExtensibleTransactionType.BankDrop;
        this.tenderCountingLines = ko.observableArray([]);
        //this._tenderCountingLines = [];
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
                let currencyAmounts: ProxyEntities.CurrencyAmount[] = response.data.result;
                if (ArrayExtensions.hasElements(currencyAmounts)) {
                    this._currencyAmountMap = new Contoso.Dictionary<ProxyEntities.CurrencyAmount>();
                    currencyAmounts.forEach((currencyAmount: ProxyEntities.CurrencyAmount) => {
                        this._currencyAmountMap.setItem(currencyAmount.CurrencyCode, currencyAmount);
                    });
                }
                return this._getTenderLinesForCountingAsync(correlationId, currencyAmounts);
        });
    }

    private _getCashDeclarationsMapAsync(): Promise<Contoso.Dictionary<ProxyEntities.CashDeclaration[]>> {
        return this._context.runtime.executeAsync(new Messages.StoreOperations.GetCashDeclarationsExtRequest<Messages.StoreOperations.GetCashDeclarationsExtResponse>(Commerce.ApplicationContext.Instance.channelConfiguration.RecordId))
            .then((response: ClientEntities.ICancelableDataResult<Messages.StoreOperations.GetCashDeclarationsExtResponse>): Promise<Contoso.Dictionary<ProxyEntities.CashDeclaration[]>> => {
                let cashDeclarations: ProxyEntities.CashDeclaration[] = response.data.result;
                let cashDeclarationsMap: Contoso.Dictionary<ProxyEntities.CashDeclaration[]> = new Contoso.Dictionary<ProxyEntities.CashDeclaration[]>();

                cashDeclarations.forEach((value: ProxyEntities.CashDeclaration) => {
                    if (!cashDeclarationsMap.hasItem(value.Currency)) {
                        cashDeclarationsMap.setItem(value.Currency, []);
                    }
                    cashDeclarationsMap.getItem(value.Currency).push(value);
                });
                return Promise.resolve(cashDeclarationsMap);
            });
    };
   

    private _getTenderLinesForCountingAsync(correlationId: string, currencies?: ProxyEntities.CurrencyAmount[]): Promise<ContosoTenderCountingLine[]> {

        let cashDeclarationsMap: Contoso.Dictionary<ProxyEntities.CashDeclaration[]>;
        let tenderList: ContosoTenderCountingLine[] = [];
        let tenderTypesMap = Commerce.ApplicationContext.Instance.tenderTypesMap;
        let tenderTypesForSalesTransaction: ProxyEntities.TenderType[] = tenderTypesMap.getTenderTypesForSalesTransactions();
        let tenderDetails: ProxyEntities.TenderDetail[] = [];

        return this._getCashDeclarationsMapAsync()
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

    public listItemSelected(item: ContosoTenderCountingLine): Promise<void> {

        let numericInputDialog: NumericInputDialog = new NumericInputDialog();
        return numericInputDialog.show(this._context, this.title)
            .then((result: string) => {
                item.totalAmount = parseFloat(result);
                return Promise.resolve();
            }).catch((reason: any) => {
                return Promise.reject(reason);
            });
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
        console.log(storeTenderTypeMap);
        console.log(accountNumber);

        return this._getCurrenciesForCurrentStoreAsync(correlationId).then((countingLines: ContosoTenderCountingLine[]): void => {
            countingLines.forEach((countingLine: ContosoTenderCountingLine): void => {
                let tenderDeclarationNeeded: boolean =
                    ClientEntities.ExtensibleTransactionType.TenderDeclaration.equals(this._tenderCountingType) &&
                    countingLine.tenderType.CountingRequired === 1;

                let bankDropNeeded: boolean =
                    ClientEntities.ExtensibleTransactionType.BankDrop.equals(this._tenderCountingType) &&
                    countingLine.tenderType.TakenToBank === 1;

                let safeDropNeeded: boolean =
                    ClientEntities.ExtensibleTransactionType.SafeDrop.equals(this._tenderCountingType) &&
                    countingLine.tenderType.TakenToSafe === 1;

                if (tenderDeclarationNeeded || bankDropNeeded || safeDropNeeded) {
                    this.tenderCountingLines.push(countingLine);
                }
            });
        })
    }

    public onSave(): Promise<void> {

        let correlationId: string = this._context.logger.getNewCorrelationId();

        Promise.all([
            this._context.runtime.executeAsync(new GetCurrentShiftClientRequest<GetCurrentShiftClientResponse>(correlationId))
                .then((response: ClientEntities.ICancelableDataResult<GetCurrentShiftClientResponse>): ProxyEntities.Shift => {
                    return response.data.result;
                }),

            this._context.runtime.executeAsync(new GetBankBagNumberClientRequest<GetBankBagNumberClientResponse>(correlationId))
                .then((response: ClientEntities.ICancelableDataResult<GetBankBagNumberClientResponse>) => {
                    return response.data.result.bagNumber;
                })
        ]).then((results: any[]) => {
            let currentShift: ProxyEntities.Shift = results[0];
            let bagNumber: string = results[1];
            let tenderDetails: ProxyEntities.TenderDetail[] = this._convertCountLinesToDetailLines(this.tenderCountingLines());

            let createBankDropTransactionClientRequest: CreateBankDropTransactionClientRequest<CreateBankDropTransactionClientResponse> =
                new CreateBankDropTransactionClientRequest<CreateBankDropTransactionClientResponse>(false, currentShift, tenderDetails, bagNumber, correlationId);

            this._context.runtime.executeAsync(createBankDropTransactionClientRequest).then((response: ClientEntities.ICancelableDataResult<CreateBankDropTransactionClientResponse>): Promise<void> => {
                if (response.canceled) {
                    let errorMessage: string = `Bank Drop is cancelled, please retry!`;
                    return Promise.reject(new ClientEntities.ExtensionError(errorMessage));
                } else {
                    return Promise.resolve();
                }
            }).catch((reason: any) => {
                let errorMessage: string = `Bank Drop Failed for the reason ${reason}`;
                return Promise.reject(new ClientEntities.ExtensionError(errorMessage));
            });
        });

        //let getCurrentShiftClientRequest: GetCurrentShiftClientRequest<GetCurrentShiftClientResponse> = new GetCurrentShiftClientRequest(correlationId);
        //this._context.runtime.executeAsync(getCurrentShiftClientRequest)
        //    .then((response: ClientEntities.ICancelableDataResult<GetCurrentShiftClientResponse>): Promise<ClientEntities.ICancelableDataResult<ProxyEntities.Shift>> => {
        //        if (response.canceled) {
        //            return Promise.resolve({ canceled: true, data: null });
        //        } else {
        //            let currentShift: ProxyEntities.Shift = response.data.result;
        //            return Promise.resolve({ canceled: false, data: currentShift });
        //        }
        //    }).then((result: ClientEntities.ICancelableDataResult<ProxyEntities.Shift>): Promise<ClientEntities.ICancelableDataResult<GetBankBagNumberClientResponse>> => {
        //        if (!result.canceled) {
        //            return Promise.resolve({ canceled: true, data: null });
        //        } else {
        //            let getBankBagNumberClientRequest: GetBankBagNumberClientRequest<GetBankBagNumberClientResponse> = new GetBankBagNumberClientRequest(correlationId);
        //            return this._context.runtime.executeAsync(getBankBagNumberClientRequest);
        //        }
        //    }).then((response: ClientEntities.ICancelableDataResult<GetBankBagNumberClientResponse>) => {

        //    })
        return Promise.resolve();
    }

    private _convertCountLinesToDetailLines(contosoTenderCountingLines: ContosoTenderCountingLine[]): ProxyEntities.TenderDetail[] {
        let tenderDetailLines: ProxyEntities.TenderDetail[] = [];
        if (ArrayExtensions.hasElements(contosoTenderCountingLines)) {
            contosoTenderCountingLines.forEach((contosoTenderCountingLine: ContosoTenderCountingLine) => {
                if (!ObjectExtensions.isNullOrUndefined(contosoTenderCountingLine)) {
                    let tenderDetailLine: ProxyEntities.TenderDetail = new ProxyEntities.TenderDetailClass();
                    tenderDetailLine.Amount = contosoTenderCountingLine.totalAmount;
                    tenderDetailLine.ForeignCurrency = contosoTenderCountingLine.currencyCode;
                    tenderDetailLine.ForeignCurrencyExchangeRate = contosoTenderCountingLine.exchangeRate;
                    tenderDetailLine.AmountInForeignCurrency = contosoTenderCountingLine.totalAmountInCurrency;
                    tenderDetailLine.TenderTypeId = contosoTenderCountingLine.tenderType.TenderTypeId;
                    tenderDetailLine.TenderRecount = contosoTenderCountingLine.numberOfTenderDeclarationRecount;

                    tenderDetailLine.DenominationDetails = [];

                    contosoTenderCountingLine.denominations.forEach((denominationLine: ProxyEntities.DenominationDetail): void => {
                        if (denominationLine.QuantityDeclared > 0) {
                            tenderDetailLine.DenominationDetails.push(denominationLine);
                        }
                    });
                    tenderDetailLines.push(tenderDetailLine);
                }
            });
        }
        return tenderDetailLines;
    }

}