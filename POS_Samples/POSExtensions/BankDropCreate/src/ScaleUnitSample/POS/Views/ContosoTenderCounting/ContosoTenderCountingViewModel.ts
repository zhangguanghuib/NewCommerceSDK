import { GetChannelConfigurationClientResponse, GetChannelConfigurationClientRequest } from "PosApi/Consume/Device";
import { GetOrgUnitTenderTypesClientRequest, GetOrgUnitTenderTypesClientResponse } from "PosApi/Consume/OrgUnits";
import { GetCurrenciesServiceRequest, GetCurrenciesServiceResponse } from "PosApi/Consume/StoreOperations";
import { IExtensionViewControllerContext } from "PosApi/Create/Views";
import {  ClientEntities, ProxyEntities } from "PosApi/Entities";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import { ContosoTenderCountingLine } from "./ContosoTenderCountingLine";

import * as Messages from "../../DataService/DataServiceRequests.g";
import { Contoso } from "./Dictionary";





declare var Commerce: any;

export default class ContosoTenderCountingViewModel {

    public tenderDetails: ContosoTenderCountingLine[];
    

    public title: string;
    public _context: IExtensionViewControllerContext;
    public _selectedItem: ProxyEntities.TenderDetail;
    public isItemSelected: () => boolean;
    public ChannelConfig: ProxyEntities.ChannelConfiguration;
    public tenderTypres: ProxyEntities.TenderType;

    constructor(context: IExtensionViewControllerContext) {
        this._context = context;
        this.title = context.resources.getString("string_0001");
        this.tenderDetails = [];
        this.isItemSelected = () => !ObjectExtensions.isNullOrUndefined(this._selectedItem);
        //this.tenderTypes:  ProxyEntities.TenderType[];

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