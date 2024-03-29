DROP PROCEDURE IF EXISTS [ext].[GETPRINTEDTRANSACTIONS];  
GO 

CREATE PROCEDURE [ext].[GETPRINTEDTRANSACTIONS]
    @tvp_QueryResultSettings    [crt].[QUERYRESULTSETTINGSTABLETYPE] READONLY,
	@AllTransIds                [crt].[STRINGIDTABLETYPE] READONLY,
	@MinDate                    DATE,
    @MaxDate                    DATE
AS
BEGIN
    SET NOCOUNT ON

	 SELECT 
		t.TRANSACTIONID,
		t.STORE,
		t.CHANNEL,
		t.TERMINAL,
		t.DATAAREAID
		FROM [EXT].CONTOSORETAILTRANSACTIONTABLE t 
		INNER JOIN @AllTransIds i ON t.TRANSACTIONID = i.STRINGID
		WHERE t.ISRECEIPTPRINTED = 1
END;

GO

GRANT EXECUTE ON [ext].[GETPRINTEDTRANSACTIONS] TO [UsersRole];
GO

GRANT EXECUTE ON [ext].[GETPRINTEDTRANSACTIONS] TO [PublishersRole];
GO

