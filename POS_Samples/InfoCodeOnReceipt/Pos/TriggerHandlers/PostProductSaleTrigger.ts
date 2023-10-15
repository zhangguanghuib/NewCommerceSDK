import * as Triggers from "PosApi/Extend/Triggers/ProductTriggers";

export default class PostProductSaleTrigger extends Triggers.PostProductSaleTrigger {
    /**
     * Executes the trigger functionality.
     * @param {Triggers.IPostProductSaleTriggerOptions} options The options provided to the trigger.
     */
    public execute(options: Triggers.IPostProductSaleTriggerOptions): Promise<void> {

                return Promise.resolve();

    }
}