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

export default class POSReportHelper {

    public _reportdataSet: ProxyEntities.ReportDataSet;
    public _reportId: string;
    public _reportColumns: Array<IreportColumn>;
    public _reportParameters: Array<IreportColumn>;
    public _context: IExtensionContext;
    public _reportName: string;
    public _isLandScapeMode: boolean;
    public _staffName: string;
    public _staffId: string;
    public _rowCount: number;
    public paramHeight: number;
    public columnHeaderHeight:number 
    public footerHeight: number;
    public spaceAfterParam: number;
    public titleHeight: number;
    public spaceBeforeSign: number;
    public dottedLineHeight: number;

    constructor(_reportdata: ProxyEntities.ReportDataSet, reportid: string, reportcolumns: IreportColumn[], context: IExtensionContext,
        reportName: string, reportParameters: IreportColumn[], islandscapeMode: boolean, staffname: string, staffid: string) {
        this._reportdataSet = _reportdata;
        this._reportId = reportid;
        this._reportColumns = reportcolumns;
        this._context = context;
        this._reportName = reportName;
        this._reportParameters = reportParameters;
        this._isLandScapeMode = islandscapeMode;
        this._staffId = staffid;
        this._staffName = staffname;
        this.paramHeight = 0;
        this.columnHeaderHeight = 30;
        this.spaceAfterParam = 10;
        this.titleHeight = 50;
        this.spaceBeforeSign = 100;
        this.dottedLineHeight = 15;
    }

    
    public GetReportFormattedHtml() {
        if (this._reportId != ""
            && !ObjectExtensions.isNullOrUndefined(this._reportdataSet)
            && !ObjectExtensions.isNullOrUndefined(this._reportColumns)
            && this._reportColumns.length > 0
        ) {
            let reportHtmlContent = "";
            // Get report title.
            reportHtmlContent += this.GetHtmlHeaderContent(this._isLandScapeMode);

            // Get Parameters Headers
            reportHtmlContent += "<div style='height:auto;width:100%'><table style='width:100%'><tbody>" + this.GetReportHeader() + "</tbody></table></div>";
            reportHtmlContent += "<div style='height:" + this.spaceAfterParam.toString() + "px'> </div>";//separator between parameter & header column.

            let _bodyHtmlContent = '';
            _bodyHtmlContent += "<div style='height: 70%'><table>";

            // Header
            _bodyHtmlContent += "<thead>" + "" + "</thead>";

            // Body
            // column header
            //let countCol: number = 0;
            _bodyHtmlContent += "<tbody>";
            _bodyHtmlContent += "<tr style='vertical-align:text-bottom;height:" + this.columnHeaderHeight + "px'>";
            this._reportColumns.forEach(x => {
                //countCol += 1;
                var textalignstylingHeader: string = 'text-align: left;';
                var _widthStyling: string = 'width:auto;';
                if (this._reportColumns.map(x => x.key).indexOf(x.key) > -1) {
                    _widthStyling = 'width:' + this._reportColumns.filter(a => { return a.key == x.key }).pop().width + ';';
                }

                let columnType = this._reportColumns.filter(a => { return a.key == x.key }).pop().DataType;

                if (columnType == "Integer" || columnType == "Long") {
                    textalignstylingHeader = 'text-align: center;';
                }
                if (columnType == "Decimal") {
                    textalignstylingHeader = 'text-align: right; padding: 0px 10px 0px 0px;';
                }
                
                //_bodyHtmlContent += "<td style='" + _widthStyling + "text-align: center'><b>" + x.Name + "</b></td>";
                _bodyHtmlContent += "<td style='" + _widthStyling + textalignstylingHeader+"'><b>" + x.Name + "</b></td>";
            })
            _bodyHtmlContent += "</tr>";

            // column data
           // let reportdataLength: number = this._reportdataSet.Output.length;
            let _counter: number = 0;
            let _dataLength: number = this._reportdataSet.Output.length;
            this._rowCount = _dataLength;

            this._reportdataSet.Output.forEach(rd => {
                if (rd.RowData.length > 0) {
                    //if (_counter == reportdataLength - 1) {
                    //    return;
                    //}
                    _counter += 1;
                    if (_counter == _dataLength)//show border as a total line for the last row.
                    {
                        if (this._isLandScapeMode)
                            _bodyHtmlContent += "<tr style='height:" + this.dottedLineHeight + "px'><td colspan='" + this._reportColumns.length.toString() + "'>--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------</td></tr>";
                        _bodyHtmlContent += "<tr class='totalLine'>";
                    }
                    else
                        _bodyHtmlContent += "<tr>";

                    rd.RowData.forEach(column => {

                        // check for column available on report
                        if (this._reportColumns.map(x => { return x.key }).indexOf(column.Key) == -1) {
                            return;
                        }

                        let _value = this.GetRowKeyValueBasedOnConfiguration(column, column.Key);
                        
                        var _widthStyling: string = 'width:auto;';
                        var _heightStyling: string = 'height:auto;';
                        if (this._reportColumns.map(x => x.key).indexOf(column.Key) > -1) {
                            _widthStyling = 'width:' + this._reportColumns.filter(a => { return a.key == column.Key }).pop().width + ';';
                            if (_counter == _dataLength)
                                _heightStyling = 'height:30px;vertical-align:top;';//for total row
                            else
                                _heightStyling = 'height:30px;';
                        }

                        var textalignstyling: string = 'text-align: left;';
                        if (this._reportColumns.map(x => x.key).indexOf(column.Key) > -1) {
                            let _datatype = this._reportColumns.filter(a => { return a.key == column.Key }).pop().DataType;
                            if (_datatype == "Decimal") {
                                textalignstyling = 'text-align: right; padding-right: 10px;';
                            } else if (_datatype == "Integer" || _datatype == "Long") {
                                textalignstyling = 'text-align: center;';
                            } 
                        }
                        
                        var boldstyling: string = '';
                        if (_counter == _dataLength) {
                            boldstyling = 'font-weight: bolder';
                        }

                        if (!StringExtensions.isEmptyOrWhitespace(_value)) {
                            _bodyHtmlContent += "<td style='" + _widthStyling + _heightStyling + textalignstyling + boldstyling + "'>" + _value + "</td>";
                        } else {
                            _bodyHtmlContent += "<td style='" + _widthStyling + _heightStyling + textalignstyling + boldstyling + "'>" + "" + "</td>";
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
                            if (this._reportdataSet.Locale == "en-US") {
                                var _date = new Date(_returnValueFormatted);
                                _returnValueFormatted = _date.toLocaleDateString("en-SG");
                            }
                            else {
                                var _date = new Date(_returnValueFormatted);
                                var pos = _returnValueFormatted.search(" ");
                                if (pos > 0)
                                    _returnValueFormatted = _returnValueFormatted.substring(0, pos);
                            }
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
        let _reportHeaderHtmlContent = "";

        // Parameters 
        //#Parameter Store
        if (this._reportdataSet.Parameters.filter(x => { return (x.Key == "nvc_StoreId1" || x.Key == "nvc_StoreId2"); }).length > 0) {
            _reportHeaderHtmlContent += "<tr style='height:15px;'>";
            this.paramHeight += 15;
            this._reportdataSet.Parameters.filter(x => { return (x.Key == "nvc_StoreId1" || x.Key == "nvc_StoreId2"); }).forEach(param => {
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
            })
            _reportHeaderHtmlContent += "</tr>";
        }
        //#Parameter item1 & item2
        if (this._reportdataSet.Parameters.filter(x => { return (x.Key == "nvc_ItemId1" || x.Key == "nvc_ItemId2"); }).length > 0) {
            _reportHeaderHtmlContent += "<tr style='height:15px;'>";
            this.paramHeight += 15;
            this._reportdataSet.Parameters.filter(x => { return (x.Key == "nvc_ItemId1" || x.Key == "nvc_ItemId2"); }).forEach(param => {
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
            })
            _reportHeaderHtmlContent += "</tr>";
        }
        //# Parameter Date
        if (this._reportdataSet.Parameters.filter(x => { return (x.Key == "dt_StartDate" || x.Key == "dt_EndDate"); }).length > 0) {
            _reportHeaderHtmlContent += "<tr style='height:15px;'>";
            this.paramHeight += 15;
            this._reportdataSet.Parameters.filter(x => { return (x.Key == "dt_StartDate" || x.Key == "dt_EndDate"); }).forEach(param => {
                if (this._reportParameters.map(x => x.key).indexOf(param.Key) > -1) {

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
        }
        
        
        // Employee Info on Top of report
        if (this._reportParameters.map(x => x.key).indexOf("nvc_UserId") > -1) {
            _reportHeaderHtmlContent += "<tr style='height:15px;'>";
            this.paramHeight += 15;
            let _value = this._staffName;
            let _name = "Employee Id";
            var _widthStyling: string = 'width:auto;';

            if (!StringExtensions.isEmptyOrWhitespace(_value)) {
                _reportHeaderHtmlContent += "<td style='" + _widthStyling + "'><b>" + _name + " : " + _value + "</b></td>";
            } else {
                _reportHeaderHtmlContent += "<td style='" + _widthStyling + "'><b>" + _name + " : " + "" + "</b></td>";
            }
            _reportHeaderHtmlContent += "</tr>";
        }
        
        
        return _reportHeaderHtmlContent;
    }

    public GetReportFooter() {
        let _reportHeaderHtmlContent = '';

        return _reportHeaderHtmlContent;
    }

    public GetHtmlHeaderContent(islandscapemode: boolean) {
        let cssstyle: string = "<style>tr.totalLine{border-top:1px solid black} td {font-family:arial,Helvetica,sans-serif; font-size:12px;} .reporttitle {font-size:20px; font-family:arial,Helvetica,sans-serif;}</style>";
        if (islandscapemode) {
            return "<!DOCTYPE html><html><head><title>POS Report</title>" + cssstyle + "</head>" +
                "<body> <div style='width: 1000px;' id = 'reportContent'> <div style='height:" + this.titleHeight + "px;text-align:center'><b class='reporttitle'>" + this._reportName + "</b> </div>";
        } else {
            return "<!DOCTYPE html><html><head><title>POS Report</title>" + cssstyle + "</head>" +
                "<body> <div style='width: 700px;' id = 'reportContent'> <div style='height:" + this.titleHeight + "px;text-align:center'><b class='reporttitle'>" + this._reportName + "</b> </div>";
        }
    }

    public GetHtmlFooterContent() {
        return "<div style='height:" + this.spaceBeforeSign + "px'></div><div style='height:100px;width:700px'><table><tbody>" +
            "<tr><td style='width:350px'>-----------------------</td><td style='width:350px; text-align: right'>------------------------</td></tr>" +
            "<tr><td style='width:350px'>Approved By</td><td style='width:350px; text-align: right'>Reviewed By</td></tr>" +
            "<tr><td style='width:350px'></td><td style='width:350px; text-align: right'></td></tr>" +
            "<tr><td style='width:350px'></td><td style='width:350px; text-align: right'></td></tr>" +
            "<tr><td style='width:350px'></td><td style='width:350px; text-align: right'></td></tr>" +
            
            "<tr><td style='width:350px; text-align: left'>------------------------</td><td style='width:350px'></td></tr>" +
            "<tr><td style='width:350px; text-align: left'>Date</td><td style='width:350px'></td></tr>" +
            "</tbody></table></div></div></body></html>";
    }

    public getSpaceHeight() {
        //A4 ;
        /*
            document title : 50px
            start date     : 15px
            employee id    : 15px
            space          : 10px

            footer         : 110px
        */
        let signHeight: number;
        let remainSpaceHeight: number;
        let pageHeight: number = 1020;//26.5cm portrait
        if (this._isLandScapeMode) {
            pageHeight = 645;
            signHeight = 110;//from GetHtmlFooterContentLandScape()
        }

        let headerHeight = this.titleHeight + this.paramHeight + this.spaceAfterParam;
        let bodyHeight = this.columnHeaderHeight + (this._rowCount * 30) + this.dottedLineHeight;
        let footerHeight = this.spaceBeforeSign + signHeight;

        let headerBodyHeight = headerHeight + bodyHeight;

        if (headerBodyHeight <= pageHeight) {
            remainSpaceHeight = pageHeight - headerBodyHeight;
            if (footerHeight <= remainSpaceHeight)
                return this.spaceBeforeSign;
            else {
                return remainSpaceHeight + this.spaceBeforeSign;//so that the footer will go to the next page.
            }
        }
        else {
            let headerBodyTailHeight = headerBodyHeight % pageHeight;
            remainSpaceHeight = pageHeight - headerBodyTailHeight;
            if (footerHeight <= remainSpaceHeight)
                return this.spaceBeforeSign;
            else {
                return remainSpaceHeight + this.spaceBeforeSign;//so that the footer will go to the next page.
            }
        }
    }

    public GetHtmlFooterContentLandScape() {
        let footerStr: string = "";
        footerStr = "<div style='height:" + this.getSpaceHeight().toString() + "px'></div><div style='height:100px;width:1000px'><table><tbody>" +
            "<tr style='height:15px;'><td style='width:500px'>-----------------------</td><td style='width:500px;text-align:right'>------------------------</td></tr>" +
            "<tr style='height:15px;'><td style='width:500px'>Approved By</td><td style='width:500px;text-align:right'>Reviewed By</td></tr>" +
            "<tr style='height:50px;'><td style='height:50px;width:500px'></td><td style='width:500px;text-align:right'></td></tr>" +
            "<tr style='height:15px;'><td style='width:500px;text-align:left'>------------------------</td><td style='width:500px'></td></tr>" +
            "<tr style='height:15px;'><td style='width:500px;text-align:left'>Date</td><td style='width:500px'></td></tr>" +
            "</tbody></table></div>";

        if (this._reportId == "131") {//Top sales report doesn't require signature.
            footerStr = "</div></body></html>";
        }
        return footerStr;
        /*
        return "</div><div style='height:100px;width:1000px'><table><tbody><tr><td style='width:500px'>Approved By</td><td style='width:500px; text-align: right'>Reviewed By</td></tr><tr><td style='width:450px'></td><td style='width:450px; text-align: right'></td></tr><tr><td style='width:450px'></td><td style='width:450px; text-align: right'></td></tr><tr><td style='width:450px'></td><td style='width:450px; text-align: right'></td></tr><tr><td style='width:450px'>-----------------------</td><td style='width:450px; text-align: right'>------------------------</td></tr>" +
            "<tr><td style='width:450px'></td><td style='width:450px; text-align: right'></td></tr>" +
            "<tr><td style='width:450px'></td><td style='width:450px; text-align: right'></td></tr>" +
            "<tr><td style='width:450px'></td><td style='width:450px; text-align: right'></td></tr>" +
            "<tr><td style='width:450px'></td><td style='width:450px; text-align: right'>Date</td></tr>" +
            "<tr><td style='width:450px'></td><td style='width:450px; text-align: right'>------------------------</td></tr>" +
            "</tbody></table></div></div></body></html>";
            */
    }
}