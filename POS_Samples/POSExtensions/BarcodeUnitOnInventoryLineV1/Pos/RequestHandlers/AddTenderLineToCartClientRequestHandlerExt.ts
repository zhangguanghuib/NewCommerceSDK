import { AddTenderLineToCartClientResponse } from "PosApi/Consume/Cart";
import { GetOrgUnitTenderTypesClientRequest, GetOrgUnitTenderTypesClientResponse } from "PosApi/Consume/OrgUnits";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { AddTenderLineToCartClientRequestHandler } from "PosApi/Extend/RequestHandlers/CartRequestHandlers";
import MessageDialog from "../Controls/Dialogs/Create/MessageDialog"

declare var Commerce: any;
export default class AddTenderLineToCartClientRequestHandlerExt extends AddTenderLineToCartClientRequestHandler {

    public executeAsync(request: Commerce.AddTenderLineToCartClientRequest<Commerce.AddTenderLineToCartClientResponse>)
        : Promise<Commerce.Client.Entities.ICancelableDataResult<Commerce.AddTenderLineToCartClientResponse>> {

        let getOrgUnitTenderTypesClientRequest: GetOrgUnitTenderTypesClientRequest<GetOrgUnitTenderTypesClientResponse>
            = new GetOrgUnitTenderTypesClientRequest();
        return this.context.runtime.executeAsync(getOrgUnitTenderTypesClientRequest)
            .then((response: ClientEntities.ICancelableDataResult<GetOrgUnitTenderTypesClientResponse>): Promise<ClientEntities.ICancelableDataResult<Commerce.AddTenderLineToCartClientResponse>> => {
                let allTenderTypes: ProxyEntities.TenderType[] = response.data.result;
                let cashTenderTypes: ProxyEntities.TenderType[] = allTenderTypes.filter((tenderType: ProxyEntities.TenderType) => {
                    return tenderType.OperationId === Commerce.Operations.RetailOperation.PayCash;
                });

                let isCashTenderType: boolean = false;
                for (let i: number = 0; i < cashTenderTypes.length; i++) {
                    if (cashTenderTypes[i].TenderTypeId == request.tenderLine.TenderTypeId) {
                        isCashTenderType = true;
                        break;
                    }
                }
                if (isCashTenderType) {                    
                    const msg: string = "Cash tender type is not allowed.";
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
}