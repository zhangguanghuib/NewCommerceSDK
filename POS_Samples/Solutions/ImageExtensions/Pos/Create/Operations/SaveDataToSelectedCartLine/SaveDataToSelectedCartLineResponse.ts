import { Response } from "PosApi/Create/RequestHandlers";
import { ProxyEntities } from "PosApi/Entities";

export default class SaveDataToSelectedCartLineResponse extends Response {
    public cart: ProxyEntities.Cart;
    constructor(cart: ProxyEntities.Cart) {
        super();
        this.cart = cart;
    }
}