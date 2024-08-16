
import { ExtensionOperationRequestType, ExtensionOperationRequestHandlerBase } from "PosApi/Create/Operations";
import AVACViewPaymTenderOperationResponse from "./AVACViewPaymTenderOperationResponse";
import AVACViewPaymTenderOperationRequest from "./AVACViewPaymTenderOperationRequest";
import { ClientEntities } from "PosApi/Entities";

/**
 * Request handler for the AVACViewPaymTenderOperationRequest class.
 */
export default class AVACViewPaymTenderOperationRequestHandler<TResponse extends AVACViewPaymTenderOperationResponse> extends ExtensionOperationRequestHandlerBase<TResponse> {
    /**
     * Gets the supported request type.
     * @return {RequestType<TResponse>} The supported request type.
     */
    public supportedRequestType(): ExtensionOperationRequestType<TResponse> {
        return AVACViewPaymTenderOperationRequest;
    }

    /**
     * Executes the request handler asynchronously.
     * @param {AVACViewPaymTenderOperationRequest<TResponse>} request The request.
     * @return {Promise<ICancelableDataResult<TResponse>>} The cancelable async result containing the response.
     */
    public executeAsync(request: AVACViewPaymTenderOperationRequest<TResponse>):
        Promise<ClientEntities.ICancelableDataResult<TResponse>> {

        this.context.logger.logInformational("Log message from AVACViewPaymTenderOperationRequestHandler executeAsync().", this.context.logger.getNewCorrelationId());
        this.context.navigator.navigate("AVACPaymTenderView");


        return Promise.resolve({
            canceled: false,
            data: <TResponse>new AVACViewPaymTenderOperationResponse()
        });

        //return Promise.resolve({
        //        canceled: false,
        //        data: null
        //    });
    }
}


//import { ExtensionOperationRequestType, ExtensionOperationRequestHandlerBase } from "PosApi/Create/Operations";
//import AVACViewPaymTenderOperationResponse from "./AVACViewPaymTenderOperationResponse";
//import AVACViewPaymTenderOperationRequest from "./AVACViewPaymTenderOperationRequest";
//import { ClientEntities } from "PosApi/Entities";

///**
// * Request handler for the AVACViewPaymTenderOperationRequest class.
// */
//export default class AVACViewPaymTenderOperationRequestHandler extends ExtensionOperationRequestHandlerBase<AVACViewPaymTenderOperationResponse> {
//    /**
//     * Gets the supported request type.
//     * @return {RequestType<TResponse>} The supported request type.
//     */
//    public supportedRequestType(): ExtensionOperationRequestType<AVACViewPaymTenderOperationResponse> {
//        return AVACViewPaymTenderOperationRequest;
//    }

//    /**
//     * Executes the request handler asynchronously.
//     * @param {AVACViewPaymTenderOperationRequest<TResponse>} request The request.
//     * @return {Promise<ICancelableDataResult<TResponse>>} The cancelable async result containing the response.
//     */
//    public executeAsync(request: AVACViewPaymTenderOperationRequest<AVACViewPaymTenderOperationResponse>):
//        Promise<ClientEntities.ICancelableDataResult<AVACViewPaymTenderOperationResponse>> {

//        this.context.logger.logInformational("Log message from AVACViewPaymTenderOperationRequestHandler executeAsync().", this.context.logger.getNewCorrelationId());
//        this.context.navigator.navigate("AVACPaymTenderView");

//        return Promise.resolve({
//            canceled: false,
//            data: null
//        });
//    }
//}