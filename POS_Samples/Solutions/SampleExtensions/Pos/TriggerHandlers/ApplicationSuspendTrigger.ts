
import * as Triggers from "PosApi/Extend/Triggers/ApplicationTriggers";

export default class ApplicationSuspendTrigger extends Triggers.ApplicationSuspendTrigger {
    /**
     * Executes the trigger functionality.
     * @param {Triggers.IApplicationSuspendTriggerOptions} options The options provided to the trigger.
     */
    public execute(options: Triggers.IApplicationSuspendTriggerOptions): Promise<void> {
        this.context.logger.logInformational("Executing ApplicationSuspendTrigger at " + new Date().getTime() + ".");
        return Promise.resolve();
    }
}