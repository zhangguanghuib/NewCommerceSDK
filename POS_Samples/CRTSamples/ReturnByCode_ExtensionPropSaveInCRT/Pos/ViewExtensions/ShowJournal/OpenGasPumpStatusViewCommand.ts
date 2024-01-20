import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as ShowJournalView from "PosApi/Extend/Views/ShowJournalView";

import {
    GetCurrentCartClientRequest, GetCurrentCartClientResponse,
    SaveExtensionPropertiesOnCartClientRequest,
    SaveExtensionPropertiesOnCartClientResponse,
    SaveExtensionPropertiesOnCartLinesClientRequest,
    SaveExtensionPropertiesOnCartLinesClientResponse
} from "PosApi/Consume/Cart";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";

import ICancelableDataResult = ClientEntities.ICancelableDataResult;
import { DateExtensions } from "PosApi/TypeExtensions";

export default class OpenGasPumpStatusViewCommand extends ShowJournalView.ShowJournalExtensionCommandBase {

    constructor(context: IExtensionCommandContext<ShowJournalView.IShowJournalToExtensionCommandMessageTypeMap>) {
        super(context);
        this.id = "openGasPumpStatusViewCommand";
        this.label = "open GasPumpStatusView";
        this.extraClass = "iconGo";
    }
    protected init(state: ShowJournalView.IShowJournalExtensionCommandState): void {
        this.isVisible = true;
        this.canExecute = true;
    }
    protected execute(): Promise<void> {
        if (this.isProcessing) {
            return Promise.resolve();
        }

        let dateStr: string = DateExtensions.now.toLocaleDateString();
        if (DateExtensions.isValidDate(dateStr)) {
            console.log(dateStr);
        }


        //let dateNow: Date = DateExtensions.convertStringToDateObject(dateStr);

        //console.log(dateNow);
        let dateStr2: string = "27/7/2023";;

        if (DateExtensions.isValidDate(dateStr2)) {
            console.log(dateStr2);
        }

        //let dateNow2: Date = DateExtensions.convertStringToDateObject(dateStr2);
    
        //console.log(dateNow2);

        this.isProcessing = true;
        console.log("Start update cart line");
        let correlationId: string = this.context.logger.getNewCorrelationId();
        let getCurrentCartClientRequest: GetCurrentCartClientRequest<GetCurrentCartClientResponse> = new GetCurrentCartClientRequest(correlationId);

        return this.context.runtime.executeAsync(getCurrentCartClientRequest)
            .then((response: ICancelableDataResult<GetCurrentCartClientResponse>): Promise<ClientEntities.ICancelableDataResult<SaveExtensionPropertiesOnCartClientResponse>> => {
                let couponCommercePropMinAmount: ProxyEntities.CommerceProperty = new ProxyEntities.CommercePropertyClass();
                couponCommercePropMinAmount.Key = "MinAmount";
                couponCommercePropMinAmount.Value = new ProxyEntities.CommercePropertyValueClass();
                couponCommercePropMinAmount.Value.IntegerValue = 10;

                let couponCommercePropVoucherCode: ProxyEntities.CommerceProperty = new ProxyEntities.CommercePropertyClass();
                couponCommercePropVoucherCode.Key = "VoucherCode";
                couponCommercePropVoucherCode.Value = new ProxyEntities.CommercePropertyValueClass();
                couponCommercePropVoucherCode.Value.StringValue = "VoucherCode";

                //let cart: ProxyEntities.Cart = response.data.result;
                //cart.ExtensionProperties.push(couponCommercePropVoucherCode);
                //cart.ExtensionProperties.push(couponCommercePropMinAmount);
                return this.context.runtime.executeAsync(new SaveExtensionPropertiesOnCartClientRequest([couponCommercePropVoucherCode, couponCommercePropMinAmount], correlationId));
            }).then((response: ClientEntities.ICancelableDataResult<SaveExtensionPropertiesOnCartClientResponse>): Promise<ICancelableDataResult<SaveExtensionPropertiesOnCartLinesClientResponse>> => {
                if (!response.canceled && response.data.result.CartLines.length) {
                    let cartLineId: string = response.data.result.CartLines[0].LineId;
                    //return this._updateAvailableQuantity(cartLineId, "1234", correlationId);

                    let cartLineExtensionProperty: ProxyEntities.CommerceProperty = new ProxyEntities.CommercePropertyClass();
                    cartLineExtensionProperty.Key = "QuantityAvailable";
                    cartLineExtensionProperty.Value = new ProxyEntities.CommercePropertyValueClass();
                    cartLineExtensionProperty.Value.StringValue = '1';

                    let PriceSensitiveProperty: ProxyEntities.CommerceProperty = new ProxyEntities.CommercePropertyClass();
                    PriceSensitiveProperty.Key = "PriceSensitive";
                    PriceSensitiveProperty.Value = new ProxyEntities.CommercePropertyValueClass();
                    PriceSensitiveProperty.Value.StringValue = '0';

                    let extensionPropertiesOnCartLine: ClientEntities.IExtensionPropertiesOnCartLine = {
                        cartLineId: cartLineId,
                        extensionProperties: [cartLineExtensionProperty, PriceSensitiveProperty]
                    };

                    let saveExtensionPropertiesOnCartLinesClientRequest: SaveExtensionPropertiesOnCartLinesClientRequest<SaveExtensionPropertiesOnCartLinesClientResponse> =
                        new SaveExtensionPropertiesOnCartLinesClientRequest([extensionPropertiesOnCartLine], correlationId);

                    return this.context.runtime.executeAsync(saveExtensionPropertiesOnCartLinesClientRequest)
                }
                return Promise.resolve({ canceled: response.canceled, data: null });
            }).then((response: ICancelableDataResult<SaveExtensionPropertiesOnCartLinesClientResponse>):
                Promise<void> => {
                let cartViewParameters: ClientEntities.CartViewNavigationParameters = new ClientEntities.CartViewNavigationParameters(correlationId);
                this.context.navigator.navigateToPOSView("CartView", cartViewParameters);
                this.isProcessing = false;
                return Promise.resolve();
            }).catch((reason: any) => {
                console.log("there are some errors");
                this.isProcessing = false;
            });
    }

