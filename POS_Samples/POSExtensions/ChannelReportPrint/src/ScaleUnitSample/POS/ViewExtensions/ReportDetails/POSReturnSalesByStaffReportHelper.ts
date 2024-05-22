import { ProxyEntities } from "PosApi/Entities";
import { IExtensionContext } from "PosApi/Framework/ExtensionContext";
import { ObjectExtensions, StringExtensions } from "PosApi/TypeExtensions";

export interface IreportColumn {
    Name: string,
    DataType: string,
    key: string,
    width: string
}

export interface IPrintPOSReportPDFRequest {
    HTMLString: string,
    PrinterName: string,
    FilePath: string,
    FileName: string
}

export default class POSReturnSalesByStaffReportHelper {

    public _reportdataSet: ProxyEntities.ReportDataSet;
    public _reportId: string;
    public _reportColumns: Array<IreportColumn>;
    public _reportParameters: Array<IreportColumn>;
    public _context: IExtensionContext;
    public _reportName: string;
    public _isLandScapeMode: boolean;
    public _staffName: string;
    public _staffId: string;
    public _storeName: string;
    public _storeId: string;

    constructor(_reportdata: ProxyEntities.ReportDataSet, reportid: string, reportcolumns: IreportColumn[], context: IExtensionContext,
        reportName: string, reportParameters: IreportColumn[], islandscapeMode: boolean, staffname: string, staffid: string, storename: string, storeid: string) {
        this._reportdataSet = _reportdata;
        this._reportId = reportid;
        this._reportColumns = reportcolumns;
        this._context = context;
        this._reportName = reportName;
        this._reportParameters = reportParameters;
        this._isLandScapeMode = islandscapeMode;
        this._staffId = staffid;
        this._staffName = staffname;
        this._storeName = storename;
        this._storeId = storeid;
    }

    public GetReportFormattedHtml() {
        if (this._reportId != ""
            && !ObjectExtensions.isNullOrUndefined(this._reportdataSet)
            && !ObjectExtensions.isNullOrUndefined(this._reportColumns)
            && this._reportColumns.length > 0
        ) {
            let reportHtmlContent = "";

            reportHtmlContent += this.GetHtmlHeaderContent(this._isLandScapeMode);

            // Parameters Header
            reportHtmlContent += "<div style='height:50px;width:100%'><table style='width:100%'><tbody>" + this.GetReportHeader() + "</tbody></table></div>";

            let _bodyHtmlContent = '';
            _bodyHtmlContent += "<div style='height: 70%'><table>";

            // Header
            _bodyHtmlContent += "<thead>" + "" + "</thead>";

            // Body
            // column header
            _bodyHtmlContent += "<tbody>";
            _bodyHtmlContent += "<tr>";
            this._reportColumns.forEach(x => {
                var _widthStyling: string = 'width:auto;';
                if (this._reportColumns.map(x => x.key).indexOf(x.key) > -1) {
                    _widthStyling = 'width:' + this._reportColumns.filter(a => { return a.key == x.key }).pop().width + ';';
                }
                _bodyHtmlContent += "<td style='" + _widthStyling + "text-align: center'><b>" + x.Name + "</b></td>";
            })
            _bodyHtmlContent += "</tr>";

            // column data
            //let reportdataLength: number = this._reportdataSet.Output.length;
            let _counter: number = 0;
            let _dataLength: number = this._reportdataSet.Output.length;
            this._reportdataSet.Output.forEach(rd => {
                if (rd.RowData.length > 0) {
                    //if (_counter == reportdataLength - 1) {
                    //    return;
                    //}
                    _counter += 1;
                    _bodyHtmlContent += "<tr>";
                    rd.RowData.forEach(column => {

                        // check for column available on report
                        if (this._reportColumns.map(x => { return x.key }).indexOf(column.Key) == -1) {
                            return;
                        }


                        let _value = this.GetRowKeyValueBasedOnConfiguration(column, column.Key);
                        var _widthStyling: string = 'width:auto;';
                        if (this._reportColumns.map(x => x.key).indexOf(column.Key) > -1) {
                            _widthStyling = 'width:' + this._reportColumns.filter(a => { return a.key == column.Key }).pop().width + ';';
                        }
                        var textalignstyling: string = 'text-align: center;';
                        if (this._reportColumns.map(x => x.key).indexOf(column.Key) > -1) {
                            let _datatype = this._reportColumns.filter(a => { return a.key == column.Key }).pop().DataType;
                            if (_datatype == "Decimal") {
                                textalignstyling = 'text-align: right;';
                            } else if (_datatype == "Integer" || _datatype == "Long") {
                                textalignstyling = 'text-align: center;';
                            }
                        }
                        var boldstyling: string = '';
                        if (_counter == _dataLength) {
                            boldstyling = 'font-weight: bolder';
                        }


                        if (!StringExtensions.isEmptyOrWhitespace(_value)) {
                            _bodyHtmlContent += "<td style='" + _widthStyling + textalignstyling + boldstyling + "'>" + _value + "</td>";
                        } else {
                            _bodyHtmlContent += "<td style='" + _widthStyling + textalignstyling + boldstyling + "'>" + "" + "</td>";
                        }
                    })
                    _bodyHtmlContent += '</tr>';
                }
            })
            _bodyHtmlContent += "</tbody>";

            // Footer
            _bodyHtmlContent += "<tfoot><tr>" + this.GetReportFooter() + "</tr></tfoot>";

            _bodyHtmlContent += '</table></div>';
            reportHtmlContent += _bodyHtmlContent;
            if (!this._isLandScapeMode) {
                reportHtmlContent += this.GetHtmlFooterContent();
            } else {
                reportHtmlContent += this.GetHtmlFooterContentLandScape();
            }

            return reportHtmlContent;
        }
        return "";
    }

