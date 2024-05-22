import { ProxyEntities, ClientEntities } from "PosApi/Entities";
import { ShowMessageDialogClientRequest, ShowMessageDialogClientResponse, IMessageDialogOptions } from "PosApi/Consume/Dialogs";
import * as CartOperations from "PosApi/Consume/Cart";
import * as CustomerOperations from "PosApi/Consume/Customer";
import { GetChannelConfigurationClientRequest, GetChannelConfigurationClientResponse } from "PosApi/Consume/Device";
import { StringExtensions } from "PosApi/TypeExtensions";

export namespace POSReports {
    export class POSReportsHelper {
        private context: any;
        public constructor(context: any) {
            this.context = context;
        }

        // Refresh the cart
        public RefreshCart(): Promise<ProxyEntities.Cart> {
            var cartRefreshRequest: CartOperations.RefreshCartClientRequest<CartOperations.RefreshCartClientResponse> =
                new CartOperations.RefreshCartClientRequest();
            return this.context.runtime.executeAsync(cartRefreshRequest)
                .then((cartRequestResult:
                    ClientEntities.ICancelableDataResult<CartOperations.GetCurrentCartClientResponse>) => {
                    return Promise.resolve(cartRequestResult.data.result);
                }).catch((error) => {
                });

        }

        // Get current cart 
        public GetCurrentCart(): Promise<ProxyEntities.Cart> {
            let cartRequest: CartOperations.GetCurrentCartClientRequest<CartOperations.GetCurrentCartClientResponse> =
                new CartOperations.GetCurrentCartClientRequest();
            return this.context.runtime.executeAsync(cartRequest).then((cartResult) => {
                if (!cartResult.canceled) return Promise.resolve(cartResult.data.result);
                return Promise.resolve(null);
            }).catch((error) => {
            });
        }

        // Show infos on UI
        public ShowMessage(message: string): Promise<void> {
            let promise: Promise<any> = new Promise<void>((resolve: (result: any) => any, reject: (reason?: any) => void) => {
                let messageDialogOptions: IMessageDialogOptions = {
                    title: "",
                    message: message,
                    showCloseX: true,
                    button1: {
                        id: "Button1Close",
                        label: "OK",
                        result: "OKResult"
                    }
                };

                let dialogRequest: ShowMessageDialogClientRequest<ShowMessageDialogClientResponse> =
                    new ShowMessageDialogClientRequest<ShowMessageDialogClientResponse>(messageDialogOptions);
                this.context.runtime.executeAsync(dialogRequest).then((result: ClientEntities.ICancelableDataResult<ShowMessageDialogClientResponse>) => {
                    resolve(result);
                }).catch((reason: any) => { });
            });
            return promise;
        }

        public GetExtensionPropertyStringValue(obj: any, extensionKey: string): string {
            var commercePropert: ProxyEntities.CommerceProperty[] = obj.ExtensionProperties.filter((item) => { return item.Key == extensionKey });
            return commercePropert.length > 0 ? commercePropert[0].Value.StringValue : StringExtensions.EMPTY;
        }

        public GetExtensionPropertyBooleanValue(obj: any, extensionKey: string): boolean {
            var commercePropert: ProxyEntities.CommerceProperty[] = obj.ExtensionProperties.filter((item) => { return item.Key == extensionKey });
            return commercePropert.length > 0 ? commercePropert[0].Value.BooleanValue : false;
        }

        public GetExtensionPropertyIntegerValue(obj: any, extensionKey: string): number {
            var commercePropert: ProxyEntities.CommerceProperty[] = obj.ExtensionProperties.filter((item) => { return item.Key == extensionKey });
            return commercePropert.length > 0 ? commercePropert[0].Value.IntegerValue : 0;
        }

        public AddExtensionPropertyToCart(properties: ProxyEntities.CommerceProperty[]): Promise<void> {
            var saveExtensionOnCartRequest: CartOperations.SaveExtensionPropertiesOnCartClientRequest<CartOperations.SaveExtensionPropertiesOnCartClientResponse> =
                new CartOperations.SaveExtensionPropertiesOnCartClientRequest(properties);

            return this.context.runtime.executeAsync(saveExtensionOnCartRequest).then((result) => {
                return this.RefreshCart().then(() => { });
            }).catch((reason: any) => {
                return Promise.resolve();
            });
        }

        public GetChannelConfiguration(): Promise<ProxyEntities.ChannelConfiguration> {
            return this.context.runtime.executeAsync(new GetChannelConfigurationClientRequest())
                .then((response: ClientEntities.ICancelableDataResult<GetChannelConfigurationClientResponse>) => {
                    return Promise.resolve(response.data.result);
                });
        }

        public GetCustomer(customerId: string): Promise<ProxyEntities.Customer> {
            let getCustomerRequest: CustomerOperations.GetCustomerClientRequest<CustomerOperations.GetCustomerClientResponse> =
                new CustomerOperations.GetCustomerClientRequest(customerId, this.context.logger.getNewCorrelationId());
            return this.context.runtime.executeAsync(getCustomerRequest).then((customerResult) => {
                return Promise.resolve(customerResult.data.result);
            }).catch((error) => {
                return Promise.resolve(null);
            });
        }
    }

}