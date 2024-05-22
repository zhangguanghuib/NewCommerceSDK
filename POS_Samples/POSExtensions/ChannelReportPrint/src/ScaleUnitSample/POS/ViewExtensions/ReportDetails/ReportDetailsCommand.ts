
import { GetHardwareProfileClientRequest, GetHardwareProfileClientResponse } from "PosApi/Consume/Device";
import { GetLoggedOnEmployeeClientRequest } from "PosApi/Consume/Employees";
import { HardwareStationDeviceActionRequest, HardwareStationDeviceActionResponse } from "PosApi/Consume/Peripherals";
import { GetCurrentProductCatalogStoreClientRequest } from "PosApi/Consume/Products";
import { ProxyEntities } from "PosApi/Entities";
import { IExtensionCommandContext } from "PosApi/Extend/Views/AppBarCommands";
import * as ReportDetailsView from "PosApi/Extend/Views/ReportDetailsView";
import { ObjectExtensions, StringExtensions } from "PosApi/TypeExtensions";
import POSReportHelper, { IPrintPOSReportPDFRequest, IreportColumn } from "POSReportHelper";
import POSReturnSalesByStaffReportHelper from "POSReturnSalesByStaffReportHelper";
import { POSReports as Helper } from "POSReportsHelper";


export default class ReportDetailsCommand extends ReportDetailsView.ReportDetailsExtensionCommandBase {

    private _reportData: ProxyEntities.ReportDataSet = null;
    public _reportTitle: string = StringExtensions.EMPTY;
    private _reportId: string = StringExtensions.EMPTY;

    private HS_DEVICENAME: string = "PDFPOSREPORTPRINTER";
    private HS_ACTIONNAME: string = "PrintPOSPDFReports";

    private helper: Helper.POSReportsHelper = new Helper.POSReportsHelper(this.context);

    constructor(context: IExtensionCommandContext<ReportDetailsView.IReportDetailsToExtensionCommandMessageTypeMap>) {
        super(context);

        this.id = "SAGPOSReportPrintCmd";
        this.label = "Print";
        this.extraClass = "iconPrint";

        this.reportDataLoadedDataHandler = (data: ReportDetailsView.ReportDataLoadedData): void => {
            this.canExecute = true;
            this._reportData = data.reportDataSet;
        };
    }


    protected init(state: ReportDetailsView.IReportDetailsExtensionCommandState): void {
        this.isVisible = true;
        this._reportId = state.reportId;
        this._reportTitle = state.reportTitle;
        if (this._reportId == "109" || this._reportId == "109SG" || //Store voided line details 
            this._reportId == "110" || this._reportId == "110SG" || //Return sales by staff
            this._reportId == "108" || this._reportId == "108SG" || //Store price overrides
            this._reportId == "105" || this._reportId == "105SG" || //Store return transactions
            this._reportId == "106" || this._reportId == "106SG" || //Store sales by discount
            this._reportId == "130" || this._reportId == "130SG" || //ReprintSalesReceiptReport
            this._reportId == "131" ||//Top sales report
            this._reportId == "110SG_ReturnSalesByStaff") {
            this.isVisible = true;
        } else {
            this.isVisible = false
        }
    }

    protected execute(): void {

        this.context.runtime.executeAsync(new GetLoggedOnEmployeeClientRequest())
            .then((empresponse) => {
                this.context.runtime.executeAsync(new GetCurrentProductCatalogStoreClientRequest())
                    .then((response) => {
                        this.InitialisePrintReport(empresponse.data.result.StaffId, empresponse.data.result.Name, response.data.store.OrgUnitName, response.data.store.OrgUnitNumber);
                    });

            })
            .catch((_error) => {
                this.InitialisePrintReport("", "", "", "");
            })



    }

