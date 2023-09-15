"use strict";

export interface IPostLogOnViewOptions {
    resolveCallback: (value?: any | PromiseLike<any>) => void;
    rejectCallback: (reason?: any) => void;
}
