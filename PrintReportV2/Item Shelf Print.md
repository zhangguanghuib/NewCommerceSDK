### How to implement Product/Item or Shelf Report print in POS?
1. Open AOT and find the RetailLabel Report,  add it into a project and open it as below:
  ![image](https://github.com/user-attachments/assets/c57e494d-d21c-4e16-8f62-c52f246d4e6e)

2. Choose one report design and open it with report designer:
   ![image](https://github.com/user-attachments/assets/24d47d7a-b56a-4288-adfd-46d9ebc17136)

3. Find the RDLC file in the file explorer:
    ![image](https://github.com/user-attachments/assets/2b3d884c-ac3f-4f4e-90a0-92a9eaf2ffed)

4. Copy the rdlc file into your project, and add two Nuget Packages:<br/>
   ![image](https://github.com/user-attachments/assets/64d0690a-8659-481d-9bb9-c701fb940397)

5. Write these below codes<br/>
    ```cs
    using Microsoft.Reporting.WinForms;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    
    namespace PrintReport
    {
        internal class Program
        {
            static void Main(string[] args)
            {
                // Prepare sample data
                DataTable dt = new DataTable();
                dt.Columns.Add("Name", typeof(string));
                dt.Columns.Add("Amount", typeof(decimal));
                dt.Rows.Add("Alice", 123.45m);
                dt.Rows.Add("Bob", 678.90m);
    
                // Create LocalReport and load RDLC
                LocalReport report = new LocalReport();
                report.ReportPath = @"./ReportDesign/SampleReport.rdlc"; // Path to your RDLC file
    
                // Add data source (name must match DataSet name in RDLC)
                report.DataSources.Add(new ReportDataSource("SampleDataSet", dt));
    
                // Render to PDF
                Warning[] warnings;
                string[] streamIds;
                string mimeType, encoding, extension;
                byte[] pdfBytes = report.Render(
                    "PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
    
                // Save PDF
                File.WriteAllBytes("SampleReport.pdf", pdfBytes);
    
                Console.WriteLine("PDF generated: SampleReport.pdf");
    
            }
        }
    }
    ```
    6. Run the project you can see the report can be printed:
       ![image](https://github.com/user-attachments/assets/b6da5f85-59d4-4ebf-a6f5-548990384253)

    7.  As for the Retail Product & Shelf label,  in D365 HQ,  its logic is like:
       ![image](https://github.com/user-attachments/assets/ea89002c-7ef3-43a4-86ce-8c5fc88c4eb7)

       ```
     private void insertTmpTable(RetailInventItemLabel _table)
    {
        int qty;
        RetailInventItemLabel   retailLabel;
        //<GEERU>
        RetailInventTable       retailInventTable;
        InventTable             inventTable;
        LanguageId              storeLanguage;
        //</GEERU>

        tmpTable.clear();
        if (columnNumber > 3)
        {
            columnNumber = 1;
        }

        tmpTable = this.initRetailLabelTmp(_table, tmpTable);

        retailInventTable = RetailInventTable::find(tmpTable.ItemId);

        if (retailInventTable)
        {
            inventTable = InventTable::find(tmpTable.ItemId);

            [storeLanguage, tmpTable.StoreAddress]   = this.getStoreInfo(_table.StoreId);

            tmpTable.CompanyName    = companyName;
            tmpTable.CompanyAddress = companyAddress;
            tmpTable.ValidOnDate    = _table.ValidOnDate;

            //<GEERU>
            if (countryRegionRU)
            {
                this.fillAttributeFields(inventTable, retailInventTable, storeLanguage);
            }
            //</GEERU>

            this.fillInventDimFields(inventTable, storeLanguage);
        }

        for (qty = 1; qty <= _table.Qty; qty++)
        {
            tmpTable.ColumnNumber = columnNumber;
            tmpTable.insert();
            columnNumber++;

            if (columnNumber > 3)
            {
                columnNumber = 1;
            }
        }

        eventSource.EventWriteReportingGenericMessage(classId2Name(ClassIdGet(this)), funcName(), strFmt('Records inserted into table: %1', tableStr(RetailLabelTmp)));

        ttsBegin;
        select forUpdate firstOnly retailLabel where retailLabel.RecId == _table.RecId;

        if (retailLabel)
        {
            retailLabel.Printed = true;
            retailLabel.update();
        }
        else
        {
            eventSource.EventWriteReportingGenericMessage(classId2Name(ClassIdGet(this)), funcName(), strFmt('Field Printed is unmarked into table: %1', tableStr(RetailInventItemLabel)));
        }

        ttsCommit;
    }
       ```


   