    private PrintPDFReport(reportHtml: string, filename: string) {
        try {
            this.isProcessing = true;

            let gethardwareProfileRequest: GetHardwareProfileClientRequest<GetHardwareProfileClientResponse> =
                new GetHardwareProfileClientRequest<GetHardwareProfileClientResponse>();
            this.context.runtime.executeAsync(gethardwareProfileRequest)
                .then((hardwareProfileResponse) => {
                    let hardwareProfile = hardwareProfileResponse.data.result;
                    if (!ObjectExtensions.isNullOrUndefined(hardwareProfile)
                        && hardwareProfile.Printers.length > 0
                        && hardwareProfile.Printers.some(x => { return x.DeviceTypeValue == 2 })) {
                        let printername: string = hardwareProfile.Printers.filter(x => { return x.DeviceTypeValue == 2 })//Windows driver
                            .pop().DeviceName;
                        if (printername == "") {
                            printername = "Microsoft Print to PDF";
                        }

                        let pdfreportrequestData: IPrintPOSReportPDFRequest = {
                            HTMLString: reportHtml,
                            PrinterName: printername,
                            FileName: filename,
                            FilePath: ""
                        };

                        let printreportPDFHSRequest: HardwareStationDeviceActionRequest<HardwareStationDeviceActionResponse> =
                            new HardwareStationDeviceActionRequest<HardwareStationDeviceActionResponse>(this.HS_DEVICENAME, this.HS_ACTIONNAME, pdfreportrequestData);
                        this.context.runtime.executeAsync(printreportPDFHSRequest)
                            .then((hsresponse) => {
                                this.isProcessing = false;

                                let result = hsresponse.data.response;

                                if (result) {
                                    this.helper.ShowMessage(this.context.resources.getString("SAGPOSReports_4"));
                                }
                                else {
                                    this.helper.ShowMessage(this.context.resources.getString("SAGPOSReports_3"));
                                }
                            })
                            .catch((_error) => {
                                this.isProcessing = false;
                                this.context.logger.logError(JSON.stringify(_error));
                            })
                    }
                })
        } catch (_error) {
            this.isProcessing = false;
            this.context.logger.logError("Error while printing PDF print for POS report " + JSON.stringify(_error));
        }

    }

