"use strict";

import { ProxyEntities } from "PosApi/Entities";

export interface IPromotionViewModelOptions {
    product: ProxyEntities.SimpleProduct;
    catalogId: number;
}