import * as Triggers from "PosApi/Extend/Triggers/ApplicationTriggers";
import { StoreOperations } from "../DataService/DataServiceRequests.g";

export default class ApplicationStartTrigger extends Triggers.ApplicationStartTrigger {
    /**
     * Executes the trigger functionality.
     * @param {Triggers.IApplicationStartTriggerOptions} options The options provided to the trigger.
     */
    public async execute(options: Triggers.IApplicationStartTriggerOptions): Promise<void> {
        this.context.logger.logInformational("Executing ApplicationStartTrigger at " + new Date().getTime() + ".");

        const queryString: string = window.location.search;
        const urlParams = new URLSearchParams(queryString);
        // Operator Id
        let userid: string = urlParams.get('username') as string;
        let pwd: string = "";

        // Retrieve pwd from Key Vault if user id is not null
        if (userid) {
            let req: StoreOperations.RetrieveStaffPwdRequest<StoreOperations.RetrieveStaffPwdResponse>
                = new StoreOperations.RetrieveStaffPwdRequest(userid);
            let response: StoreOperations.RetrieveStaffPwdResponse =
                await (await this.context.runtime.executeAsync(req)).data;
            //let pwd: string = urlParams.get('password') as string;
            pwd = response.result;
        }

        if (userid && pwd) {
            var signInInterval = setInterval(function () {
                //Operator id
                let operatorBox: HTMLInputElement = document.getElementById("operatorTextBox") as HTMLInputElement;
                if (operatorBox) {
                    operatorBox.value = userid;
                    operatorBox.dispatchEvent(new Event('change', { 'bubbles': true }));
                }

                //Password
                let passwordBox: HTMLInputElement = document.getElementById("passwordBox") as HTMLInputElement;
                if (passwordBox) {
                    passwordBox.value = pwd;
                    passwordBox.dispatchEvent(new Event('change', { 'bubbles': true }));
                }

                //Signin button
                var signInButton = document.getElementById("signInButton");
                if (signInButton && passwordBox && operatorBox) {
                    clearInterval(signInInterval);
                    signInButton.click();
                }
            }, 100);
        }

        return Promise.resolve();
    }
}