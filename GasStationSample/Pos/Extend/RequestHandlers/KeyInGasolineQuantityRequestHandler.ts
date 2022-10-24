import { GetKeyedInQuantityClientRequestHandler } from "PosApi/Extend/RequestHandlers/CartRequestHandlers";

export default class AutomatedKeyInGasolineQuantityRequestHandler extends GetKeyedInQuantityClientRequestHandler {

    public executeAsync(request: Commerce.GetKeyedInQuantityClientRequest<Commerce.GetKeyedInQuantityClientResponse>): Promise<Commerce.Client.Entities.ICancelableDataResult<Commerce.GetKeyedInQuantityClientResponse>> {
        throw new Error("Method not implemented.");
    }

}