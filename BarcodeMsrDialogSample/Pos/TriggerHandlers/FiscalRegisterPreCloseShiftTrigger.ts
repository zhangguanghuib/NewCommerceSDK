import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import * as Triggers from "PosApi/Extend/Triggers/OperationTriggers";

export default class FiscalRegisterPreCloseShiftTrigger extends Triggers.PreOperationTrigger {
    /**
     * Executes the trigger functionality.
     * @param {Triggers.IOperationTriggerOptions} options The options provided to the trigger.
     */
    public execute(options: Triggers.IOperationTriggerOptions): Promise<ClientEntities.ICancelable> {
        let operationId: ProxyEntities.RetailOperation = options.operationRequest.operationId;

        let isCloseShiftOperation: boolean =
            operationId === ProxyEntities.RetailOperation.BlindCloseShift ||
            operationId === ProxyEntities.RetailOperation.CloseShift ||
            operationId === ProxyEntities.RetailOperation.SuspendShift;

        if (!isCloseShiftOperation) {
            return Promise.resolve({ canceled: false });
        } else {
            return Promise.resolve({ canceled: false });
        }
    }
}