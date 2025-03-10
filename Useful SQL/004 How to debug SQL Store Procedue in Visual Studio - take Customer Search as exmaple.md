# How to debug SQL Server Store Procedure by Visual Studio 2022 : take "Customer Search on POS" as example
## Bakground,  debug SQL Script in SQL Server Management Studio is not availble 
   See in SSMS, there are only <mark>"Execute"</mark> button but no <mark>"Debug"</mark> button:<br/>
   ![image](https://github.com/user-attachments/assets/a7d35d5a-057e-4b9b-843f-f02a476b3b9e)

## Steps to configure to debug SQL Server Store Procedure by Visual Studio 2022
   1. Open Visual Studio 2022 "Continue Without Code"<br/>
       ![image](https://github.com/user-attachments/assets/db474031-cd7f-42e5-bd56-b9659e6422b0)<br/>
    2. Tools->Connect to Database...<br/>
       ![image](https://github.com/user-attachments/assets/e2bcc613-38dd-46f0-bfb9-4db88a2101e6)<br/>
       ![image](https://github.com/user-attachments/assets/fa7d2c63-5fc1-4161-bf10-6b225c5e481e)<br/>
     3. Tools->SQL Server->New QUery<br/>
        ![image](https://github.com/user-attachments/assets/e98b4c56-e1c1-405e-8855-d3f6ffcfc8ec)<br/>
        ![image](https://github.com/user-attachments/assets/23013a64-4325-4074-bb66-03add99cceb7)<br/>
        Click "Connect"<br/>
      4.You can write a simple SQL query to verify if it works<br/>
         ![image](https://github.com/user-attachments/assets/3de1abac-3615-41af-a097-4fe18d9835db)<br/>
      5. How to debug a SQL Store Procedure in Visual Studio?<br/>
      Prepare a SQL for debugging<br/>
 ```sql
declare @p1 crt.CUSTOMERSEARCHBYFIELDCRITERIATABLETYPE
insert into @p1 values(N'Default',N'"Contoso*"',0)

declare @p6 crt.QUERYRESULTSETTINGSTABLETYPE
insert into @p6 values(0,81,0,N'RANKING',0)

exec [crt].GETCUSTOMERSEARCHRESULTSBYFIELDS @tvp_CustomerSearchByFieldCriteria=@p1,@bi_ChannelId=5637144592,@nvc_DataAreaId=N'usrt',@i_MaxTop=2147483647,@i_MinCharsForWildcardEmailSearch=7,@TVP_QUERYRESULTSETTINGS=@p6          

```

 Add break point, and choose "Execute with debugger"<br/>
 ![image](https://github.com/user-attachments/assets/9185934f-41d7-445a-83eb-7c6d07d36dac)<br/>

 6. Then you can click F5/F10/F11 to debug the SQL as you debug C#/C++ in Visual Studio?<br/>
    ![image](https://github.com/user-attachments/assets/ad22fc84-3b1a-4d84-a7cd-b000e9f81756)<br/>
     Press F11<br/>
     ![image](https://github.com/user-attachments/assets/cb777199-bd2e-4021-a365-697e9883c222)<br/>
    Press F11<br/>
    ![image](https://github.com/user-attachments/assets/3e0dd6bf-47fb-4bc0-9ec0-0e79a7a8257e)<br/>

7. Some useful SQL to POS customer search(SQL-Based)<br/>
01-SQL 1:<br/>
```sql
DECLARE @tvp_CustomerSearchResults1 [crt].[CUSTOMERSEARCHRESULTTABLETYPE];
DECLARE @tvp_CustomerSearchResults2 [crt].[CUSTOMERSEARCHRESULTTABLETYPE];

DECLARE @b_FilterResults BIT;
SET @b_FilterResults = 0;

DECLARE @searchTerm NVARCHAR(255) = 'Contoso*';
DECLARE @bi_ChannelId BIGINT = 5637144592;
DECLARE @i_MaxTop INT;
DECLARE @exactMatch BIT = 0;

DECLARE @tvp_CustomerSearchByFieldCriteria crt.CUSTOMERSEARCHBYFIELDCRITERIATABLETYPE;
INSERT INTO @tvp_CustomerSearchByFieldCriteria VALUES (N'Name', N'"Whitehead*"', 0);

DECLARE @tvp_QueryResultSettings crt.QUERYRESULTSETTINGSTABLETYPE;
INSERT INTO @tvp_QueryResultSettings VALUES (0, 81, 0, N'', 1);

IF (1 = (SELECT COUNT(*) FROM @tvp_CustomerSearchByFieldCriteria))
BEGIN
    SET @i_MaxTop = (SELECT TOP 1 [SKIP] FROM @tvp_QueryResultSettings) + 
                    (SELECT TOP 1 [TOP] FROM @tvp_QueryResultSettings); -- Top + Skip
END

INSERT INTO @tvp_CustomerSearchResults2
EXEC [crt].GETCUSTOMERSEARCHRESULTSBYNAMECONTAINSSEARCH @searchTerm, @bi_ChannelId, @tvp_CustomerSearchResults1, @b_FilterResults, @i_MaxTop;

IF @exactMatch = 0
BEGIN
    INSERT INTO @tvp_CustomerSearchResults2
    EXEC [crt].GETCUSTOMERSEARCHRESULTSBYNAMEFREETEXTSEARCH @searchTerm, @bi_ChannelId, @tvp_CustomerSearchResults1, @b_FilterResults, @i_MaxTop;
END

SELECT PARTYID, SUM(RANKING) AS RANKING 
FROM (
    SELECT
        customerSearchResults.PARTYID,
        customerSearchResults.RANKING
    FROM @tvp_CustomerSearchResults2 customerSearchResults
) pagedResults
GROUP BY pagedResults.PARTYID
ORDER BY RANKING DESC
OFFSET (SELECT TOP 1 [SKIP] FROM @tvp_QueryResultSettings) ROWS
FETCH NEXT (SELECT TOP 1 [TOP] FROM @tvp_QueryResultSettings) ROWS ONLY;
```

02-SQL
```sql
DECLARE @tvp_CustomerSearchResults1 [crt].[CUSTOMERSEARCHRESULTTABLETYPE];
DECLARE @tvp_CustomerSearchResults2 [crt].[CUSTOMERSEARCHRESULTTABLETYPE];

DECLARE @b_FilterResults BIT;
SET @b_FilterResults = 0;

DECLARE @searchTerm NVARCHAR(255) = 'Contoso*';
DECLARE @bi_ChannelId BIGINT = 5637144592;
DECLARE @i_MaxTop INT= 80;
DECLARE @exactMatch BIT = 0;

DECLARE @tvp_CustomerSearchByFieldCriteria crt.CUSTOMERSEARCHBYFIELDCRITERIATABLETYPE;
INSERT INTO @tvp_CustomerSearchByFieldCriteria VALUES (N'Name', N'"Whitehead*"', 0);

DECLARE @tvp_QueryResultSettings crt.QUERYRESULTSETTINGSTABLETYPE;
INSERT INTO @tvp_QueryResultSettings VALUES (0, 81, 0, N'', 1);

--IF (1 = (SELECT COUNT(*) FROM @tvp_CustomerSearchByFieldCriteria))
--BEGIN
--    SET @i_MaxTop = (SELECT TOP 1 [SKIP] FROM @tvp_QueryResultSettings) + 
--                    (SELECT TOP 1 [TOP] FROM @tvp_QueryResultSettings); -- Top + Skip
--END

INSERT INTO @tvp_CustomerSearchResults2
SELECT TOP (@i_MaxTop)
            [dpt].RECID AS PARTYID,
            results.[RANKING]
        FROM
        (
            -- search by customer name with CONTAINS to match partial names
            SELECT
                [CustomerNameFullTextKey_Key].[KEY] AS [KEY],
                COALESCE([CustomerNameFullTextKey_Key].[RANK], 0) AS RANKING
            FROM CONTAINSTABLE([ax].DIRPARTYTABLE, [NAME], @searchTerm, @i_MaxTop) CustomerNameFullTextKey_Key

 UNION ALL

            -- search by customer search name/alias with CONTAINS to match partial names
            SELECT
                [CustomerNameFullTextKey_Key].[KEY] AS [KEY],
                COALESCE([CustomerNameFullTextKey_Key].[RANK], 0) AS RANKING
            FROM CONTAINSTABLE([ax].DIRPARTYTABLE, [NAMEALIAS], @searchTerm, @i_MaxTop) CustomerNameFullTextKey_Key
        ) results
        INNER JOIN [ax].DIRADDRESSBOOKPARTY dabp ON [dabp].PARTY = results.[KEY]
        INNER JOIN [ax].DIRPARTYTABLE dpt ON [dpt].RECID = results.[KEY]
        INNER JOIN [ax].RETAILSTOREADDRESSBOOK rsab ON [dabp].ADDRESSBOOK = [rsab].ADDRESSBOOK AND [rsab].STORERECID = @bi_ChannelId AND [rsab].ADDRESSBOOKTYPE = 0  -- The customer address book type
        LEFT JOIN [ax].OMINTERNALORGANIZATION oio ON oio.RECID = results.[KEY]
        WHERE oio.RECID IS NULL

        UNION ALL

        -- search async customers by customer name
        SELECT
           [CustomerNameFullTextKey_Key].[KEY] AS PARTYID, COALESCE([CustomerNameFullTextKey_Key].[RANK], 0) AS RANKING
        FROM CONTAINSTABLE([ax].RETAILASYNCCUSTOMER, [CUSTNAME], @searchTerm, @i_MaxTop) CustomerNameFullTextKey_Key
        INNER JOIN [ax].RETAILASYNCCUSTOMER rac ON [rac].REPLICATIONCOUNTERFROMORIGIN = [CustomerNameFullTextKey_Key].[KEY]
        AND NOT EXISTS (SELECT 1 FROM [ax].RETAILCUSTTABLE rct WHERE rct.CUSTACCOUNTASYNC = rac.CUSTACCOUNTASYNC) AND rac.STORERECID = @bi_ChannelId

IF @exactMatch = 0
BEGIN
    INSERT INTO @tvp_CustomerSearchResults2
    SELECT Result.PARTYID, Result.RANKING from
       (SELECT
            [dpt].RECID AS PARTYID, COALESCE([CustomerNameFullTextKey_Key].[RANK], 0) AS RANKING
        FROM FREETEXTTABLE([ax].DIRPARTYTABLE, [NAME], @searchTerm, @i_MaxTop) CustomerNameFullTextKey_Key
        INNER JOIN [ax].DIRPARTYTABLE dpt ON [dpt].RECID = [CustomerNameFullTextKey_Key].[KEY]
        INNER JOIN (SELECT PARTY, ADDRESSBOOK FROM [ax].DIRADDRESSBOOKPARTY WHERE PARTY NOT IN (SELECT RECID FROM [ax].OMINTERNALORGANIZATION)) dap on [dpt].RECID = [dap].PARTY
        INNER JOIN [ax].RETAILSTOREADDRESSBOOK rsab on [dap].ADDRESSBOOK = [rsab].ADDRESSBOOK AND [rsab].STORERECID = @bi_ChannelId AND [rsab].ADDRESSBOOKTYPE = 0  -- The customer address book type

        UNION ALL

        -- search by customer search name/alias with FREETEXT to match inflections of names
        SELECT
            [dpt].RECID AS PARTYID, COALESCE([CustomerSearchNameFullTextKey_Key].[RANK], 0) AS RANKING
        FROM FREETEXTTABLE([ax].DIRPARTYTABLE, [NAMEALIAS], @searchTerm, @i_MaxTop) CustomerSearchNameFullTextKey_Key
        INNER JOIN [ax].DIRPARTYTABLE dpt ON [dpt].RECID = [CustomerSearchNameFullTextKey_Key].[KEY]
        INNER JOIN (SELECT PARTY, ADDRESSBOOK FROM [ax].DIRADDRESSBOOKPARTY WHERE PARTY NOT IN (SELECT RECID FROM [ax].OMINTERNALORGANIZATION)) dap on [dpt].RECID = [dap].PARTY
        INNER JOIN [ax].RETAILSTOREADDRESSBOOK rsab on [dap].ADDRESSBOOK = [rsab].ADDRESSBOOK AND [rsab].STORERECID = @bi_ChannelId AND [rsab].ADDRESSBOOKTYPE = 0  -- The customer address book type

        UNION ALL

        -- search async customers by customer name with FREETEXT to match inflections of names
        SELECT
           [CustomerNameFullTextKey_Key].[KEY] AS PARTYID, COALESCE([CustomerNameFullTextKey_Key].[RANK], 0) AS RANKING
        FROM FREETEXTTABLE([ax].RETAILASYNCCUSTOMER, [CUSTNAME], @searchTerm, @i_MaxTop) CustomerNameFullTextKey_Key
        INNER JOIN [ax].RETAILASYNCCUSTOMER rac ON [rac].REPLICATIONCOUNTERFROMORIGIN = [CustomerNameFullTextKey_Key].[KEY]
        AND NOT EXISTS (SELECT 1 FROM [ax].RETAILCUSTTABLE rct WHERE rct.CUSTACCOUNTASYNC = rac.CUSTACCOUNTASYNC) AND rac.STORERECID = @bi_ChannelId) as Result
END

SELECT PARTYID, SUM(RANKING) AS RANKING 
FROM (
    SELECT
        customerSearchResults.PARTYID,
        customerSearchResults.RANKING
    FROM @tvp_CustomerSearchResults2 customerSearchResults
) pagedResults
GROUP BY pagedResults.PARTYID
ORDER BY RANKING DESC
OFFSET (SELECT TOP 1 [SKIP] FROM @tvp_QueryResultSettings) ROWS
FETCH NEXT (SELECT TOP 1 [TOP] FROM @tvp_QueryResultSettings) ROWS ONLY;
```
03-SQL <br/>
```sql
DECLARE @i_MaxTop INT = 1000
DECLARE @nvc_SearchTerm VARCHAR(20) = '"Contoso"'
 
SELECT *
FROM
(
    -- search by customer name with CONTAINS to match partial names
    SELECT
       [CustomerNameFullTextKey_Key].[KEY] AS [KEY],
        COALESCE([CustomerNameFullTextKey_Key].[RANK], 0) AS RANKING
    FROM CONTAINSTABLE([ax].DIRPARTYTABLE, [NAME], @nvc_SearchTerm, @i_MaxTop) CustomerNameFullTextKey_Key
    UNION ALL
     --search by customer search name/alias with CONTAINS to match partial names
    SELECT
        [CustomerNameFullTextKey_Key].[KEY] AS [KEY],
      COALESCE([CustomerNameFullTextKey_Key].[RANK], 0) AS RANKING
    FROM CONTAINSTABLE([ax].DIRPARTYTABLE, [NAMEALIAS], @nvc_SearchTerm, @i_MaxTop) CustomerNameFullTextKey_Key
) results
INNER JOIN [ax].DIRADDRESSBOOKPARTY dabp ON [dabp].PARTY = results.[KEY]
INNER JOIN [ax].DIRPARTYTABLE dpt ON [dpt].RECID = results.[KEY]
```
