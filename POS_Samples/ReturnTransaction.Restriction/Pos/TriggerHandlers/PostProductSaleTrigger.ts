import { GetReasonCodeLinesClientRequest, GetReasonCodeLinesClientResponse, SaveReasonCodeLinesOnCartLinesClientRequest, SaveReasonCodeLinesOnCartLinesClientResponse } from "PosApi/Consume/Cart";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import * as Triggers from "PosApi/Extend/Triggers/ProductTriggers";

export default class PostProductSaleTrigger extends Triggers.PostProductSaleTrigger {

    public execute(options: Triggers.IPostProductSaleTriggerOptions): Promise<void> {

        let correlationId: string = this.context.logger.getNewCorrelationId();

        let getReasonCodeLinesClientRequest: GetReasonCodeLinesClientRequest<GetReasonCodeLinesClientResponse>
            = new GetReasonCodeLinesClientRequest(['IMEA'], correlationId);

        return this.context.runtime.executeAsync(getReasonCodeLinesClientRequest)
            .then((response: ClientEntities.ICancelableDataResult<GetReasonCodeLinesClientResponse>): Promise<ClientEntities.ICancelableDataResult<SaveReasonCodeLinesOnCartLinesClientResponse>> => {
                let reasonCodeLines: ProxyEntities.ReasonCodeLine[] = response.data.result;
                let reasonCodeLinesOnCartLine: ClientEntities.IReasonCodeLinesOnCartLine =  {
                    cartLineId: options.cart.CartLines[0].LineId,
                    reasonCodeLines: options.cart.CartLines[0].ReasonCodeLines.concat(reasonCodeLines)
                };

                let saveReasonCodeLinesOnCartLinesClientRequest: SaveReasonCodeLinesOnCartLinesClientRequest<SaveReasonCodeLinesOnCartLinesClientResponse>
                    = new SaveReasonCodeLinesOnCartLinesClientRequest([reasonCodeLinesOnCartLine], correlationId);
                return this.context.runtime.executeAsync(saveReasonCodeLinesOnCartLinesClientRequest);
            }).then((response: ClientEntities.ICancelableDataResult<SaveReasonCodeLinesOnCartLinesClientResponse>): Promise<void> => {
                return Promise.resolve();
            });
    }
}