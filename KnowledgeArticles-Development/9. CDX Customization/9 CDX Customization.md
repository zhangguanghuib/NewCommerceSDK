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
    # https://learn.microsoft.com/en-us/dynamics365/commerce/dev-itpro/cdx-extensibility
    # https://learn.microsoft.com/en-us/dynamics365/commerce/dev-itpro/channel-db-extensions#adding-a-new-table

