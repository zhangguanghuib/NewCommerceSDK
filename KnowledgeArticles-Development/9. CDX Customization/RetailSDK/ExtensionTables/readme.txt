HOW TO CUSTOMIZE EXISTING RETAIL TRANSACTIONAL TABLES.

SAMPLE OVERVIEW:
This sample description details the main steps and best practices for customizing retail transactional tables by using extension tables and also show how to customize CDX so as to upload the customized (extension) channel side tables back to AX. At the end there is also a section that describes how to test the customization.

SETUP STEPS:
It is advised that you do these changes on a untouched Retail SDK. Ideally, you would have it under source control (VSO, or similar) and no files are changed so far. This is ideal, as you could revert at any steps without much work. 

It is best if you first import the .axpp package located in the SDK and also run the SQL update script on your channel database before going through the "how to" steps and descriptions below.
To import the AX side package that contains the customization code
   1) copy the .axpp package from the SDK folder 
   2) open VS > click on Dynamics 365>  click on "Import project"
   3) in the import project form specify the .axpp file path.
   4) select 'current solution' or 'new solution' based on your preference.
   5) click OK to start importing the package.
   6) once the import is done you will have the ax files in your solution explorer.
   7) build the solution. 
   8) right click on the project and click on Synchronize database.
To run the SQL update script	
   1) copy the ContosoRetailExtensionTablesUpdate.sql. from the Retail SDK folder.
   2) open the script in SQL browser and run it against your channel database.
   3) This will create the extension tables and views required to customize the transactional tables.  Note that the script also create other tables which are used for other sample      scenarios.
   
   
SAMPLE HOW TO STEPS AND EXPLANATIONS

1. Extending the AX data:
The AX side table extension is already created in the sample. But if you were to create that from scratch this is how it would be done.

   - Launch Visual Studio.
   - Go to View > Application Explorer.
   - Select Data Model > Tables > RetailTransactionTable, right-click on it, select "Create extension".
   - As a BEST PRACTICE it's good to change the default name to something like RetailTransactionTable.ContosoRetailExtension. Basically add your unique prefix. In this sample 'ContosoRetail' is used as a unique prefix. This is helpful to avoid naming conflicts in the event that the table is extended by multiple ISVs.
   - At the new table "RetailTransactionTable.ContosoRetailExtension", create two new fields:
       - Type=string, name=ContosoRetailServerStaffId - set the Extended data type property to RetailStaffId.
       - Type=int, name=ContosoRetailSeatNumber - Extended data type property is set to ContosoRetailSeatNumber.  
	   *Note that the unique prefix is added to the new column names as a BEST PRACTICE to avoid future naming conflicts.
	      -The naming conflict can occur if another ISV creates a column with the same name or if Microsoft ships an update which uses a column with the same name.
       *Note that even though the extension table is created in a different AOT asset, in SQL the new columns are actually added to the original table.
	   *Note that in X++ to access the extension column the original table name is used. i.e. RetailTransaction.ContosoRetailExtensions is not used in code.
	   
   - Save the changes and build your project.
   - Right click on your project and click on synchronize the database.
 
