
import { ExtensionOperationRequestType, ExtensionOperationRequestHandlerBase } from "PosApi/Create/Operations";
import GetMemberVouchersOperationResponse from "./GetMemberVouchersOperationResponse";
import GetMemberVouchersOperationRequest from "./GetMemberVouchersOperationRequest";
import { ClientEntities } from "PosApi/Entities";
import { ProxyEntities } from "PosApi/Entities";
import {
    GetCurrentCartClientRequest, GetCurrentCartClientResponse,
} from "PosApi/Consume/Cart";
import { StringExtensions, ObjectExtensions } from "PosApi/TypeExtensions"
import MessageDialog from "Views/Controls/Dialog/MessageDialog";
import { IMemberVouchersExtensionViewModelOptions } from "Views/MemberVouchersView/NavigationContracts";
/**
 * (Sample) Request handler for the GetMemberVouchersOperationRequest class.
 */
export default class GetMemberVouchersOperationRequestHandler<TResponse extends GetMemberVouchersOperationResponse> extends ExtensionOperationRequestHandlerBase<TResponse> {
    /**
     * Gets the supported request type.
     * @return {RequestType<TResponse>} The supported request type.
     */
    public supportedRequestType(): ExtensionOperationRequestType<TResponse> {
        return GetMemberVouchersOperationRequest;
    }

    /**
     * Executes the request handler asynchronously.
     * @param {GetMemberVouchersOperationRequest<TResponse>} request The request.
     * @return {Promise<ICancelableDataResult<TResponse>>} The cancelable async result containing the response.
     */
    public executeAsync(request: GetMemberVouchersOperationRequest<TResponse>): Promise<ClientEntities.ICancelableDataResult<any>> {

        this.context.logger.logInformational("Log message from GetMemberVouchersOperationRequestHandler executeAsync().");

        //let response: GetMemberVouchersOperationResponse = new GetMemberVouchersOperationResponse();
        //return new Promise((resolve: (value?: ClientEntities.ICancelableDataResult<GetMemberVouchersOperationResponse>) => void) => {
        // Simulating delay so that busy indicator is shown until timeout.
        let correlationId = this.context.logger.getNewCorrelationId();
        let getCurrentCartClientRequest: GetCurrentCartClientRequest<GetCurrentCartClientResponse> = new GetCurrentCartClientRequest(correlationId);
        return this.context.runtime.executeAsync(getCurrentCartClientRequest)
            .then((getCurrentCartClientResponse: ClientEntities.ICancelableDataResult<GetCurrentCartClientResponse>) => {
                if (getCurrentCartClientResponse.canceled) {
                    return Promise.resolve(<ClientEntities.ICancelableDataResult<GetCurrentCartClientResponse>>{ canceled: true, data: null });
                }
                let cart: ProxyEntities.Cart = getCurrentCartClientResponse.data.result;


                //TODO add validations for Getting Member Vouchers

                if (StringExtensions.isNullOrWhitespace(cart.CustomerId)) {
                    return MessageDialog.show(this.context, "Get member vouchers", true, true, false, "Please select customer").then(() => {
                        return Promise.resolve(<ClientEntities.ICancelableDataResult<GetCurrentCartClientResponse>>{ canceled: false, data: null });
                    });
                }


                let memberProperties: ProxyEntities.CommerceProperty[] = cart.ExtensionProperties.filter(
                    (value: ProxyEntities.CommerceProperty): boolean => {
                        return value.Key === "CRMMemberNumber";
                    }
                );
                let memberNo: string = memberProperties.length > 0 ? memberProperties[0].Value.StringValue : StringExtensions.EMPTY;

                this.context.logger.logInformational(memberNo);
                let desiredProperties: ProxyEntities.CommerceProperty[] = cart.ExtensionProperties.filter(
                    (value: ProxyEntities.CommerceProperty): boolean => {
                        return value.Key === "CRMCardNumber";
                    }
                );
                let cardNo: string = desiredProperties.length > 0 ? desiredProperties[0].Value.StringValue : StringExtensions.EMPTY;

                let membershipStatusProperties: ProxyEntities.CommerceProperty[] = cart.ExtensionProperties.filter(
                    (value: ProxyEntities.CommerceProperty): boolean => {
                        return value.Key === "CRMMembershipStatus";
                    }
                );
                let membershipStatus: string = membershipStatusProperties.length > 0 ? membershipStatusProperties[0].Value.StringValue : StringExtensions.EMPTY;

                if (membershipStatus == "PENDING PROFILE UPDATE") {
                    return MessageDialog.show(this.context, "Get member vouchers", true, true, false, "Member with membership='PENDING PROFILE UPDATE' is not allowed to make redemption.").then(() => {
                        return Promise.resolve(<ClientEntities.ICancelableDataResult<GetCurrentCartClientResponse>>{ canceled: false, data: null });
                    });
                }

                if (StringExtensions.isNullOrWhitespace(cardNo)) {
                    return MessageDialog.show(this.context, "Get member vouchers", true, true, false, "No member details").then(() => {
                        return Promise.resolve(<ClientEntities.ICancelableDataResult<GetCurrentCartClientResponse>>{ canceled: false, data: null });
                    });
                }

                if (ObjectExtensions.isNullOrUndefined(cart.TotalAmount) || cart.TotalAmount <= 0) {
                    return MessageDialog.show(this.context, "Get member vouchers", true, true, false, "Please add items to cart before proceeding").then(() => {
                        return Promise.resolve(<ClientEntities.ICancelableDataResult<GetCurrentCartClientResponse>>{ canceled: false, data: null });
                    });
                }

                if (cart.TotalManualDiscountPercentage > 0) {
                    return MessageDialog.show(this.context, "Restricted operation", true, true, false, "The member discount and the discount percentage cannot be set at the same time.").then(() => {
                        return Promise.resolve(<ClientEntities.ICancelableDataResult<GetCurrentCartClientResponse>>{ canceled: false, data: null });
                    });
                }


                this.context.logger.logInformational(cardNo);

                var transactionId: string = cart.Id;

                let options: IMemberVouchersExtensionViewModelOptions = {
                    MemberNumber: memberNo,
                    CardNo: cardNo,
                    TransactionId: transactionId
                };
                this.context.navigator.navigate("MemberVouchersView", options);

                return Promise.resolve(<ClientEntities.ICancelableDataResult<GetCurrentCartClientResponse>>{ canceled: false, data: null });
            })

            .then((result: ClientEntities.ICancelableDataResult<GetCurrentCartClientResponse>)
                : ClientEntities.ICancelableDataResult<GetMemberVouchersOperationResponse> => {

                return <ClientEntities.ICancelableDataResult<GetMemberVouchersOperationResponse>>{
                    canceled: result.canceled,
                    data: result.canceled ? null : new GetMemberVouchersOperationResponse()
                };
            });
        //});
    }
}
