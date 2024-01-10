import OnlineOrderReceiptPrintClientRequest from "./OnlineOrderReceiptPrintClientRequest";
import OnlineOrderReceiptPrintClientResponse from "./OnlineOrderReceiptPrintClientResponse";
import { ClientEntities} from "PosApi/Entities";
import { ExtensionRequestHandlerBase, ExtensionRequestType } from "PosApi/Create/RequestHandlers";
import PrintOnlineOrderReceiptResponse from "../Operations/PrintOnlineOrderReceiptResponse";

export default class OnlineOrderReceiptPrintHandler<TResponse extends PrintOnlineOrderReceiptResponse> extends ExtensionRequestHandlerBase<TResponse>
{
    public supportedRequestType(): ExtensionRequestType<TResponse> {
        return OnlineOrderReceiptPrintClientRequest;
    }

    public executeAsync(request: OnlineOrderReceiptPrintClientRequest<TResponse>): Promise<ClientEntities.ICancelableDataResult<TResponse>> {
        console.log(request.searchCriteria);
        let res: OnlineOrderReceiptPrintClientResponse = new OnlineOrderReceiptPrintClientResponse();

        return Promise.resolve(<ClientEntities.ICancelableDataResult<TResponse>> {
            canceled: false,
            data: res
        });
    }
}
