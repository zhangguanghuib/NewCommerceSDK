import { AddTenderLineToCartClientResponse, GetCurrentCartClientRequest, GetCurrentCartClientResponse } from "PosApi/Consume/Cart";
import { GetOrgUnitTenderTypesClientRequest, GetOrgUnitTenderTypesClientResponse } from "PosApi/Consume/OrgUnits";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { AddTenderLineToCartClientRequestHandler } from "PosApi/Extend/RequestHandlers/CartRequestHandlers";
import { ObjectExtensions, StringExtensions } from "PosApi/TypeExtensions";
import MessageDialog from "../Controls/Dialogs/Create/MessageDialog"
import * as Messages from "../DataService/DataServiceRequests.g";

declare var Commerce: any;
export default class AddTenderLineToCartClientRequestHandlerExt extends AddTenderLineToCartClientRequestHandler {
    public executeAsync(request: Commerce.AddTenderLineToCartClientRequest<Commerce.AddTenderLineToCartClientResponse>)
        : Promise<Commerce.Client.Entities.ICancelableDataResult<Commerce.AddTenderLineToCartClientResponse>> {

        return Promise.all([this.isCashTenderType(request.tenderLine.TenderTypeId), this.getCustomerDimensionValue("Department")]).then((result: any[]) => {
            let isCashTenderType: boolean = result[0];
            let dimensionValue: string = result[1];
            if (isCashTenderType && dimensionValue === '022') {
                const msg: string = `Cash tender type is not allowed from this cusomer with Department Dimension Value: ${dimensionValue}`;
                return MessageDialog.show(this.context, msg).then(() => {
                    return Promise.resolve<ClientEntities.ICancelableDataResult<AddTenderLineToCartClientResponse>>(
                        {
                            canceled: true,
                            data: null,
                        }
                    )
                });
            } else {
                return this.defaultExecuteAsync(request);
            }
        });
    }

    private isCashTenderType(tenderTypeId: string): Promise<boolean> {
        let getOrgUnitTenderTypesClientRequest: GetOrgUnitTenderTypesClientRequest<GetOrgUnitTenderTypesClientResponse>
            = new GetOrgUnitTenderTypesClientRequest();
        return this.context.runtime.executeAsync(getOrgUnitTenderTypesClientRequest)
            .then((response: ClientEntities.ICancelableDataResult<GetOrgUnitTenderTypesClientResponse>): Promise<boolean> => {
                let allTenderTypes: ProxyEntities.TenderType[] = response.data.result;
                let cashTenderTypes: ProxyEntities.TenderType[] = allTenderTypes.filter((tenderType: ProxyEntities.TenderType) => {
                    return tenderType.OperationId === Commerce.Operations.RetailOperation.PayCash;
                });

                let isCashTenderType: boolean = false;
                for (let i: number = 0; i < cashTenderTypes.length; i++) {
                    if (cashTenderTypes[i].TenderTypeId == tenderTypeId) {
                        isCashTenderType = true;
                        break;
                    }
                }
                if (isCashTenderType) { return Promise.resolve(true) }
                else { return Promise.resolve(false) }
            }
        );
    }

    private getCustomerDimensionValue(dimensionAttribute: string): Promise<string> {
        let getCurrentCart: GetCurrentCartClientRequest<GetCurrentCartClientResponse> = new GetCurrentCartClientRequest<GetCurrentCartClientResponse>();
        return this.context.runtime.executeAsync(getCurrentCart).then(
            (response: ClientEntities.ICancelableDataResult<GetCurrentCartClientResponse>): Promise<ClientEntities.ICancelableDataResult<string>> => {
                if (!response.canceled) {
                    let cart: ProxyEntities.Cart = response.data.result;
                    if (!ObjectExtensions.isNullOrUndefined(cart)) {
                        let customerAccount: string = cart.CustomerId;
                        if (!StringExtensions.isNullOrWhitespace(customerAccount)) {
                            return Promise.resolve(
                                {
                                    canceled: false,
                                    data: customerAccount,
                                });
                        }
                    }
                }
                return Promise.resolve(
                    {
                        canceled: false,
                        data: "",
                    });
            }

        ).then((response: ClientEntities.ICancelableDataResult<string>): Promise<string> => {
            if (StringExtensions.isEmptyOrWhitespace(response.data)) {
                return Promise.resolve("");
            }
            let accountNum: string = response.data;
            let request: Messages.StoreOperations.GetCustomerDimensionByDimensionAttributeRequest<Messages.StoreOperations.GetCustomerDimensionByDimensionAttributeResponse>
                = new Messages.StoreOperations.GetCustomerDimensionByDimensionAttributeRequest<Messages.StoreOperations.GetCustomerDimensionByDimensionAttributeResponse>(accountNum, dimensionAttribute);
            return this.context.runtime.executeAsync(request).then((response: ClientEntities.ICancelableDataResult<Messages.StoreOperations.GetCustomerDimensionByDimensionAttributeResponse>): Promise<string> => {
                if (!response.canceled) {
                    let dimensionValue: string = response.data.result;
                    if (!StringExtensions.isNullOrWhitespace(dimensionValue)) {
                        return Promise.resolve(dimensionValue);
                    }
                }
                return Promise.resolve("");
            })
        });
    }
}