    public GetRowKeyValueBasedOnConfiguration(_commerceProp: ProxyEntities.CommerceProperty, _key: string) {
        let _returnValueFormatted: string = "";
        try {
            if (this._reportColumns.length > 0
                && this._reportColumns.map(x => { return x.key }).indexOf(_key) > -1) {
                let _dataType: string = "";
                this._reportColumns.forEach(y => {
                    if (y.key == _key) {
                        _dataType = y.DataType;
                    }
                })
                switch (_dataType) {
                    case "Boolean":
                        _returnValueFormatted = _commerceProp.Value.BooleanValue == true ? "Yes" : "No";
                        break;
                    case "DateTime":
                        _returnValueFormatted = _commerceProp.Value.DateTimeOffsetValue.toDateString();
                        if (_returnValueFormatted == "1900/01/01 12:00:00 AM") { _returnValueFormatted = ""; }
                        if (_returnValueFormatted != "") {
                            var _date = new Date(_returnValueFormatted);
                            _returnValueFormatted = _date.toLocaleDateString("en-SG");
                            if (_date
                                && _date.getFullYear() < 1950) {
                                _returnValueFormatted = "";
                            }
                        }
                        break;
                    case "Decimal":
                        _returnValueFormatted = _commerceProp.Value.DecimalValue.toFixed(2);
                        break;
                    case "Integer":
                        _returnValueFormatted = _commerceProp.Value.IntegerValue.toString();
                        break;
                    case "Long":
                        _returnValueFormatted = _commerceProp.Value.LongValue.toString();
                        break;
                    case "String":
                        _returnValueFormatted = _commerceProp.Value.StringValue;
                        if (_returnValueFormatted == "1900/01/01 12:00:00 AM") { _returnValueFormatted = ""; }
                        if (_returnValueFormatted != "" && _key == "Date") {
                            var _date = new Date(_returnValueFormatted);
                            _returnValueFormatted = _date.toLocaleDateString("en-SG");
                            if (_date
                                && _date.getFullYear() < 1950) {
                                _returnValueFormatted = "";
                            }

                        }
                        break;
                    default:
                        _returnValueFormatted = _commerceProp.Value.StringValue;
                        break;
                }
            } else {
                return "";
            }
        } catch (_error) {
            this._context.logger.logError("Error on method GetRowKeyValueBasedOnConfiguration :: " + JSON.stringify(_error));
        }
        return _returnValueFormatted;
    }

