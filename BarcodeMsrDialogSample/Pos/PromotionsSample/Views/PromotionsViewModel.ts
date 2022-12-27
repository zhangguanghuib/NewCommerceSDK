import ko from "knockout";
import { AddItemToCartOperationRequest, AddItemToCartOperationResponse, GetCurrentCartClientRequest, GetCurrentCartClientResponse, TotalDiscountPercentOperationRequest, TotalDiscountPercentOperationResponse } from "PosApi/Consume/Cart";
import { ICustomViewControllerBaseState, ICustomViewControllerContext } from "PosApi/Create/Views";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { ArrayExtensions, ObjectExtensions, StringExtensions } from "PosApi/TypeExtensions";
import MessageHelps from "PromotionsSample/Utilities/MessageHelpers";
import KnockoutExtensionViewModelBase from "./BaseClasses/KnockoutExtensionViewModelBase";
import { IPromotionViewModelOptions } from "./NavigationContract";

export default class PromotionsViewModel extends KnockoutExtensionViewModelBase {
    public title: ko.Observable<string>;
    public canAddItem: ko.Computed<boolean>;
    public availablePromotions: ProxyEntities.DiscountCode[];
    public upcomingPromotions: ProxyEntities.DiscountCode[];

    private _context: ICustomViewControllerContext;
    private _product: ko.Observable<ProxyEntities.SimpleProduct>;
    private _catalogId: number;
    private _customViewControllerBaseState: ICustomViewControllerBaseState;

    constructor(context: ICustomViewControllerContext, state: ICustomViewControllerBaseState) {
        super();
        this.availablePromotions = [];
        this.upcomingPromotions = [];
        this._context = context;
        this._product = ko.observable<ProxyEntities.SimpleProduct>(null);
        this.title = ko.observable(context.resources.getString("string_0"));
        this._customViewControllerBaseState = state;
        this._customViewControllerBaseState.isProcessing = false;
        this.canAddItem = ko.computed<boolean>(() => {
            return !this._customViewControllerBaseState.isProcessing && !ObjectExtensions.isNullOrUndefined(this._product())
        }, this);
    }

    get Product(): ProxyEntities.SimpleProduct {
        return this._product();
    }

    set Product(product: ProxyEntities.SimpleProduct) {
        this._product(product);
    }

    public loadAsync(options: IPromotionViewModelOptions): Promise<void> {
        this._customViewControllerBaseState.isProcessing = true;
        this._context.logger.logInformational("PromotionsViewModel: Loading promotions");

        if (ObjectExtensions.isNullOrUndefined(options)) {
            this.Product = null;
            this._catalogId = Number.NaN;
        } else {
            this.Product = options.product;
            this._catalogId = options.catalogId;
            this.title(StringExtensions.format(
                this._context.resources.getString("string_1"),
                this.Product.Description
            ));
        }

        return this.loadPromotionsAsyn()
            .then((promotions: ProxyEntities.DiscountCode[]) => {
                if (ArrayExtensions.hasElements(promotions)) {
                    let currentDate: Date = new Date();
                    promotions.forEach((promotion: ProxyEntities.DiscountCode) => {
                        if (!ObjectExtensions.isNullOrUndefined(promotion) && promotion.IsEnabled) {
                            if ((promotion.ValidFrom <= currentDate) && (promotion.ValidTo >= currentDate)) {
                                this.availablePromotions.push(promotion);
                            }
                            else if (promotion.ValidFrom > currentDate) {
                                this.upcomingPromotions.push(promotion);
                            }
                        }
                    });

                }
                this._context.logger.logInformational("PromotionViewModel: Loading promotios succeed");
                this._customViewControllerBaseState.isProcessing = false;
            }).catch((reason: any) => {
                this._context.logger.logError("PromotionsViewModel: Loading promotions failed. Error: " + JSON.stringify(reason));
                this._customViewControllerBaseState.isProcessing = false;
                return Promise.reject(reason);
            });
    }

    public addToSaleAsync(): Promise<void> {
        return this.addItemToCart(false);
    }

