import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as ReportDetailsView from "PosApi/Extend/Views/ReportDetailsView";
export default class ReportDetailsCommand extends ReportDetailsView.ReportDetailsExtensionCommandBase {

    /**
     * Creates a new instance of the ReportDetailsCommand class.
     * @param {IExtensionCommandContext<ReportDetailsView.IReportDetailsToExtensionCommandMessageTypeMap>} context The context.
     * @remarks The command context contains APIs through which a command can communicate with POS.
     */
    constructor(context: IExtensionCommandContext<ReportDetailsView.IReportDetailsToExtensionCommandMessageTypeMap>) {
        super(context);

        //this.id = "sampleReportDetailsCommand";
        //this.label = "Print";
        //this.extraClass = "iconPrint";

        //this.reportDataLoadedDataHandler = (data: ReportDetailsView.ReportDataLoadedData): void => {
        //    this.canExecute = true;
        //};
    }

    /**
     * Initializes the command.
     * @param {ReportDetailsView.IReportDetailsExtensionCommandState} state The state used to initialize the command.
     */
    protected init(state: ReportDetailsView.IReportDetailsExtensionCommandState): void {
        //this.isVisible = true;
      
    }

    protected execute(): void {
       
    }
}