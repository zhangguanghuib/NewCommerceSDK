
import { CurrencyFormatter, /*DateFormatter,*/ TransactionTypeFormatter } from "PosApi/Consume/Formatters";
import { ProxyEntities } from "PosApi/Entities";
import { ICustomColumnsContext } from "PosApi/Extend/Views/CustomListColumns";
import { IShowJournalTransactionListColumn } from "PosApi/Extend/Views/ShowJournalView";

class TransactionListColumns {

    private _context: ICustomColumnsContext;

    constructor(context: ICustomColumnsContext) {
        this._context = context;
    }

    public get columns(): IShowJournalTransactionListColumn[] {
        let columns: IShowJournalTransactionListColumn[] = [
            //{
            //    title: this._context.resources.getString("OperatorIdColumnName"),
            //    computeValue: (row: ProxyEntities.Transaction): string => row.StaffId,
            //    ratio: -1,
            //    collapseOrder: -1,
            //    minWidth: 100
            //},
            //{
            //    title: this._context.resources.getString("RegisterColumnName"),
            //    computeValue: (row: ProxyEntities.Transaction): string => row.TerminalId,
            //    ratio: -1,
            //    collapseOrder: -1,
            //    minWidth: 100
            //},
            {
                title: this._context.resources.getString("TypeColumnName"),
                computeValue: (row: ProxyEntities.Transaction): string => TransactionTypeFormatter.toName(row.TransactionTypeValue),
                ratio: -1,
                collapseOrder: -1,
                minWidth: 100
            },
            {
                title: this._context.resources.getString("TotalColumnName"),
                computeValue: (row: ProxyEntities.Transaction): string => CurrencyFormatter.toCurrency(row.TotalAmount),
                ratio: -1,
                collapseOrder: -1,
                minWidth: 100
            },
            //{
            //    title: this._context.resources.getString("ReceiptColumnName"),
            //    computeValue: (row: ProxyEntities.Transaction): string => row.ReceiptId,
            //    ratio: -1,
            //    collapseOrder: -1,
            //    minWidth: 100
            //},
            //{
            //    title: this._context.resources.getString("DateColumnName"),
            //    computeValue: (row: ProxyEntities.Transaction): string => DateFormatter.toShortDateAndTime(row.CreatedDateTime),
            //    ratio: -1,
            //    collapseOrder: -1,
            //    minWidth: 100
            //},
            {
                title: this._context.resources.getString("ServiceChargeAmountColumnName"),
                computeValue: (row: ProxyEntities.Transaction): string => {
                    let value: ProxyEntities.CommercePropertyValue = this.getPropertyValue(row.ExtensionProperties, "ServiceChargeAmount");
                    let decimalValue: number = (value && value.DecimalValue) ? value.DecimalValue : 0;
                    return CurrencyFormatter.toCurrency(decimalValue);
                },
                ratio: -1,
                collapseOrder: -1,
                minWidth: 100
            },
            {
                title: this._context.resources.getString("ReturnRemainingDaysColumnName"),
                computeValue: (row: ProxyEntities.Transaction): string => {
                    let value: ProxyEntities.CommercePropertyValue = this.getPropertyValue(row.ExtensionProperties, "ReturnRemainingDays");
                    return (value && value.IntegerValue) ? value.IntegerValue.toString() : this._context.resources.getString("NotAvailableMessage");
                },
                ratio: -1,
                collapseOrder: -1,
                minWidth: 100
            },
            {
                title: this._context.resources.getString("CustomeNameColumnName"),
                computeValue: (row: ProxyEntities.Transaction): string => {
                    let value: ProxyEntities.CommercePropertyValue = this.getPropertyValue(row.ExtensionProperties, "CustomerNameExtension");
                    return (value && value.StringValue) ? value.StringValue.toString() : this._context.resources.getString("NotAvailableMessage");
                },
                ratio: -1,
                collapseOrder: -1,
                minWidth: 100
            }
        ];

        columns.forEach((column: IShowJournalTransactionListColumn, index: number) => {
            column.collapseOrder = index + 1;
            column.ratio = (100 / columns.length);
            column.isRightAligned = (index == (columns.length - 1));
        });

        return columns;
    }

    private getPropertyValue(extensionProperties: ProxyEntities.CommerceProperty[], column: string): ProxyEntities.CommercePropertyValue {
        let prop: ProxyEntities.CommerceProperty = (extensionProperties || []).filter((prop: ProxyEntities.CommerceProperty) => { return prop.Key === column })[0];
        return prop ? prop.Value : undefined;
    }
}

export default (context: ICustomColumnsContext): IShowJournalTransactionListColumn[] => {
    let transactionLtColumns: TransactionListColumns = new TransactionListColumns(context);
    return transactionLtColumns.columns;
}