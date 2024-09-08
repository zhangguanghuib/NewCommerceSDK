import {
    CustomPackingItem, ICustomPackingItemContext, CustomPackingItemPosition, ICustomPackingItemState
} from "PosApi/Extend/Header";
import ko from "knockout";
export default class GasPumpStatusPackingItem extends CustomPackingItem {
    /**
     * The position of the custom packing item relative to the out-of-the-box items.
     */
    public readonly position: CustomPackingItemPosition = CustomPackingItemPosition.After;

    /**
     * Initializes a new instance of the CartAmountDuePackingItem class.
     * @param {string} id The item identifier.
     * @param {ICustomPackingItemContext} context The custom packing item context.
     */
    constructor(id: string, context: ICustomPackingItemContext) {
        super(id, context);
    }

    /**
     * Called when the control element is ready.
     * @param {HTMLElement} packedElement The DOM element of the packed element.
     * @param {HTMLElement} unpackedElement The DOM element of the unpacked element.
     */
    public onReady(packedElement: HTMLElement, unpackedElement: HTMLElement): void {
    }

    /**
     * Initializes the control.
     * @param {ICustomPackingItemState} state The custom control state.
     */
    public init(state: ICustomPackingItemState): void {
        this.visible = false;
    }

    /**
     * Disposes the control releasing its resources.
     */
    public dispose(): void {
        super.dispose();
    }
}