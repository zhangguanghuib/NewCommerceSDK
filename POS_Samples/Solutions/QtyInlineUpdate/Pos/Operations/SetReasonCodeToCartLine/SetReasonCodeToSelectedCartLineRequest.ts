import { ExtensionOperationRequestBase } from "PosApi/Create/Operations";
import SetReasonCodeToSelectedCartLineResponse from "./SetReasonCodeToSelectedCartLineResponse";
export default class SetReasonCodeToSelectedCartLineRequest<TResponse extends SetReasonCodeToSelectedCartLineResponse> extends ExtensionOperationRequestBase<TResponse>
{
    constructor(correlationId: string) {
        super(9003, correlationId);
    }
}