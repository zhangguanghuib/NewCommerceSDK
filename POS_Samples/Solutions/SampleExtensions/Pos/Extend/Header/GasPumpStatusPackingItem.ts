﻿import {
    CustomPackingItem,  CustomPackingItemPosition, ICustomPackingItemContext
} from "PosApi/Extend/Header";
import ko from "knockout";
export default class GasPumpStatusPackingItem extends CustomPackingItem {

    public readonly position: CustomPackingItemPosition = CustomPackingItemPosition.After;
    private static readonly OK_STATUS_COLOR: string = "green";
    private static readonly OK_STATUS_LABEL: string = "Pump Statuses: OK";
    public backgroundColor: ko.Computed<string>;
    public label: ko.Computed<string>;

    constructor(id: string, context: ICustomPackingItemContext) {
        super(id, context);

        this.backgroundColor = ko.computed((): string => {
                return GasPumpStatusPackingItem.OK_STATUS_COLOR;
        }, this);

        this.label = ko.computed((): string => {
            return GasPumpStatusPackingItem.OK_STATUS_LABEL;
        }, this);
    }

    public onReady(packedElement: HTMLElement, unpackedElement: HTMLElement): void {
        ko.applyBindingsToNode(unpackedElement, {
            template: {
                name: "Microsoft_Pos_GasStationHeaderExtensionSample_UnpackedGasPumpStatusItem",
                data: this
            }
        });

        ko.applyBindingsToNode(packedElement, {
            template: {
                name: "Microsoft_Pos_GasStationHeaderExtensionSample_PackedGasPumpStatusItem",
                data: this
            }
        });
    }

    protected init(state: Commerce.Extensibility.ICustomHeaderPackingItemState): void {
        this.visible = false;

        document.addEventListener('wheel', function (e) {
            if (e.ctrlKey) {
                e.preventDefault();
            }
        }, { passive: false });

        document.addEventListener('keydown', function (e) {
            if ((e.ctrlKey && e.key === '+') || (e.ctrlKey && e.key === '-') || (e.ctrlKey && e.key === '0')) {
                e.preventDefault();
            }
        });
    }

    public dispose(): void {
        super.dispose();
    }

    public onItemClickedHandler(): void {
    }
}