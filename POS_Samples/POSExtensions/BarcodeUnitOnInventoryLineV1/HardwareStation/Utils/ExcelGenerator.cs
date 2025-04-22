namespace Contoso
{
    namespace Commerce.HardwareStation
    {
        using Contoso.StoreCommercePackagingSample.HardwareStation.Messages;
        using DocumentFormat.OpenXml;
        using DocumentFormat.OpenXml.Packaging;
        using DocumentFormat.OpenXml.Spreadsheet;
        using Microsoft.Dynamics.Commerce.Runtime.DataModel;
        using System;
        using System.IO;

        public class ExcelGenerator
        {
            public void ExportToExcelFile(string filePath, InventoryInboundOutboundSourceDocumentLine[] lines)
            {
                // Create the Excel file
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
                {
                    // Add a WorkbookPart to the document
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    // Add a WorksheetPart to the WorkbookPart
                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet(new SheetData());

                    // Add Sheets to the Workbook
                    Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());

                    // Append a new sheet and associate it with the WorksheetPart
                    Sheet sheet = new Sheet()
                    {
                        Id = document.WorkbookPart.GetIdOfPart(worksheetPart),
                        SheetId = 1,
                        Name = "InventoryInboundOutboundSourceDocumentLines"
                    };
                    sheets.Append(sheet);

                    // Get the SheetData from the Worksheet
                    SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                    // Add the header row
                    Row headerRow = new Row();
                    var properties = typeof(InventoryInboundOutboundSourceDocumentLine).GetProperties();
                    foreach (var property in properties)
                    {
                        headerRow.Append(CreateCell(property.Name, CellValues.String));
                    }
                    sheetData.Append(headerRow);

                    // Add data rows
                    foreach (var line in lines)
                    {
                        Row dataRow = new Row();
                        foreach (var property in properties)
                        {
                            var value = property.GetValue(line);
                            if (value == null)
                            {
                                dataRow.Append(CreateCell(string.Empty, CellValues.String));
                            }
                            else
                            {
                                // Determine the data type and create the cell accordingly
                                if (value is int || value is long || value is decimal)
                                {
                                    dataRow.Append(CreateCell(value.ToString(), CellValues.Number));
                                }
                                else if (value is DateTime || value is DateTimeOffset)
                                {
                                    dataRow.Append(CreateCell(((DateTime)value).ToString("o"), CellValues.Date));
                                }
                                else if (value is bool)
                                {
                                    dataRow.Append(CreateCell((bool)value ? "1" : "0", CellValues.Boolean));
                                }
                                else
                                {
                                    dataRow.Append(CreateCell(value.ToString(), CellValues.String));
                                }
                            }
                        }
                        sheetData.Append(dataRow);
                    }

                    // Save the workbook
                    workbookPart.Workbook.Save();
                }
            }

            private Cell CreateCell(string value, CellValues dataType)
            {
                return new Cell()
                {
                    CellValue = new CellValue(value),
                    DataType = dataType
                };
            }

        }

    }
}
