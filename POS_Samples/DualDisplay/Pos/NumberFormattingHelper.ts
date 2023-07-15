export abstract class NumberFormattingHelper {
    public static getRoundedStringValue(value: number, decimalPrecision: number): string {
        return NumberFormattingHelper.roundToNdigits(value, decimalPrecision).toFixed(decimalPrecision);
    }

    public static roundToNdigits(value: number, decimalPrecision: number): number {
        if (decimalPrecision === 0) {
            return Math.round(value);
        }

        return Math.round(value * Math.pow(10, decimalPrecision) / Math.pow(10, decimalPrecision));
    }
}