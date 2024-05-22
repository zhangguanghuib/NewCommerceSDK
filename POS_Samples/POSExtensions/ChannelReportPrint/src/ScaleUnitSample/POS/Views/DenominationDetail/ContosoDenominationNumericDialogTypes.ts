import { ProxyEntities } from "PosApi/Entities";

export type ContosoDenominationInputType = "Quantity" | "Amount"

export interface IContosoDenominationInputDialogResult {
    canceled?: boolean;
    inputType?: ContosoDenominationInputType,
    item?: ProxyEntities.DenominationDetail,
    value?: number;
}