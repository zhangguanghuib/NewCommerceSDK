﻿import {
    ICustomDeliveryGridColumnContext,
    CustomDeliveryGridColumnBase
} from "PosApi/Extend/Views/CartView";
import { CustomGridColumnAlignment } from "PosApi/Extend/Views/CustomGridColumns";
import { ProxyEntities } from "PosApi/Entities";

/**
 * HOW TO ENABLE THIS SAMPLE
 *
 * 1) In HQ, go to Retail > Channel setup > POS > Screen layouts
 * 2) Filter results to "Fabrikam Manager"
 * 3) Under Layout sizes, select the resolution of your MPOS, and click on Layout designer
 * 4) Download, run and sign in to the designer.
 * 5) Right click on Delivery, and click on customize.
 * 6) Find CUSTOM COLUMN 1 in Available columns and move it to Selected columns.
 * 7) Click OK and close the designer.
 * 8) Back in HQ, Go to Retail > Retail IT > Distribution schedule.
 * 9) Select job "9999" and click on Run now.
 */

export default class DeliveryCustomGridColumn1 extends CustomDeliveryGridColumnBase {

    /**
     * Creates a new instance of the DeliveryCustomGridColumn1 class.
     * @param {ICustomDeliveryGridColumnContext} context The extension context.
     */
    constructor(context: ICustomDeliveryGridColumnContext) {
        super(context);
    }

    /**
     * Gets the custom column title.
     * @return {string} The column title.
     */
    public title(): string {
        return "Installation Date"; 
    }

    /**
     * The custom column cell compute value.
     * @param {ProxyEntities.CartLine} tenderLine The tender line.
     * @return {string} The cell value.
     */
    public computeValue(cartLine: ProxyEntities.CartLine): string {

        let installationDate: string = "";

        cartLine.ExtensionProperties.forEach((extensionProperty: ProxyEntities.CommerceProperty) => {
            if (extensionProperty.Key === "InstallationDate") {
                installationDate = extensionProperty.Value.StringValue;
            }
        });

        return installationDate;
    }

    /**
     * Gets the custom column alignment.
     * @return {CustomGridColumnAlignment} The alignment.
     */
    public alignment(): CustomGridColumnAlignment {
        return CustomGridColumnAlignment.Right;
    }
}