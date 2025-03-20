
"use strict";

/**
 * Type interface for simple next view model constructor options.
 */
export interface ISimpleNextViewModelOptions {
    message: string;
}

/**
 * Type interface for simple extension view model constructor options.
 */
export interface ISimpleExtensionViewModelOptions {
    displayMessage: string;
}

/**
 * Type interface for post logon view constructor options.
 */
export interface IPostLogOnViewOptions {
    resolveCallback: (value?: any | PromiseLike<any>) => void;
    rejectCallback: (reason?: any) => void;
}