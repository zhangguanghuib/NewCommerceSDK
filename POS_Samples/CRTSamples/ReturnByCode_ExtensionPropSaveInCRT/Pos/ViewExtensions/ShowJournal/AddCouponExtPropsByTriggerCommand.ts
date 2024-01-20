import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as ShowJournalView from "PosApi/Extend/Views/ShowJournalView";
import {
    AddCouponsOperationRequest,
    AddCouponsOperationResponse,
    GetCurrentCartClientRequest, GetCurrentCartClientResponse,
    RefreshCartClientRequest, RefreshCartClientResponse,
} from "PosApi/Consume/Cart";
import { ClientEntities } from "PosApi/Entities";

import ICancelableDataResult = ClientEntities.ICancelableDataResult;
import AlphanumericInputDialog from "../../Dialogs/AlphanumericInputDialog";

export default class AddCouponExtPropsByTriggerCommand extends ShowJournalView.ShowJournalExtensionCommandBase {

    constructor(context: IExtensionCommandContext<ShowJournalView.IShowJournalToExtensionCommandMessageTypeMap>) {
        super(context);
        this.id = "AddCouponExtPropsByTriggerCommand";
        this.label = "Add Coupon Ext Props by Trigger";
        this.extraClass = "iconMoney";
    }
    protected init(state: ShowJournalView.IShowJournalExtensionCommandState): void {
        this.isVisible = true;
        this.canExecute = true;
    }
    protected execute(): Promise<void> {
        if (this.isProcessing) {
            return Promise.resolve();
        }

        this.isProcessing = true;
        console.log("Start update cart line");
        let correlationId: string = this.context.logger.getNewCorrelationId();
        let getCurrentCartClientRequest: GetCurrentCartClientRequest<GetCurrentCartClientResponse> = new GetCurrentCartClientRequest(correlationId);

        return this.context.runtime.executeAsync(getCurrentCartClientRequest)
            .then((response: ICancelableDataResult<GetCurrentCartClientResponse>): Promise<string> => {
                let alphanumericInputDialog: AlphanumericInputDialog = new AlphanumericInputDialog();
                return alphanumericInputDialog.show(this.context, "Input Coupon");
            })
            .then((couponCode: string): Promise<ClientEntities.ICancelableDataResult<AddCouponsOperationResponse>> => {
                return this.context.runtime.executeAsync(new AddCouponsOperationRequest(this.context.logger.getNewCorrelationId(), couponCode));
            })
            .then((couponsresponse: ClientEntities.ICancelableDataResult<AddCouponsOperationResponse>)
                : Promise<ClientEntities.ICancelableDataResult<RefreshCartClientResponse>> => {
                let request: RefreshCartClientRequest<RefreshCartClientResponse> = new RefreshCartClientRequest<RefreshCartClientResponse>();
                return this.context.runtime.executeAsync(request);
            }).then((result: ClientEntities.ICancelableDataResult<RefreshCartClientResponse>)
                : Promise<void> => {
                let cartViewParameters: ClientEntities.CartViewNavigationParameters = new ClientEntities.CartViewNavigationParameters(correlationId);
                this.context.navigator.navigateToPOSView("CartView", cartViewParameters);
                this.isProcessing = false;
                return Promise.resolve();
            }).catch((reason: any) => {
                console.log("there are some errors");
                this.isProcessing = false;
            });
    }
}


