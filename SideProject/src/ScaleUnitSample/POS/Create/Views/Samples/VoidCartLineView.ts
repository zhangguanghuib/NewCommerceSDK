import * as Views from "PosApi/Create/Views";
import { ObjectExtensions } from "PosApi/TypeExtensions";
import ko from "knockout";

ko.bindingHandlers.qrcode = {
    init: function (element, valueAccessor) {
        // Create the QRCode object
        const qrCode = new QRCode(element, {
            text: ko.unwrap(valueAccessor()),
            width: 512,
            height: 512,
            colorDark: "#000000",
            colorLight: "#ffffff",
            correctLevel: QRCode.CorrectLevel.H
        });

        // Update the QRCode when the value changes
        ko.computed(() => {
            const value = ko.unwrap(valueAccessor());
            qrCode.clear();  // Clear the current QR code
            qrCode.makeCode(value);  // Generate a new QR code with the new value
        });

        // Prevent Knockout from updating the DOM element itself
        return { controlsDescendantBindings: true };
    }
};

export default class VoidCartLineView extends Views.CustomViewControllerBase {
    public qrText: ko.Observable<string>;

    /**
     * Creates a new instance of the VoidCartLineView class.
     * @param {Views.ICustomViewControllerContext} context The custom view controller context.
     * @param {any} [options] The options used to initialize the view state.
     */
    constructor(context: Views.ICustomViewControllerContext, options?: any) {
        // Do not save in history
        super(context);
        this.state.title = "Show Invoice QR Code";
        this.qrText = ko.observable("https://www.microsoft.com");
    }

    /**
     * Bind the html element with view controller.
     * @param {HTMLElement} element DOM element.
     */
    public onReady(element: HTMLElement): void {
  
        ko.applyBindings(this, element);
    }

    /**
     * Called when the object is disposed.
     */
    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }
}