import { ExtensionOperationRequestType, ExtensionOperationRequestHandlerBase } from "PosApi/Create/Operations";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import OnlineOrderReceiptService from "../Services/OnlineOrderReceiptService";
import PrintOnlineOrderReceiptRequest from "./PrintOnlineOrderReceiptRequest";
import PrintOnlineOrderReceiptResponse from "./PrintOnlineOrderReceiptResponse";
import SearchTransactionsDialog from "../Controls/Dialogs/SearchTransactionsDialog";
import { ObjectExtensions } from "PosApi/TypeExtensions";

export default class PrintOnlineOrderReceiptRequestHandler<TResponse extends PrintOnlineOrderReceiptResponse>
    extends ExtensionOperationRequestHandlerBase<TResponse> {

    private timerId: number = null;
    private PrintOnlineOrderReceiptTimerId: string = 'PrintOnlineOrderReceiptTimerId';

    public executeAsync(request: PrintOnlineOrderReceiptRequest<TResponse>): Promise<ClientEntities.ICancelableDataResult<TResponse>> {

        let onlineOrderReceiptService: OnlineOrderReceiptService = new OnlineOrderReceiptService(this.context);

        let response: PrintOnlineOrderReceiptResponse = new PrintOnlineOrderReceiptResponse();

        //Debounce
        if (localStorage.getItem(this.PrintOnlineOrderReceiptTimerId)) {
            this.timerId = Number(localStorage.getItem(this.PrintOnlineOrderReceiptTimerId));
            clearInterval(this.timerId);
        }

        let dialog: SearchTransactionsDialog = new SearchTransactionsDialog();
        return dialog.open().then((searchCriteria: ProxyEntities.TransactionSearchCriteria) => {
            if (!ObjectExtensions.isNullOrUndefined(searchCriteria)) {
                this.timerId = setInterval(async (): Promise<void> => {
                    await onlineOrderReceiptService.processByAsyncAwait(searchCriteria);
                }, 5000);

                localStorage.setItem(this.PrintOnlineOrderReceiptTimerId, this.timerId.toString());
            }
            return Promise.resolve();
        }).then((): ClientEntities.ICancelableDataResult<TResponse> => {
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