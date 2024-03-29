﻿import * as Triggers from "PosApi/Extend/Triggers/ProductTriggers";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import { ClientEntities } from "PosApi/Entities";

export default class PreProductSaleTrigger extends Triggers.PreProductSaleTrigger {
    /**
     * Executes the trigger functionality.
     * @param {Triggers.IPreProductSaleTriggerOptions} options The options provided to the trigger.
     */
    public execute(options: Triggers.IPreProductSaleTriggerOptions): Promise<ClientEntities.ICancelable> {
        this.context.logger.logVerbose("Executing PreProductSaleTrigger with options " + JSON.stringify(options) + " at " + new Date().getTime() + ".");

        if (ObjectExtensions.isNullOrUndefined(options)) {
            // This will never happen, but is included to demonstrate how to return a rejected promise when validation fails.
            let error: ClientEntities.ExtensionError
                = new ClientEntities.ExtensionError("The options provided to the PreProductSaleTrigger were invalid. Please select a product and try again.");

            return Promise.reject(error);
        } else {
            return Promise.resolve({ canceled: false });
        }
    }
}