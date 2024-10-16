## D365 Commerce CDX Customization.

1. <ins>Background:</ins><br/>
During the D365 Commerce Project Implementation, create custom table and push and pull data between HQ and CSU database is very important, this article will show cases how to make CDX customization based on the official samples.
2. First of all, every time when a new extension sql script is deployed to CSU, it will be recorded into the below table:<br/>
   ```sql
   SELECT *
   FROM [crt].[RETAILUPGRADEHISTORY]
   ```
   <img width="1164" alt="image" src="https://github.com/user-attachments/assets/db5d4248-0a55-4a53-8136-701fc58ec386"><br/>
   The same sql script will not be deployed two times, so in order to make some change on the original script,  you have to make a new script and named it in the aphebetic order, like ExtensionTableScriptV1.sql, ExtensionTableScriptV2.sql, ExtensionTableScriptV3.sql... ect.<br/>

3. The most knowledge of the CDX customization comes from these two official documents: <br/>
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
   
       <!--
   				Adding additional columns to (existing) RetailTransactionTable so they are pulled back to AX.
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
           <ScheduledByJob>P-0001</ScheduledByJob>
         </ScheduledByJobs>
         <AxFields>
           <Field Name="SuggestionId" />
           <Field Name="StoreId" />        
           <Field Name="TerminalId" />
           <Field Name="Staff" />
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
+ Step 2: Check the Distribution Scheduler and Jobs and Subjobs created by above XML.<br/>
  1  <mark>Table DirPartyTable=>Adding existing subjob to another job.</mark>  <br/>
    <img width="721" alt="image" src="https://github.com/user-attachments/assets/ada946d6-b4fa-4eee-9063-ae0f402a4e40">
    <hr/>
  2. <mark>RetailTransactionTable=>Upload Session=>Add new custom columns to existing table</mark>:<br/>
    <img width="1148" alt="image" src="https://github.com/user-attachments/assets/139ee594-de62-4463-9561-cf9e185d4f02"><br/>
    and from this table we can find the Channel Table Name and its fields:<br/>
    <img width="679" alt="image" src="https://github.com/user-attachments/assets/745723f0-2e72-400e-b320-7e06e49266f5"><br/>
  3. <mark>RetailTransactionPaymentTrans=>Upload Session=>Add new custom columns to existing table</mark>:<br/>
     <img width="1175" alt="image" src="https://github.com/user-attachments/assets/9c032705-7e4b-4b94-a06f-a061c97a80c4"><br/>
     and from this table we can find the Channel Table Name and its fields:<br/>
     <img width="715" alt="image" src="https://github.com/user-attachments/assets/24582c54-1d08-4ec2-b631-e65942d404a6"><br/>
  4. <mark>ContosoRetailStaffSuggestions =>Upload Session=>Totally new table in HQ and Channel Database</mark>:<br/>
  
    ![image](https://github.com/user-attachments/assets/48c370d0-4c8d-4a96-baf9-8671094241d1)<br/>
    mi=RetailConnLocationDesignTable:<br/>   
    <img width="677" alt="image" src="https://github.com/user-attachments/assets/16cafa67-f607-481c-a39c-5ffcbf9a5b99"><br/>
  5. <mark>ContosoRetailSeatingData：</mark>a new table to push the data from HQ to Channel:<br/>
      (HQ table name: ContosoRetailSeatingData/ Channel Table Name: ext.ContosoRetailTableData):<br/>
       ![image](https://github.com/user-attachments/assets/661592ec-197a-4451-a5c1-96d40c6d4742)<br/>
       ![image](https://github.com/user-attachments/assets/8b0bc3e6-97ca-4c39-bb6d-18bc7b82bef0)<br/>
       ![image](https://github.com/user-attachments/assets/68682463-f85e-4746-a9cf-ad34efa92a2a)<br/>
  6. <mark>RetailChannelTable => Download Session=>Existing table with existing columns and new columns<br/>
     <img width="1277" alt="image" src="https://github.com/user-attachments/assets/5850440e-a8c2-4716-b2a1-c58cc0bc5aa9"><br/>
     <img width="708" alt="image" src="https://github.com/user-attachments/assets/675e630a-46dc-4974-8ae5-00abe7094282"><br/>
  7. <mark>RetailCustTable=> Download Session=>Existing table with existing columns and new columns</mark><br/>
     <img width="1278" alt="image" src="https://github.com/user-attachments/assets/cbe31e7e-99be-4022-bb97-c3b377719b59"><br/>
     <img width="611" alt="image" src="https://github.com/user-attachments/assets/0db393c4-bd13-453d-a5c3-75f0ed9765df"><br/>
+ Step 3: Table Structure in HQ and Channel Database<br/>
    1 <mark>ContosoRetailSeatingData：</mark>a new table to push the data from HQ to Channel:<br/>
       Table in FO AOT <br/>
       <img width="704" alt="image" src="https://github.com/user-attachments/assets/74d3753f-ce31-4f8f-8438-9d6d63708ad9"><br/>
       Table in Channel DB <br/>
       ![image](https://github.com/user-attachments/assets/f2abf0a4-52b0-43d8-bcc8-38dcefdc19f1)<br/><br/>
```sql
         IF (SELECT OBJECT_ID('[ext].[CONTOSORETAILTABLEDATA]')) IS NOT NULL 
         BEGIN
            DROP TABLE [EXT].[CONTOSORETAILTABLEDATA]
         END
         
         CREATE TABLE [ext].[CONTOSORETAILTABLEDATA](
            [SEATNUMBER] [int] NOT NULL,
            [NUMBEROFCHAIRS] [int] NOT NULL,
            [STORENUMBER] [nvarchar](10) NOT NULL,
            [RECID] [bigint] NOT NULL,
             CONSTRAINT [PK_EXT_CONTOSORETAILTABLEDATA_RECID] PRIMARY KEY CLUSTERED (
            [RECID] ASC
         )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
            CONSTRAINT [PK_EXT_CONTOSORETAILTABLEDATA]  UNIQUE NONCLUSTERED (
            [SEATNUMBER] ASC,
            [STORENUMBER] ASC
         )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
         ) ON [PRIMARY]
         GO
         
         GRANT INSERT, DELETE, UPDATE, SELECT ON OBJECT::[ext].[CONTOSORETAILTABLEDATA] TO [DataSyncUsersRole];
         GO
```
       
   2  <mark>RetailChannelTable => Download Session=>Existing table with existing columns and new columns<br/>
      - <mark>Table in Channel Table</mark><br/>
      ![image](https://github.com/user-attachments/assets/a02706a7-913d-4d46-b530-3372bfd53298)<br/>
      - <mark>Table in AOT</mark><br/>
      <img width="711" alt="image" src="https://github.com/user-attachments/assets/500218c1-6e31-4f1e-a780-d40fe637c9c2"><br/>

```sql
         IF (SELECT OBJECT_ID('[ext].[CONTOSORETAILCHANNELTABLE]')) IS NOT NULL  
         BEGIN
            DROP TABLE [EXT].[CONTOSORETAILCHANNELTABLE]
         END
         
         CREATE TABLE [ext].[CONTOSORETAILCHANNELTABLE](
         	[PAYMENT] [nvarchar](10) NOT NULL,
         	[PAYMMODE] [nvarchar](10) NOT NULL,
         	[CONTOSORETAILWALLPOSTMESSAGE] [nvarchar](255) NOT NULL,
         	[RECID] [bigint] NOT NULL,
         	 CONSTRAINT [PK_EXT_CONTOSORETAILCHANNELTABLE_RECID] PRIMARY KEY CLUSTERED 
         (
         	[RECID] ASC
         )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
         ) ON [PRIMARY]
         GO
         
         GRANT INSERT, DELETE, UPDATE, SELECT ON OBJECT::[ext].[CONTOSORETAILCHANNELTABLE] TO [DataSyncUsersRole];
         GO
```
   3 <mark>RetailCustTable:</mark> push existing table columns and new columns to Channel Table from FO HQ Table<br/>
      - <mark>Table in Channel Database<mark/><br/>
      ![image](https://github.com/user-attachments/assets/98659cd9-68c0-4770-b5b0-d6c936b8c7e8)<br/>
      - <mark>Table in AOT</mark><br/>
      <img width="729" alt="image" src="https://github.com/user-attachments/assets/583dd6aa-2b39-403f-867e-a3b2ad24d5b3"><br/>

```sql
      IF (SELECT OBJECT_ID('[ext].[CONTOSORETAILCUSTTABLE]')) IS NOT NULL  
      BEGIN
        DROP TABLE [EXT].CONTOSORETAILCUSTTABLE
      END
      
      CREATE TABLE [ext].[CONTOSORETAILCUSTTABLE](
          [ACCOUNTNUM] [nvarchar](20) NOT NULL,
      	[DATAAREAID] [nvarchar](4) NOT NULL,
      	[RETURNTAXGROUP_W] [nvarchar](10) NOT NULL,
      	[CONTOSORETAILSSNNUMBER] [nvarchar](11) NOT NULL,
       CONSTRAINT [PK_EXT_CONTOSORETAILCUSTTABLE] PRIMARY KEY CLUSTERED 
      (
      	[ACCOUNTNUM] ASC,
      	[DATAAREAID] ASC
      )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
      ) ON [PRIMARY]
      GO
      
      GRANT INSERT, DELETE, UPDATE, SELECT ON OBJECT::[ext].[CONTOSORETAILCUSTTABLE] TO [DataSyncUsersRole];
      GO
```
  4. RetailTransactionTable<br/>
      1. AOT<br/>
        <img width="1031" alt="image" src="https://github.com/user-attachments/assets/85e3c868-5c1f-44f3-a048-062a00c1bda7"><br/>

     2. Channel<br/>
        ![image](https://github.com/user-attachments/assets/5b9a1294-b1c3-4fb9-aec6-f07b870305b6)<br/>
     3. Sql Script<br/>
     ```sql
        IF (SELECT OBJECT_ID('[ext].[CONTOSORETAILTRANSACTIONTABLE]')) IS NOT NULL 
         BEGIN
            DROP TABLE [EXT].CONTOSORETAILTRANSACTIONTABLE
         END
         GO
         
         CREATE TABLE [ext].[CONTOSORETAILTRANSACTIONTABLE](
             [CONTOSORETAILSEATNUMBER] [int] NOT NULL,
         	[CONTOSORETAILSERVERSTAFFID] [nvarchar](25) NOT NULL,
         	[TRANSACTIONID] [nvarchar](44) NOT NULL,
         	[STORE] [nvarchar](10) NOT NULL,
         	[CHANNEL] [bigint] NOT NULL,
         	[TERMINAL] [nvarchar](10) NOT NULL,
         	[DATAAREAID] [nvarchar](4) NOT NULL,
          CONSTRAINT [PK_EXT_CONTOSORETAILTRANSACTIONTABLE] PRIMARY KEY CLUSTERED 
         (
         	[TRANSACTIONID] ASC,
         	[STORE] ASC,
         	[CHANNEL] ASC,
         	[TERMINAL] ASC,
         	[DATAAREAID] ASC
         )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
         ) ON [PRIMARY]
         GO
         
         GRANT INSERT, DELETE, UPDATE, SELECT ON OBJECT::[ext].[CONTOSORETAILTRANSACTIONTABLE] TO [DataSyncUsersRole];
         GO
     ```

     5. RetailTransactionPaymentTrans:<br/>
       + AOT<br/>
       <img width="1030" alt="image" src="https://github.com/user-attachments/assets/d1b116d0-b4e2-4bc8-86ab-5b84abfa14e7">

       + Channel<br/>
         ![image](https://github.com/user-attachments/assets/1aac8f3d-8bc8-4dda-a0c2-f62320437c22)

       + Sql Script <br/>
       ```sql
        IF (SELECT OBJECT_ID('[ext].[CONTOSORETAILTRANSACTIONPAYMENTTRANS]')) IS NOT NULL 
         BEGIN
            DROP TABLE [EXT].CONTOSORETAILTRANSACTIONPAYMENTTRANS
         END
         GO
         
         CREATE TABLE [ext].[CONTOSORETAILTRANSACTIONPAYMENTTRANS](
         	[CHANNEL]             [bigint] NOT NULL,
             [STORE]               [nvarchar](10) NOT NULL,
         	[TERMINAL]            [nvarchar](10) NOT NULL,
         	[DATAAREAID]          [nvarchar](4) NOT NULL,
             [TRANSACTIONID]       [nvarchar](44) NOT NULL,	
             [LINENUM]             [numeric](32, 16) NOT NULL,
             [BANKTRANSFERCOMMENT] [nvarchar](100) NOT NULL DEFAULT('')
          CONSTRAINT [P_EXT_CONTOSORETAILTRANSACTIONPAYMENTTRANS] PRIMARY KEY CLUSTERED 
         (
         	[CHANNEL] ASC,
         	[STORE] ASC,
         	[TERMINAL] ASC,
         	[TRANSACTIONID] ASC,
         	[LINENUM] ASC,
         	[DATAAREAID] ASC
         )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
         ) ON [PRIMARY]
         GO
         
         GRANT INSERT, DELETE, UPDATE, SELECT ON OBJECT::[ext].[CONTOSORETAILTRANSACTIONPAYMENTTRANS] TO [DataSyncUsersRole];
         GO
         
         GRANT SELECT, INSERT, UPDATE, DELETE ON OBJECT::[ext].[CONTOSORETAILTRANSACTIONPAYMENTTRANS] TO [UsersRole]
         GO
         
         GRANT SELECT, INSERT, UPDATE, DELETE ON OBJECT::[ext].[CONTOSORETAILTRANSACTIONPAYMENTTRANS] TO [DeployExtensibilityRole]
         GO
       ```
     6. ContosoRetailStaffSuggestions <br/>
        + AOT<br/>
        <img width="1096" alt="image" src="https://github.com/user-attachments/assets/6fada673-cc06-42f0-885f-26de4f67fb76"><br/>

        + Channel<br/>
        ![image](https://github.com/user-attachments/assets/90974125-6255-4f32-9de5-168cdfe36cb6)<br/>

        + SQL Script<br/>
        ```sql
         IF (SELECT OBJECT_ID('[ext].[CONTOSORETAILSTAFFSUGGESTIONS]')) IS NOT NULL  
         BEGIN
             DROP TABLE [EXT].[CONTOSORETAILSTAFFSUGGESTIONS]
         END
         
         CREATE TABLE [ext].CONTOSORETAILSTAFFSUGGESTIONS(
         	[SUGGESTIONID] [int] NOT NULL,
         	[STOREID] [nvarchar](10) NOT NULL,
         	[STAFF] [nvarchar](25) NOT NULL,
         	[TERMINALID] [nvarchar](10) NOT NULL,
         	[SUGGETION] [nvarchar](255) NOT NULL,
         	[DATAAREAID] [nvarchar](4) NOT NULL,
         	[DATELOGGED] [date] NOT NULL,
         	[ROWVERSION] [timestamp] NOT NULL,
         	[REPLICATIONCOUNTERFROMORIGIN] [int] IDENTITY(1,1) NOT NULL,
         	 CONSTRAINT [PK_EXT_CONTOSORETAILSTAFFSUGGESTIONS] PRIMARY KEY CLUSTERED 
         (
         	[SUGGESTIONID] ASC,
         	[STOREID] ASC,
         	[TERMINALID] ASC,
         	[DATAAREAID] ASC
         )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
         ) ON [PRIMARY]
         GO
         
         GRANT INSERT, DELETE, UPDATE, SELECT ON OBJECT::[ext].[CONTOSORETAILSTAFFSUGGESTIONS] TO [DataSyncUsersRole];
         GO
        ```
  + Step 4: How to verify the custome CDX is working or not? <br/>
      + <mark>RetailTrasactionTable and RetailTransactionPaymentTrans=>Upload Sessions=>Exend Existing Table to add new fields</mark><br/>
         1. Create a new record and provide value to the custom field:
        ```sql
         select  T.TRANSACTIONID, * from ax.retailTransactionTable as T where T.RECEIPTID =  'STONN-42100236'
         
         insert into ext.CONTOSORETAILTRANSACTIONTABLE
         (TRANSACTIONID, STORE, CHANNEL, TERMINAL, DATAAREAID, CONTOSORETAILSEATNUMBER, CONTOSORETAILSERVERSTAFFID) 
         select TRANSACTIONID TRANSACTIONID, STORE, CHANNEL, TERMINAL, DATAAREAID, 10, '000160' from ax.retailTransactionTable as T
         where T.RECEIPTID =  'STONN-42100236'
         
         INSERT INTO [ext].[CONTOSORETAILTRANSACTIONPAYMENTTRANS] ([CHANNEL],[STORE],[TERMINAL],[DATAAREAID],[TRANSACTIONID],[LINENUM],[BANKTRANSFERCOMMENT])
         SELECT [CHANNEL],[STORE],[TERMINAL],[DATAAREAID],[TRANSACTIONID],[LINENUM],'BankTransfer'
         from ax.RETAILTRANSACTIONPAYMENTTRANS where TransactionId = 'HOUSTON-HOUSTON-42-1728640504337'
         
         select * from ext.CONTOSORETAILTRANSACTIONTABLE where TransactionId = 'HOUSTON-HOUSTON-42-1728640504337'
         select * from ext.ContosoRetailTransactionPaymentTrans where TransactionId = 'HOUSTON-HOUSTON-42-1728640504337'
        ```
        ![image](https://github.com/user-attachments/assets/1d65b76f-1a39-4cf8-82cd-0850700c4064)<br/>
        2. You can see after run P-Job,  the custom fields column values are uploaded to D365 F&O Head Quarter:<br/>
        <img width="1365" alt="image" src="https://github.com/user-attachments/assets/4b5bf620-0a43-4393-b6e8-ca2048ee71cc"><br/>
        <img width="1290" alt="image" src="https://github.com/user-attachments/assets/e1eadaa5-24dc-490b-ac45-42d665b87488"><br/>
     + For <mark>ContosoRetailSeatingData=>Download Sessions=>Totally New Table</mark><br/>
        1. On FO UI, create a new record<br/>
        ![image](https://github.com/user-attachments/assets/dbb663bb-9a12-441a-bafc-f2e71cb293b7)<br/>
        2. Run this distribution scheduler job<br/>
        ![image](https://github.com/user-attachments/assets/bdf0a360-e45e-4b2c-9782-21654cd1cbd7)<br/>
        3. Make sure download session applied<br/>
        ![image](https://github.com/user-attachments/assets/21b783ef-b838-4032-ab82-658e9d012ec9)<br/>
        4. Check channel database and confirm the data got pushed to channel database.<br/>
        ![image](https://github.com/user-attachments/assets/4fad267e-132a-490c-9b88-6a28a0c4d049)<br/>
     
     + <mark>RetailChannelTable=>Dowload Sessions=> Existing Table with new custom fields and existing fields</mark><br/>
        1. On FO Store form, set the value for the 3 new fields, one is custom field, the other two are existing field to push<br/>
        <img width="1175" alt="image" src="https://github.com/user-attachments/assets/e7bdbb28-aa07-40a8-a2f6-db774975febf"><br/>
        2. Run 1070 Job, then check Channel Database<br/>
        ![image](https://github.com/user-attachments/assets/7b0e1d29-0af3-4d57-9a8a-af8992014166)<br/>
        3. The SQL script to run<br/>
        ```sql
         select  T.Payment, T.PaymMode, T.ContosoRetailWallPostMessage, T.RECID,  T1.STORENUMBER from ext.ContosoRETAILCHANNELTABLE as T
          join ax.RETAILSTORETABLE as T1 on T.RECID = T1.RECID
          where T1.STORENUMBER = 'HOUSTON'
        ```
        <br/>

     + <mark>RetailCustTable=>Dowload Sessions=>New fields on existint table</mark><br/>
        1. On UI (Customer forms), set values to the custom fields<br/>
        <img width="1153" alt="image" src="https://github.com/user-attachments/assets/151b5b8f-8653-49fd-9fb7-2cdf2e6b9f89"><br/>
        2. Check the FO table browser to confirm the custom fields have values<br/>
        <img width="1192" alt="image" src="https://github.com/user-attachments/assets/a5c58287-7a51-413b-96bd-5a64e708ba93"><br/>
        3. Run download session and check the channel database<br/>
           ![image](https://github.com/user-attachments/assets/e4f793ec-ad7b-4586-8dba-3a6bf2d281c4)

     + <u><mark>ContosoRetailStaffSuggestions => Upload Sessions => Totally New Table</u></mark><br/>
        1. Insert records into channel database:<br/>
        ```
           USE [RetailChannelDatabase]
            GO
            
          INSERT INTO [ext].[CONTOSORETAILSTAFFSUGGESTIONS]([SUGGESTIONID], [STOREID], [STAFF], [TERMINALID], [SUGGETION], [DATAAREAID], [DATELOGGED])
               VALUES(1 ,'HOUSTON', '000160', 'HOUSTON-42', 'Good Service', 'USRT', GetDate())
         
          INSERT INTO [ext].[CONTOSORETAILSTAFFSUGGESTIONS]([SUGGESTIONID], [STOREID], [STAFF], [TERMINALID], [SUGGETION], [DATAAREAID], [DATELOGGED])
               VALUES(2 ,'HOUSTON', '000137', 'HOUSTON-42', 'Best Service', 'USRT', GetDate())
          GO
          SELECT * from [ext].[CONTOSORETAILSTAFFSUGGESTIONS]
         ```
        ![image](https://github.com/user-attachments/assets/eb50c78c-837a-4ef9-8f90-28b598e1f809)<br/>
        2. Check the Upload Sessions<br/>
        <img width="1369" alt="image" src="https://github.com/user-attachments/assets/c02e5209-79f8-4e80-b580-f7e94c42940e"><br/>
        3. Find the menu items<br/>
        <img width="186" alt="image" src="https://github.com/user-attachments/assets/95e81a6d-ec94-4b3d-be37-fe2f8bf34512"><br/>
        4. Open the form and verify the data got uploaded<br/>
        ![image](https://github.com/user-attachments/assets/341d1468-db2b-46bd-a987-dc13fc781ea9)<br/>


# Code link:
1. Channel Database Script:<br/>
https://github.com/zhangguanghuib/NewCommerceSDK/tree/main/POS_Samples/Solutions/CDXCustomization/ChannelDatabase<br/>
2. HQ all object:<br/>
https://github.com/zhangguanghuib/NewCommerceSDK/tree/main/KnowledgeArticles-Development/9.%20CDX%20Customization<br/>

# Key points of CDX customization:
1. If you want to push a totally new table from channel database to D365 FO HQ  database, please follow this rules:<br/>
   ![image](https://github.com/user-attachments/assets/8d8e1919-cee1-44e9-9ef8-7dfd746e289a)<br/>
   Otherwise, you will see this error:<br/>
   <img width="1018" alt="image" src="https://github.com/user-attachments/assets/21951051-c61e-44dc-9c79-b9c10b12cd1c"><br/>
2. For offline database transactions that can not be uploaded online channel database,  checking this API is a good point:<br/>
   ```
   union *
   | where customDimensions.TenantId == "<EnvironmentID>"
   | where customDimensions.ScaleUnitId == "<CSUID>"
   | where timestamp between(todatetime('2022-03-09T08:56:57.484Z')..todatetime('2022-03-09T08:56:58.14Z'))
   | where customDimensions.requestUri has "/PostOfflineTransactions?api-version=7.3"
   | order by timestamp asc
   | extend exception_ = tostring(customDimensions.exception)
   | project timestamp, exception_, customDimensions
   ```
3. Again, please keep in mind the below things:<br/>
   <img width="672" alt="image" src="https://github.com/user-attachments/assets/3ac06a86-97f0-4849-8c1e-05f49a54c10f"><br/>
4. Another two kind of frequent issues we faced about CDX customization are:
   + Partner extend one EDT in HQ to extend the string-size(like from 20 to 30), but in Channel Databse Table Column with length is still the original size(length = 20)
   + Customer HQ is on newer version but CSU kept on old version,  this situation normally happened in local CSU,  and it happend Microsoft extend the column size both in HQ  and Channel Database. 

    






















   


