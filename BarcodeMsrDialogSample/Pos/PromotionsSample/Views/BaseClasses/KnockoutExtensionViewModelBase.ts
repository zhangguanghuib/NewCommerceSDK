"use strict";

import { ObjectExtensions } from "PosApi/TypeExtensions";

abstract class KnockoutExtensionViewModelBase  {
    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }
}

export default KnockoutExtensionViewModelBase;