2. Extending the channel side database:
   - From the Retail SDK folder of this same readme.txt file, open and run at SQL Server file 'ContosoRetailExtensionTablesUpdate.sql' 
     For this section of the sample we are interested in the SQL code under the -----RETAILTRANSACTIONTABLE CUSTOMIZATION SECTION ---------
     This will create:
       1 - Table [ext].ContosoRetailTransactionTable with the FK and custom (extension) fields. 
		   1.1 (Note that the prefix is used in naming the extension table to void naming conflict with different ISVs.
		   1.2 Note that in addition to the two extension column we added in the AX side the channel side extension table need to have the same PK columns as the original channel side table.
		       That's why [ext].RetailTransactionTable_ContosoRetailExtension has the four PK columns used in [ax].RetialTransactionTable.
			   As a BEST PRACTICE when u add the PK columns to the channel side extension table keep the name of the columns the same as the PK column names on original.  This will help u uptake the CDX enhancement Microsoft will ship out in the future.		
   
3. Configure CDX to upload/pull the custom columns from the channel extension table back to AX:
   - The resource RetailCDXSeedDataAX7 contains the AX to channel table mapping information that is used by CDX to create the necessary data transfer scheduler jobs and sub jobs.
   - To include your new extension tables or columns in the data transfer you have to provide a resource that specifies the CDX data transfer customization.  
   * As a BEST PRACTICE - use this naming convention to avoid conflicts. RetailCDXSeedDataAX7_ContosoRetailExtension. 'ContosoRetial' your unique extension.
   
   The sample CDX resource file contain additional customizations but for our RetailTransactionTable extension example this is the only section that would be required to pull data from the channel side back to AX.
				<RetailCdxSeedData Name="AX7" ChannelDBSchema="ext" ChannelDBMajorVersion="7"> 
				  <Subjobs>
					<!--
						Adding additional columns to (existing) RetailTransactionTable and wants to pull it back to AX.
						For upload subjobs, set the OverrideTarget property to  "false", as ilustrate below. This will tell CDX to use the table defined by TargetTableName and TargetTableSchema as extension table on this subjob.
					-->
					<Subjob Id="RetailTransactionTable" TargetTableName ="CONTOSORETAILTRANSACTIONTABLE" TargetTableSchema="ext" OverrideTarget="false">
					  <!--Notice that there is no mention of the <ScheduledByJobs></ScheduledByJobs> because the subjob is already part of an upload job. -->
					  <AxFields>
						<!--If you notice the existing columns are not listed here in the <Field> tag, this is because the existing fields are already mapped in the main RetailCdxSeedData resource file we only add the delta here. -->
						<Field Name="SeatNumber" />
						<Field Name="ServerStaffId" />
					  </AxFields>
					</Subjob>
				  </Subjobs>
				 </RetailCdxSeedData>
   
   - Notice the following fields at this resource (which contains the CDX seed data extension):
       - ChannelDBSchema='ext' so it reads from the extension schema at channel DB.
       - <Subjob Id="RetailTransactionTable" TargetTableName ="CONTOSORETAILTRANSACTIONTABLE" TargetTableSchema="ext" OverrideTarget="false">
         - Note that we are using the same subjob Id that is shipped by Microsoft to pull data from the channel side RetailTransactionTable back to AX.
		 - It's important to make sure the Id is the same so that the extensibility framework knows that you are customizing that subjob.
		 - For upload subjobs (the ones that bring data from the channel to the headquarts), OverrideTarget when set to "false" will tell CDX that the table defined by TargetTableName is an extension table, this is, its data will be uploaded along with the primary table already defined in the subjob.
			If OverrideTarget is set to "true" (default value, if omitted), the table defined by TargetTableName will override the primary table for the subjob.
			For instance, in this sample, if you were to set this value to true, this would mean that instead of uploading the data from ax.RetailTransactionTable, CDX would only upload the data from ext.CONTOSORETAILTRANSACTIONTABLE.
		 - The "TargetTableName" attribute is set to the name of the extension table on the channel side. This table must have the same primary key as the extended table.
		 - The AxTableName attribute is not specified as the framework already knows the AxTableName that the specified subjob uses as a sink. This is tipically not changed.
		   A pattern that will be evident to you soon is that you only need to specify the deltas when customizing the RetailCDXSeedDataAX7 resource. Any data that the framework can infer is not required to be added by partners.
		 - If you apply the above pattern to the <AXFields></AXFields> section you can see that we only specified the custom/new fields belonging to the table defined by TargetTableName. The reason for this is that the extensibility framework can figure out the remaining list of fields from the specified subjob Id. 
  

4. Updating the CDX module with the CDX customization resource.
   To apply the customization specified in RetailCDXSeedDataAX7_ContosoRetailExtension we need to subscribe to the registerCDXSeedDataExtension delegate.
   Subscribing to this event guarantee that the customization is applied when the CDX seed data initialization is run.
   How to subscribe to the registerCDXSeedDataExtension delegate.
   1) Go to View > Application Explorer.
   2) Search for the class RetailCDXSeedDataBase
   3) Right click on the class and click open in designer.
   4) Select the registerCDXSeedDataExtension delegate from the list of delegates and methods shown on the designer.
   5) right-click and click on copy event handler.  This will copy the method signature you need to implement so that CDX picks up the customized CDX seed data resource.
   6) Create a new class, and name the class ContosoRetailCDXSeedDataAX7EventHandler .. any name will do but as a BEST PRACTICE don't forget to prefix the class name with your prefix.
   7) Paste the code you copied in step 5. 
				class ContosoRetailCDXSeedDataAX7EventHandler
				{
					/// <summary>
					/// Registers the extension CDX seed data resource to be used during CDX seed data generation..
					/// </summary>
					/// <param name="result">The result object which is used to return the resource name.</param>
					[SubscribesTo(classStr(RetailCDXSeedDataBase), delegateStr(RetailCDXSeedDataBase, registerCDXSeedDataExtension))]
					public static void RetailCDXSeedDataBase_registerCDXSeedDataExtension(str originalCDXSeedDataResource, List resources)
					{
					}
				}
   8) This method will be called by the CDX extensibility framework when you run the CDX initialization.
      To ensure the CDX customization is used by the CDX extensibility module put the below code in the above method
	          if (originalCDXSeedDataResource == resourceStr(RetailCDXSeedDataAX7))
			  {
				  resources.addEnd(resourceStr(RetailCDXSeedDataAX7_ContosoRetailExtension));
			  }
	    It's important to check the originalCDXSeedDataResource being processed is RetailCDXSeedDataAX7 before adding your custom resource to the list. Not doing so may result in unintended consequence that we won't delve into at this point.
		
   9) To (re)initialize CDX module with the customized configuration execute the following steps.
      9.1) Go to Retail> Headquarters setup> Retail scheduler> Scheduler jobs> 
	  9.2) click on Initialize retail scheduler
	  9.3) on the dialog that opens check the Delete existing configuration.
	  9.4) Click ok to start the initialization.
	when the initialization is completed the CDX scheduler jobs, subjob definitions and distribution schedules will be updated using the original RetailCDXSeedDataAX7 and  the customized RetailCDXSeedDataAX7_ContosoRetailExtension resources.
	
	Testing that the customization works properly.
	1) To see that your customization is working properly.
		1.1) After the initialization is completed go to Retail> Headquarters setup> Retail scheduler> and click on "Scheduler subjobs" link
		1.2) On the subjobs table search for "RetailTransactionTable" subjob id.
		1.3) On the detail section go to the "Channel field mapping" section and verify that the new custom (extension) columns are listed in the mapping.
		1.4) CDX will keep your extension table metadata on table RETAILCONNLOCATIONDESIGNTABLE with a reference to the table it extends.
	
	2) To test that the CDX job will upload/pull from the channel side original and extension table.
	   2.1 Create a couple of transactions in MPOS.
	   2.2 Since the extension table is not used in the CRT/MPOS we have to manually insert data to the extension table.
	       Run the following script after changing the necessary values.
				INSERT INTO [ext].[CONTOSORETAILTRANSACTIONTABLE] (
				 [CONTOSORETAILSEATNUMBER], 
				 [CONTOSORETAILSERVERSTAFFID], 
				 [TRANSACTIONID], 
				 [STORE], 
				 [CHANNEL], 
				 [TERMINAL], 
				 [DATAAREAID])
				VALUES (
				 1, /*normally this needs to be an existing seat number from ContosoRetailSeatingData table, but for this test add any number*/
				 '000160' /*add any staff ID here*/, 
				 'HOUSTON-HOUSTON-11-101',/*add the transaction id you just created */
				 'HOUSTON', /*add the store used to create the transaction */
				 5637144592, /*add the channel RecId of the store used to create the transaction*/
				 'HOUSTON-11',  /*add the terminalId used to create the transaction*/
				 'USRT' /*add the dataareaId used by the store*/)
			    GO

			* Repeat the same for the other transactions.
			* Do not add a corresponding data in the [ext].[CONTOSORETAILTRANSACTIONTABLE] for some of the transactions you created in POS. *This is required to check that the data from [ax].RetailTransactionTable is pulled/uploaded even if there is no corresponding data in the extension table.

		2.3 Go to AX > Retail> Retail IT > click on Distribution schedule.
		2.4 From the list of distribution schedule select P-0001 which contains the RetialTransactionTable subjob we customized.
		2.5 Click on "Run" from the top Action menu pane.  Click Yes when the confirmation dialog pops up.
		2.6 Click on "History" from the Action menu pane. (This is where we check if the uploaded session is completed successfully or not.
		2.7 on the history page - Check if there is a new upload session record and that its status is set to Applied and Rows Affected is not zero.
		
		If the upload session is Applied successfully 
		-Go to Retail> Inquiries and reports> Retail store transactions> and search for the new transaction that were just uploaded.
		-Verify the transactions exist.
		-Verify the seat number and the server staff Id CUSTOM columns have the expected value.
		-Verify the transactions which do not have a corresponding record in the channel side extension table [ext].ContosoRetailTransactionTable are also uploaded 
		  Also for these transactions Verify the seat number and the server staff ID is set to a default value . this means seat number = 0 and serer staff id = ''
		  
		

	
   
   
