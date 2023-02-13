import { CustomPackingItem, CustomPackingItemPosition } from "PosApi/Extend/Header";

export default class CartAmountDuePackingItem extends CustomPackingItem {

    public readonly position: CustomPackingItemPosition = CustomPackingItemPosition.After;

    public onReady(packedElement: HTMLElement, unpackedElement: HTMLElement): void {
        
    }

    protected init(state: Commerce.Extensibility.ICustomHeaderPackingItemState): void {
        return;
    }

}

