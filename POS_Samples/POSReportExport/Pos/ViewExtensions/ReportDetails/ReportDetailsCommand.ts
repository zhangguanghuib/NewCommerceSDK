import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as ReportDetailsView from "PosApi/Extend/Views/ReportDetailsView";
import { ProxyEntities } from "PosApi/Entities";
import { ObjectExtensions, StringExtensions } from "PosApi/TypeExtensions";
import jspdf from "jspdf"; 


export default class ReportDetailsCommand extends ReportDetailsView.ReportDetailsExtensionCommandBase {

    private _reportData: ProxyEntities.ReportDataSet = null;
    private _reportTitle: string = StringExtensions.EMPTY;
    private _reportId: string = StringExtensions.EMPTY;

    /**
     * Creates a new instance of the ReportDetailsCommand class.
     * @param {IExtensionCommandContext<ReportDetailsView.IReportDetailsToExtensionCommandMessageTypeMap>} context The context.
     * @remarks The command context contains APIs through which a command can communicate with POS.
     */
    constructor(context: IExtensionCommandContext<ReportDetailsView.IReportDetailsToExtensionCommandMessageTypeMap>) {
        super(context);

        this.id = "sampleReportDetailsCommand";
        this.label = "Print";
        this.extraClass = "iconPrint";

        this.reportDataLoadedDataHandler = (data: ReportDetailsView.ReportDataLoadedData): void => {
            this.canExecute = true;
            this._reportData = data.reportDataSet;
        };
    }

    /**
     * Initializes the command.
     * @param {ReportDetailsView.IReportDetailsExtensionCommandState} state The state used to initialize the command.
     */
    protected init(state: ReportDetailsView.IReportDetailsExtensionCommandState): void {
        this.isVisible = true;
        this._reportId = state.reportId;
        this._reportTitle = state.reportTitle;
    }

    protected execute(): void {
        this.context.logger.logInformational("Report title: " + JSON.stringify(this._reportTitle));
        this.context.logger.logInformational("Report Id: " + JSON.stringify(this._reportId));
        this.context.logger.logInformational("Print report data: " + JSON.stringify(this._reportData));

        let lineHeight: number = 20;
        let colWidth: number = 70;
        let x: number = 10;
        let y: number = 10;
        const doc = new jspdf({
            format: "a3"
        });
        this._reportData.Output.forEach((row: ProxyEntities.ReportRow) => { 
            row.RowData.forEach((property: ProxyEntities.CommerceProperty) => {
                x = 20;
                doc.text(x, y, property.Key + ":");
                x += colWidth;
                if (!ObjectExtensions.isNullOrUndefined(property.Value.BooleanValue)) {
                    doc.text(x, y, property.Value.BooleanValue.toString());
                } else if (!ObjectExtensions.isNullOrUndefined(property.Value.StringValue)) {
                    doc.text(x, y, property.Value.StringValue);
                } else if (!ObjectExtensions.isNullOrUndefined(property.Value.DateTimeOffsetValue)) {
                    doc.text(x, y, property.Value.DateTimeOffsetValue.toString());
                } else if (!ObjectExtensions.isNullOrUndefined(property.Value.DecimalValue)) {
                    doc.text(x, y, property.Value.DecimalValue.toString());
                } else if (!ObjectExtensions.isNullOrUndefined(property.Value.IntegerValue)) {
                    doc.text(x, y, property.Value.IntegerValue.toString());
                } else if (!ObjectExtensions.isNullOrUndefined(property.Value.LongValue)) {
                    doc.text(x, y, property.Value.LongValue.toString());
                } else if (!ObjectExtensions.isNullOrUndefined(property.Value.ByteValue)) {
                    doc.text(x, y, property.Value.ByteValue.toString());
                }
                y += lineHeight;
            });
            doc.line(20, y, x + colWidth, y);
            y += lineHeight;
        })
        doc.save(this._reportTitle +'.pdf');
    }
}