import { ExtensionOperationRequestType, ExtensionOperationRequestHandlerBase } from "PosApi/Create/Operations";
import { ClientEntities } from "PosApi/Entities";
import OnlineOrderReceiptService from "../Services/OnlineOrderReceiptService";
import PrintOnlineOrderReceiptRequest from "./PrintOnlineOrderReceiptRequest";
import PrintOnlineOrderReceiptResponse from "./PrintOnlineOrderReceiptResponse";

export default class PrintOnlineOrderReceiptRequestHandler<TResponse extends PrintOnlineOrderReceiptResponse>
    extends ExtensionOperationRequestHandlerBase<TResponse> {

    public executeAsync(request: PrintOnlineOrderReceiptRequest<TResponse>): Promise<ClientEntities.ICancelableDataResult<TResponse>> {

        let onlineOrderReceiptService: OnlineOrderReceiptService = new OnlineOrderReceiptService(this.context);

        let response: PrintOnlineOrderReceiptResponse = new PrintOnlineOrderReceiptResponse();

        setInterval(async (): Promise<void> => {
            await onlineOrderReceiptService.processByAsyncAwait();
        }, 5000);

        return Promise.resolve().then((): ClientEntities.ICancelableDataResult<TResponse> => {
            return <ClientEntities.ICancelableDataResult<TResponse>>{
                canceled: false,
                data: response
            };
        });
    }

    public supportedRequestType(): ExtensionOperationRequestType<TResponse> {
        return PrintOnlineOrderReceiptRequest;
    }

}