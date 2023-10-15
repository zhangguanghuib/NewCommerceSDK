import * as Triggers from "PosApi/Extend/Triggers/ProductTriggers";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { GetCurrentCartClientRequest, GetCurrentCartClientResponse, GetReasonCodeLinesClientRequest, GetReasonCodeLinesClientResponse, RefreshCartClientRequest, RefreshCartClientResponse, SaveReasonCodeLinesOnCartLinesClientRequest, SaveReasonCodeLinesOnCartLinesClientResponse } from "PosApi/Consume/Cart";

export default class PreProductSaleTrigger extends Triggers.PreProductSaleTrigger {
    /**
     * Executes the trigger functionality.
     * @param {Triggers.IPreProductSaleTriggerOptions} options The options provided to the trigger.
     */
    public execute(options: Triggers.IPreProductSaleTriggerOptions): Promise<ClientEntities.ICancelable> {

   //     return Promise.resolve({ canceled: false });
        let UPOS_ConsoleBOInfoReasonCode = "ConsoleBO";
        let reasonCode: ProxyEntities.ReasonCodeLine[] = options.cart.ReasonCodeLines;

        let getReasonCodeLinesRequest: GetReasonCodeLinesClientRequest<GetReasonCodeLinesClientResponse>
            = new GetReasonCodeLinesClientRequest<GetReasonCodeLinesClientResponse>([UPOS_ConsoleBOInfoReasonCode]);
        return this.context.runtime.executeAsync(getReasonCodeLinesRequest)
            .then((getReasonCodeLinesResult: ClientEntities.ICancelableDataResult<GetReasonCodeLinesClientResponse>) => {
                if (getReasonCodeLinesResult.canceled) { // user cancel to select reason code
                    return null // to cancel the operation                     
                }
                return getReasonCodeLinesResult.data.result;
            }).then((value: ProxyEntities.ReasonCodeLine[]): Promise<ClientEntities.ICancelableDataResult<GetCurrentCartClientResponse>> => {
                reasonCode = value;
                let getCartRequest: GetCurrentCartClientRequest<GetCurrentCartClientResponse> = new GetCurrentCartClientRequest<GetCurrentCartClientResponse>();
                return this.context.runtime.executeAsync(getCartRequest);
            }).then((value: ClientEntities.ICancelableDataResult<GetCurrentCartClientResponse>) => {
                let cart: ProxyEntities.Cart = (<GetCurrentCartClientResponse>value.data).result;
                return cart;
            }).then((cart: ProxyEntities.Cart): Promise<ClientEntities.ICancelableDataResult<SaveReasonCodeLinesOnCartLinesClientResponse>> => {
                let cartline: ProxyEntities.CartLine
                    = cart.CartLines.filter((line: ProxyEntities.CartLine) => { line.ProductId === options.productSaleDetails[0].productId })[0];
                if (cartline) {
                    let reasonCodeLinesOnCartline: ClientEntities.IReasonCodeLinesOnCartLine = {
                        cartLineId: cartline.LineId,
                        reasonCodeLines: reasonCode
                    };
                    let saveReasonCodeLinesOnCartLinesClientRequest: SaveReasonCodeLinesOnCartLinesClientRequest<SaveReasonCodeLinesOnCartLinesClientResponse>
                        = new SaveReasonCodeLinesOnCartLinesClientRequest([reasonCodeLinesOnCartline]);
                    return this.context.runtime.executeAsync(saveReasonCodeLinesOnCartLinesClientRequest);
                } else {
                    return Promise.resolve({ data: null, canceled: true });
                }
            }).then((response: ClientEntities.ICancelableDataResult<SaveReasonCodeLinesOnCartLinesClientResponse>)
                : Promise<ClientEntities.ICancelableDataResult<RefreshCartClientResponse>> => {
                let request: RefreshCartClientRequest<RefreshCartClientResponse> = new RefreshCartClientRequest<RefreshCartClientResponse>();
                return this.context.runtime.executeAsync(request);
            }).then((result: ClientEntities.ICancelableDataResult<RefreshCartClientResponse>) => {
                return Promise.resolve({ canceled: false });
            });
    }
}