    public GetReportParameterDataBasedOnConfiguration(_commerceProp: ProxyEntities.CommerceProperty, _key: string) {
        let _returnValueFormatted: string = "";
        try {
            if (this._reportParameters.length > 0
                && this._reportParameters.map(x => { return x.key }).indexOf(_key) > -1) {
                let _dataType: string = "";
                this._reportParameters.forEach(y => {
                    if (y.key == _key) {
                        _dataType = y.DataType;
                    }
                })
                switch (_dataType) {
                    case "Boolean":
                        _returnValueFormatted = _commerceProp.Value.BooleanValue == true ? "Yes" : "No";
                        break;
                    case "DateTime":
                        _returnValueFormatted = _commerceProp.Value.DateTimeOffsetValue.toDateString();
                        if (_returnValueFormatted == "1900/01/01 12:00:00 AM") { _returnValueFormatted = ""; }
                        if (_returnValueFormatted != "") {
                            // dd/MM/yyyy format
                            var _date = new Date(_returnValueFormatted);
                            _returnValueFormatted = _date.toLocaleDateString("en-SG");
                            if (_date
                                && _date.getFullYear() < 1950) {
                                _returnValueFormatted = "";
                            }
                        }
                        break;
                    case "Decimal":
                        _returnValueFormatted = _commerceProp.Value.DecimalValue.toFixed(2);
                        break;
                    case "Integer":
                        _returnValueFormatted = _commerceProp.Value.IntegerValue.toString();
                        break;
                    case "Long":
                        _returnValueFormatted = _commerceProp.Value.LongValue.toString();
                        break;
                    case "String":
                        _returnValueFormatted = _commerceProp.Value.StringValue;
                        if (_returnValueFormatted == "1900/01/01 12:00:00 AM") { _returnValueFormatted = ""; }
                        if (_returnValueFormatted != "" && _key == "Date") {
                            var _date = new Date(_returnValueFormatted);
                            _returnValueFormatted = _date.toLocaleDateString("en-SG");
                            if (_date
                                && _date.getFullYear() < 1950) {
                                _returnValueFormatted = "";
                            }
                        }
                        break;
                    default:
                        _returnValueFormatted = _commerceProp.Value.StringValue;
                        break;
                }

            } else {
                return "";
            }
        } catch (_error) {
            this._context.logger.logError("Error on method GetRowKeyValueBasedOnConfiguration :: " + JSON.stringify(_error));
        }
        return _returnValueFormatted;
    }


    public GetReportHeader() {
        let _reportHeaderHtmlContent = "<tr>";

        // Parameters 
        //let idx = 1;
        this._reportdataSet.Parameters.forEach(param => {
            if (this._reportParameters.map(x => x.key).indexOf(param.Key) > -1
                && param.Key != "nvc_UserId") {
                //if (idx % 2 == 0) {
                //    _reportHeaderHtmlContent += "";
                //}
                let _value = this.GetReportParameterDataBasedOnConfiguration(param, param.Key);
                let _name = "";
                this._reportParameters.forEach(a => {
                    if (a.key == param.Key) {
                        _name = a.Name;
                    }
                })
                var _widthStyling: string = 'width:auto;';
                if (this._reportParameters.map(x => x.key).indexOf(param.Key) > -1) {
                    _widthStyling = 'width:' + this._reportParameters.filter(a => { return a.key == param.Key }).pop().width + ';';
                }

                if (!StringExtensions.isEmptyOrWhitespace(_value)) {
                    _reportHeaderHtmlContent += "<td style='" + _widthStyling + "'><b>" + _name + " : " + _value + "</b></td>";
                } else {
                    _reportHeaderHtmlContent += "<td style='" + _widthStyling + "'><b>" + _name + " : " + "" + "</b></td>";
                }
                //if (idx % 2 == 0) {
                //    _reportHeaderHtmlContent += "</tr>";
                //}
                //idx++;
            }
        })

        _reportHeaderHtmlContent += "</tr>";

        // Employee Info on Top of report
        if (this._reportParameters.map(x => x.key).indexOf("nvc_UserId") > -1
        ) {
            _reportHeaderHtmlContent += "<tr>";
            let _value = this._staffId + "-" + this._staffName;
            let _name = "Employee Id";
            var _widthStyling: string = 'width:auto;';

            if (!StringExtensions.isEmptyOrWhitespace(_value)) {
                _reportHeaderHtmlContent += "<td style='" + _widthStyling + "'><b>" + _name + " : " + _value + "</b></td>";
            } else {
                _reportHeaderHtmlContent += "<td style='" + _widthStyling + "'><b>" + _name + " : " + "" + "</b></td>";
            }
            //_reportHeaderHtmlContent += "</tr>";

        }

        //_reportHeaderHtmlContent += "</tr>";

        // Store Info on Top of report
        if (this._reportParameters.map(x => x.key).indexOf("nvc_UserId") > -1
        ) {
            //_reportHeaderHtmlContent += "<tr>";
            let _value = this._storeName + "-" + this._storeId;
            let _name = "Store";
            var _widthStyling: string = 'width:auto;';

            if (!StringExtensions.isEmptyOrWhitespace(_value)) {
                _reportHeaderHtmlContent += "<td style='" + _widthStyling + "'><b>" + _name + " : " + _value + "</b></td>";
            } else {
                _reportHeaderHtmlContent += "<td style='" + _widthStyling + "'><b>" + _name + " : " + "" + "</b></td>";
            }
            _reportHeaderHtmlContent += "</tr>";
            _reportHeaderHtmlContent += "</tr>";
            _reportHeaderHtmlContent += "</tr>";

        }

        return _reportHeaderHtmlContent;
    }

