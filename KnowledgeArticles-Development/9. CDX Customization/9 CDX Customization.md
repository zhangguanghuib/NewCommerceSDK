## D365 Commerce CDX Customization.

1. <ins>Background:</ins><br/>
During the D365 Commerce Project Implementation, create custom table and push and pull data between HQ and CSU database is very important, this article will show cases how to make CDX customization based on the official samples.
2. First of all, every time when a new extension sql script is deployed to CSU, it will recorded into the below table:<br/>
   ```sql
   SELECT *
   FROM [crt].[RETAILUPGRADEHISTORY]
   ```
   <img width="1164" alt="image" src="https://github.com/user-attachments/assets/db5d4248-0a55-4a53-8136-701fc58ec386"><br/>
   The same sql script will not be deployed two times, so in order to make some change on the original script,  you have to make a new script and named it in the aphebetic order, like ExtensionTableScriptV1.sql, ExtensionTableScriptV2.sql, ExtensionTableScriptV3.sql... ect.<br/>

3. The most knowledge of the CDX customization comes from these two official document: <br/>
    ### https://learn.microsoft.com/en-us/dynamics365/commerce/dev-itpro/cdx-extensibility
    ### https://learn.microsoft.com/en-us/dynamics365/commerce/dev-itpro/channel-db-extensions#adding-a-new-table
