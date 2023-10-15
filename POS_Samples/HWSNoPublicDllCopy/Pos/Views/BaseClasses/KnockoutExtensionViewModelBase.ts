"use strict";

import { ObjectExtensions } from "PosApi/TypeExtensions";
import ko from "knockout";
/**
 * Represents the base class for knockout based view models.
 * @remarks Implements a dispose method to ensure that resources are properly released.
 */
abstract class KnockoutExtensionViewModelBase implements ko.IDisposable {

    /**
     * Disposes of the view model's resources.
     */
    public dispose(): void {
        ObjectExtensions.disposeAllProperties(this);
    }
}

export default KnockoutExtensionViewModelBase;