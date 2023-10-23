import SetReasonCodeToSelectedCartLineResponse from "./SetReasonCodeToSelectedCartLineResponse";
import SetReasonCodeToSelectedCartLineRequest from "./SetReasonCodeToSelectedCartLineRequest";
import { ExtensionOperationRequestHandlerBase, ExtensionOperationRequestType } from "PosApi/Create/Operations";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { GetCurrentCartClientRequest, GetCurrentCartClientResponse, GetReasonCodeLinesClientRequest, GetReasonCodeLinesClientResponse, SaveReasonCodeLinesOnCartLinesClientRequest, SaveReasonCodeLinesOnCartLinesClientResponse } from "PosApi/Consume/Cart";
import CartViewController from "../../ViewExtensions/Cart/CartViewController";
import { StringExtensions } from "PosApi/TypeExtensions";
import { IExtensionContext } from "PosApi/Framework/ExtensionContext";
import { IListInputDialogItem, IListInputDialogOptions, ShowListInputDialogClientRequest, ShowListInputDialogClientResponse } from "PosApi/Consume/Dialogs";
export default class SetReasonCodeToSelectedCartLineHandler<TResponse extends SetReasonCodeToSelectedCartLineResponse>
    extends ExtensionOperationRequestHandlerBase<TResponse>
{
    private reasonCode: string = "IMEA";

    public supportedRequestType(): ExtensionOperationRequestType<TResponse> {
        return SetReasonCodeToSelectedCartLineRequest;
    }

    public executeAsync(request: SetReasonCodeToSelectedCartLineRequest<TResponse>): Promise<ClientEntities.ICancelableDataResult<TResponse>> {

        let getCurrentCartClientRequest: GetCurrentCartClientRequest<GetCurrentCartClientResponse> = new GetCurrentCartClientRequest(request.correlationId);

        return this.context.runtime.executeAsync(getCurrentCartClientRequest)
            .then((getCurrentCartClientResponse: ClientEntities.ICancelableDataResult<GetCurrentCartClientResponse>) => {
                if (getCurrentCartClientResponse.canceled) {
                    return Promise.resolve(<ClientEntities.ICancelableDataResult<TResponse>>{ canceled: true, data: null });
                }

                let selectedCartLineId: string = CartViewController.selectedCartLineId;
                if (StringExtensions.isNullOrWhitespace(selectedCartLineId)) {
                    return this._showDialog(this.context, getCurrentCartClientResponse.data.result)
                        .then((dialogResult: ClientEntities.ICancelableDataResult<string>) => {
                            if (dialogResult.canceled) {
                                return Promise.resolve(<ClientEntities.ICancelableDataResult<TResponse>>{ canceled: true, data: null });
                            } else {
                                return this._setReasonCodeToCartLineByCartLineId(
                                    dialogResult.data,
                                    this.reasonCode,
                                    request.correlationId,
                                    getCurrentCartClientResponse.data.result);
                            }
                        });
                } else {
                    return this._setReasonCodeToCartLineByCartLineId(
                        selectedCartLineId,
                        this.reasonCode,
                        request.correlationId,
                        getCurrentCartClientResponse.data.result);
                }
            });
    }

    private _setReasonCodeToCartLineByCartLineId(cartLineId: string, reasonCode: string, correlationId: string, cart: ProxyEntities.Cart):
        Promise<ClientEntities.ICancelableDataResult<TResponse>> {

        let cartLine: ProxyEntities.CartLine = cart.CartLines.filter(l => l.LineId === cartLineId)[0];

        let getReasonCodeLinesClientRequest: GetReasonCodeLinesClientRequest<GetReasonCodeLinesClientResponse>
            = new GetReasonCodeLinesClientRequest([reasonCode], correlationId);

        return this.context.runtime.executeAsync(getReasonCodeLinesClientRequest)
            .then((response: ClientEntities.ICancelableDataResult<GetReasonCodeLinesClientResponse>): Promise<ClientEntities.ICancelableDataResult<SaveReasonCodeLinesOnCartLinesClientResponse>> => {
                let reasonCodeLines: ProxyEntities.ReasonCodeLine[] = response.data.result;
                let reasonCodeLinesOnCartLine: ClientEntities.IReasonCodeLinesOnCartLine = {
                    cartLineId: cartLineId,
                    reasonCodeLines: cartLine.ReasonCodeLines.concat(reasonCodeLines)
                };

                let saveReasonCodeLinesOnCartLinesClientRequest: SaveReasonCodeLinesOnCartLinesClientRequest<SaveReasonCodeLinesOnCartLinesClientResponse>
                    = new SaveReasonCodeLinesOnCartLinesClientRequest([reasonCodeLinesOnCartLine], correlationId);
                return this.context.runtime.executeAsync(saveReasonCodeLinesOnCartLinesClientRequest);
            }).then((response: ClientEntities.ICancelableDataResult<SaveReasonCodeLinesOnCartLinesClientResponse>) => {
                if (response.canceled) {
                    return Promise.resolve(<ClientEntities.ICancelableDataResult<TResponse>>{ canceled: true, data: null });
                } else {
                    return Promise.resolve(<ClientEntities.ICancelableDataResult<TResponse>>{
                        canceled: false,
                        data: new SetReasonCodeToSelectedCartLineResponse(cart)
                    });
                }
            });
    }

    private _showDialog(context: IExtensionContext, cart: ProxyEntities.Cart): Promise<ClientEntities.ICancelableDataResult<string>> {
        let convertedListItems: IListInputDialogItem[] = cart.CartLines.map((cartLine: ProxyEntities.CartLine): IListInputDialogItem => {
            return {
                label: cartLine.Description,
                value: cartLine.LineId
            };
        });

        let listInputDialogOptions: IListInputDialogOptions = {
            title: "Select cart line",
            subTitle: "Cart lines",
            items: convertedListItems
        };

        let dialogRequest: ShowListInputDialogClientRequest<ShowListInputDialogClientResponse> =
            new ShowListInputDialogClientRequest<ShowListInputDialogClientResponse>(listInputDialogOptions);

        return this.context.runtime.executeAsync(dialogRequest)
            .then((result: ClientEntities.ICancelableDataResult<ShowListInputDialogClientResponse>) => {
                if (result.canceled) {
                    return Promise.resolve(<ClientEntities.ICancelableDataResult<string>>{ canceled: true, data: StringExtensions.EMPTY });
                } else {
                    return Promise.resolve(<ClientEntities.ICancelableDataResult<string>>{ canceled: true, data: result.data.result.value.value });
                }
            })
    }

}