    public saveCouponExtAsync(cart: ProxyEntities.Cart): Promise<ClientEntities.ICancelableDataResult<SaveExtensionPropertiesOnCartClientResponse>> {

        let couponCommercePropMinAmount: ProxyEntities.CommerceProperty = new ProxyEntities.CommercePropertyClass();
        couponCommercePropMinAmount.Key = "MinAmount";
        couponCommercePropMinAmount.Value = new ProxyEntities.CommercePropertyValueClass();
        couponCommercePropMinAmount.Value.IntegerValue = 10;


        let couponCommercePropVoucherCode: ProxyEntities.CommerceProperty = new ProxyEntities.CommercePropertyClass();
        couponCommercePropVoucherCode.Key = "VoucherCode";
        couponCommercePropVoucherCode.Value = new ProxyEntities.CommercePropertyValueClass();
        couponCommercePropVoucherCode.Value.StringValue = "VoucherCode";

        if (cart.Coupons.length > 0) {
            cart.Coupons[0].ExtensionProperties.push(couponCommercePropMinAmount, couponCommercePropVoucherCode);
        }

        return this.context.runtime.executeAsync(new SaveExtensionPropertiesOnCartClientRequest([]));
    }

    public _updateAvailableQuantity(cartLineId: string, value: string, correlationId: string):
        Promise<ClientEntities.ICancelableDataResult<SaveExtensionPropertiesOnCartLinesClientResponse>> {

        let cartLineExtensionProperty: ProxyEntities.CommerceProperty = new ProxyEntities.CommercePropertyClass();
        cartLineExtensionProperty.Key = "QuantityAvailable";
        cartLineExtensionProperty.Value = new ProxyEntities.CommercePropertyValueClass();
        cartLineExtensionProperty.Value.StringValue = '1';

        let PriceSensitiveProperty: ProxyEntities.CommerceProperty = new ProxyEntities.CommercePropertyClass();
        PriceSensitiveProperty.Key = "PriceSensitive";
        PriceSensitiveProperty.Value = new ProxyEntities.CommercePropertyValueClass();
        PriceSensitiveProperty.Value.StringValue = '0';

        let extensionPropertiesOnCartLine: ClientEntities.IExtensionPropertiesOnCartLine = {
            cartLineId: cartLineId,
            extensionProperties: [cartLineExtensionProperty, PriceSensitiveProperty]
        };

        let saveExtensionPropertiesOnCartLinesClientRequest: SaveExtensionPropertiesOnCartLinesClientRequest<SaveExtensionPropertiesOnCartLinesClientResponse> =
            new SaveExtensionPropertiesOnCartLinesClientRequest([extensionPropertiesOnCartLine], correlationId);

        return this.context.runtime.executeAsync(saveExtensionPropertiesOnCartLinesClientRequest)
    }
}