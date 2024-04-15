import { ProxyEntities } from "PosApi/Entities";

export interface ISimpleNextViewModelOptions {
    message: string;
}

/**
 * Type interface for simple extension view model constructor options.
 */
export interface ISimpleExtensionViewModelOptions {
    displayMesssge: string;
}

/**
 * Type interface for post logon view constructor options.
 */
export interface IPostLogOnViewOptions {
    resolveCallback: (value?: any | PromiseLike<any>) => void;
    rejectCallback: (reason?: any) => void;
}


export interface IContosoDenominationDetailViewOptions {
    title?: string;
    denominations: ProxyEntities.DenominationDetail[];
}


 export interface IDenominationsViewOptions {
        /**
         * The operation name calling the denominations view.
         * @remarks TenderDeclaration, BankDrop, SafeDrop, StartAmount, FloatEntry, TenderRemoval.
         */
        operationTitle: string;

        /**
         * The tender name.
         * @remarks Cash, Voucher.
         */
        tenderName: string;

        /**
         * The denomination details for a currency.
         */
        denominationDetails: ProxyEntities.DenominationDetail[];

        /**
         * The selection handler for completing the current operation.
         */
        selectionHandler?: any;

        /**
         * The correlation id.
         */
        correlationId: string;

        /**
         * Show default filtering icon or not.
         */
        shouldShowFilteringIcon?: boolean;

        /**
         * The last selected currency in denominations view.
         */
        lastSelectedCurrencyInDenominationsView?: string;
    }
