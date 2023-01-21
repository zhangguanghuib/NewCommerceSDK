
import ko from "knockout";

import { ExtensionOperationRequestType, ExtensionOperationRequestHandlerBase } from "PosApi/Create/Operations";
import PaymentOperationResponse from "../PaymentOperationResponse";
import NETSOperationRequest from "./NETSOperationRequest";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { ArrayExtensions, ObjectExtensions } from "PosApi/TypeExtensions";
import { GetCurrentCartClientRequest, GetCurrentCartClientResponse } from "PosApi/Consume/Cart";
import { GetDeviceConfigurationClientRequest, GetDeviceConfigurationClientResponse } from "PosApi/Consume/Device";
import { GetOrgUnitTenderTypesClientRequest, GetOrgUnitTenderTypesClientResponse } from "PosApi/Consume/OrgUnits";

import { IPaymentExtensionViewModelOptions } from "../../Views/NavigationContracts";
import MessageDialog from "../../Controls/DialogSample/MessageDialog";

export default class NETSOperationRequestHandler<TResponse extends PaymentOperationResponse> extends ExtensionOperationRequestHandlerBase<TResponse> {


    public supportedRequestType(): ExtensionOperationRequestType<TResponse> {
        return NETSOperationRequest;
    }


    public executeAsync(request: NETSOperationRequest<TResponse>): Promise<ClientEntities.ICancelableDataResult<any>> {
        this.context.logger.logInformational("NETSOperationRequestHandler executeAsync().");

        let cart: ko.Observable<ProxyEntities.Cart> = ko.observable<ProxyEntities.Cart>(null);                                                     // The cart
        let deviceConfiguration: ko.Observable<ProxyEntities.DeviceConfiguration> = ko.observable<ProxyEntities.DeviceConfiguration>(null);        // The device configuration
        let tenderType: ko.Observable<ProxyEntities.TenderType> = ko.observable<ProxyEntities.TenderType>(null);                                   // The tender type

        let response: PaymentOperationResponse = new PaymentOperationResponse();

        let getCartRequest: GetCurrentCartClientRequest<GetCurrentCartClientResponse> =
            new GetCurrentCartClientRequest<GetCurrentCartClientResponse>();

        return this.context.runtime.executeAsync(getCartRequest)
            .then((result: ClientEntities.ICancelableDataResult<GetCurrentCartClientResponse>):
                Promise<ClientEntities.ICancelableDataResult<GetDeviceConfigurationClientResponse>> => {
                let isValid: boolean = true;
                if (!result.canceled) {
                    cart(result.data.result);

                    if (ObjectExtensions.isNullOrUndefined(cart()) ||
                        !ArrayExtensions.hasElements(cart().CartLines)) {
                        MessageDialog.show(this.context, Commerce.StringExtensions.EMPTY, false, true, false, this.context.resources.getString("string_13")).then(() => {
                            return Promise.resolve({
                                canceled: true,
                                data: null
                            });
                        });
                        isValid = false;

                    }

                    if (ObjectExtensions.isNullOrUndefined(cart().TotalAmount) || cart().TotalAmount <= 0) {
                        MessageDialog.show(this.context, "Card Payment", true, true, false, "Please add items to cart before proceeding").then(() => {
                            return Promise.resolve({
                                canceled: true,
                                data: null
                            });
                        });

                        isValid = false;
                    }
                    if (isValid) {
                        let getDeviceConfigurationRequest: GetDeviceConfigurationClientRequest<GetDeviceConfigurationClientResponse> =
                            new GetDeviceConfigurationClientRequest<GetDeviceConfigurationClientResponse>();

                        return this.context.runtime.executeAsync(getDeviceConfigurationRequest).then((result1) => {
                            deviceConfiguration(result1.data.result);

                            let tenderTypeRequest: GetOrgUnitTenderTypesClientRequest<GetOrgUnitTenderTypesClientResponse> =
                                new GetOrgUnitTenderTypesClientRequest<GetOrgUnitTenderTypesClientResponse>();

                            return this.context.runtime.executeAsync(tenderTypeRequest).then((result1: ClientEntities.ICancelableDataResult<GetOrgUnitTenderTypesClientResponse>) => {
                                result1.data.result.forEach((response) => {
                                    if (response.TenderTypeId == NETSOperationRequest.PAYMENT_METHOD) {
                                        tenderType(response);
                                    }
                                });
                                let options: IPaymentExtensionViewModelOptions = {
                                    tenderId: NETSOperationRequest.PAYMENT_METHOD,
                                    cart: cart(),
                                    deviceConfiguration: deviceConfiguration(),
                                    tenderType: tenderType()
                                };
                                this.context.navigator.navigate("PaymentExtensionView", options);
                                return <ClientEntities.ICancelableDataResult<GetDeviceConfigurationClientResponse>>{
                                    canceled: false,
                                    data: response
                                };
                            })
                        });

                    }
                }
                return Promise.resolve(<ClientEntities.ICancelableDataResult<GetDeviceConfigurationClientResponse>>{
                    canceled: true,
                    data: null
                });
            });
    }
}