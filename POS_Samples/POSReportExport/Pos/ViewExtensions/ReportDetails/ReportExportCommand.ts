import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as ReportDetailsView from "PosApi/Extend/Views/ReportDetailsView";
import { ProxyEntities } from "PosApi/Entities";
import { StringExtensions } from "PosApi/TypeExtensions";
import jspdf from "jspdf"; 
import html2canvas from "html2canvas"; 

export default class ReportExportCommand extends ReportDetailsView.ReportDetailsExtensionCommandBase {

    private _reportData: ProxyEntities.ReportDataSet = null;
    private _reportTitle: string = StringExtensions.EMPTY;
    private _reportId: string = StringExtensions.EMPTY;

    constructor(context: IExtensionCommandContext<ReportDetailsView.IReportDetailsToExtensionCommandMessageTypeMap>) {
        super(context);

        this.id = "sampleReportExportCommand";
        this.label = "Export";
        this.extraClass = "iconPackage";

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

        html2canvas(document.body, {
            windowWidth: document.documentElement.scrollWidth,
            windowHeight: document.documentElement.scrollHeight
        }).then(canvas => {
            let img = canvas.toDataURL("image/png");
            const doc = new jspdf({
                format: "a3"
            });
            let width = doc.internal.pageSize.width;
            let height = doc.internal.pageSize.height;
            doc.addImage(img, 'JPEG', 0, 0, width, height);
            doc.save("page.pdf");
        });
    }
}