import * as Triggers from "PosApi/Extend/Triggers/InventoryTriggers";

export default class ContosoPreUpdateInventoryDocumentTrigger extends Triggers.PreUpdateInventoryDocumentTrigger {

    public execute(options: Triggers.IPreUpdateInventoryDocumentTriggerOptions): Promise<any> {
        return Promise.resolve();
    }
}