import ko from "knockout";

import {
    CartViewCustomControlBase,
    ICartViewCustomControlState,
    ICartViewCustomControlContext,
    CartLineSelectedData
} from "PosApi/Extend/Views/CartView";

import { ClientEntities } from "PosApi/Entities";
import { ProxyEntities } from "PosApi/Entities";
import {
    ObjectExtensions,
    StringExtensions,
    ArrayExtensions
} from "PosApi/TypeExtensions";

import {
    GetDeviceConfigurationClientRequest,
    GetDeviceConfigurationClientResponse
} from "PosApi/Consume/Device";

import { StoreOperations } from "../../DataService/DataServiceRequests.g";

export default class LineDetailsCustomControl extends CartViewCustomControlBase {
    private static readonly TEMPLATE_ID: string = "Microsoft_Pos_Extensibility_Samples_LineDetails";
    public readonly cartLineItemId: ko.Computed<string>;
    public readonly cartLineDescription: ko.Computed<string>;
    public readonly isCartLineSelected: ko.Computed<boolean>;
    private readonly _cartLine: ko.Observable<ProxyEntities.CartLine>;
    private currentChannelId: number;

    public _state: ICartViewCustomControlState;

    public orderNoList: ko.Observable<string>;
    public _isLoaderVisible: ko.Observable<boolean>;

    public constructor(id: string, context: ICartViewCustomControlContext) {
        super(id, context);
        this._cartLine = ko.observable(null);
        this._isLoaderVisible = ko.observable(false);
        this.orderNoList = ko.observable("");

        this.cartLineItemId = ko.computed(() => {
            let cartLine: ProxyEntities.CartLine = this._cartLine();
            if (!ObjectExtensions.isNullOrUndefined(cartLine)) {
                return cartLine.ItemId;
            }
            return StringExtensions.EMPTY;
        });

        this.cartLineDescription = ko.computed(() => {
            let cartLine: ProxyEntities.CartLine = this._cartLine();
            if (!ObjectExtensions.isNullOrUndefined(cartLine)) {
                return cartLine.Description;
            }
            return StringExtensions.EMPTY;
        });

        this.isCartLineSelected = ko.computed(() => !StringExtensions.isEmptyOrWhitespace(this.orderNoList()));
        this.cartLineSelectedHandler = (data: CartLineSelectedData) => {
            if (ArrayExtensions.hasElements(data.cartLines)) {
                this._cartLine(data.cartLines[0]);
            }
        };

        this.cartLineSelectionClearedHandler = () => {
            this._cartLine(null);
        };
    }

    /**
    * Binds the control to the specified element.
    * @param {HTMLElement} element The element to which the control should be bound.
    */
    public onReady(element: HTMLElement): void {
        ko.applyBindingsToNode(element, {
            template: {
                name: LineDetailsCustomControl.TEMPLATE_ID,
                data: this
            }
        }, this);

        this.addRedDot();
    }

    /**
    * Initializes the control.
    * @param {ICartViewCustomControlState} state The initial state of the page used to initialize the control.
    */
    public init(state: ICartViewCustomControlState): void {
        this._state = state;

        this.context.runtime.executeAsync(new GetDeviceConfigurationClientRequest())
            .then((response: ClientEntities.ICancelableDataResult<GetDeviceConfigurationClientResponse>): ProxyEntities.DeviceConfiguration => {
                return response.data.result;
            }).then((deviceConfiguration: ProxyEntities.DeviceConfiguration)
                : Promise<void> => {
                this.currentChannelId = deviceConfiguration.ChannelId;
                return Promise.resolve();
            });

        setInterval((): void => {
            let request: StoreOperations.GetPickupOrdersCreatedFromOtherStoreRequest<StoreOperations.GetPickupOrdersCreatedFromOtherStoreResponse>
                = new StoreOperations.GetPickupOrdersCreatedFromOtherStoreRequest(this.currentChannelId);

            this.context.runtime.executeAsync(request).then((getPickupOrdersCreatedFromOtherStoreResponse: ClientEntities.ICancelableDataResult<StoreOperations.GetPickupOrdersCreatedFromOtherStoreResponse>):
                Promise<string> => {
                if (getPickupOrdersCreatedFromOtherStoreResponse.canceled) {
                    return Promise.resolve("");
                } else {
                    if (getPickupOrdersCreatedFromOtherStoreResponse.data.result.length <= 0) {
                        return Promise.resolve("");
                    } else {
                        let orderNoStr: string = '';
                        getPickupOrdersCreatedFromOtherStoreResponse.data.result.forEach((order: ProxyEntities.SalesOrder) => {
                            orderNoStr += order.SalesId + " ";
                        });
                        console.log(orderNoStr);
                        return Promise.resolve(orderNoStr);
                    }
                }
            }).then((orderlist: string) => {
                this.orderNoList(orderlist);
                let divRedDot: HTMLElement = document.getElementById('notifyRedDot');
                if (orderlist.length > 0) {
                    if (!this._isLoaderVisible()) {
                        this._isLoaderVisible(true);
                    }
                    divRedDot.style.visibility = "visible";
                } else {
                    
                    divRedDot.style.visibility = "hidden";
                }
                setTimeout((): void => {
                    this._isLoaderVisible(false);
                }, 4000);
            });
        }, 10000);
    }


    public addRedDot(): void {
        //let notificationIcon: HTMLCollectionOf<Element> = document.getElementsByClassName("iconActionCenterNotification");
        const notificationIcons: Element[] = Array.from(document.querySelectorAll('[data-bind*="showNotificationCenterDialog"]'));
    
        if (!ObjectExtensions.isNullOrUndefined(notificationIcons) && notificationIcons.length > 0) {
            let notificationDiv: HTMLDivElement = notificationIcons[0] as HTMLDivElement;
            notificationDiv.classList.add("relativePos");

            let divRedDot: HTMLDivElement = document.createElement("div");
            divRedDot.id = "notifyRedDot"
            divRedDot.classList.add('reddot');
            notificationDiv.appendChild(divRedDot);
            //console.log(notificationDiv);
        }
    }
}