import * as ShowJournalView from "PosApi/Extend/Views/ShowJournalView";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import { ClientEntities } from "PosApi/Entities";
import  AppliedCouponsDialog  from "../Controls/AppliedCouponsDialog";

export default class ShowAppliedCouponCommand extends ShowJournalView.ShowJournalExtensionCommandBase {

    constructor(context: IExtensionCommandContext<ShowJournalView.IShowJournalToExtensionCommandMessageTypeMap>) {
        super(context);
        this.id = "ShowAppliedCouponCommand";
        this.label = "Show Applied Coupons";
        this.extraClass = "iconPickup";
    }

    protected init(state: ShowJournalView.IShowJournalExtensionCommandState): void {
        this.isVisible = true;
        this.canExecute = true;
    }


    protected execute(): Promise<void> {
        let correlationId: string = this.context.logger.getNewCorrelationId();
        let appliedCouponsDialog: AppliedCouponsDialog = new AppliedCouponsDialog();

        return appliedCouponsDialog.open().then((): Promise<void> => {
            let cartViewParameters: ClientEntities.CartViewNavigationParameters = new ClientEntities.CartViewNavigationParameters(correlationId);
            this.context.navigator.navigateToPOSView("CartView", cartViewParameters);
            return Promise.resolve();
        });
    }
}