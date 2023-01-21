

import PaymentOperationResponse from "../PaymentOperationResponse";
import CreditCardOperationRequest from "./CreditCardOperationRequest";
import { ExtensionOperationRequestFactoryFunctionType, IOperationContext } from "PosApi/Create/Operations";
import { ClientEntities } from "PosApi/Entities";
import MessageDialog from "../../Controls/DialogSample/MessageDialog"
import { GetOrgUnitTenderTypesClientRequest } from "PosApi/Consume/OrgUnits"

let getOperationRequest: ExtensionOperationRequestFactoryFunctionType<PaymentOperationResponse> =

    function (
        context: IOperationContext,
        operationId: number,
        actionParameters: string[],
        correlationId: string
    ): Promise<ClientEntities.ICancelableDataResult<CreditCardOperationRequest<PaymentOperationResponse>>> {

        let operationRequest: CreditCardOperationRequest<PaymentOperationResponse> =
            new CreditCardOperationRequest<PaymentOperationResponse>(correlationId, "10002");

        // Validate the tender for Store/ Channel
        return ValidateTenderforOrg(operationId.toString(), context)
            .then((isValid) => {
                if (isValid) {
                    return Promise.resolve(<ClientEntities.ICancelableDataResult<CreditCardOperationRequest<PaymentOperationResponse>>>{
                        canceled: false,
                        data: operationRequest
                    }).catch((error: any) => {
                        context.logger.logError("Tender Validation failed with " + JSON.stringify(error));
                        return Promise.reject("Tender Validation for store failed");
                    });
                }
                else {
                    MessageDialog.show(context, "Payment Validation Errors", true, true, false,
                        "Payment is not allowed for this Store"
                    );
                    return Promise.resolve(<ClientEntities.ICancelableDataResult<CreditCardOperationRequest<PaymentOperationResponse>>>{
                        canceled: true,
                        data: null
                    });
                }
            });

    };

export default getOperationRequest;

let ValidateTenderforOrg = function (tenderTypeId: string, context: IOperationContext): Promise<boolean> {

    return context.runtime.executeAsync(new GetOrgUnitTenderTypesClientRequest())
        .then((orgTenderTypesResponse) => {
            return orgTenderTypesResponse.data.result;
        })
        .then((tenderTypes) => {
            let tenderTypeIds: Array<string> = [];
            tenderTypes.forEach(x => {
                tenderTypeIds.push(x.TenderTypeId);
            });
            return tenderTypeIds;
        })
        .then((tenderIds) => {
            if (Commerce.ArrayExtensions.hasElement(tenderIds, tenderTypeId)) {
                // valid tender for Store
                return true;
            } else {
                return false;
            }
        })
        .catch((error: any) => {
            context.logger.logError("Error while validating custom tender for store" + JSON.stringify(error));
            return false;
        })

}