    public GetReportFooter() {
        let _reportHeaderHtmlContent = '';

        return _reportHeaderHtmlContent;
    }

    public GetHtmlHeaderContent(islandscapemode: boolean) {
        if (islandscapemode) {
            return "<!DOCTYPE html><html><head><title>POS Report</title></head><body><div style='width: 900px;' id='reportContent'><div style='height:50px;'></div><div style='height:50px;text-align:center'><b>" + this._reportName + "</b></div>";
        } else {
            return "<!DOCTYPE html><html><head><title>POS Report</title></head><body><div style='width: 700px;' id='reportContent'><div style='height:50px;'></div><div style='height:50px;text-align:center'><b>" + this._reportName + "</b></div>";
        }
    }

    public GetHtmlFooterContent() {
        return "<div style='height:100px'></div><div style='height:100px;width:700px'><table><tbody><tr><td style='width:350px'>Approved By</td><td style='width:350px; text-align: right'>Reviewed By</td></tr><tr><td style='width:350px'></td><td style='width:350px; text-align: right'></td></tr><tr><td style='width:350px'></td><td style='width:350px; text-align: right'></td></tr><tr><td style='width:350px'></td><td style='width:350px; text-align: right'></td></tr><tr><td style='width:350px'>-----------------------</td><td style='width:350px; text-align: right'>------------------------</td></tr>" +
            "<tr><td style='width:350px'></td><td style='width:350px; text-align: right'></td></tr>" +
            "<tr><td style='width:350px'></td><td style='width:350px; text-align: right'></td></tr>" +
            "<tr><td style='width:350px'></td><td style='width:350px; text-align: right'></td></tr>" +
            "<tr><td style='width:350px'></td><td style='width:350px; text-align: right'>Date</td></tr>" +
            "<tr><td style='width:350px'></td><td style='width:350px; text-align: right'>------------------------</td></tr>" +
            "</tbody></table></div></div></body></html>";
    }
    public GetHtmlFooterContentLandScape() {
        return "<div style='height:100px'></div><div style='height:100px;width:900px'><table><tbody><tr><td style='width:450px'>Approved By</td><td style='width:450px; text-align: right'>Reviewed By</td></tr><tr><td style='width:450px'></td><td style='width:450px; text-align: right'></td></tr><tr><td style='width:450px'></td><td style='width:450px; text-align: right'></td></tr><tr><td style='width:450px'></td><td style='width:450px; text-align: right'></td></tr><tr><td style='width:450px'>-----------------------</td><td style='width:450px; text-align: right'>------------------------</td></tr>" +
            "<tr><td style='width:450px'></td><td style='width:450px; text-align: right'></td></tr>" +
            "<tr><td style='width:450px'></td><td style='width:450px; text-align: right'></td></tr>" +
            "<tr><td style='width:450px'></td><td style='width:450px; text-align: right'></td></tr>" +
            "<tr><td style='width:450px'></td><td style='width:450px; text-align: right'>Date</td></tr>" +
            "<tr><td style='width:450px'></td><td style='width:450px; text-align: right'>------------------------</td></tr>" +
            "</tbody></table></div></div></body></html>";
    }
}