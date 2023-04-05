"use strict";

export interface ISimpleNextViewModelOptions {
    message: string;
}

export interface ISimpleExtensionViewModelOptions {
    displayMesssge: string;
}

export interface IPostLogOnViewOptions {
    resolveCallback: (value?: any | PromiseLike<any>) => void;
    rejectCallback: (reason?: any) => void;
}
