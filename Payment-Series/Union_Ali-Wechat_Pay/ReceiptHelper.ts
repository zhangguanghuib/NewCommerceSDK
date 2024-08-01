namespace Commerce {
    "use strict";

    export class ReceiptHelper {

        private static readonly TEXT_ESCAPE_MARKER: string = "&#x1B;";
        private static readonly TEXT_BOLD_MARKER: string = ReceiptHelper.TEXT_ESCAPE_MARKER + "|2C";
        private static readonly TEXT_DOUBLEZISE_BOLD_MARKER: string = ReceiptHelper.TEXT_ESCAPE_MARKER + "|4C";
        private static readonly TEXT_NEW_LINE_MARKER: string = "\r\n";
        private static readonly HTML_NEW_LINE_TAG: string = "<br/>";
        private static readonly LOGO_OR_IMAGE_TAG_REGEXP: RegExp = /<(L|I)>/g;
        private static readonly LOGO_OR_IMAGE_WITH_DATA_TAG_REGEXP: RegExp = /<(L|I):(.+?)>/g;

        /**
         * Convert the raw receipt text to HTML and remove the logo text.
         * @param {string} receiptText The receipt text.
         * @return {string} The converted receipt text in HTML format.
         */
        public static convertToHtml(receiptText: string): string {
            if (StringExtensions.isNullOrWhitespace(receiptText)) {
                return StringExtensions.EMPTY;
            }

            receiptText = Utilities.ReceiptHelper.translateReceiptContent(receiptText, Commerce.StringResourceManager);

            let endOfBoldMarkerIndex: number = -1;
            // Convert bold text markers.
            while (true) {
                let boldMarkerIndex: number = receiptText.indexOf(ReceiptHelper.TEXT_BOLD_MARKER, endOfBoldMarkerIndex + 1);

                if (boldMarkerIndex < 0) {
                    break;
                }

                let endOfLineMarkerIndex: number = receiptText.indexOf(ReceiptHelper.TEXT_NEW_LINE_MARKER, boldMarkerIndex);
                let nextStyleMarkerIndex: number = receiptText.indexOf(ReceiptHelper.TEXT_ESCAPE_MARKER, boldMarkerIndex + 1);
                endOfBoldMarkerIndex = Math.min(endOfLineMarkerIndex, nextStyleMarkerIndex);

                // The entire bold item, e.g: "012532    ".
                let entireBoldItem: string = receiptText.substring(boldMarkerIndex + ReceiptHelper.TEXT_BOLD_MARKER.length, endOfBoldMarkerIndex);
                receiptText = this._updateReceiptTextContainingBoldText(receiptText, entireBoldItem, endOfBoldMarkerIndex);
            }

            // Convert double size bold text markers.
            endOfBoldMarkerIndex = -1;
            while (true) {
                let boldMarkerIndex: number = receiptText.indexOf(ReceiptHelper.TEXT_DOUBLEZISE_BOLD_MARKER, endOfBoldMarkerIndex + 1);

                if (boldMarkerIndex < 0) {
                    break;
                }

                let endOfLineMarkerIndex: number = receiptText.indexOf(ReceiptHelper.TEXT_NEW_LINE_MARKER, boldMarkerIndex);
                let nextStyleMarkerIndex: number = receiptText.indexOf(ReceiptHelper.TEXT_ESCAPE_MARKER, boldMarkerIndex + 1);
                endOfBoldMarkerIndex = Math.min(endOfLineMarkerIndex, nextStyleMarkerIndex);

                // The entire double size bold item, e.g: "012532    ".
                let entireBoldItem: string = receiptText.substring(boldMarkerIndex + ReceiptHelper.TEXT_DOUBLEZISE_BOLD_MARKER.length, endOfBoldMarkerIndex);
                receiptText = this._updateReceiptTextContainingBoldText(receiptText, entireBoldItem, endOfBoldMarkerIndex);
            }

            let rawReceiptText: string = ReceiptHelper._removeLogoAndImageTags(receiptText);
            let formatedReceiptText: string = EscapingHelper
                .escapeHtml(rawReceiptText.replace(/&#x1B;\|1C|&#x1B;\|2C|&#x1B;\|3C|&#x1B;\|4C/g, ""))
                .replace(/\r\n/g, ReceiptHelper.HTML_NEW_LINE_TAG);
            return formatedReceiptText;
        }

        /**
         * Determines if a sales order can contain a gift receipt. Sales Order with invalid sales lines
         * or Suspended transaction or quotation should not contain gift receipt.
         * @param {Proxy.Entities.SalesOrder} salesOrder The sales order.
         * @return {boolean} True: if the sales order can contain a gift receipt. False: otherwise.
         */
        public static canSalesOrderContainGiftReceipt(salesOrder: Proxy.Entities.SalesOrder): boolean {
            return ArrayExtensions.hasElements(salesOrder.SalesLines) &&
                (ObjectExtensions.isNullOrUndefined(salesOrder.CustomerOrderTypeValue)
                    || salesOrder.CustomerOrderTypeValue === Proxy.Entities.CustomerOrderType.SalesOrder) &&
                (ObjectExtensions.isNullOrUndefined(salesOrder.TransactionTypeValue)
                    || salesOrder.TransactionTypeValue !== Proxy.Entities.TransactionType.SuspendedTransaction) &&
                salesOrder.SalesLines.some((salesLine: Proxy.Entities.SalesLine) =>
                    !(salesLine.IsGiftCardLine || salesLine.IsVoided || salesLine.IsCustomerAccountDeposit
                        || salesLine.IsReturnByReceipt || salesLine.IsInvoiceLine)
                    && (salesLine.Quantity > 0 && salesLine.ItemId !== StringExtensions.EMPTY));
        }

        /**
         * Determines if a receipt can be printed.
         * @param {Proxy.Entities.Receipt} receipt The receipt.
         * @return {boolean} True: if the receipt can be printed. False: otherwise.
         */
        public static canReceiptBePrinted(receipt: Proxy.Entities.Receipt): boolean {
            let canBePrinted: boolean = false;

            if (receipt != null && ArrayExtensions.hasElements(receipt.Printers)) {
                canBePrinted = !receipt.Printers.every((printer: Proxy.Entities.Printer) =>
                    printer.PrintBehaviorValue === Proxy.Entities.PrintBehavior.Never);
            }

            return canBePrinted;
        }

        /**
         * Determines if any receipt in the list can be printed.
         * @param {Proxy.Entities.Receipt[]} receiptsList List of receipts.
         * @return {boolean} True: if any receipt can be printed. False: otherwise.
         */
        public static canAnyReceiptBePrinted(receiptsList: Proxy.Entities.Receipt[]): boolean {

            return receiptsList.some((receipt: Proxy.Entities.Receipt): boolean => {
                return this.canReceiptBePrinted(receipt);
            });
        }

        /*
         * Converts an array of receipts to printable receipts
         * @param {Proxy.Entities.Receipt[]} receipts The list of receipts to convert
         * @return {Proxy.Entities.PrintableReceipt[]} The array of printable receipts
         */
        public static getPrintableReceipts(receipts: Proxy.Entities.Receipt[]): Proxy.Entities.PrintableReceipt[] {
            let printableReceiptArray: Proxy.Entities.PrintableReceipt[] = [];

            if (ArrayExtensions.hasElements(receipts)) {
                receipts.forEach((receipt: Proxy.Entities.Receipt) => {
                    let printerList: Proxy.Entities.Printer[] = [];

                    if (ArrayExtensions.hasElements(receipt.Printers)) {
                        printerList = receipt.Printers;
                    }

                    printerList.forEach((printer: Proxy.Entities.Printer) => {
                        let printableReceipt: Proxy.Entities.PrintableReceipt = new Proxy.Entities.PrintableReceipt(
                            receipt, printer, ApplicationContext.Instance.deviceConfiguration.HardwareConfigurations.PrinterConfigurations);
                        if (printableReceipt.shouldPrint) {
                            printableReceiptArray.push(printableReceipt);
                        }
                    });
                });
            }

            return printableReceiptArray;
        }

        /*
         * Returns the valid sales lines to print a gift receipt from the order.
         * @param {TSalesLine} salesLines The sales lines to filter.
         * @return {TSalesLine[]} The sales lines that are valid for a gift receipt.
         */
        public static getSalesLinesForGiftReceipt<TSalesLine extends Proxy.Entities.SalesLine>(salesLines: TSalesLine[]): TSalesLine[] {
            salesLines = ArrayExtensions.hasElements(salesLines) ? salesLines : [];

            // Log sales lines before filtering.
            this._logSalesLinesForGiftReceipt(false, salesLines);

            // Filter the sales lines to only include sales lines that are not return or voided
            // nor gift card.
            let validSalesLines: TSalesLine[] = salesLines.filter((salesLine: TSalesLine): boolean => {
                return !salesLine.IsVoided && !salesLine.IsGiftCardLine && !salesLine.IsInvoiceLine && salesLine.Quantity > 0;
            });

            // Log sales lines after filtering.
            this._logSalesLinesForGiftReceipt(true, validSalesLines);

            return validSalesLines;
        }

        /**
         * Retrieves the list of gift receipts configured "As Required".
         * @param {Proxy.Entities.Receipt[]} receipts The list of all receipts.
         * @returns {Proxy.Entities.Receipt[]} The list of gift receipts configured "As Required".
         */
        public static getAsRequiredGiftReceipts(receipts: Proxy.Entities.Receipt[]): Proxy.Entities.Receipt[] {
            if (!ObjectExtensions.isNullOrUndefined(receipts)) {
                return receipts.filter((receipt: Proxy.Entities.Receipt) =>
                    receipt.ReceiptTypeValue === Proxy.Entities.ReceiptType.GiftReceipt &&
                    receipt.Printers.some((printer: Proxy.Entities.Printer) =>
                        printer.PrintBehaviorValue === Proxy.Entities.PrintBehavior.AsRequired));
            } else {
                return [];
            }
        }

        /**
         * Logs the number of items in the array and their item IDs. If no item ID is available, then an empty string is used.
         * @param {boolean} filtered True: if the sales lines were filtered; otherwise, False.
         * @param {string[]} salesLines array of Sales Line objects.
         */
        private static _logSalesLinesForGiftReceipt<TSalesLine extends Proxy.Entities.SalesLine>(filtered: boolean, salesLines: TSalesLine[]): void {
            let itemIds: string[] = [];

            salesLines.forEach((salesLine: Proxy.Entities.SalesLine) => {
                itemIds.push(!StringExtensions.isNullOrWhitespace(salesLine.ItemId) ? salesLine.ItemId : StringExtensions.EMPTY);
            });

            let itemIdsCsv: string = itemIds.join(", ");
            let occurance: string = filtered ? "AFTER" : "BEFORE";
            RetailLogger.receiptHelperGetSalesLinesForGiftReceiptFilter(occurance, salesLines.length, itemIdsCsv);
        }

        /**
         * Properly aligns the bold text if it is center aligned; otherwise, postfixes the necessary empty space.
         * @param {string} receiptText entire receipt text.
         * @param {string} entireBoldItem individual bold text.
         * @param {number} endOfBoldMarkerIndex index where bold mark is located in receipt text.
         * @return {string} updated receipt text with correctly aligned bold text.
         */
        private static _updateReceiptTextContainingBoldText(receiptText: string, entireBoldItem: string, endOfBoldMarkerIndex: number): string {
            let emptySpaces: string = StringExtensions.EMPTY;
            let updatedReceiptText: string = StringExtensions.EMPTY;

            if (this._receiptBoldItemIsCenterAligned(entireBoldItem)) {
                // If centered, then add first half of empty space, then the actual text, followed by last half of empty space,
                // and finally the rest of the receipt text.
                emptySpaces = Array(Math.round((entireBoldItem.length + 1) / 2)).join(" ");
                updatedReceiptText = receiptText.substring(0, endOfBoldMarkerIndex - (entireBoldItem.length - 1))
                    + emptySpaces + entireBoldItem + emptySpaces + receiptText.substring(endOfBoldMarkerIndex);
            } else {
                emptySpaces = Array(entireBoldItem.length + 1).join(" ");
                updatedReceiptText = receiptText.substring(0, endOfBoldMarkerIndex) + emptySpaces + receiptText.substring(endOfBoldMarkerIndex);
            }

            return updatedReceiptText;
        }

        /**
         * Checks whether the passed-in string has text that is center aligned.
         * This isn't intended to be a thorough check of whether the text is centered
         * but since center-aligned text is surrounded by white-space, this method simply checks the two ends of the string.
         * @param {string} boldTextToCheck string to check whether it's centered aligned.
         * @return {boolean} True: str contains white space at both ends. False: otherwise.
         */
        private static _receiptBoldItemIsCenterAligned(boldTextToCheck: string): boolean {
            // If the string is null or (all) whitespace, return false.
            if (StringExtensions.isNullOrWhitespace(boldTextToCheck)) {
                return false;
            }

            // Check if first index is whitespace.
            let firstIndexIsWhitespace: boolean = StringExtensions.compare(boldTextToCheck.charAt(0), " ", true) === 0;

            // Check if last index is whitespace.
            let lastIndexIsWhitespace: boolean = StringExtensions.compare(boldTextToCheck.charAt(boldTextToCheck.length - 1), " ", true) === 0;

            return firstIndexIsWhitespace && lastIndexIsWhitespace;
        }

        /**
         * Removes logo and image tags from a receipt.
         * @param {string} receiptText The receipt text.
         * @return {string} The receipt text without logo and image tags.
         */
        private static _removeLogoAndImageTags(receiptText: string): string {
            return receiptText
                .replace(ReceiptHelper.LOGO_OR_IMAGE_TAG_REGEXP, StringExtensions.EMPTY)
                .replace(ReceiptHelper.LOGO_OR_IMAGE_WITH_DATA_TAG_REGEXP, StringExtensions.EMPTY);
        }
    }
}