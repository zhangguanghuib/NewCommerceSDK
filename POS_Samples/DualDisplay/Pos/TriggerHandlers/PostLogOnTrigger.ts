import { GetHardwareProfileClientRequest, GetHardwareProfileClientResponse } from "PosApi/Consume/Device";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import * as Triggers from "PosApi/Extend/Triggers/ApplicationTriggers";

export default class PostLogOnTrigger extends Triggers.PostLogOnTrigger {
    /**
     * Executes the trigger functionality.
     * @param {Triggers.IPostLogOnTriggerOptions} options The options provided to the trigger.
     */
    public execute(options: Triggers.IPostLogOnTriggerOptions): Promise<void> {

        return this.context.runtime.executeAsync(new GetHardwareProfileClientRequest())
            .then((response: ClientEntities.ICancelableDataResult<GetHardwareProfileClientResponse>)
                : Promise<void> => {
                let hardwareProfile: ProxyEntities.HardwareProfile = response.data.result;
                localStorage.setItem("DualDisplayWebBrowserUrl", hardwareProfile.DualDisplayWebBrowserUrl);
                localStorage.setItem("DualDisplayImageRotatorPath", hardwareProfile.DualDisplayImageRotatorPath);
                return Promise.resolve();
            });
    }
}