
import { RefreshCartClientRequest, RefreshCartClientResponse, SaveExtensionPropertiesOnCartClientRequest } from "PosApi/Consume/Cart";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import * as Triggers from "PosApi/Extend/Triggers/PaymentTriggers"
import { StringExtensions } from "PosApi/TypeExtensions";

import TextInputDialog from "../Controls/Dialogs/TextInputDialog";

export default class PrePaymentTrigger extends Triggers.PrePaymentTrigger {

    public execute(options: Triggers.IPrePaymentTriggerOptions): Promise<Commerce.Client.Entities.ICancelable> {

        this.context.logger.logInformational("Executing PrePaymentTrigger at " + new Date().getTime() + ".");

        if (options.tenderType.TenderTypeId != '13') {
            // 13 is the tender type id for Bank Transfer
            return Promise.resolve({ canceled: false })
        }

        let textInputDialog: TextInputDialog = new TextInputDialog();
        return textInputDialog.show(this.context, "银行转账备注")
            .then((result: string): Promise<Commerce.Client.Entities.ICancelable> => {
                if (!StringExtensions.isEmptyOrWhitespace(result)) {
                    let cart: ProxyEntities.Cart = options.cart;
                  
                    let bankTransferProperty: ProxyEntities.CommerceProperty = {
                        Key: "BankTransfer",
                        Value: { StringValue: result }
                    };

                    return this.context.runtime.executeAsync(new SaveExtensionPropertiesOnCartClientRequest(
                        [bankTransferProperty]
                    )).then(() => {
                        let refreshCartClientRequest: RefreshCartClientRequest<RefreshCartClientResponse>
                            = new RefreshCartClientRequest();
                        this.context.runtime.executeAsync(refreshCartClientRequest);
                    }).then(() => {
                        return Promise.resolve({ canceled: false, data: result });
                    });
                } else {
                    return Promise.resolve({ canceled: true, data: "银行转账备注信息为空" });
                }
            });
    }

}
