"use strict";

import ko from "knockout";
import { ObjectExtensions } from "PosApi/TypeExtensions";

abstract class KnockoutExtensionViewModelBase implements ko.IDisposable {
    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }
}

export default KnockoutExtensionViewModelBase;