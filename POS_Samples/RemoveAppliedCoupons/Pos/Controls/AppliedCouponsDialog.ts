import ko from "knockout";

import * as Dialogs from "PosApi/Create/Dialogs";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import { ArrayExtensions, ObjectExtensions} from "PosApi/TypeExtensions";
import { IDataList, DataListInteractionMode } from "PosApi/Consume/Controls";
import { GetCurrentCartClientRequest, GetCurrentCartClientResponse, RefreshCartClientRequest, RefreshCartClientResponse } from "PosApi/Consume/Cart";
import { Carts } from "./../DataService/DataServiceRequests.g";

export default class AppliedCouponsDialog extends Dialogs.ExtensionTemplatedDialogBase {
    private static readonly DATA_LIST_ELEMENT_ID: string = "Microsoft_AppliedCouponsDialog_DataList";
    private static readonly OK_BUTTON_ID: string = "Ok";
    private static readonly CANCEL_BUTTON_ID: string = "Cancel";
    private _dataList: IDataList<ProxyEntities.Coupon>;
    private _selectedCoupons: ProxyEntities.Coupon[];
    private _allAppliedCoupons: ProxyEntities.Coupon[];

    private _resolve: () => void;

    public constructor() {
        super();
        this._selectedCoupons = [];
        this._allAppliedCoupons = [];
    }

    public onReady(element: HTMLElement): void {
        ko.applyBindings(this, element);

        let dataListElement: HTMLDivElement = element.querySelector('#' + AppliedCouponsDialog.DATA_LIST_ELEMENT_ID) as HTMLDivElement;
        this._dataList = this.context.controlFactory.create<ProxyEntities.Coupon>(
            this.context.logger.getNewCorrelationId(),
            "DataList",
            {
                columns: [
                    {
                        collapseOrder: 2,
                        computeValue: (row: ProxyEntities.Coupon): string => {
                            return row.Code;
                        },
                        isRightAligned: false,
                        minWidth: 100,
                        ratio: 40,
                        title: "Coupon Code"
                    },
                    {
                        collapseOrder: 1,
                        computeValue: (row: ProxyEntities.Coupon): string => {
                            return row.DiscountOfferId
                        },
                        isRightAligned: false,
                        minWidth: 50,
                        ratio: 60,
                        title: "Discount Offer Id"
                    }

                ],
                data: this._allAppliedCoupons,
                interactionMode: DataListInteractionMode.MultiSelect,
                equalityComparer: (left: ProxyEntities.Coupon, right: ProxyEntities.Coupon): boolean => {
                    return left.CodeId === right.CodeId;
                }
            },
            dataListElement
        );

        this._dataList.addEventListener("SelectionChanged", (selectionData: { items: ProxyEntities.Coupon[] }): void => {
            this._selectedCoupons = selectionData.items
        });

        this.loadAsync().then(() => {
            this._dataList.data = this._allAppliedCoupons;
        });
    }

    public open(): Promise<void> {
        let promise: Promise<void> = new Promise<void>((resolve: () => void, reject: (reason: any) => void): void => {
            this._resolve = resolve;

            let option: Dialogs.ITemplatedDialogOptions = {
                title: "Applied Coupons",
                onCloseX: this.onCloseX.bind(this),
                button1: {
                    id: AppliedCouponsDialog.OK_BUTTON_ID,
                    label: "Remove",
                    isPrimary: true,
                    onClick: this._removeCheckedCoupons.bind(this)
                },
                button2: {
                    id: AppliedCouponsDialog.CANCEL_BUTTON_ID,
                    label: "Cancel",
                    isPrimary: false,
                    onClick: this.buttonCancelClickHandler.bind(this)
                    }
            };

            this.openDialog(option);
        });

        return promise;

    }

    private _removeCheckedCoupons(): Promise<boolean> {
        if (this.isProcessing) {
            return Promise.resolve(true);
        }

        this.isProcessing = true;

        if (ObjectExtensions.isNullOrUndefined(this._selectedCoupons) || !ArrayExtensions.hasElements(this._selectedCoupons)) {
            return Promise.resolve(true);
        }

        let couponCodes: string[] = this._selectedCoupons.map((coupon: ProxyEntities.Coupon): string => {
            return coupon.Code;
        });

        let correlationId: string = this.context.logger.getNewCorrelationId();
        let getCurrentCartClientRequest: GetCurrentCartClientRequest<GetCurrentCartClientResponse> = new GetCurrentCartClientRequest(correlationId);

        return this.context.runtime.executeAsync(getCurrentCartClientRequest)
            .then((response: ClientEntities.ICancelableDataResult<GetCurrentCartClientResponse>):
                Promise<ClientEntities.ICancelableDataResult<Carts.RemoveAppliedCouponsFromCartResponse>> => {
                    let request: Carts.RemoveAppliedCouponsFromCartRequest<Carts.RemoveAppliedCouponsFromCartResponse>
                        = new Carts.RemoveAppliedCouponsFromCartRequest(response.data.result.Id, couponCodes);
                return this.context.runtime.executeAsync(request);
            }).then((response: ClientEntities.ICancelableDataResult<Carts.RemoveAppliedCouponsFromCartResponse>) => {
                let request: RefreshCartClientRequest<RefreshCartClientResponse> = new RefreshCartClientRequest<RefreshCartClientResponse>();
                return this.context.runtime.executeAsync(request);
            }).then((result: ClientEntities.ICancelableDataResult<RefreshCartClientResponse>): Promise<boolean> => {
                this.isProcessing = false;
                return Promise.resolve(true);
            }).catch((reason: any) => {
                console.log("there are some errors");
                this.isProcessing = false;
                return Promise.reject(false);
            });
    }

    public loadAsync(): Promise<void> {
        this.isProcessing = true;

        let correlationId: string = this.context.logger.getNewCorrelationId();
        let getCurrentCartClientRequest: GetCurrentCartClientRequest<GetCurrentCartClientResponse> = new GetCurrentCartClientRequest(correlationId);
        return this.context.runtime.executeAsync(getCurrentCartClientRequest)
            .then((response: ClientEntities.ICancelableDataResult<GetCurrentCartClientResponse>):
                Promise<ClientEntities.ICancelableDataResult<Carts.GetAppliedCouponsResponse>> => {
                let request: Carts.GetAppliedCouponsRequest<Carts.GetAppliedCouponsResponse> = new Carts.GetAppliedCouponsRequest(response.data.result.Id);
                return this.context.runtime.executeAsync(request);
            }).then((response: ClientEntities.ICancelableDataResult<Carts.GetAppliedCouponsResponse>): Promise<void> => {
                if (ObjectExtensions.isNullOrUndefined(response)
                    || ObjectExtensions.isNullOrUndefined(response.data)
                    || response.canceled) {
                    return Promise.resolve();
                }

                response.data.result.forEach((coupon: ProxyEntities.Coupon) => {
                    this._allAppliedCoupons.push(coupon);
                });
                this.isProcessing = false;
                return Promise.resolve();
            }).catch((reason: any) => {
                this.context.logger.logError("Wrong occurred when get applied discount" + JSON.stringify(reason));
                this.isProcessing = false;
            });
    }

    private buttonCancelClickHandler(): boolean {
        this.resolvePromise();
        return false;
    }

    private onCloseX(): boolean {
        this.resolvePromise();
        return true;
    }

    private resolvePromise(): void {
        if (ObjectExtensions.isFunction(this._resolve)) {
            this._resolve();
            this._resolve = null;
        }
    }
}