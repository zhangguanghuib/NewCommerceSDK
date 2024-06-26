﻿import { StringExtensions, ObjectExtensions } from "PosApi/TypeExtensions";
import { ProxyEntities } from "PosApi/Entities";
import { CancelableTriggerResult } from "PosApi/Extend/Triggers/Triggers";
import MessageDialog from "../Create/Dialogs/DialogSample/MessageDialog";
import * as Triggers from "PosApi/Extend/Triggers/SalesOrderTriggers";
import IPreElevateUserTriggerOptions = Triggers.IPreSearchOrdersTriggerOptions;

/**
 * Example implementation of an PreSearchOrdersTrigger trigger that modifies the order search criteria and logs to the console.
 */
export default class PreSearchOrdersTrigger extends Triggers.PreSearchOrdersTrigger {
    /**
     * Executes the trigger functionality.
     * The orderSearchCriteria can be updated on the newOptions and this becomes the default value in the orders search view.
     * @param {Triggers.IPreUnlockTerminalTriggerOptions} options The options provided to the trigger.
     */
    public execute(options: IPreElevateUserTriggerOptions): Promise<CancelableTriggerResult<IPreElevateUserTriggerOptions>> {
        const NEW_ORDER_SEARCH_CRITERIA: ProxyEntities.OrderSearchCriteria = {
            CustomerAccountNumber: "testCustomerAccountNumber",
            CustomerName: "testCustomerName",
            EmailAddress: "testEmailAddress",
            SalesId: "testSalesId",
            ReceiptId: "testReceiptId",
            OrderStatusValues: [1, 2, 3],
            StartDateTime: new Date(),
            EndDateTime: new Date(),
            ChannelReferenceId: "testChannelReferenceId",
            StoreId: "testStoreId",
            OrderType: ProxyEntities.CustomerOrderType.SalesOrder,
            CustomFilters: undefined
        };
        let newOptions: IPreElevateUserTriggerOptions = ObjectExtensions.clone(options);

        newOptions.orderSearchCriteria = NEW_ORDER_SEARCH_CRITERIA;

        return MessageDialog.show(this.context, StringExtensions.format("Executing PreSearchOrdersTrigger with options {0}.", JSON.stringify(newOptions)))
            .then((): Promise<CancelableTriggerResult<IPreElevateUserTriggerOptions>> => {
                // Returning a CancelableTriggerResult is needed for the data modification in the options to take effect.
                return Promise.resolve(new CancelableTriggerResult(false, newOptions));
            });
    }
}