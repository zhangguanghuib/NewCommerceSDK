
import * as Triggers from "PosApi/Extend/Triggers/PaymentTriggers";
import { ClientEntities} from "PosApi/Entities";

import { GetGiftCardByIdServiceRequest, GetGiftCardByIdServiceResponse } from "PosApi/Consume/Payments";

export default class PostGetGiftCardNumberTrigger extends Triggers.PostGetGiftCardNumberTrigger {

    public execute(options: Triggers.IPostGetGiftCardNumberTriggerOptions): Promise<Commerce.Client.Entities.ICancelable> {

        let correlationId: string = this.context.logger.getNewCorrelationId();
        let getGiftCardByIdServiceRequest: GetGiftCardByIdServiceRequest<GetGiftCardByIdServiceResponse> = new GetGiftCardByIdServiceRequest(correlationId, options.giftCardNumber);

        return this.context.runtime.executeAsync(getGiftCardByIdServiceRequest).then(
            (response: ClientEntities.ICancelableDataResult<GetGiftCardByIdServiceResponse>): Promise<ClientEntities.ICancelable> => {
                if (response.canceled) {
                    return Promise.resolve({ canceled: true, data: null });
                }
                else if (response.data && response.data.giftCard) {
                    return Promise.resolve({ canceled: false, data: response.data });
                } else {
                    return Promise.reject("The gift card number does not exist, please enter another number");
                }
            }).catch((reason: any) => {
                if (options.giftCardOperation === ClientEntities.GiftCardOperation.Pay) {
                    return Promise.reject(reason);
                } else {
                    return Promise.resolve(reason);
                }
            });
        //throw new Error("Method not implemented.");
    }
}