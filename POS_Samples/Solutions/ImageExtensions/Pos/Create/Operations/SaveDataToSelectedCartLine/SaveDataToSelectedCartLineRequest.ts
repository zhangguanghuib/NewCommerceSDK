
import { ExtensionOperationRequestBase } from "PosApi/Create/Operations";
import SaveDataToSelectedCartLineResponse from "./SaveDataToSelectedCartLineResponse";

export default class SaveDataToSelectedCartLineRequest<TResponse extends SaveDataToSelectedCartLineResponse> extends ExtensionOperationRequestBase<TResponse> {
    public readonly installationDate: string;
    constructor(correlationId: string, installationDate: string) {
        super(60004, correlationId);
        this.installationDate = installationDate;
    }
}
