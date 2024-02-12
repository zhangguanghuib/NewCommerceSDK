import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as ReportDetailsView from "PosApi/Extend/Views/ReportDetailsView";
import { ProxyEntities } from "PosApi/Entities";
import { ObjectExtensions, StringExtensions } from "PosApi/TypeExtensions";
import $ from 'jquery';

export default class ExportToCSVCommand extends ReportDetailsView.ReportDetailsExtensionCommandBase {

    private _reportData: ProxyEntities.ReportDataSet = null;
    private _reportTitle: string = StringExtensions.EMPTY;
    private _reportId: string = StringExtensions.EMPTY;

    constructor(context: IExtensionCommandContext<ReportDetailsView.IReportDetailsToExtensionCommandMessageTypeMap>) {
        super(context);
        this.id = "sampleExportToCSVCommand";
        this.label = "ExportToCSV";
        this.extraClass = "iconPickup";

        this.reportDataLoadedDataHandler = (data: ReportDetailsView.ReportDataLoadedData): void => {
            this.canExecute = true;
            this._reportData = data.reportDataSet;
        };
    }

    protected init(state: ReportDetailsView.IReportDetailsExtensionCommandState): void {
        this.isVisible = true;
        this._reportId = state.reportId;
        this._reportTitle = state.reportTitle;
    }

    protected execute(): void {
        let headerInited: boolean = false;
        let csvRPTHeader: string = "";
        let csvRPTContent: string = "";

        $('sampleExportToCSVCommand').hide();

        this.context.logger.logInformational("Report title: " + JSON.stringify(this._reportTitle));
        this.context.logger.logInformational("Report Id: " + JSON.stringify(this._reportId));
        this.context.logger.logInformational("Print report data: " + JSON.stringify(this._reportData));

        this._reportData.Output.forEach((row: ProxyEntities.ReportRow) => {
            row.RowData.forEach((property: ProxyEntities.CommerceProperty) => {
                // Add header title
                if (!headerInited) {
                    csvRPTHeader += property.Key;
                }
                if (!ObjectExtensions.isNullOrUndefined(property.Value.BooleanValue)) {
                    csvRPTContent += property.Value.BooleanValue.toString();
                } else if (!ObjectExtensions.isNullOrUndefined(property.Value.StringValue)) {
                    csvRPTContent += property.Value.StringValue;
                } else if (!ObjectExtensions.isNullOrUndefined(property.Value.DateTimeOffsetValue)) {
                    csvRPTContent += property.Value.DateTimeOffsetValue.toString();
                } else if (!ObjectExtensions.isNullOrUndefined(property.Value.DecimalValue)) {
                    csvRPTContent += property.Value.DecimalValue.toString();
                } else if (!ObjectExtensions.isNullOrUndefined(property.Value.IntegerValue)) {
                    csvRPTContent += property.Value.IntegerValue.toString();
                } else if (!ObjectExtensions.isNullOrUndefined(property.Value.LongValue)) {
                    csvRPTContent += property.Value.LongValue.toString();
                } else if (!ObjectExtensions.isNullOrUndefined(property.Value.ByteValue)) {
                    csvRPTContent += property.Value.ByteValue.toString();
                }
                if (!headerInited) {
                    csvRPTHeader += ",";
                }
                csvRPTContent += ",";
            });

            // Header title will be only initialized once
            if (!headerInited) {
                csvRPTHeader = csvRPTHeader.substr(0, csvRPTHeader.length - 1);
                csvRPTHeader += "\n";
            }
            csvRPTContent = csvRPTContent.substr(0, csvRPTContent.length - 1);
            csvRPTContent += "\n";
            headerInited = true;
        });

        // Create anchor element and set download attribute
        const link = document.createElement("a");
        link.setAttribute("href",
            `data:text/csv;charset=utf-8,${encodeURIComponent(csvRPTHeader + csvRPTContent)}`);
        link.setAttribute("download", "data.csv");

        // Append link to body and trigger click event
        document.body.appendChild(link);
        link.click();

        // Remove link from body
        document.body.removeChild(link);
    }
}