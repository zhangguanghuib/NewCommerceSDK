import * as InventoryLookupView from "PosApi/Extend/Views/InventoryLookupView";
import { ProxyEntities } from "PosApi/Entities";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import { ObjectExtensions, StringExtensions } from "PosApi/TypeExtensions";

declare var Commerce: any;
export default class DownloadDocCommand extends InventoryLookupView.InventoryLookupExtensionCommandBase {

    public _locationAvailability: ProxyEntities.OrgUnitAvailability;
    public _locationAvailabilities: ProxyEntities.OrgUnitAvailability[];
    public mediaBaseUrl: string = Commerce.Utilities.ImageHelper.getFormattedChannelRichMediaBaseUrl(Commerce.ApplicationContext.Instance.channelConfiguration);

    /**
     * Creates a new instance of the DownloadDocCommand class.
     * @param {IExtensionCommandContext<Extensibility.IInventoryLookupToExtensionCommandMessageTypeMap>} context The command context.
     * @remarks The command context contains APIs through which a command can communicate with POS.
     */
    constructor(context: IExtensionCommandContext<InventoryLookupView.IInventoryLookupToExtensionCommandMessageTypeMap>) {
        super(context);

        this.id = "DownloadDocCommand";
        this.label = "Download document";
        this.extraClass = "iconInvoice";

        this.productChangedHandler = (data: InventoryLookupView.InventoryLookupProductChangedData): void => {
            this._productChanged(data);
        };

        this.locationSelectionHandler = (data: InventoryLookupView.InventoryLookupLocationSelectedData): void => {
            this._locationAvailability = data.locationAvailability;
            this.isVisible = !ObjectExtensions.isNullOrUndefined(this._locationAvailability);
        };
    }

    /**
     * Initializes the command.
     * @param {Extensibility.IInventoryLookupExtensionCommandState} state The state used to initialize the command.
     */
    protected init(state: InventoryLookupView.IInventoryLookupExtensionCommandState): void {
        this._locationAvailabilities = state.locationAvailabilities;
        this.isVisible = false;
    }

    /**
     * Executes the command.
     */
    protected execute(): void {
        this.isProcessing = true;
        window.setTimeout((): void => {
            this.isProcessing = false;
        }, 2000);
    }

    /**
     * Handles the product changed message by sending a message by updating the command state.
     * @param {Extensibility.InventoryLookupProductChangedData} data The information about the selected product.
     */
    private _productChanged(data: InventoryLookupView.InventoryLookupProductChangedData): void {
        this._locationAvailabilities = data.locationAvailabilities;
        const productVariantImageUrl: string = this.mediaBaseUrl + data.product.PrimaryImageUrl;
        let productMasterImageUrl: string = StringExtensions.EMPTY;
         
        data.product.ExtensionProperties.forEach((extensionProperty: ProxyEntities.CommerceProperty): void => {
            if (extensionProperty.Key === 'ProductMasterImageUrl') {
                productMasterImageUrl = this.mediaBaseUrl + extensionProperty.Value.StringValue;
            }
        });

        const productImageNode: HTMLImageElement = this.findProductImageNode();
        if (!ObjectExtensions.isNullOrUndefined(productImageNode)) {
            this.loadProductImage(productImageNode, productVariantImageUrl).then((isImageExists: boolean): Promise<boolean> => {
                if (isImageExists) {
                    return Promise.resolve(true);
                } else {
                    return this.loadProductImage(productImageNode, productMasterImageUrl);
                }
            }).then((imageLoaded: boolean) => {
                if (!imageLoaded) {
                    productImageNode.alt = data.product.Name;
                }
            });
        }

        //Commerce.Utilities.ImageHelper.ImageUrlFormatter(productImage().source, Commerce.DefaultImages.ProductLarge)
        this.canExecute = false;
        this.isVisible = false;
    }

    private findProductImageNode(): HTMLImageElement {
        const inventoryLookupDiv = document.querySelector<HTMLDivElement>('div[action="InventoryLookupView"]') as HTMLDivElement;
        let imgNode: HTMLImageElement = null;
        if (inventoryLookupDiv) {
            imgNode = inventoryLookupDiv.querySelector<HTMLImageElement>('img') as HTMLImageElement;
            if (imgNode) {
                console.log('Image node found:', imgNode);
            }
        }
        return imgNode
    }


    private loadProductImage(imageNode: HTMLImageElement, url: string): Promise<boolean> {
        return new Promise((resolve) => {
            imageNode.onload = () => resolve(true);
            imageNode.onerror = () => resolve(false);  
            imageNode.src = url;
        });
    }
}