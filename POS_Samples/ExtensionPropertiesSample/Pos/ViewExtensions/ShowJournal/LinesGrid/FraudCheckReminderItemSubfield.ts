import {
    ICustomLinesGridItemSubfieldContext,
    CustomLinesGridItemSubfieldBase
} from "PosApi/Extend/Views/ShowJournalView";
import { ProxyEntities } from "PosApi/Entities";
import { /*ObjectExtensions,*/ StringExtensions } from "PosApi/TypeExtensions";

export default class FraudCheckReminderItemSubfield extends CustomLinesGridItemSubfieldBase {

    constructor(context: ICustomLinesGridItemSubfieldContext) {
        super(context);
    }

    /**
     * Computes a value to display as an item subfield based on the given sales line.
     * @param {ClientEntities.ISalesLineForDisplay} salesLine The sales line.
     * @returns {string} The computed value do display as an item subfield.
     */
    public computeValue(salesLine: ProxyEntities.SalesLine): string {
        let value: string = StringExtensions.EMPTY;

        value = salesLine.Comment;
        //if (!ObjectExtensions.isNullOrUndefined(salesLine) && salesLine.TotalAmount > 100) {
        //    value = "Please check the purchasers I.D. in order to prevent fraud.";
        //}

        return value;
    }
}