    public sellNowAsync(): Promise<void> {
        return this.addItemToCart(true);
    }

    public listItemInvoked(item: any): void {
        let discount: ProxyEntities.DiscountCode = item;
        this._context.logger.logInformational("Promotion invoked:" + discount.Name);
    }

    public setTransactionDiscount(discountPercentage: number): Promise<void> {

        if (this._customViewControllerBaseState.isProcessing) {
            return Promise.resolve();
        }

        this._context.logger.logInformational("PromotionsViewModel: Setting transaction discount. Discount percentage: " + discountPercentage.toString());

        this._customViewControllerBaseState.isProcessing = true;
        let cart: ProxyEntities.Cart = null;
        let cartRequest: GetCurrentCartClientRequest<GetCurrentCartClientResponse> = new GetCurrentCartClientRequest();

        return this._context.runtime.executeAsync(cartRequest).then((result: ClientEntities.ICancelableDataResult<GetCurrentCartClientResponse>): void => {
            if (!result.canceled) {
                cart = result.data.result;
            }
        }).then((): Promise<ClientEntities.ICancelableDataResult<TotalDiscountPercentOperationResponse>> => {
            if (ObjectExtensions.isNullOrUndefined(cart)) {
                this._customViewControllerBaseState.isProcessing = false;
                let noopResponse: ClientEntities.ICancelableDataResult<TotalDiscountPercentOperationResponse> = {
                    canceled: true,
                    data: null
                };
                return Promise.resolve(noopResponse);
            }

            let request: TotalDiscountPercentOperationRequest<TotalDiscountPercentOperationResponse> =
                new TotalDiscountPercentOperationRequest(cart, this._context.logger.getNewCorrelationId(), discountPercentage);
            return this._context.runtime.executeAsync(request);
        }).then((response: ClientEntities.ICancelableDataResult<TotalDiscountPercentOperationResponse>) => {
            this._customViewControllerBaseState.isProcessing = false;
            this._context.logger.logInformational("PromotionsViewModel: Setting transaction discount succeeded");
            if (response.canceled) {
                return Promise.resolve();
            } else {
                return MessageHelps.ShowMessage(this._context,
                    this._context.resources.getString("string_27"),
                    StringExtensions.format(this._context.resources.getString("string_28"), discountPercentage.toString()
                    )
                );
            }
        }).catch((reason: any) => {
            this._customViewControllerBaseState.isProcessing = false;
            this._context.logger.logError("PromotionsViewModel: Setting total discount failed. Error: " + JSON.stringify(reason));
            return MessageHelps.ShowErrorMessage(this._context, JSON.stringify(reason), reason);
        });
    }

    private addItemToCart(quickSale: boolean): Promise<void> {
        if (this._customViewControllerBaseState.isProcessing) {
            return Promise.resolve();
        } else if (ObjectExtensions.isNullOrUndefined(this.Product)) {
            return MessageHelps.ShowMessage(
                this._context,
                this._context.resources.getString("string_30"),
                this._context.resources.getString("string_31")
            );
        }

        this._customViewControllerBaseState.isProcessing = true;

        let productSaleDetails: ClientEntities.IProductSaleReturnDetails = {
            product: this.Product,
            quantity: 1,
            catalogId: this._catalogId
        };

        let request: AddItemToCartOperationRequest<AddItemToCartOperationResponse> = new AddItemToCartOperationRequest([productSaleDetails], this._context.logger.getNewCorrelationId());
        return this._context.runtime.executeAsync(request).then((result: ClientEntities.ICancelableDataResult<AddItemToCartOperationResponse>) => {
            // If the item was added to the cart and it is a quick sale, navigate to the cart view
            if ((!result.canceled) && (quickSale)) {
                let correlationId: string = this._context.logger.logInformational("PromotionsViewModel: Navigating to CartView...");
                let cartViewParameters: ClientEntities.CartViewNavigationParameters = new ClientEntities.CartViewNavigationParameters(correlationId);
                this._context.navigator.navigateToPOSView("CartView", cartViewParameters);
            }

            // Mark that a product is currently not being added to the cart
            this._customViewControllerBaseState.isProcessing = false;
        }).catch((reason: any) => {
            // Mark that a product is currently not being added to the cart
            this._customViewControllerBaseState.isProcessing = false;
            this._context.logger.logError("PromotionsViewModel: Adding item to cart failed. Error: " + JSON.stringify(reason));

            return MessageHelps.ShowErrorMessage(
                this._context,
                JSON.stringify(reason),
                reason
            );
        });
    }

