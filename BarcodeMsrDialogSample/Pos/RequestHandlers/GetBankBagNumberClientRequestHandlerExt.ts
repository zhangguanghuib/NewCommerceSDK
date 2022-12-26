import { GetBankBagNumberClientRequestHandler } from "PosApi/Extend/RequestHandlers/StoreOperationsRequestHandlers";
import { GetBankBagNumberClientRequest, GetBankBagNumberClientResponse } from "PosApi/Consume/StoreOperations";
import { ClientEntities } from "PosApi/Entities";

export default class GetBankBagNumberClientRequestHandlerExt extends GetBankBagNumberClientRequestHandler {

    public executeAsync(request: GetBankBagNumberClientRequest<GetBankBagNumberClientResponse>):
        Promise<ClientEntities.ICancelableDataResult<GetBankBagNumberClientResponse>> {

        const isoStr = new Date().toISOString();

        let response: GetBankBagNumberClientResponse = new GetBankBagNumberClientResponse({ bagNumber: isoStr });

        return Promise.resolve(<ClientEntities.ICancelableDataResult<GetBankBagNumberClientResponse>>{
            canceled: false,
            data: response
        });

        // return this.defaultExecuteAsync(request);
    }
}
