import { IAlphanumericInputDialogOptions, IAlphanumericInputDialogResult, ShowAlphanumericInputDialogClientRequest, ShowAlphanumericInputDialogClientResponse, ShowAlphanumericInputDialogError } from "PosApi/Consume/Dialogs";
import { GetGiftCardByIdServiceRequest, GetGiftCardByIdServiceResponse } from "PosApi/Consume/Payments";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { IExtensionContext } from "PosApi/Framework/ExtensionContext";
import { ObjectExtensions } from "PosApi/TypeExtensions";

export default class GiftCardNumberDialog {

    public show(context: IExtensionContext, correlationId: string): Promise<ClientEntities.ICancelableDataResult<ProxyEntities.GiftCard>> {
        let giftcard: ProxyEntities.GiftCard = null;

        let alphanumericInputDialogOptions: IAlphanumericInputDialogOptions = {
            title: "Gift Card balance",
            subTitle: "Enter the card numbetr to check the balance of the gift card.",
            numPadLabel: "Card number",
            enableBarcodeScanner: true,
            enableMagneticStripReader: true,
            onBeforeClose: ((result: ClientEntities.ICancelableDataResult<IAlphanumericInputDialogResult>): Promise<void> => {
                return this._onBeforeClose(result, context, correlationId).then((result: ProxyEntities.GiftCard) => {
                    giftcard = result;
                });
            })
        };

        let dialogRequest: ShowAlphanumericInputDialogClientRequest<ShowAlphanumericInputDialogClientResponse> =
            new ShowAlphanumericInputDialogClientRequest<ShowAlphanumericInputDialogClientResponse>(alphanumericInputDialogOptions);

        return context.runtime.executeAsync(dialogRequest).then((result: ClientEntities.ICancelableDataResult<ShowAlphanumericInputDialogClientResponse>) => {
            return Promise.resolve({ canceled: result.canceled, data: result.canceled ? null : giftcard });
        })
    }

    public _onBeforeClose(
        result: ClientEntities.ICancelableDataResult<IAlphanumericInputDialogResult>,
        context: IExtensionContext,
        correlationId: string): Promise<ProxyEntities.GiftCard> {
        if (!result.canceled) {
            if (!ObjectExtensions.isNullOrUndefined(result.data)) {
                const incorrectNumberMesage: string = "The gift card number does not existm . Enter another number";
                return this._getGiftCardByIdAsync(result.data.value, context, correlationId).then((result: ProxyEntities.GiftCard) => {
                    if (!ObjectExtensions.isNullOrUndefined(result)) {
                        return Promise.resolve(result);
                    } else {
                        let error: ShowAlphanumericInputDialogError = new ShowAlphanumericInputDialogError(incorrectNumberMesage);
                        return Promise.reject(error);
                    }
                }).catch((reason: any) => {
                    let error: ShowAlphanumericInputDialogError = new ShowAlphanumericInputDialogError(incorrectNumberMesage);
                    return Promise.reject(error);
                })
            } else {
                const noNumberMessage: string = "The gift card number is required. Enter the gift card number, and then try again";
                let error: ShowAlphanumericInputDialogError = new ShowAlphanumericInputDialogError(noNumberMessage);
                return Promise.reject(error);
            }
        } else {
            return Promise.resolve(null);
        }
    }

    private _getGiftCardByIdAsync(giftCardId: string, context: IExtensionContext, correlationId: string): Promise<ProxyEntities.GiftCard> {
        let request: GetGiftCardByIdServiceRequest<GetGiftCardByIdServiceResponse> = new GetGiftCardByIdServiceRequest(correlationId, giftCardId);
        return context.runtime.executeAsync(request).then((response: ClientEntities.ICancelableDataResult<GetGiftCardByIdServiceResponse>) => {
            return Promise.resolve(response.canceled? null: response.data.giftCard);
        })
    }
}