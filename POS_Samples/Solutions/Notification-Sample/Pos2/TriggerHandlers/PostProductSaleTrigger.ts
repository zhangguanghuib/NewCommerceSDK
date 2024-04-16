import * as Triggers from "PosApi/Extend/Triggers/ProductTriggers";

/**
 * Example implementation of an PostProductSale trigger that logs to the console.
 */
export default class PostProductSaleTrigger extends Triggers.PostProductSaleTrigger {
    /**
     * Executes the trigger functionality.
     * @param {Triggers.IPostProductSaleTriggerOptions} options The options provided to the trigger.
     */
    public execute(options: Triggers.IPostProductSaleTriggerOptions): Promise<void> {
        console.log("Executing PostProductSaleTrigger with options " + JSON.stringify(options) + ".");
        return Promise.resolve();
    }
}