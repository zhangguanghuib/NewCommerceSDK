/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */
import ko from "knockout";

import { ICustomViewControllerContext, ICustomViewControllerBaseState } from "PosApi/Create/Views";
import { ClientEntities, ProxyEntities } from "PosApi/Entities";
import KnockoutExtensionViewModelBase from "./BaseClasses/KnockoutExtensionViewModelBase";
import { GetCurrentCartClientRequest, GetCurrentCartClientResponse } from "PosApi/Consume/Cart";
import ICancelableDataResult = ClientEntities.ICancelableDataResult;


export default class CartLinesViewModel extends KnockoutExtensionViewModelBase {
    public title: ko.Observable<string>;
    public currentCartLines: ProxyEntities.CartLine[];
    public readonly _cartLinesObservable: ko.ObservableArray<ProxyEntities.CartLine>;

    private _context: ICustomViewControllerContext;
    private _customViewControllerBaseState: ICustomViewControllerBaseState;

    constructor(context: ICustomViewControllerContext, state: ICustomViewControllerBaseState) {
        super();

        this._context = context;
        this.title = "Current Cart Lines";
        this._customViewControllerBaseState = state;
        this._customViewControllerBaseState.isProcessing = true;
        this.currentCartLines = [];
        this._cartLinesObservable = ko.observableArray<ProxyEntities.CartLine>([]);
    }

    /**
     * Loads the view model asynchronously.
     */
    public loadAsync(): Promise<void> {

        this._customViewControllerBaseState.isProcessing = true;

        let correlationId: string = this._context.logger.getNewCorrelationId();

        let getCurrentCartClientRequest: GetCurrentCartClientRequest<GetCurrentCartClientResponse> = new GetCurrentCartClientRequest(correlationId);

        return this._context.runtime.executeAsync(getCurrentCartClientRequest)
            .then((response: ICancelableDataResult<GetCurrentCartClientResponse>): Promise<void> => {
                if (!response.canceled) {
                    response.data.result.CartLines.forEach((cartline: ProxyEntities.CartLine) => {
                        this.currentCartLines.push(cartline);
                    });
                    this._cartLinesObservable(response.data.result.CartLines);
                }
                this._customViewControllerBaseState.isProcessing = false;
                return Promise.resolve();
            }).catch((reason: any) => {
                this._context.logger.logError("StoreHoursView.StoreHoursDialog: " + JSON.stringify(reason));
                this._customViewControllerBaseState.isProcessing = false;
            });
    }
}
