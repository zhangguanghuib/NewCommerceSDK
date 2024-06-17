## Retail Store Statement Number is not generated correctly?

1. The cursor version to check with store missing number sequence:<br/>
```sql
DECLARE @numberSequenceDatatype BIGINT = 5637144589;
--select RECID, * from numberSequenceDatatype 
--where numberSequenceDatatype.ScopeType = 4
--order by Module
--print @numberSequenceDatatype;

DECLARE @RECID BIGINT,
        @NAME NVARCHAR(255),
        @NAMEALIAS NVARCHAR(255);

DECLARE dirparty_cursor CURSOR FOR
SELECT T1.RECID,
       T1.NAME,
       T1.NAMEALIAS
FROM DIRPARTYTABLE T1
WHERE T1.PARTITION = 5637144576
  AND T1.INSTANCERELATIONTYPE IN (SELECT ID 
                                  FROM dbo.TABLEIDTABLE 
                                  WHERE NAME = 'OMOperatingUnit')
ORDER BY T1.NAME;

OPEN dirparty_cursor;

FETCH NEXT FROM dirparty_cursor INTO @RECID, @NAME, @NAMEALIAS;

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT 'RECID: ' + CAST(@RECID AS NVARCHAR(20)) + ', NAME: ' + @NAME + ', NAMEALIAS: ' + @NAMEALIAS;

	DECLARE @ScopeRECID BIGINT;

	select @ScopeRECID = scope.RECID from NumberSequenceScope scope
        where
            scope.DataArea = '' and
            scope.LegalEntity = 0 and
            scope.OperatingUnit = @RECID and
            scope.FiscalCalendarPeriod = 0 and
            scope.OperatingUnitType = 0;
	IF (@ScopeRECID != 0)
	BEGIN
	   PRINT '@ScopeRECID is : ' + CAST(@ScopeRECID AS NVARCHAR(20)) +' for operating unit ' + @NAME;
	END
	ELSE
	BEGIN
	    PRINT '@ScopeRECID: for ' + CAST(@RECID AS NVARCHAR(20)) +' is empty'+' for operating unit ' + @NAME;
	END

	DECLARE @NumberSeqRefRecId BIGINT;
	DECLARE @NumberSeqId BIGINT;
	DECLARE @NumberSeqName nvarchar(30);


	IF EXISTS (
    SELECT 1
    FROM NUMBERSEQUENCEREFERENCE T1 
    WHERE PARTITION = 5637144576
      AND NUMBERSEQUENCEDATATYPE = 5637144589
      AND NUMBERSEQUENCESCOPE = @ScopeRECID
      AND NUMBERSEQUENCEID <> 0
      AND T1.RECID <> 0
      AND T1.NUMBERSEQUENCEID <> 0
	)
	BEGIN
	        SELECT @NumberSeqName = T2.NUMBERSEQUENCE
			FROM NUMBERSEQUENCEREFERENCE T1 
			join NUMBERSEQUENCETABLE as T2 on T1.NUMBERSEQUENCEID = T2.RECID
			WHERE T1.PARTITION = 5637144576
			  AND T1.NUMBERSEQUENCEDATATYPE = 5637144589
			  AND T1.NUMBERSEQUENCESCOPE = @ScopeRECID
			  AND T1.NUMBERSEQUENCEID <> 0
			  AND T1.RECID <> 0
			  AND T1.NUMBERSEQUENCEID <> 0
		  PRINT 'Operating Unit ' + @NAME +' already a number sequence, it is ' + @NumberSeqName;
	END
	ELSE
	BEGIN
		 PRINT 'Operating Unit ' + @NAME +' need create a number sequence'
	END

    Print '------------------------------------------------------------------------------------------------------------'
    FETCH NEXT FROM dirparty_cursor INTO @RECID, @NAME, @NAMEALIAS;
END

CLOSE dirparty_cursor;

DEALLOCATE dirparty_cursor;

```
2. Find all Operating Unit and check its number sequence<br/>
```sql
select * from dbo.TABLEIDTABLE as T where T.NAME ='OMOperatingUnit'


SELECT T1.RECID,
	T1.INSTANCERELATIONTYPE,  T1.NAME, T1.NAMEALIAS, *
FROM DIRPARTYTABLE T1 
WHERE ((T1.PARTITION=5637144576) 
	AND (T1.INSTANCERELATIONTYPE IN (select ID from dbo.TABLEIDTABLE as T where T.NAME ='OMOperatingUnit') ))
	AND RECID= 22565427447
	--and T1.NAME ='Boston'
	order by T1.NAME


	select scope.RECID from NumberSequenceScope scope
        where
            scope.DataArea = '' and
            scope.LegalEntity = 0 and
            scope.OperatingUnit = 22565427447 and
            scope.FiscalCalendarPeriod = 0 and
            scope.OperatingUnitType = 0;

SELECT T1.RECID, T1.NUMBERSEQUENCEID,
    T1.ALLOWSAMEAS,
	T1.NUMBERSEQUENCEDATATYPE,
	T1.NUMBERSEQUENCEID,
	T1.NUMBERSEQUENCESCOPE,
	T1.RECVERSION,
	T1.PARTITION,
	T1.RECID 
FROM NUMBERSEQUENCEREFERENCE T1 
WHERE PARTITION=5637144576
	AND NUMBERSEQUENCEDATATYPE=5637144589
	AND NUMBERSEQUENCESCOPE=22565423776 
	AND NUMBERSEQUENCEID<>0
	AND T1.RECID <> 0
    AND T1.NUMBERSEQUENCEID <> 0


	 SELECT 1
    FROM NUMBERSEQUENCEREFERENCE T1 
    WHERE PARTITION = 5637144576
      AND NUMBERSEQUENCEDATATYPE = 5637144728
      AND NUMBERSEQUENCESCOPE = 22565422006
      AND NUMBERSEQUENCEID <> 0
      --AND T1.RECID <> 0
      --AND T1.NUMBERSEQUENCEID <> 0
```
<img width="813" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/f3493154-2fff-4e1c-974f-b7f7f4fc5ecb">