    private loadPromotionsAsyn(): Promise<ProxyEntities.DiscountCode[]> {
        if (ObjectExtensions.isNullOrUndefined(this.Product)) {
            return Promise.resolve([]);
        }

        let samplePromotions: ProxyEntities.DiscountCode[] = [];
        let samplePromotion: ProxyEntities.DiscountCodeClass;

        samplePromotion = new ProxyEntities.DiscountCodeClass();
        samplePromotion.Code = "ST100001";
        samplePromotion.Description = "Baseball sale - Disabled";
        samplePromotion.IsEnabled = false;
        samplePromotion.ValidFrom = this.getDate(-5);
        samplePromotion.ValidTo = this.getDate(100);
        samplePromotion.IsDiscountCodeRequired = false;
        samplePromotion.ConcurrencyMode = 1;
        samplePromotions.push(samplePromotion);

        samplePromotion = new ProxyEntities.DiscountCodeClass();
        samplePromotion.Code = "ST100002";
        samplePromotion.Description = "Water Bottle Promo";
        samplePromotion.IsEnabled = true;
        samplePromotion.ValidFrom = this.getDate(-1);
        samplePromotion.ValidTo = this.getDate(5);
        samplePromotion.IsDiscountCodeRequired = false;
        samplePromotion.ConcurrencyMode = 1;
        samplePromotions.push(samplePromotion);

        samplePromotion = new ProxyEntities.DiscountCodeClass();
        samplePromotion.Code = "ST100003";
        samplePromotion.Description = "BMX helmet sale";
        samplePromotion.IsEnabled = true;
        samplePromotion.ValidFrom = this.getDate(-5);
        samplePromotion.ValidTo = this.getDate(100);
        samplePromotion.IsDiscountCodeRequired = true;
        samplePromotion.ConcurrencyMode = 1;
        samplePromotions.push(samplePromotion);

        samplePromotion = new ProxyEntities.DiscountCodeClass();
        samplePromotion.Code = "ST100005";
        samplePromotion.Description = "SLR Future Discounts";
        samplePromotion.IsEnabled = true;
        samplePromotion.ValidFrom = this.getDate(1);
        samplePromotion.ValidTo = this.getDate(10);
        samplePromotion.IsDiscountCodeRequired = false;
        samplePromotion.ConcurrencyMode = 1;
        samplePromotions.push(samplePromotion);

        samplePromotion = new ProxyEntities.DiscountCodeClass();
        samplePromotion.Code = "ST100006";
        samplePromotion.Description = "Headphone Discounts - Past";
        samplePromotion.IsEnabled = true;
        samplePromotion.ValidFrom = this.getDate(-10);
        samplePromotion.ValidTo = this.getDate(-1);
        samplePromotion.IsDiscountCodeRequired = false;
        samplePromotion.ConcurrencyMode = 1;
        samplePromotions.push(samplePromotion);

        samplePromotion = new ProxyEntities.DiscountCodeClass();
        samplePromotion.Code = "ST100007";
        samplePromotion.Description = "Laptop Discounts";
        samplePromotion.IsEnabled = true;
        samplePromotion.ValidFrom = this.getDate(0);
        samplePromotion.ValidTo = this.getDate(10);
        samplePromotion.IsDiscountCodeRequired = false;
        samplePromotion.ConcurrencyMode = 1;
        samplePromotions.push(samplePromotion);

        return Promise.resolve(samplePromotions);
    }

    private getDate(days: number): Date {
        let newDate: Date = new Date();
        newDate.setDate(newDate.getDate() + days);
        return newDate;
    }

}