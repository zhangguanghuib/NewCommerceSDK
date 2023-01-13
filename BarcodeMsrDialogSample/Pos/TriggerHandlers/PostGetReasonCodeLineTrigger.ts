import * as Triggers from "PosApi/Extend/Triggers/ReasonCodeTriggers";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { ObjectExtensions, StringExtensions} from "PosApi/TypeExtensions";

export default class PostGetReasonCodeLineTrigger extends Triggers.PostGetReasonCodeLineTrigger {
    public execute(options: Triggers.IPostGetReasonCodeLineTriggerOptions): Promise<ClientEntities.ICancelable> {

        let prop  =  options.reasonCode.ExtensionProperties.filter((prop: ProxyEntities.CommerceProperty) => prop.Key === "BeginWith")
            .map((prop: ProxyEntities.CommerceProperty) => prop.Value)[0];

        if (!ObjectExtensions.isNullOrUndefined(prop)) {
            if (prop.StringValue !== "" && StringExtensions.beginsWith(options.reasonCodeLine.Information, prop.StringValue, false)) {
                return Promise.resolve({ canceled: false, data: options });
            } else {
                return Promise.reject(new ClientEntities.ExtensionError("The input information does not match require pattern"));
            }
        } else {
            return Promise.resolve({ canceled: false, data: options });
        }
    }
}