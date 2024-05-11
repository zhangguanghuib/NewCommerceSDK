import { ArrayExtensions, ObjectExtensions } from "PosApi/TypeExtensions";

namespace Contoso {
    "use strict";
    export class Dictionary<T> {
        private _items: any = Object.create(null);
        private _length: number = 0;

        public removeItem(key: number | string): T {
            let value: T;
            if (typeof (this._items[key]) !== "undefined") {
                value = this._items[key];
                this._items[key] = undefined;
                delete this._items[key];
                this._length--;
            }

            return value;
        }

        /**
         * Returns an item based on the key.
         * @param {number | string} key The key.
         * @returns {T} The item associated to the key.
         */
        public getItem(key: number | string): T {
            return this._items[key];
        }

        public setItem(key: number | string, value: T): T {
            if (typeof (value) !== "undefined") {
                if (!this.hasItem(key)) {
                    this._length++;
                }

                this._items[key] = value;
            } else {
                throw "Could not set item as value is not defined";
            }

            return value;
        }

        /**
         * Adds the elements of the array to the dictionary.
         *
         * @param {T[]} array - The array whose elements should be added to the dictionary.
         * @param {(element: T) => string | number} keySelector - A function to extract a key from each element.
         * @returns {T[]} The array whose elements were added to the dictionary.
         * @remarks  If the specified key already exists in the Dictionary, setting the element overwrites the old value.
         */
        public setItems(array: T[], keySelector: (element: T) => string | number): T[] {
            if (!ObjectExtensions.isFunction(keySelector)) {
                throw "keySelector is incorrect";
            }

            if (ArrayExtensions.hasElements(array)) {
                array.forEach((element: T) => {
                    this.setItem(keySelector(element), element);
                });
            }

            return array;
        }

        /**
         * Checks if the key is present in the dictionary.
         * @param {number | string} key The key.
         * @returns {boolean} True if the item exists in the dictionary, false otherwise.
         */
        public hasItem(key: number | string): boolean {
            return typeof (this._items[key]) !== "undefined";
        }

        public length(): number {
            return this._length;
        }

        public forEach(callback: (key: string, value: T) => void): void {
            this.getKeys().forEach((key: string): void => {
                callback(key, this._items[key]);
            });
        }

        public clear(): void {
            this._items = Object.create(null);
            this._length = 0;
        }

        /**
         * Filters dictionary by provided callback.
         *
         * @param {(key: string, value: T) => boolean } callback - The callback context.
         * @returns {Dictionary<T>} Filtered dictionary.
         */
        public filter(callback: (key: string, value: T) => boolean): Dictionary<T> {
            let result: Dictionary<T> = new Dictionary<T>();
            this.getKeys().forEach((key: string): void => {
                if (callback(key, this._items[key]) === true) {
                    result.setItem(key, this._items[key]);
                }
            });

            return result;
        }

        /**
         * Returns all items from the dictionary.
         *
         * @returns {T[]} Dictionary values.
         */
        public getItems(): T[] {
            let result: T[] = [];
            this.getKeys().forEach((key: string): void => {
                result.push(this._items[key]);
            });

            return result;
        }

        /**
         * Returns all keys of the dictionary.
         *
         * @returns {string[]} Dictionary keys.
         */
        public getKeys(): string[] {
            return Object.keys(this._items);
        }
    }
}


export { Contoso };