4.  The below knowledge all comes from the above two documents<br/>
+ Step 1:  let us review the XML for Custom Job and Subjobs creation including the table field mapping between HQ table and Channel Table:<br/>
```xml
<RetailCdxSeedData Name="AX7" ChannelDBSchema="ext" ChannelDBMajorVersion="7">
  <Jobs>
    <!--Adding a new download job to push the records in ContosoRetailSeatingData. It is not a must to create a new Job. Note that the subjob can be configured to run under existing jobs-->
    <Job Id="ContosoRetial7000" Description="Seating data Job" DescriptionLabelId="@ContosoRetailLabel:SeattingDataCDXJob" />
  </Jobs>
  
  <Subjobs>
    <!--Adding existing subjob to another job. -->
    <Subjob Id="DirPartyTable">
      <ScheduledByJobs>
        <ScheduledByJob>1000</ScheduledByJob> <!--add existing subjob to another job-->
      </ScheduledByJobs>
      <!--Notice that there is no mention of the <AxFields></AxFields> because the subjob is already mapped in the main RetailCdxSeedData resource file an we are not adding any new or previously unmapped field. -->
    </Subjob>

    <!--Adding additional columns to (existing) RetailTransactionTable so they are pulled back to AX.
For upload subjobs, set the OverrideTarget property to  "false", as ilustrate below. This will tell CDX to use the table defined by TargetTableName and TargetTableSchema as extension table on this subjob.
        This table must be created on the channel DB with the same primary key as the extended table (in this example, RetailTransactionTable).
    -->
    <Subjob Id="RetailTransactionTable" TargetTableName="CONTOSORETAILTRANSACTIONTABLE" TargetTableSchema="ext" OverrideTarget="false">
      <!--Notice that there is no mention of the <ScheduledByJobs></ScheduledByJobs> because the subjob is already part of an upload job. -->
      <AxFields>
        <!--If you notice the existing columns are not listed here in the <Field> tag, this is because the eisting fields are already mapped mapped in the main RetailCdxSeedData resource file we only add the delta here. -->
        <Field Name="ContosoRetailSeatNumber" />
        <Field Name="ContosoRetailServerStaffId" />
      </AxFields>
    </Subjob>
    
    <Subjob Id="RetailTransactionPaymentTrans" TargetTableName="CONTOSORETAILTRANSACTIONPAYMENTTRANS" TargetTableSchema="ext" OverrideTarget="false">
      <AxFields>
        <Field Name="BankTransferComment" />
      </AxFields>
    </Subjob>

    <!-- Adding new subjob to existing upload/pull job to pull the records from ContosoRetailStaffSuggestions -->
    <Subjob Id="ContosoRetailStaffSuggestions" AxTableName ="ContosoRetailStaffSuggestions" ReplicationCounterFieldName="ReplicationCounterFromOrigin" IsUpload="true" TargetTableSchema="ext">
      <ScheduledByJobs>
        <!--Here we specify what Job is used to run this subjob. This is because the subjob is new and we need to specify which jobs its part of.-->
        <ScheduledByJob>P-1000</ScheduledByJob>
      </ScheduledByJobs>
      <AxFields>
        <Field Name="SuggestionId" />
        <Field Name="StoreId" />        
        <Field Name="TerminalId" />
        <!--If the partner has named this column differently on the channel side this is how to map the ax side column to the channel side column which has a different name  -->
        <Field Name="SuggestionOrRequest" ToName="Suggetion" /> <!-- the column is named suggestion on channel side but on AX its SuggestionOrRequest-->
        <Field Name="DateLogged" />
        <Field Name="ReplicationCounterFromOrigin" />
      </AxFields>
    </Subjob>

    <!--Adding new subjob to existing download job-->
    <!--Note the TargetTableName attribute is used if the AX side table and its corresponding channelTable names are different. 
    If the AX and channel side table are the same there is no need to specify the TargetTableName only specifying the AXTableName is enough.-->
    <Subjob Id="ContosoRetailSeatingData" AxTableName ="ContosoRetailSeatingData" TargetTableName="ContosoRetailTableData" TargetTableSchema="ext">
      <ScheduledByJobs>
        <ScheduledByJob>ContosoRetial7000</ScheduledByJob>
      </ScheduledByJobs>
      <AxFields>
        <Field Name="SeatNumber" />
        <!--Not a best practice but if you want the column names to be different on the ax and channel side u can do it using the toName attribute of <Field>-->
        <Field Name="Capacity" ToName="NumberOfChairs" />
        <Field Name="StoreNumber" />
        <Field Name="RecId" />
      </AxFields>
    </Subjob>
    
    <!--How to update subjob to include previously not pushed columns or new columns added as extensions on the existing table-->
    <!--Notice that the AXTableName is not specified in the <subjob .... > because we are modifying an existing subjob and
    the AXTableName is already specified in the original CDXSeedData resource file. 
    Notice that the name of the table on the target/channel side is set using the  TargetTableName="ContosoRetailChannelTable" change the targetTableName 
    is a recommended practice if you are modifying an existing table.  This will help to avoid naming conflict if the same table is customized by different partners.-->
    <Subjob Id="RetailChannelTable" TargetTableName="ContosoRetailChannelTable" TargetTableSchema="ext">
      <AxFields>
        <Field Name="Payment" /> <!-- Existing column which was not pushed to channel db-->
        <Field Name="PaymMode" /> <!-- Existing column which was not pushed to channel db-->
        <Field Name="ContosoRetailWallPostMessage" /> <!-- New column from the extended table -->
      </AxFields>
    </Subjob>
    
    <!--Updating subjob to include previously not pushed columns or new extension columns-->
    <!--table with non RECID primary key-->
    <!--Notice that the target table name is changed to avoid naming conflict in case the same table is customized by several partners. This is a best practice eventhough using the same name will also work.-->
    <Subjob Id="RetailCustTable" TargetTableName="ContosoRetailCustTable" TargetTableSchema="ext"> <!--The AXTableName attribute is not used here because this is an already existing subjob and we are only changing the mapping. So no need to specify values of the attributes that we dont need to change -->
      <AxFields>
        <Field Name="ReturnTaxGroup_W" /> <!-- Existing column which was not pushed to channel db-->
        <Field Name="ContosoRetailSSNNumber" /> <!-- New column from the extended table-->
      </AxFields>
    </Subjob>

  </Subjobs>
  </RetailCdxSeedData>
```
+ Step 2: Check the Distribution Scheduler and Job and Subjobs<mark>For Upload Data from Channel DB to FO DB</mark>:
  - Table DirPartyTable <br/>
    <img width="721" alt="image" src="https://github.com/user-attachments/assets/ada946d6-b4fa-4eee-9063-ae0f402a4e40">
  - RetailTransactionTable<br/>
    <img width="1148" alt="image" src="https://github.com/user-attachments/assets/139ee594-de62-4463-9561-cf9e185d4f02"><br/>
    and from this table we can find the Channel Table Name and its fields:<br/>
    <img width="679" alt="image" src="https://github.com/user-attachments/assets/745723f0-2e72-400e-b320-7e06e49266f5">
  - RetailTransactionPaymentTrans<br/>
  <img width="1175" alt="image" src="https://github.com/user-attachments/assets/9c032705-7e4b-4b94-a06f-a061c97a80c4"><br/>
  <img width="715" alt="image" src="https://github.com/user-attachments/assets/24582c54-1d08-4ec2-b631-e65942d404a6"><br/>
  - ContosoRetailStaffSuggestions <br/>
    ![image](https://github.com/user-attachments/assets/48c370d0-4c8d-4a96-baf9-8671094241d1)<br/>
    In Channel Database:<br/>
    ![image](https://github.com/user-attachments/assets/278cb01d-4b84-437d-b368-6d0c34120d64)
    In AOT: <br/>
    <img width="702" alt="image" src="https://github.com/user-attachments/assets/68be75f4-f456-4e3d-8f9f-98dca320554e"><br/>
+ Step 3: Check the Distribution Scheduler and Job and Subjobs<mark>For Push Data from FO DB to CHannel DB(download sessions)</mark






   


