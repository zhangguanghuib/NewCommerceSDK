import { CreateEmptyCartServiceRequest, CreateEmptyCartServiceResponse, SetCustomerOnCartOperationRequest, SetCustomerOnCartOperationResponse } from "PosApi/Consume/Cart";
import { ClientEntities } from "PosApi/Entities";
import * as ShowJournalView from "PosApi/Extend/Views/ShowJournalView";
export default class CreateCartCommand extends ShowJournalView.ShowJournalExtensionCommandBase {

    protected init(state: ShowJournalView.IShowJournalExtensionCommandState): void {
        this.id = "CreateCartCommand";
        this.label = "Create Cart";
        this.extraClass = "iconInvoice";
    }

    protected execute(): void {
        let correlationId: string = this.context.logger.getNewCorrelationId();
        let createEmptyCartServiceRequest: CreateEmptyCartServiceRequest =
            new CreateEmptyCartServiceRequest(correlationId);

        this.context.runtime.executeAsync(createEmptyCartServiceRequest)
            .then((result: ClientEntities.ICancelableDataResult<CreateEmptyCartServiceResponse>) => {
                if (!result.canceled) {
                    let setCustomerOnCartOperationRequest: SetCustomerOnCartOperationRequest<SetCustomerOnCartOperationResponse>
                        = new SetCustomerOnCartOperationRequest(correlationId, '004007');
                    return this.context.runtime.executeAsync(setCustomerOnCartOperationRequest);
                } else {
                    return Promise.resolve(<ClientEntities.ICancelableDataResult<SetCustomerOnCartOperationResponse>>{ canceled: true, data: null });
                }
            }).then((result: ClientEntities.ICancelableDataResult<SetCustomerOnCartOperationResponse>) => {
                let cartViewOptions: ClientEntities.CartViewNavigationParameters = new ClientEntities.CartViewNavigationParameters(correlationId);
                this.context.navigator.navigateToPOSView("CartView", cartViewOptions);
            });
    }
}