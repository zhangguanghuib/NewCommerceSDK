## Retail Store Statement Number is not generated correctly?

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
