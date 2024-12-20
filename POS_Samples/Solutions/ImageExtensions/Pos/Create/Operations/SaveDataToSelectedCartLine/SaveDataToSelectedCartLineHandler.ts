import { GetCurrentCartClientRequest, GetCurrentCartClientResponse, SaveExtensionPropertiesOnCartLinesClientRequest, SaveExtensionPropertiesOnCartLinesClientResponse } from "PosApi/Consume/Cart";
import { IListInputDialogItem, IListInputDialogOptions, ShowListInputDialogClientRequest, ShowListInputDialogClientResponse } from "PosApi/Consume/Dialogs";
import { ExtensionOperationRequestHandlerBase, ExtensionOperationRequestType } from "PosApi/Create/Operations";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { IExtensionContext } from "PosApi/Framework/ExtensionContext";
import { StringExtensions } from "PosApi/TypeExtensions";
import CartViewController from "../../../Extend/ViewExtensions/Cart/CartViewController";
import SaveDataToSelectedCartLineRequest from "./SaveDataToSelectedCartLineRequest";
import SaveDataToSelectedCartLineResponse from "./SaveDataToSelectedCartLineResponse";

export default class SaveDataToSelectedCartLineHandler<TResponse extends SaveDataToSelectedCartLineResponse>
    extends ExtensionOperationRequestHandlerBase<TResponse>{

    public supportedRequestType(): ExtensionOperationRequestType<TResponse> {
        return SaveDataToSelectedCartLineRequest;
    }

    public executeAsync(request: SaveDataToSelectedCartLineRequest<TResponse>): Promise<ClientEntities.ICancelableDataResult<TResponse>> {
        this.context.logger.logInformational("Log message from SaveDataToSelectedCartLineHandler executeAsync().", request.correlationId);
        let getCurrentCartRequest: GetCurrentCartClientRequest<GetCurrentCartClientResponse> = new GetCurrentCartClientRequest(request.correlationId);

        return this.context.runtime.executeAsync(getCurrentCartRequest)
            .then((getCurrentCartClientResponse: ClientEntities.ICancelableDataResult<GetCurrentCartClientResponse>) => {
                if (getCurrentCartClientResponse.canceled) {
                    return Promise.resolve(<ClientEntities.ICancelableDataResult<TResponse>>{
                        canceled: true,
                        data: null
                    });
                }

                let selectedCartLineId: string = CartViewController.selectedCartLineId;
                if (StringExtensions.isNullOrWhitespace(selectedCartLineId)) {
                    return this._showDialog(this.context, getCurrentCartClientResponse.data.result)
                        .then((dialogResult: ClientEntities.ICancelableDataResult<string>) => {
                            if (dialogResult.canceled) {
                                return Promise.resolve(<ClientEntities.ICancelableDataResult<TResponse>>{ canceled: true, data: null });
                            }

                            return this._saveDataToCartLineByCartLineId(
                                dialogResult.data,
                                request.installationDate,
                                request.correlationId,
                                getCurrentCartClientResponse.data.result
                            );
                        });
                } else {
                    return this._saveDataToCartLineByCartLineId(
                        selectedCartLineId,
                        request.installationDate,
                        request.correlationId,
                        getCurrentCartClientResponse.data.result
                    );
                }
            });
    }

    private _saveDataToCartLineByCartLineId(cartLineId: string, value: string, correlationId: string, cart: ProxyEntities.Cart):
        Promise<ClientEntities.ICancelableDataResult<TResponse>> {

        let cartLineExtensionProperty: ProxyEntities.CommerceProperty = new ProxyEntities.CommercePropertyClass();
        cartLineExtensionProperty.Key = "installationDate";
        cartLineExtensionProperty.Value = new ProxyEntities.CommercePropertyValueClass();
        cartLineExtensionProperty.Value.StringValue = value;

        let extensionPropertiesOnCartLine: ClientEntities.IExtensionPropertiesOnCartLine = {
            cartLineId: cartLineId,
            extensionProperties: [cartLineExtensionProperty]
        };

        let saveExtensionPropertiesOnCartLineRequest: SaveExtensionPropertiesOnCartLinesClientRequest<SaveExtensionPropertiesOnCartLinesClientResponse> =
            new SaveExtensionPropertiesOnCartLinesClientRequest([extensionPropertiesOnCartLine], correlationId);

        return this.context.runtime.executeAsync(saveExtensionPropertiesOnCartLineRequest)
            .then((saveExtensionPropertiesOnCartLinesClientResponse: ClientEntities.ICancelableDataResult<SaveExtensionPropertiesOnCartLinesClientResponse>) => {
                if (saveExtensionPropertiesOnCartLinesClientResponse.canceled) {
                    return Promise.resolve(<ClientEntities.ICancelableDataResult<TResponse>>{
                        canceled: true,
                        data: null
                    });
                }
                return Promise.resolve(<ClientEntities.ICancelableDataResult<TResponse>>{
                    canceled: false,
                    data: new SaveDataToSelectedCartLineResponse(cart)
                });
            });
    }

    private _showDialog(context: IExtensionContext, cart: ProxyEntities.Cart): Promise<ClientEntities.ICancelableDataResult<string>> {
        let convertedListItems: IListInputDialogItem[] = cart.CartLines.map((cartline: ProxyEntities.CartLine): IListInputDialogItem => {
            return {
                label: cartline.Description,
                value: cartline.LineId
            };
        });

        let listInputDialogOptions: IListInputDialogOptions = {
            title: "Select a cart line",
            subTitle: "Cart lines",
            items: convertedListItems
        };

        let dialogRequest: ShowListInputDialogClientRequest<ShowListInputDialogClientResponse> =
            new ShowListInputDialogClientRequest<ShowListInputDialogClientResponse>(listInputDialogOptions);

        return context.runtime.executeAsync(dialogRequest)
            .then((result: ClientEntities.ICancelableDataResult<ShowListInputDialogClientResponse>) => {
                if (result.canceled) {
                    return Promise.resolve(<ClientEntities.ICancelableDataResult<string>>{
                        canceled: true,
                        data: null
                    });
                }

                return Promise.resolve(<ClientEntities.ICancelableDataResult<string>>{
                    canceled: false,
                    data: result.data.result.value.value
                });
            });
    }
}