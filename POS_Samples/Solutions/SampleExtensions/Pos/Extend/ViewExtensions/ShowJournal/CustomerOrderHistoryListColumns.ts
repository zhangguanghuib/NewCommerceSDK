import { CurrencyFormatter, DateFormatter, TransactionTypeFormatter } from "PosApi/Consume/Formatters";
import { ProxyEntities } from "PosApi/Entities";
import { ICustomColumnsContext } from "PosApi/Extend/Views/CustomListColumns";
import { IShowJournalCustomerOrderHistoryListColumn } from "PosApi/Extend/Views/ShowJournalView";


export default (context: ICustomColumnsContext): IShowJournalCustomerOrderHistoryListColumn[] => {
    return [
        {
            title: "Date_Customized",
            computeValue: (row: ProxyEntities.SalesOrder): string => { return DateFormatter.toShortDateAndTime(row.CreatedDateTime); },
            ratio: 20,
            collapseOrder: 7,
            minWidth: 200
        },
        {
            title: "Operator ID",
            computeValue: (row: ProxyEntities.SalesOrder): string => { return "Neato custom  Staff ID:" + row.StaffId; },
            ratio: 10,
            collapseOrder: 1,
            minWidth: 100
        },
        {
            title: "Register",
            computeValue: (row: ProxyEntities.SalesOrder): string => { return row.TerminalId; },
            ratio: 15,
            collapseOrder: 2,
            minWidth: 100
        },
        {
            title: "Type",
            computeValue: (row: ProxyEntities.SalesOrder): string => { return TransactionTypeFormatter.toName(row.TransactionTypeValue); },
            ratio: 15,
            collapseOrder: 3,
            minWidth: 200
        },
        {
            title: "Status",
            computeValue: (row: ProxyEntities.SalesOrder): string => { return TransactionTypeFormatter.toName(row.StatusValue); },
            ratio: 10,
            collapseOrder: 4,
            minWidth: 100,
        },
        {
            title: "Receipt",
            computeValue: (row: ProxyEntities.SalesOrder): string => { return "Customized: " + row.ReceiptId; },
            ratio: 20,
            collapseOrder: 6,
            minWidth: 200
        }, 
        {
            title: "Total",
            computeValue: (row: ProxyEntities.SalesOrder): string => { return CurrencyFormatter.toCurrency(row.TotalAmount); },
            ratio: 10,
            collapseOrder: 5,
            minWidth: 100,
            isRightAligned: true
        }
    ]

}