    private InitialisePrintReport(empid: string, empName: string, storename: string, storeid: string) {

        let islandscapemode: boolean = true;

        if (this._reportId == "109" || this._reportId == "109SG") {
            let _reportColumns: IreportColumn[] = [];
            _reportColumns.push({ key: "Date", Name: "Date", DataType: "String", width: "70" });
            _reportColumns.push({ key: "Transaction number", Name: "Transaction No.", DataType: "String", width: "140" });
            _reportColumns.push({ key: "Employee ID", Name: "Empl. ID", DataType: "String", width: "80" });
            _reportColumns.push({ key: "REASONCODE", Name: "Reason Code", DataType: "String", width: "150" });
            _reportColumns.push({ key: "Product number", Name: "Product No.", DataType: "String", width: "150" });
            _reportColumns.push({ key: "Product", Name: "Product", DataType: "String", width: "250" });
            _reportColumns.push({ key: "Quantity", Name: "Qty", DataType: "Integer", width: "60" });
            _reportColumns.push({ key: "Void amount", Name: "Void amount", DataType: "Decimal", width: "100" });

            let _reportParameters: IreportColumn[] = [];
            _reportParameters.push({ key: "dt_StartDate", Name: "Start Date", DataType: "DateTime", width: "50%" });
            _reportParameters.push({ key: "dt_EndDate", Name: "End Date", DataType: "DateTime", width: "50%" });
            _reportParameters.push({ key: "nvc_UserId", Name: "Employee Id", DataType: "String", width: "50%" });

            let _posreportHelper: POSReportHelper = new POSReportHelper(
                this._reportData, "109", _reportColumns, this.context, "Store Voided Line Details", _reportParameters, islandscapemode
                , empName, empid);
            let reportHtml: string = _posreportHelper.GetReportFormattedHtml();

            this.PrintPDFReport(reportHtml, "Store_Voided_Line_Details");

        }
        else if (this._reportId == "110" || this._reportId == "110SG") {
            let _reportColumns: IreportColumn[] = [];
            _reportColumns.push({ key: "Transaction number", Name: "Transaction No.", DataType: "String", width: "15%" });
            //_reportColumns.push({ key: "SAGRECEIPTNO", Name: "Receipt No", DataType: "String", width: "15%" });
            _reportColumns.push({ key: "Product number", Name: "Product No.", DataType: "String", width: "15%" });
            _reportColumns.push({ key: "Product", Name: "Product", DataType: "String", width: "20%" });
            _reportColumns.push({ key: "REASONCODE", Name: "Reason Code", DataType: "String", width: "15%" });
            _reportColumns.push({ key: "Quantity", Name: "Qty", DataType: "Integer", width: "10%" });
            _reportColumns.push({ key: "Return amount", Name: "Return amount", DataType: "Decimal", width: "10%" });

            let _reportParameters: IreportColumn[] = [];
            _reportParameters.push({ key: "dt_StartDate", Name: "Start Date", DataType: "DateTime", width: "50%" });
            _reportParameters.push({ key: "dt_EndDate", Name: "End Date", DataType: "DateTime", width: "50%" });
            _reportParameters.push({ key: "nvc_UserId", Name: "Employee Id", DataType: "String", width: "50%" });

            let _posreportHelper: POSReportHelper = new POSReportHelper(
                this._reportData, "110", _reportColumns, this.context, "Return Sales by Staff", _reportParameters, islandscapemode, empName, empid);
            let reportHtml: string = _posreportHelper.GetReportFormattedHtml();

            this.PrintPDFReport(reportHtml, "Return_salesByStaff");

        }
        else if (this._reportId == "110SG_ReturnSalesByStaff") {
            let _reportColumns: IreportColumn[] = [];
            _reportColumns.push({ key: "Transaction number", Name: "Transaction No.", DataType: "String", width: "15%" });
            _reportColumns.push({ key: "TRANSDATETIME", Name: "Transaction time", DataType: "String", width: "15%" });
            _reportColumns.push({ key: "Product number", Name: "Product No.", DataType: "String", width: "15%" });
            _reportColumns.push({ key: "Product", Name: "Product", DataType: "String", width: "20%" });
            _reportColumns.push({ key: "REASONCODE", Name: "Reason Code", DataType: "String", width: "15%" });
            _reportColumns.push({ key: "Quantity", Name: "Qty", DataType: "Integer", width: "10%" });
            _reportColumns.push({ key: "Return amount", Name: "Return amount", DataType: "Decimal", width: "10%" });

            let _reportParameters: IreportColumn[] = [];
            _reportParameters.push({ key: "dt_StartDate", Name: "Start Date", DataType: "DateTime", width: "50%" });
            _reportParameters.push({ key: "dt_EndDate", Name: "End Date", DataType: "DateTime", width: "50%" });
            _reportParameters.push({ key: "nvc_UserId", Name: "Employee Id", DataType: "String", width: "50%" });

            let _posreportHelper: POSReturnSalesByStaffReportHelper = new POSReturnSalesByStaffReportHelper(
                this._reportData, "110", _reportColumns, this.context, "Return Sales by Staff", _reportParameters, islandscapemode, empName, empid, storename, storeid);
            let reportHtml: string = _posreportHelper.GetReportFormattedHtml();

            this.PrintPDFReport(reportHtml, "Return_salesByStaff");

        }
        else if (this._reportId == "108" || this._reportId == "108SG") {
            let _reportColumns: IreportColumn[] = [];
            _reportColumns.push({ key: "Date", Name: "Date", DataType: "String", width: "70" });
            _reportColumns.push({ key: "Transaction number", Name: "Transaction No.", DataType: "String", width: "140" });
            _reportColumns.push({ key: "Employee ID", Name: "Empl. ID", DataType: "String", width: "60" });
            _reportColumns.push({ key: "REASONCODE", Name: "Reason Code", DataType: "String", width: "150" });
            _reportColumns.push({ key: "Product number", Name: "Product No.", DataType: "String", width: "130" });
            _reportColumns.push({ key: "Product", Name: "Product", DataType: "String", width: "250" });
            _reportColumns.push({ key: "Original price", Name: "Original Price", DataType: "Decimal", width: "80" });
            _reportColumns.push({ key: "Entered price", Name: "Entered Price", DataType: "Decimal", width: "70" });
            _reportColumns.push({ key: "Quantity", Name: "Qty", DataType: "Integer", width: "50" });

            let _reportParameters: IreportColumn[] = [];
            _reportParameters.push({ key: "dt_StartDate", Name: "Start Date", DataType: "DateTime", width: "50%" });
            _reportParameters.push({ key: "dt_EndDate", Name: "End Date", DataType: "DateTime", width: "50%" });
            _reportParameters.push({ key: "nvc_UserId", Name: "Employee Id", DataType: "String", width: "50%" });

            let _posreportHelper: POSReportHelper = new POSReportHelper(
                this._reportData, "108", _reportColumns, this.context, "Store Price Override Details", _reportParameters, islandscapemode, empName, empid);
            let reportHtml: string = _posreportHelper.GetReportFormattedHtml();

            this.PrintPDFReport(reportHtml, "Store_price_override");

        }
        else if (this._reportId == "105" || this._reportId == "105SG") {
            let _reportColumns: IreportColumn[] = [];
            _reportColumns.push({ key: "Date", Name: "Date", DataType: "String", width: "70" });
            _reportColumns.push({ key: "Transaction number", Name: "Transaction No.", DataType: "String", width: "150" });
            _reportColumns.push({ key: "Employee ID", Name: "Employee ID", DataType: "String", width: "90" });
            _reportColumns.push({ key: "REASONCODE", Name: "Reason Code", DataType: "String", width: "150" });
            _reportColumns.push({ key: "Product number", Name: "Product No.", DataType: "String", width: "140" });
            _reportColumns.push({ key: "Product", Name: "Product", DataType: "String", width: "250" });
            _reportColumns.push({ key: "Quantity", Name: "Qty", DataType: "Integer", width: "50" });
            _reportColumns.push({ key: "Return amount", Name: "Return amount", DataType: "Decimal", width: "100" });
            let _reportParameters: IreportColumn[] = [];
            _reportParameters.push({ key: "dt_StartDate", Name: "Start Date", DataType: "DateTime", width: "50%" });
            _reportParameters.push({ key: "dt_EndDate", Name: "End Date", DataType: "DateTime", width: "50%" });
            _reportParameters.push({ key: "nvc_UserId", Name: "Employee Id", DataType: "String", width: "50%" });

            let _posreportHelper: POSReportHelper = new POSReportHelper(
                this._reportData, "105", _reportColumns, this.context, "Store Return Transactions", _reportParameters, islandscapemode, empName, empid);
            let reportHtml: string = _posreportHelper.GetReportFormattedHtml();

            this.PrintPDFReport(reportHtml, "Store_return_transactions");
        }
        else if (this._reportId == "130" || this._reportId == "130SG") {
            let _reportColumns: IreportColumn[] = [];
            _reportColumns.push({ key: "Date", Name: "Date", DataType: "String", width: "120" });
            _reportColumns.push({ key: "Transaction number", Name: "Transaction No.", DataType: "String", width: "250" });
            _reportColumns.push({ key: "Employee ID", Name: "Employee ID", DataType: "String", width: "160" });
            //_reportColumns.push({ key: "Product number", Name: "Product No.", DataType: "String", width: "140" });
            //_reportColumns.push({ key: "Product", Name: "Product", DataType: "String", width: "290" });
            _reportColumns.push({ key: "Quantity", Name: "Qty", DataType: "Integer", width: "100" });
            _reportColumns.push({ key: "SALESRECEIPTAMOUNT", Name: "Amount", DataType: "Decimal", width: "100" });
            _reportColumns.push({ key: "REASONCODE", Name: "Reason Code", DataType: "String", width: "270" });

            let _reportParameters: IreportColumn[] = [];
            _reportParameters.push({ key: "dt_StartDate", Name: "Start Date", DataType: "DateTime", width: "50%" });
            _reportParameters.push({ key: "dt_EndDate", Name: "End Date", DataType: "DateTime", width: "50%" });
            _reportParameters.push({ key: "nvc_UserId", Name: "Employee Id", DataType: "String", width: "50%" });

            let _posreportHelper: POSReportHelper = new POSReportHelper(
                this._reportData, "130", _reportColumns, this.context, "Reprint Sales Receipt Report", _reportParameters, islandscapemode, empName, empid);
            let reportHtml: string = _posreportHelper.GetReportFormattedHtml();

            this.PrintPDFReport(reportHtml, "Reprint_Sales_Receipt_Report");
        }
        else if (this._reportId == "106" || this._reportId == "106SG") {
            let _reportColumns: IreportColumn[] = [];
            //_reportColumns.push({ key: "Product number", Name: "Product number", DataType: "String", width: "15%" });
            //_reportColumns.push({ key: "Discount", Name: "Discount", DataType: "String", width: "15%" });
            //_reportColumns.push({ key: "Discount percentage", Name: "Discount percentage", DataType: "Decimal", width: "10%" });
            //_reportColumns.push({ key: "Discount amount", Name: "Discount amount", DataType: "Decimal", width: "20%" });
            //_reportColumns.push({ key: "REASONCODE", Name: "Reason Code", DataType: "String", width: "15%" });
            //_reportColumns.push({ key: "Sales transactions", Name: "Sales transactions", DataType: "Integer", width: "15%" });
            //_reportColumns.push({ key: "Sales amount", Name: "Sales amount", DataType: "Decimal", width: "20%" });
            //_reportColumns.push({ key: "Return transactions", Name: "Return transactions", DataType: "Integer", width: "8%" });
            //_reportColumns.push({ key: "Return amount", Name: "Return amount", DataType: "Decimal", width: "17%" });
            //_reportColumns.push({ key: "Net amount", Name: "Net amount", DataType: "Decimal", width: "17%" });

            _reportColumns.push({ key: "Date", Name: "Date", DataType: "String", width: "70" });
            _reportColumns.push({ key: "Transaction number", Name: "Transaction No.", DataType: "String", width: "150" });
            _reportColumns.push({ key: "Employee ID", Name: "Empl. ID", DataType: "String", width: "60" });
            _reportColumns.push({ key: "Product number", Name: "Product No.", DataType: "String", width: "130" });
            _reportColumns.push({ key: "Product", Name: "Product", DataType: "String", width: "250" });
            _reportColumns.push({ key: "Original price", Name: "Original Price", DataType: "Decimal", width: "80" });
            _reportColumns.push({ key: "ENTEREDPRICE", Name: "Entered Price", DataType: "Decimal", width: "80" });
            _reportColumns.push({ key: "Quantity", Name: "Qty", DataType: "Integer", width: "40" });
            _reportColumns.push({ key: "MANUALDISCOUNT", Name: "Manual Disc.", DataType: "Decimal", width: "60" });
            _reportColumns.push({ key: "Sales amount", Name: "Sales Amount", DataType: "Decimal", width: "80" });
            _reportColumns.push({ key: "REASONCODE", Name: "Reason <br/> Code", DataType: "String", width: "150" });

            let _reportParameters: IreportColumn[] = [];
            _reportParameters.push({ key: "dt_StartDate", Name: "Start Date", DataType: "DateTime", width: "50%" });
            _reportParameters.push({ key: "dt_EndDate", Name: "End Date", DataType: "DateTime", width: "50%" });
            _reportParameters.push({ key: "nvc_UserId", Name: "Employee Id", DataType: "String", width: "50%" });

            let _posreportHelper: POSReportHelper = new POSReportHelper(
                this._reportData, "106", _reportColumns, this.context, "Store Sales by Manual Discount", _reportParameters, islandscapemode, empName, empid);
            let reportHtml: string = _posreportHelper.GetReportFormattedHtml();

            this.PrintPDFReport(reportHtml, "Store_sales_by_manual discount");
        }
        else if (this._reportId == "131") {
            let _reportColumns: IreportColumn[] = [];

            _reportColumns.push({ key: "Variant", Name: "Variant", DataType: "String", width: "200" });
            _reportColumns.push({ key: "Product", Name: "Product", DataType: "String", width: "200" });
            _reportColumns.push({ key: "Sold quantity", Name: "Sold Qty.", DataType: "Decimal", width: "120" });
            _reportColumns.push({ key: "Sales amount", Name: "Sales Amount", DataType: "Decimal", width: "120" });
            _reportColumns.push({ key: "Return quantity", Name: "Return Qty.", DataType: "Decimal", width: "120" });
            _reportColumns.push({ key: "Return amount", Name: "Return amount", DataType: "Decimal", width: "120" });
            _reportColumns.push({ key: "Net amount", Name: "Net amount", DataType: "Decimal", width: "120" });

            let _reportParameters: IreportColumn[] = [];
            _reportParameters.push({ key: "nvc_StoreId1", Name: "Store From", DataType: "String", width: "50%" });
            _reportParameters.push({ key: "nvc_StoreId2", Name: "Store To", DataType: "String", width: "50%" });
            _reportParameters.push({ key: "nvc_ItemId1", Name: "Product Number From", DataType: "String", width: "50%" });
            _reportParameters.push({ key: "nvc_ItemId2", Name: "Product Number To", DataType: "String", width: "50%" });
            _reportParameters.push({ key: "dt_StartDate", Name: "Start Date", DataType: "DateTime", width: "50%" });
            _reportParameters.push({ key: "dt_EndDate", Name: "End Date", DataType: "DateTime", width: "50%" });
            _reportParameters.push({ key: "nvc_UserId", Name: "Employee Id", DataType: "String", width: "50%" });

            let _posreportHelper: POSReportHelper = new POSReportHelper(
                this._reportData, "131", _reportColumns, this.context, "Top Sales Report", _reportParameters, islandscapemode, empName, empid);
            let reportHtml: string = _posreportHelper.GetReportFormattedHtml();

            this.PrintPDFReport(reportHtml, "Top_sales_report");
        }
    }
}