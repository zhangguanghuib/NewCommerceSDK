import { ProxyEntities } from "PosApi/Entities";
import { ObjectExtensions } from "PosApi/TypeExtensions";
declare var Commerce: any;

export default class ContosoNumberExtensions {


    public static readonly DEFAULT_DECIMAL_PRECISION: number = 2;

    public static isNullOrZero(value: number): boolean {
        return (ObjectExtensions.isNullOrUndefined(value) || value === 0);
    }

    /**
     * Verifies whether the number is undefined, null, NaN or equal to 0.
     * @param {number} value The number.
     * @return {boolean} True if the number is undefined, null, NaN or equal to zero, false otherwise.
     */
    public static isNullNaNOrZero(value: number): boolean {
        return (ContosoNumberExtensions.isNullOrZero(value) || isNaN(value));
    }

    /**
     * Verifies whether the number is undefined, null or NaN.
     * @param {number} value The number.
     * @return {boolean} True if the number is undefined, null or NaN, false otherwise.
     */
    public static isNullOrNaN(value: number): boolean {
        return (ObjectExtensions.isNullOrUndefined(value) || isNaN(value));
    }

    /**
     * Verifies whether the number is an integer value.
     * @param {number} value The number to check.
     * @returns {boolean} True if the number is an integer, false otherwise.
     */
    public static isInteger(value: number): boolean {
        return !ObjectExtensions.isNullOrUndefined(value)
            && !isNaN(value)
            && !isNaN(parseInt(value.toString(), 10))
            && parseInt(value.toString(), 10) === value;
    }


    public static roundToNDigits(value: number, decimalPrecision: number): number {
        decimalPrecision = ContosoNumberExtensions._toDecimalPrecision(decimalPrecision, ContosoNumberExtensions.DEFAULT_DECIMAL_PRECISION);
        if (decimalPrecision === 0) {
            return Math.round(value);
        }

        // Use this instead of toFixed otherwise it will not round anything, simply lose digits
        return Math.round(value * Math.pow(10, decimalPrecision)) / Math.pow(10, decimalPrecision);
    }


    private static _toDecimalPrecision(decimalPrecision: number, defaultIfInvalid: number): number {
        if (ObjectExtensions.isNumber(decimalPrecision) && decimalPrecision >= 0) {
            return Math.round(decimalPrecision);
        }

        return defaultIfInvalid;
    }


    public static getDecimalPrecision(currencyCode: string = Commerce.ApplicationContext.Instance.deviceConfiguration.Currency): number {
        let currency: ProxyEntities.Currency = Commerce.ApplicationContext.Instance.currenciesMap.getItem(currencyCode);
        if (ObjectExtensions.isNullOrUndefined(currency)) {
            return ContosoNumberExtensions.DEFAULT_DECIMAL_PRECISION;
        }

        return ContosoNumberExtensions._toDecimalPrecision(currency.NumberOfDecimals, ContosoNumberExtensions.DEFAULT_DECIMAL_PRECISION);
    }

}