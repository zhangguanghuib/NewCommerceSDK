import * as Triggers from "PosApi/Extend/Triggers/InventoryTriggers";

export default class ContosoPreCreateInventoryDocumentTrigger extends Triggers.PreCreateInventoryDocumentTrigger {
    public execute(options: Triggers.IPreCreateInventoryDocumentTriggerOptions): Promise<any> {
        return Promise.resolve();
    }
    
}