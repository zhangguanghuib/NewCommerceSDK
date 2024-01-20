
import { ExtensionOperationRequestType, ExtensionOperationRequestHandlerBase } from "PosApi/Create/Operations";
import { ClientEntities } from "PosApi/Entities";
import RemoveAppliedCouponsOperationResponse from "./RemoveAppliedCouponsOperationResponse";
import RemoveAppliedCouponsOperationRequest from "./RemoveAppliedCouponsOperationRequest";
import AppliedCouponsDialog from "../../Controls/AppliedCouponsDialog";


export default class RemoveAppliedCouponsOperationRequestHandler<TResponse extends RemoveAppliedCouponsOperationResponse>
    extends ExtensionOperationRequestHandlerBase<TResponse>
{
    public supportedRequestType(): ExtensionOperationRequestType<TResponse> {
        return RemoveAppliedCouponsOperationRequest;
    }

    public executeAsync(request: RemoveAppliedCouponsOperationRequest<TResponse>): Promise<ClientEntities.ICancelableDataResult<TResponse>> {

        let response: RemoveAppliedCouponsOperationResponse = new RemoveAppliedCouponsOperationResponse();

    //    return new Promise((resolve: (value?: ClientEntities.ICancelableDataResult<TResponse>) => void) => {
    //        let appliedCouponsDialog: AppliedCouponsDialog = new AppliedCouponsDialog();
    //        return appliedCouponsDialog.open();
    //    }).then((): Promise<ClientEntities.ICancelableDataResult<TResponse> >=> {
    //        return Promise.resolve({
    //            canceled: false,
    //            data: <TResponse>response
    //        });
    //    });
    //}

            let appliedCouponsDialog: AppliedCouponsDialog = new AppliedCouponsDialog();
            return appliedCouponsDialog.open().then((): Promise<ClientEntities.ICancelableDataResult<TResponse> >=> {
            return Promise.resolve({
                canceled: false,
                data: <TResponse>response
            });
        });
    }

}