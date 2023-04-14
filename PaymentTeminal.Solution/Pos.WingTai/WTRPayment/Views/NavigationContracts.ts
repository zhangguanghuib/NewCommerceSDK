
"use strict";
import { ProxyEntities } from "PosApi/Entities";


export interface IPaymentExtensionViewModelOptions {
    tenderId: string;
    cart: ProxyEntities.Cart;
    deviceConfiguration: ProxyEntities.DeviceConfiguration;
    tenderType: ProxyEntities.TenderType;
}