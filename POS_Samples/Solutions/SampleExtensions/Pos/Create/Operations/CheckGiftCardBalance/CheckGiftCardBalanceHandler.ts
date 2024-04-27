import { ExtensionOperationRequestHandlerBase, ExtensionOperationRequestType } from "PosApi/Create/Operations";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import GiftCardBalanceDialog from "../../Dialogs/GiftCardBalanceDialog/GiftCardBalanceDialog";
import GiftCardNumberDialog from "../../Dialogs/GiftCardNumberDialog";
import CheckGiftCardBalanceRequest from "./CheckGiftCardBalanceRequest";
import CheckGiftCardBalanceResponse from "./CheckGiftCardBalanceResponse";

export default class CheckGiftCardBalanceHandler<TResponse extends CheckGiftCardBalanceResponse> extends ExtensionOperationRequestHandlerBase<TResponse>{

    public supportedRequestType(): ExtensionOperationRequestType<TResponse> {
        return CheckGiftCardBalanceRequest;
    }

    public executeAsync(request: CheckGiftCardBalanceRequest<TResponse>): Promise<ClientEntities.ICancelableDataResult<TResponse>> {
        let giftCardNumberDialog: GiftCardNumberDialog = new GiftCardNumberDialog();
        return giftCardNumberDialog.show(this.context, request.correlationId)
            .then((result: ClientEntities.ICancelableDataResult<ProxyEntities.GiftCard>): Promise<ClientEntities.ICancelableDataResult<TResponse>> => {
                if (result.canceled) {
                    return Promise.resolve({ canceled: true, data: null });
                }

                let giftCardBalanceDialog: GiftCardBalanceDialog = new GiftCardBalanceDialog();
                return giftCardBalanceDialog.open(result.data).then(() => {
                    return Promise.resolve({ canceled: false, data: <TResponse>new CheckGiftCardBalanceResponse() });
                });
            });
    }
}