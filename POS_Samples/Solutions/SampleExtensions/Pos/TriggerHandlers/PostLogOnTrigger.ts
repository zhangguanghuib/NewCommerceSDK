

import * as Triggers from "PosApi/Extend/Triggers/ApplicationTriggers";
declare var Commerce: any;

export default class PostLogOnTrigger extends Triggers.PostLogOnTrigger {
    /**
     * Executes the trigger functionality.
     * @param {Triggers.IPostLogOnTriggerOptions} options The options provided to the trigger.
     */
    public execute(options: Triggers.IPostLogOnTriggerOptions): Promise<any> {
        this.context.logger.logInformational("Executing PostLogOnTrigger with options " + JSON.stringify(options) + ".");

        let promise = new Promise((resolve, reject) => {
            resolve("Done");
        });

        promise.then((value: any) => {
            this.context.logger.logInformational("Promise resolved");
        });

        return promise;
    }
}