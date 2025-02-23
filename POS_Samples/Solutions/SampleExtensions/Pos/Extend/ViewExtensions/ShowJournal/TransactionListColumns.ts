import { IShowJournalTransactionListColumn } from "PosApi/Extend/Views/ShowJournalView";
import { ICustomColumnsContext } from "PosApi/Extend/Views/CustomListColumns";
import { DateFormatter, TransactionTypeFormatter, CurrencyFormatter } from "PosApi/Consume/Formatters";
import { ProxyEntities } from "PosApi/Entities";

export default (context: ICustomColumnsContext): IShowJournalTransactionListColumn[] => {
    return [
        {
            title: "Date",
            computeValue: (row: ProxyEntities.Transaction): string => { return DateFormatter.toShortDateAndTime(row.CreatedDateTime); },
            ratio: 20,
            collapseOrder: 7,
            minWidth: 150
        }, {
            title: "Operator ID",
            computeValue: (row: ProxyEntities.Transaction): string => { return row.StaffId; },
            ratio: 15,
            collapseOrder: 1,
            minWidth: 150
        }, {
            title: "Register",
            computeValue: (row: ProxyEntities.Transaction): string => { return row.TerminalId; },
            ratio: 15,
            collapseOrder: 2,
            minWidth: 100
        }, {
            title: "Type",
            computeValue: (row: ProxyEntities.Transaction): string => {
                return TransactionTypeFormatter.toName(
                    row.TransactionTypeValue, row.TransactionStatusValue);
            },
            ratio: 10,
            collapseOrder: 3,
            minWidth: 100
        }, {
            title: "Receipt",
            computeValue: (row: ProxyEntities.Transaction): string => { return row.ReceiptId; },
            ratio: 20,
            collapseOrder: 5,
            minWidth: 150
        }, {
            title: "Total",
            computeValue: (row: ProxyEntities.Transaction): string => { return CurrencyFormatter.toCurrency(row.TotalAmount); },
            ratio: 10,
            collapseOrder: 4,
            minWidth: 100,
            isRightAligned: true
        },
        {
            title: "TradeNo",
            computeValue: (row: ProxyEntities.Transaction): string =>
            {
                let cps: Array<ProxyEntities.CommerceProperty>
                    = row.ExtensionProperties.filter((cp) => cp.Key === "CONTOSORETAILSEATNUMBER");

                if (cps.length >= 1) {
                    let cp: ProxyEntities.CommerceProperty = cps[0];
                    return cp.Value.IntegerValue+"";
                } else {
                    return "";
                }   
            },
            ratio: 10,
            collapseOrder: 6,
            minWidth: 100,
            isRightAligned: true
        }
    ];
};