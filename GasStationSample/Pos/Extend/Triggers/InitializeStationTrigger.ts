import { ApplicationStartTrigger } from "PosApi/Extend/Triggers/ApplicationTriggers";
import { GasStationDataStore } from "../../GasStationDataStore";
import ko from "knockout"

export default class InitializeStationTrigger extends ApplicationStartTrigger {

    public execute(options: Commerce.Triggers.IApplicationStartTriggerOptions): Promise<void> {
        ko.bindingHandlers.__postStopExtensionBinding = {
            init: () => {
                return {
                    controlsDescendantBindings: true
                }
            }
        };

        return GasStationDataStore.instance.initAsync(this.context);
    }

}