3.  SQL Script from Number Sequence to find the operating unit and Reference<br/>
```sql

-- From Number Sequence to check its operating unit and confirm if it is retail Store

select T.RECID, * from dbo.NUMBERSEQUENCETABLE as T where T.NUMBERSEQUENCE = 'Reta_1595'

select T.NUMBERSEQUENCEDATATYPE, T.NUMBERSEQUENCESCOPE,* from dbo.NUMBERSEQUENCEREFERENCE as T where T.NUMBERSEQUENCEID = (select T.RECID from dbo.NUMBERSEQUENCETABLE as T where T.NUMBERSEQUENCE = 'Reta_1595')

select  T.DATATYPEID, * from dbo.NUMBERSEQUENCEDATATYPE as T where T.RECID = 5637144589 --NUMBERSEQUENCEREFERENCE.NUMBERSEQUENCEDATATYPE

select  T.OPERATINGUNIT, T.OPERATINGUNITTYPE, * from dbo.NUMBERSEQUENCESCOPE as T where T.RECID = 22565429094 --NUMBERSEQUENCEREFERENCE.NUMBERSEQUENCESCOPE

select  T.OMOPERATINGUNITNUMBER, T.RECID, * from dbo.DIRPARTYTABLE as T where T.RECID = 22565450331 -- NUMBERSEQUENCESCOPE.OPERATINGUNIT

select  T.OMOPERATINGUNITID, T.STORENUMBER, * from dbo.RETAILCHANNELTABLE as T where T.OMOPERATINGUNITID = 22565450331

select T.Id, T.Name, * from dbo.SYSTYPEIDVIEW as T where T.Id = 6722 -- NUMBERSEQUENCEDATATYPE.DATATYPEID
```

4.  From retail Store to find its related number sequence:
```sql
-- From Retail Store to find its statement number sequence

select  T.OMOPERATINGUNITID, T.STORENUMBER, * from dbo.RETAILCHANNELTABLE as T where T.STORENUMBER = 'HOUSTON'

select  T.OMOPERATINGUNITNUMBER, T.RECID, T.INSTANCERELATIONTYPE, * from dbo.DIRPARTYTABLE as T where T.RECID = 22565427458 -- RETAILCHANNELTABLE.OMOPERATINGUNITID
select * from dbo.TABLEIDTABLE as T where T.ID = 8363 --DIRPARTYTABLE.INSTANCERELATIONTYPE

select  T.OPERATINGUNIT, T.OPERATINGUNITTYPE, RECID, * from dbo.NUMBERSEQUENCESCOPE as T where T.OPERATINGUNIT = 22565427458 --DIRPARTYTABLE.RecId

select T.NUMBERSEQUENCEDATATYPE, T.NUMBERSEQUENCESCOPE, T.NUMBERSEQUENCEID, T2.NUMBERSEQUENCE, T4.Id, T4.Name, * from dbo.NUMBERSEQUENCEREFERENCE as T
join NUMBERSEQUENCETABLE as T2 on T.NUMBERSEQUENCEID = T2.RECID
join NUMBERSEQUENCEDATATYPE as T3 on T3.RECID = T.NUMBERSEQUENCEDATATYPE
join dbo.SYSTYPEIDVIEW as T4 on T4.Id = T3.DATATYPEID
where T.NUMBERSEQUENCESCOPE = 22565423787
```
5.  Some code screenshot:
   <img width="774" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/027ca964-a072-4a49-992f-984e221f4f0b">
   <img width="778" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/471b7420-bbda-4d62-bb6d-2032e9d99ac3">

6. Simplified version:<br/>

```sql
select T.Recid, T.scopetype, * from dbo.numbersequencedatatype as T where T.ScopeType = 4

select  T.STORENUMBER, T2.OMOPERATINGUNITNUMBER, * from dbo.RETAILCHANNELTABLE as T join dbo.DIRPARTYTABLE as T2 on T.OMOPERATINGUNITID = T2.RECID;

SELECT T1.RECID,
       T1.NAME,
       T1.NAMEALIAS,
	   T1.OMOPERATINGUNITNUMBER
FROM DIRPARTYTABLE T1
WHERE T1.PARTITION = 5637144576 and 
  T1.OMOPERATINGUNITNUMBER = '15524'
  AND T1.INSTANCERELATIONTYPE IN (SELECT ID 
                                  FROM dbo.TABLEIDTABLE 
                                  WHERE NAME = 'OMOperatingUnit')


select scope.RECID, * from NumberSequenceScope scope
        where
            scope.DataArea = '' and
            scope.LegalEntity = 0 and
            scope.OperatingUnit = 5639229646 and
            scope.FiscalCalendarPeriod = 0 and
            scope.OperatingUnitType = 0;


 SELECT T2.NUMBERSEQUENCE,T1.NUMBERSEQUENCEDATATYPE, * FROM NUMBERSEQUENCEREFERENCE T1 
			join NUMBERSEQUENCETABLE as T2 on T1.NUMBERSEQUENCEID = T2.RECID
			WHERE T1.PARTITION = 5637144576
			  --AND T1.NUMBERSEQUENCEDATATYPE = 5637144589
			  AND T1.NUMBERSEQUENCESCOPE = 5637153015
			  AND T1.NUMBERSEQUENCEID <> 0
			  AND T1.RECID <> 0
			  AND T1.NUMBERSEQUENCEID <> 0
```


