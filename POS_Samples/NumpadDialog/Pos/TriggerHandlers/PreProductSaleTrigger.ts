import * as Triggers from "PosApi/Extend/Triggers/ProductTriggers";
import { ClientEntities } from "PosApi/Entities";
import {
    ShowNumericInputDialogClientRequest, ShowNumericInputDialogClientResponse,
    INumericInputDialogOptions
} from "PosApi/Consume/Dialogs";

export default class PreProductSaleTrigger extends Triggers.PreProductSaleTrigger {
    /**
     * Executes the trigger functionality.
     * @param {Triggers.IPreProductSaleTriggerOptions} options The options provided to the trigger.
     */
    public execute(options: Triggers.IPreProductSaleTriggerOptions): Promise<ClientEntities.ICancelable> {
        this.context.logger.logVerbose("Executing PreProductSaleTrigger with options " + JSON.stringify(options) + " at " + new Date().getTime() + ".");

        let subTitleMsg: string = "Enter voice call verification code or void transaction.\n\n"
            + "The merchant is responsible for entering a valid Auth Code.\n\n"
            + "If an invalid auth code is sent to the bank, the batch cannot be settled and the merchant may get fined ($50 for each instance currently).";

        let numericInputDialogOptions: INumericInputDialogOptions = {
            title: "Voice Call",
            subTitle: subTitleMsg,
            numPadLabel: "Please enter code:",
            defaultNumber: "4",
            decimalPrecision: 2
        };

        let dialogRequest: ShowNumericInputDialogClientRequest<ShowNumericInputDialogClientResponse> =
            new ShowNumericInputDialogClientRequest<ShowNumericInputDialogClientResponse>(numericInputDialogOptions);

        return this.context.runtime.executeAsync(dialogRequest)
            .then((result: ClientEntities.ICancelableDataResult<ShowNumericInputDialogClientResponse>) => {
                if (!result.canceled) {
                    this.context.logger.logInformational("NumericInputDialog result: " + result.data.result.value);
                    return Promise.reject(result.data.result.value);
                } else {
                    this.context.logger.logInformational("NumericInputDialog is canceled.");
                    return Promise.resolve(null);
                }
            }).catch((reason: any) => {
                this.context.logger.logError(JSON.stringify(reason));
                return Promise.reject(reason);
            });
    }
}