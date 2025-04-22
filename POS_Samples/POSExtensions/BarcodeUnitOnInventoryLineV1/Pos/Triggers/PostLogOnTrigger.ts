import * as Triggers from "PosApi/Extend/Triggers/ApplicationTriggers";
import { IPostLogOnViewOptions } from "../Views/NavigationContracts";

declare var Commerce: any;
export default class PostLogOnTrigger extends Triggers.PostLogOnTrigger {
    public execute(options: Triggers.IPostLogOnTriggerOptions): Promise<any> {
        this.context.logger.logInformational("Executing PostLogOnTrigger with options " + JSON.stringify(options) + ".");

        let promise = new Promise((resolve, reject) => {
            let options: IPostLogOnViewOptions = {
                resolveCallback: resolve,
                rejectCallback: reject
            };

            this.context.logger.logInformational("Navigating to PostLogOnView...");
            this.context.navigator.navigate("PostLogOnView", options);
           //Commerce.ViewModelAdapter.navigate("HealthCheckView", options);
        });

        promise.then((value: any) => {
            this.context.logger.logInformational("Promise resolved");
        });

        return promise;
    }
}