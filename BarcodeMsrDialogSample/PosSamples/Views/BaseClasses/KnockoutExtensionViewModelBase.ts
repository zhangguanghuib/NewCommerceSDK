"use strict";

import { ObjectExtensions } from "PosApi/TypeExtensions";

abstract class KnockoutExtensionViewModelBase implements Commerce.IDisposable {
    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }
}

export default KnockoutExtensionViewModelBase;