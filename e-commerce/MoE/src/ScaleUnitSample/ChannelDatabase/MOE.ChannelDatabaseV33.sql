---==============================================Tables===============================================================

---=================================================[ext].[MOERETAILPARAMETERS]======================================================

IF  NOT EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID('[ext].[MOERETAILPARAMETERS]') AND type in (N'U'))

BEGIN

CREATE TABLE [ext].[MOERETAILPARAMETERS](
	[RECID] [bigint] NOT NULL,
	[MOEDOWNLOADLINKEXPIRY] [int] NOT NULL,
    [MOEDIGITALBASEURL] [nvarchar](500),
 CONSTRAINT [I_MOERETAILPARAMETERSRECID] PRIMARY KEY CLUSTERED 
(
	[RECID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [ext].[MOERETAILPARAMETERS] ADD  DEFAULT ((0)) FOR [MOEDOWNLOADLINKEXPIRY]

END
GO

GRANT SELECT, INSERT, UPDATE, DELETE ON OBJECT::[ext].[MOERETAILPARAMETERS] TO [DataSyncUsersRole]
GO


IF NOT EXISTS(SELECT * FROM sys.columns 
				WHERE (Name = N'MOEDIGITALBASEURL') AND Object_ID = Object_ID(N'[ext].[MOERETAILPARAMETERS]'))

BEGIN
Alter table ext.MOERETAILPARAMETERS
add MOEDIGITALBASEURL [nvarchar](500) DEFAULT('')
END

---=================================================[ext].[MOERETAILCONFIGURATIONPARAMETERS]======================================================


IF  NOT EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID('[ext].[MOERETAILCONFIGURATIONPARAMETERS]') AND type in (N'U'))

BEGIN

CREATE TABLE [ext].[MOERETAILCONFIGURATIONPARAMETERS](
	[DATAAREAID] [nvarchar](4) NOT NULL,
	[RECID] [bigint] NOT NULL,
	[KEYPASSWORDSTR] [nvarchar](60) NOT NULL,
	[DIGITALKEY] [nvarchar](64) NOT NULL,
 CONSTRAINT [IDX_PK_MOEDIGITALKEY] PRIMARY KEY NONCLUSTERED 
(
	[DATAAREAID] ASC,
	[DIGITALKEY] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

END
GRANT SELECT, INSERT, UPDATE, DELETE ON OBJECT::[ext].MOERETAILCONFIGURATIONPARAMETERS TO [DataSyncUsersRole]

---=================================================[ext].[MOEATTRIBUTEMAORITRANSLATIONTABLE]======================================================

IF  NOT EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID('[ext].[MOEATTRIBUTEMAORITRANSLATIONTABLE]') AND type in (N'U'))

BEGIN

	CREATE TABLE [ext].[MOEATTRIBUTEMAORITRANSLATIONTABLE](
 		[ECORESATTRIBUTERECID] [bigint] NOT NULL,
		[TRANSLATEDNAME] [nvarchar](60) NOT NULL,
		[RECID] [bigint] NOT NULL,
	CONSTRAINT [I_MOEATTRIBUTEMAORITRANSLATIONTABLE_RECID] PRIMARY KEY CLUSTERED
	(
		[RECID] ASC
	))

END
GO

GRANT SELECT, INSERT, UPDATE, DELETE ON OBJECT::[ext].[MOEATTRIBUTEMAORITRANSLATIONTABLE] TO [DataSyncUsersRole]
GO

---=================================================[ext].[MOERETAILCHANNELTABLE]======================================================

IF  NOT EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID('[ext].[MOERETAILCHANNELTABLE]') AND type in (N'U'))

BEGIN

CREATE TABLE [ext].[MOERETAILCHANNELTABLE](
	[RECID] [bigint] NOT NULL,
	[MOELOCALEDOMAINMAPPINGS] [nvarchar](500) NOT NULL,
	[MOECHANNELPRIORITY] [nvarchar](60) NOT NULL,
 CONSTRAINT [IDX_RETAILCHANNELTABLE_RECID] PRIMARY KEY CLUSTERED 
(
	[RECID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

END

GRANT SELECT, INSERT, UPDATE, DELETE ON OBJECT::[ext].[MOERETAILCHANNELTABLE] TO [DataSyncUsersRole]

---=================================================[ext].[MOERESOURCEARTIFACT]======================================================
declare @artifact varchar(60) = '[ext].[MOERESOURCEARTIFACT]'

IF  NOT EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(@artifact) AND type in (N'U'))

BEGIN

    CREATE TABLE [ext].[MOERESOURCEARTIFACT](
        [ARTIFACTID] [nvarchar](20) NOT NULL,
        [ITEMID] [nvarchar](20) NOT NULL,
        [MEDIANAME] [nvarchar](60) NOT NULL,
        [FILETYPE] [nvarchar](259) NOT NULL,
        [MEDIAURL] [nvarchar](500) NOT NULL,
        [ISRETIREARTIFACT] [int] NOT NULL,
        [DATAAREAID] [nvarchar](4) NOT NULL,
        [RECID] [bigint] NOT NULL,
        [LANGUAGEID] [nvarchar](60) NOT NULL,
        [FILESIZE] [nvarchar](20) NOT NULL, 
    CONSTRAINT [IDX_PK_MOERESOURCEARTIFACT] PRIMARY KEY NONCLUSTERED 
    (
        [DATAAREAID] ASC,
        [ARTIFACTID] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]

END
GRANT SELECT, INSERT, UPDATE, DELETE ON OBJECT::[ext].MOERESOURCEARTIFACT TO [DataSyncUsersRole]

IF NOT EXISTS(SELECT * FROM sys.columns 
				WHERE (Name = N'ISRETIREARTIFACT') AND Object_ID = Object_ID(@artifact))

BEGIN
    Alter table ext.MOERESOURCEARTIFACT
    add ISRETIREARTIFACT [int] DEFAULT (0)
END

IF NOT EXISTS(SELECT * FROM sys.columns 
				WHERE (Name = N'LANGUAGEID') AND Object_ID = Object_ID(@artifact))

BEGIN
    Alter table ext.MOERESOURCEARTIFACT
    add LANGUAGEID [nvarchar](60) DEFAULT ('')
END

IF NOT EXISTS(SELECT * FROM sys.columns 
				WHERE (Name = N'FILESIZE') AND Object_ID = Object_ID('[ext].[MOERESOURCEARTIFACT]'))

BEGIN
    ALTER TABLE EXT.MOERESOURCEARTIFACT
    ADD FILESIZE [NVARCHAR](20) DEFAULT ('')
END

-- Change Field length to 20
IF EXISTS(SELECT * FROM sys.columns 
				WHERE (Name = N'FILESIZE') AND Object_ID = Object_ID('[ext].[MOERESOURCEARTIFACT]'))

BEGIN
  ALTER TABLE EXT.MOERESOURCEARTIFACT ALTER COLUMN FILESIZE [NVARCHAR](20) 
END

---=================================================[ext].[MOERETAILTRANSACTIONARTIFACTTRANS]======================================================
IF  NOT EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID('[ext].[MOERETAILTRANSACTIONARTIFACTTRANS]') AND type in (N'U'))

BEGIN

CREATE TABLE [ext].[MOERETAILTRANSACTIONARTIFACTTRANS](
	[STORE] [nvarchar](10) NOT NULL,
	[CHANNEL] [bigint] NOT NULL,
	[TRANSACTIONID] [nvarchar](44) NOT NULL,
	[TERMINALID] [nvarchar](10) NOT NULL,
	[LINENUM] [numeric](32, 16) NOT NULL,
	[ITEMID] [nvarchar](20) NOT NULL,
	[ARTIFACTID] [nvarchar](20) NOT NULL,
	[REPLICATIONCOUNTERFROMORIGIN] [int] IDENTITY(1,1) NOT NULL,
	[DATAAREAID] [nvarchar](4) NOT NULL,
	[RESOURCEPAGEURL] [nvarchar](500) NULL,
    [ROWVERSION] [timestamp] NOT NULL,
 CONSTRAINT [IDX_PK_TRANSACTIONLINEIDX] PRIMARY KEY CLUSTERED 
(
	[DATAAREAID] ASC,
	[CHANNEL] ASC,
	[STORE] ASC,
	[TERMINALID] ASC,
	[TRANSACTIONID] ASC,
	[LINENUM] ASC,
    [ARTIFACTID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

END
GRANT SELECT, INSERT, UPDATE, DELETE ON OBJECT::[ext].MOERETAILTRANSACTIONARTIFACTTRANS TO [DataSyncUsersRole]
GO

---=================================================[ext].[MOERETAILTRANSACTIONTABLE]======================================================
IF  NOT EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID('[ext].[MOERETAILTRANSACTIONTABLE]') AND type in (N'U'))

BEGIN

CREATE TABLE [ext].[MOERETAILTRANSACTIONTABLE](
	[STORE] [nvarchar](10) NOT NULL,
	[CHANNEL] [bigint] NOT NULL,
	[TRANSACTIONID] [nvarchar](44) NOT NULL,
	[TERMINAL] [nvarchar](10) NOT NULL,
	[EMAILDOWNLOADLINK] [int] NOT NULL,
	[REPLICATIONCOUNTERFROMORIGIN] [int] IDENTITY(1,1) NOT NULL,
	[DATAAREAID] [nvarchar](4) NOT NULL,
    [ROWVERSION] [timestamp] NOT NULL,
CONSTRAINT [IDX_PK_TRANSACTIONIDX] PRIMARY KEY CLUSTERED 
(
    [DATAAREAID] ASC,
	[CHANNEL] ASC,
	[STORE] ASC,
	[TERMINAL] ASC,
	[TRANSACTIONID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

END
GO

GRANT SELECT, INSERT, UPDATE, DELETE ON OBJECT::[ext].MOERETAILTRANSACTIONTABLE TO [DataSyncUsersRole]
GO

---=================================================[ext].[MOEINVENTTABLE]======================================================


IF  NOT EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID('[ext].[MOEINVENTTABLE]') AND type in (N'U'))

BEGIN

CREATE TABLE [EXT].[MOEINVENTTABLE](
	[ITEMID] [NVARCHAR](20) NOT NULL,
	[DATAAREAID] [NVARCHAR](4) NOT NULL,
	[RECID] [BIGINT] NOT NULL,
	[MOEPUBLISHSTATUS] [INT] NOT NULL,
	
 CONSTRAINT [IDX_PK_MOEINVENTTABLE] PRIMARY KEY NONCLUSTERED 
(
	[ITEMID] ASC,
	[DATAAREAID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

END
GRANT SELECT, INSERT, UPDATE, DELETE ON OBJECT::[ext].MOEINVENTTABLE TO [DataSyncUsersRole]

---=================================================[ext].[MOEINVENTTABLE]======================================================

---====================================================TableType=================================================================


---====================================================Procedures=================================================================

---=================================================[ext].[INSERTRETAILTRANSACTIONEXT]======================================================

IF EXISTS(SELECT 1 FROM sys.objects WHERE type = 'P' and object_id = OBJECT_ID('[ext].[INSERTRETAILTRANSACTIONEXT]'))
BEGIN
DROP PROCEDURE [ext].[INSERTRETAILTRANSACTIONEXT]
END
GO

CREATE PROCEDURE [ext].[INSERTRETAILTRANSACTIONEXT]
    @nvc_Store [nvarchar](10),
    @nvc_Terminal [nvarchar](10),
    @nvc_TransactionId [nvarchar](44),
    @bi_ChannelId BIGINT,
    @nvc_DataArea [nvarchar](4),
    @i_MOEEmailDownloadLink int
AS
BEGIN

    SET NOCOUNT ON;

    DECLARE @i_ReturnCode               INT;
    DECLARE @i_TransactionIsOurs        INT;
    DECLARE @i_Error                    INT;

    -- initializes the return code and assume the transaction is not ours by default
    SET @i_ReturnCode = 0;
    SET @i_TransactionIsOurs = 0;

    IF @@TRANCOUNT = 0
    BEGIN
        BEGIN TRANSACTION;

        SELECT @i_Error = @@ERROR;
        IF @i_Error <> 0
        BEGIN
            SET @i_ReturnCode = @i_Error;
            GOTO exit_label;
        END;

        SET @i_TransactionIsOurs = 1;
    END;

    -- Perform Insert
    INSERT INTO ext.MOERETAILTRANSACTIONTABLE
    (
        [STORE],
        [CHANNEL],
        [TRANSACTIONID],
        [TERMINAL],
        [EMAILDOWNLOADLINK],
        [DATAAREAID]
    )
    VALUES
	(
        @nvc_Store,
        @bi_ChannelId,
        @nvc_TransactionId,
        @nvc_Terminal,
        @i_MOEEmailDownloadLink,
        @nvc_DataArea
	)

    SELECT @i_Error = @@ERROR;
    IF @i_Error <> 0
    BEGIN
        SET @i_ReturnCode = @i_Error;
        GOTO exit_label;
    END;

    IF @i_TransactionIsOurs = 1
    BEGIN
        COMMIT TRANSACTION;

        SET @i_Error = @@ERROR;
        IF @i_Error <> 0
        BEGIN
            SET @i_ReturnCode = @i_Error;
            GOTO exit_label;
        END;

        SET @i_TransactionIsOurs = 0;
    END;

exit_label:

    IF @i_TransactionIsOurs = 1
    BEGIN
        ROLLBACK TRANSACTION;
    END;

    RETURN @i_ReturnCode;
END;
GO

GRANT EXECUTE ON [ext].[INSERTRETAILTRANSACTIONEXT] TO [DeployExtensibilityRole];
GO
GRANT EXECUTE ON [ext].[INSERTRETAILTRANSACTIONEXT] TO [UsersRole];
GO

---=================================================[ext].[MOEINSERTARTIFACTSTRANSACTION]======================================================

IF EXISTS(SELECT 1 FROM sys.objects WHERE type = 'P' and object_id = OBJECT_ID('[ext].[MOEINSERTARTIFACTSTRANSACTION]'))
BEGIN
DROP PROCEDURE [ext].[MOEINSERTARTIFACTSTRANSACTION]
END
GO

CREATE PROCEDURE [ext].[MOEINSERTARTIFACTSTRANSACTION]
    @nvc_Store [nvarchar](10),
    @nvc_Terminal [nvarchar](10),
    @nvc_TransactionId [nvarchar](44),
    @bi_ChannelId BIGINT,
    @nvc_DataArea [nvarchar](4),
    @d_LineNum [numeric](32, 16),
	@nvc_ItemId [nvarchar](20),
	@nvc_ArtifactsId [nvarchar](20),
	@nvc_ResourceURL [nvarchar](500)

AS
BEGIN

    SET NOCOUNT ON;

    DECLARE @i_ReturnCode               INT;
    DECLARE @i_TransactionIsOurs        INT;
    DECLARE @i_Error                    INT;

    -- initializes the return code and assume the transaction is not ours by default
    SET @i_ReturnCode = 0;
    SET @i_TransactionIsOurs = 0;

    IF @@TRANCOUNT = 0
    BEGIN
        BEGIN TRANSACTION;

        SELECT @i_Error = @@ERROR;
        IF @i_Error <> 0
        BEGIN
            SET @i_ReturnCode = @i_Error;
            GOTO exit_label;
        END;

        SET @i_TransactionIsOurs = 1;
    END;

    -- Perform Insert
    INSERT INTO ext.MOERETAILTRANSACTIONARTIFACTTRANS
    (
        [STORE],
        [CHANNEL],
        [TRANSACTIONID],
        [TERMINALID],
		[LINENUM],
		[ITEMID],
		[ARTIFACTID],
		[DATAAREAID],
		[RESOURCEPAGEURL]
    )
    VALUES
	(
        @nvc_Store,
        @bi_ChannelId,
        @nvc_TransactionId,
        @nvc_Terminal,
        @d_LineNum,
		@nvc_ItemId,
		@nvc_ArtifactsId,
		@nvc_DataArea,
		@nvc_ResourceURL
	)

    SELECT @i_Error = @@ERROR;
    IF @i_Error <> 0
    BEGIN
        SET @i_ReturnCode = @i_Error;
        GOTO exit_label;
    END;

    IF @i_TransactionIsOurs = 1
    BEGIN
        COMMIT TRANSACTION;

        SET @i_Error = @@ERROR;
        IF @i_Error <> 0
        BEGIN
            SET @i_ReturnCode = @i_Error;
            GOTO exit_label;
        END;

        SET @i_TransactionIsOurs = 0;
    END;

exit_label:

    IF @i_TransactionIsOurs = 1
    BEGIN
        ROLLBACK TRANSACTION;
    END;

    RETURN @i_ReturnCode;
END;
GO

GRANT EXECUTE ON [ext].[INSERTRETAILTRANSACTIONEXT] TO [DeployExtensibilityRole];
GO
GRANT EXECUTE ON [ext].[INSERTRETAILTRANSACTIONEXT] TO [UsersRole];
GO

---=================================================[ext].[GETCUSTOMERINFO]======================================================

IF EXISTS(SELECT 1 FROM sys.objects WHERE type = 'P' and object_id = OBJECT_ID('[ext].[GETCUSTOMERINFO]'))
BEGIN
DROP PROCEDURE [ext].[GETCUSTOMERINFO]
END
GO

CREATE PROCEDURE [ext].[GETCUSTOMERINFO]
    @nvc_DataAreaId            NVARCHAR(4),
    @tvp_CustomerAccountId    NVARCHAR(40)
AS
BEGIN

    SELECT
	   cv.[ACCOUNTNUMBER]
      ,cv.[EMAIL]
      ,cv.[PARTY]
      ,cpa.[NAME] as 'ADDRESSDESCRIPTION'
	  ,cpa.[ADDRESS]
	  ,cpa.[COUNTRYREGIONID]
      ,cpa.[ZIPCODE]
      ,cpa.[STATE]
      ,cpa.[STATENAME]
      ,cpa.[CITY]
      ,cpa.[STREET]
	  ,cev.[LOCATIONDESCRIPTION] as 'EMAILDESCRIPTION'
	  ,cev.[LOCATOR]
  FROM [crt].[CUSTOMERSVIEW] cv
  JOIN [crt].[CUSTOMERPOSTALADDRESSESVIEW] cpa
  ON cpa.DIRPARTYTABLERECID = cv.PARTY
  JOIN [crt].CUSTOMERELECTRONICADDRESSESVIEW cev
  ON cev.DIRPARTYRECORDID = cv.PARTY
  WHERE cv.ACCOUNTNUMBER = @tvp_CustomerAccountId
  AND cv.DATAAREAID = @nvc_DataAreaId 
  AND cpa.ISPRIMARY = 1
  AND cev.METHODYTPE = 2
  AND cev.ISPRIMARY = 1 
 
END;
GO

GRANT EXECUTE ON [ext].[GETCUSTOMERINFO] TO [DeployExtensibilityRole];
GO
GRANT EXECUTE ON [ext].[GETCUSTOMERINFO] TO [UsersRole];
GO        

---=================================================[ext].[VALIDATECUSTOMEREMAIL]======================================================

IF EXISTS(SELECT 1 FROM sys.objects WHERE type = 'P' and object_id = OBJECT_ID('[ext].[VALIDATECUSTOMEREMAIL]'))
BEGIN
DROP PROCEDURE [ext].[VALIDATECUSTOMEREMAIL]
END
GO

CREATE PROCEDURE [ext].[VALIDATECUSTOMEREMAIL]
    @nvc_EmailAddress        NVARCHAR(100)
AS
BEGIN
    IF EXISTS (SELECT * FROM [crt].[CUSTOMERSVIEW]
                    WHERE
                        EMAIL = @nvc_EmailAddress)
        SELECT 1 AS VALIDCUSTOMER;
    ELSE
        SELECT 0 AS VALIDCUSTOMER;
END;
GO

GRANT EXECUTE ON [ext].[VALIDATECUSTOMEREMAIL] TO [DeployExtensibilityRole];
GO
GRANT EXECUTE ON [ext].[VALIDATECUSTOMEREMAIL] TO [UsersRole];
GO

---=================================================[ext].[GetMoeATTRIBUTEMAORITRANSLATIONTABLE]======================================================

IF EXISTS(SELECT 1 FROM sys.objects WHERE type = 'P' and object_id = OBJECT_ID('[ext].[GetMoeATTRIBUTEMAORITRANSLATIONTABLE]'))
BEGIN
DROP PROCEDURE [ext].[GetMoeATTRIBUTEMAORITRANSLATIONTABLE]
END
GO

CREATE PROCEDURE [ext].[GetMoeATTRIBUTEMAORITRANSLATIONTABLE]
AS
BEGIN
	SELECT ECORESATTRIBUTERECID AS RECID, TRANSLATEDNAME AS NAME
	FROM [EXT].[MOEATTRIBUTEMAORITRANSLATIONTABLE]
END
GO

GRANT EXECUTE ON [ext].[GetMoeATTRIBUTEMAORITRANSLATIONTABLE] TO [DeployExtensibilityRole];
GO
GRANT EXECUTE ON [ext].[GetMoeATTRIBUTEMAORITRANSLATIONTABLE] TO [UsersRole];
GO

---=================================================[ext].[MOEGETPRODUCTRELATIONSHIPS_V2]======================================================

IF EXISTS(SELECT 1 FROM sys.objects WHERE type = 'P' and object_id = OBJECT_ID('[ext].[MOEGETPRODUCTRELATIONSHIPS_V2]'))
BEGIN
DROP PROCEDURE [ext].[MOEGETPRODUCTRELATIONSHIPS_V2]
END
GO

CREATE PROCEDURE [ext].[MOEGETPRODUCTRELATIONSHIPS_V2]
	@bi_ChannelId   BIGINT,
    @dt_ChannelDate DATETIME,
    @nvc_Locale     NVARCHAR(7),
    @bi_ProductId   BIGINT,
    @b_IsRemote     BIT

AS
BEGIN
SELECT results.RECID as PRODUCT1
FROM
(
    SELECT TOP 1 isnull([erprt_relation].PRODUCT1 , 0) AS RECID
    FROM [ax].ECORESPRODUCTRELATIONTABLE erprt_relation
    INNER JOIN (
        SELECT PRODUCTID FROM crt.GETASSORTEDPRODUCTS_V2(@bi_ChannelId, @dt_ChannelDate) WHERE ISREMOTE = @b_IsRemote
    ) lap ON lap.PRODUCTID = [erprt_relation].PRODUCT1
    INNER JOIN [ax].ECORESPRODUCTRELATIONTYPE erprt_type ON [erprt_type].RECID = [erprt_relation].PRODUCTRELATIONTYPE
    WHERE
        erprt_relation.PRODUCT2 = @bi_ProductId
) results
INNER JOIN [ax].RETAILCHANNELTABLE rct ON [rct].RECID = @bi_ChannelId
INNER JOIN [ax].INVENTTABLE it ON [it].PRODUCT = results.RECID AND [it].DATAAREAID = [rct].INVENTLOCATIONDATAAREAID
INNER JOIN [ax].ECORESPRODUCT erp ON [erp].RECID = results.RECID
LEFT OUTER JOIN [ax].ECORESPRODUCTTRANSLATION erpt ON [erpt].PRODUCT = results.RECID AND [erpt].LANGUAGEID = @nvc_Locale
LEFT OUTER JOIN [ax].INVENTTABLEMODULE itm ON [itm].ITEMID = [it].ITEMID AND [itm].DATAAREAID = [it].DATAAREAID AND [itm].MODULETYPE = 2  -- Sales
OUTER APPLY (SELECT URI FROM [crt].[GETDEFAULTMEDIAURIBYPRODUCTID](@bi_ChannelId, 0 /* Catalog Id = 0 */, erp.RECID, [erp].DISPLAYPRODUCTNUMBER, @nvc_Locale)) gpml

END;
GO

GRANT EXECUTE ON [ext].[MOEGETPRODUCTRELATIONSHIPS_V2] TO [DeployExtensibilityRole];
GO
GRANT EXECUTE ON [ext].[MOEGETPRODUCTRELATIONSHIPS_V2] TO [UsersRole];


---=================================================[ext].[GETMOERETAILCONFIGURATIONPARAMETERS]======================================================

IF EXISTS(SELECT 1 FROM sys.objects WHERE type = 'P' and object_id = OBJECT_ID('[ext].[GETMOERETAILCONFIGURATIONPARAMETERS]'))
BEGIN
DROP PROCEDURE [ext].[GETMOERETAILCONFIGURATIONPARAMETERS]
END
GO

CREATE PROCEDURE [ext].[GETMOERETAILCONFIGURATIONPARAMETERS]
    @nvc_DataAreaId            NVARCHAR(4)
AS
BEGIN

    SELECT
	   rcp.[DATAAREAID]
      ,rcp.[DIGITALKEY]
      ,rcp.[KEYPASSWORDSTR]
	  ,rcp.[RECID]
	  ,mrpm.[MOEDigitalBaseURL]
  FROM [ext].MOERETAILCONFIGURATIONPARAMETERS rcp
  JOIN [ax].RETAILPARAMETERS rpm
  ON rpm.DATAAREAID = rcp.DATAAREAID
  JOIN [ext].MOERETAILPARAMETERS mrpm
  ON mrpm.RECID = rpm.RECID
  WHERE rcp.DATAAREAID = @nvc_DataAreaId 
 
END;
GO

GRANT EXECUTE ON [ext].[GETMOERETAILCONFIGURATIONPARAMETERS] TO [DeployExtensibilityRole];
GO
GRANT EXECUTE ON [ext].[GETMOERETAILCONFIGURATIONPARAMETERS] TO [UsersRole];
GO

---=================================================[ext].[GETMOERETAILCHANNELTABLE]======================================================

IF EXISTS(SELECT 1 FROM sys.objects WHERE type = 'P' and object_id = OBJECT_ID('[ext].[GETMOERETAILCHANNELTABLE]'))
BEGIN
DROP PROCEDURE [ext].[GETMOERETAILCHANNELTABLE]
END
GO

CREATE PROCEDURE [ext].[GETMOERETAILCHANNELTABLE]
    @nvc_DataAreaId            NVARCHAR(4),
	@Channel_RecId			   bigint
AS
BEGIN

    SELECT
	   mrct.MOELOCALEDOMAINMAPPINGS
      ,mrct.MOECHANNELPRIORITY
      ,mrct.RECID
  FROM [ext].MOERETAILCHANNELTABLE mrct
  JOIN [ax].RETAILCHANNELTABLE rct
  ON rct.RECID = mrct.RECID
  WHERE rct.INVENTLOCATIONDATAAREAID = @nvc_DataAreaId 
  AND   rct.RECID = @Channel_RecId
 
END;
GO

GRANT EXECUTE ON [ext].[GETMOERETAILCHANNELTABLE] TO [DeployExtensibilityRole];
GO
GRANT EXECUTE ON [ext].[GETMOERETAILCHANNELTABLE] TO [UsersRole];
GO

---=================================================[ext].[TRANSACTIONIDTABLETYPE]======================================================

IF NOT EXISTS (SELECT * FROM sys.table_types 
WHERE user_type_id = TYPE_ID(N'[ext].[TRANSACTIONIDTABLETYPE]'))
Begin
	CREATE TYPE [ext].[TRANSACTIONIDTABLETYPE] AS TABLE(
		[VALUE] [nvarchar](44) NOT NULL
	)
End
GO

GRANT EXECUTE ON TYPE::[ext].[TRANSACTIONIDTABLETYPE] TO [DeployExtensibilityRole];
GO

GRANT EXECUTE ON TYPE::[ext].[TRANSACTIONIDTABLETYPE] TO [UsersRole];
GO

---=================================================[ext].[GETORDERHISTORYARTIFACTSDETAILS]======================================================

IF EXISTS(SELECT 1 FROM sys.objects WHERE type = 'P' and object_id = OBJECT_ID('[ext].[GETORDERHISTORYARTIFACTSDETAILS]'))
BEGIN
	DROP PROCEDURE [ext].[GETORDERHISTORYARTIFACTSDETAILS]
END
GO

CREATE PROCEDURE [ext].[GETORDERHISTORYARTIFACTSDETAILS]
    @nvc_TransactionIds             [ext].[TRANSACTIONIDTABLETYPE] READONLY,
    @bi_ChannelId                   BIGINT,
    @nvc_DataAreaId                 NVARCHAR(4)
AS
BEGIN
   SET NOCOUNT ON
   SELECT
        RTT.TRANSACTIONID,
        RTAT.LINENUM,
        RTAT.RESOURCEPAGEURL,
        sa.ARTIFACTID,
        sa.ITEMID,
        sa.MEDIANAME,
        sa.FILETYPE,
        sa.MEDIAURL,
        sa.ISRETIREARTIFACT,
        sa.RECID
    FROM [ext].MOERETAILTRANSACTIONARTIFACTTRANS RTAT
    JOIN [ax].[RETAILTRANSACTIONTABLE] RTT
        ON RTT.TRANSACTIONID = RTAT.TRANSACTIONID
            AND RTT.DATAAREAID = RTAT.DATAAREAID
            AND RTT.CHANNEL = RTAT.CHANNEL
    JOIN @nvc_TransactionIds transactions
		ON transactions.VALUE = RTT.TRANSACTIONID
	JOIN [ext].[MOERESOURCEARTIFACT] sa
        ON RTAT.ARTIFACTID = sa.ARTIFACTID
    WHERE RTAT.CHANNEL = @bi_ChannelId
        AND RTAT.DATAAREAID = @nvc_DataAreaId
END;
GO

GRANT EXECUTE ON [ext].[GETORDERHISTORYARTIFACTSDETAILS] TO [DeployExtensibilityRole];
GO
GRANT EXECUTE ON [ext].[GETORDERHISTORYARTIFACTSDETAILS] TO [UsersRole];
GO


---=================================================[ext].[GETATTRIBUTEMETADATA]======================================================

IF EXISTS(SELECT 1 FROM sys.objects WHERE type = 'P' and object_id = OBJECT_ID('[ext].[GETATTRIBUTEMETADATA]'))
BEGIN
	DROP PROCEDURE [ext].[GETATTRIBUTEMETADATA]
END
GO

CREATE PROCEDURE [ext].[GETATTRIBUTEMETADATA]
AS
BEGIN
	SET NOCOUNT ON
   
	select r.METADATA, r.ATTRIBUTE, e.CATEGORY, e.CATEGORYATTRIBUTE, e.RECID from ax.ECORESCATEGORYATTRIBUTELOOKUP e
	join ax.RETAILATTRIBUTEMETADATA r on e.ATTRIBUTE = r.ATTRIBUTE

END
GO

GRANT EXECUTE ON [ext].[GETATTRIBUTEMETADATA] TO [DeployExtensibilityRole];
GO
GRANT EXECUTE ON [ext].[GETATTRIBUTEMETADATA] TO [UsersRole];
GO

---=================================================[ext].[MOEBIGINTTABLETYPE]======================================================

IF NOT EXISTS (SELECT * FROM sys.table_types 
WHERE user_type_id = TYPE_ID(N'[ext].[MOEBIGINTTABLETYPE]'))
Begin
	CREATE TYPE [ext].[MOEBIGINTTABLETYPE] AS TABLE(
		[VALUE] [bigint] NOT NULL
	)
End
GO

GRANT EXECUTE ON TYPE::[ext].[MOEBIGINTTABLETYPE] TO [DeployExtensibilityRole];
GO

GRANT EXECUTE ON TYPE::[ext].[MOEBIGINTTABLETYPE] TO [UsersRole];
GO

---=================================================[ext].[MOEGETITEMDETAILS]======================================================

IF EXISTS(SELECT 1 FROM sys.objects WHERE type = 'P' and object_id = OBJECT_ID('[ext].[MOEGETITEMDETAILS]'))
BEGIN
DROP PROCEDURE [ext].[MOEGETITEMDETAILS]
END
GO

CREATE PROCEDURE [ext].[MOEGETITEMDETAILS]
(
	@nvc_Product	[ext].[MOEBIGINTTABLETYPE] READONLY
)
AS

BEGIN 
    SET NOCOUNT ON

	SELECT DISTINCT
		T.NAME,
		T.DESCRIPTION,
		T.LANGUAGEID,
		T.PRODUCT,
		T.RECID,
        rct.RECID AS CHANNELID
		FROM ax.ECORESPRODUCTTRANSLATION T
		JOIN @nvc_Product products ON products.VALUE = T.PRODUCT
        JOIN [crt].PRODUCTASSORTMENTRULES_V2 par ON T.PRODUCT = par.PRODUCTID
        JOIN [ax].RETAILASSORTMENTLOOKUPCHANNELGROUP ralcg ON ralcg.ASSORTMENTID = par.ASSORTMENTID
        JOIN [ax].RETAILCHANNELTABLE rct ON rct.OMOPERATINGUNITID = ralcg.OMOPERATINGUNITID
		WHERE GETDATE() BETWEEN PAR.VALIDFROM AND PAR.VALIDTO
		AND par.EXCLUDED = 0
        AND T.LANGUAGEID = 'en-nz'

END
GO

GRANT EXECUTE ON [ext].[MOEGETITEMDETAILS] TO [DeployExtensibilityRole];
GO
GRANT EXECUTE ON [ext].[MOEGETITEMDETAILS] TO [UsersRole];
GO
GRANT EXECUTE ON [ext].[MOEGETITEMDETAILS] TO [PublishersRole];
GO

---=================================================[ext].[MoeGETPRODUCTARTIFACTS]======================================================

IF EXISTS(SELECT 1 FROM sys.objects WHERE type = 'P' and object_id = OBJECT_ID('[ext].[MoeGETPRODUCTARTIFACTS]'))
BEGIN
DROP PROCEDURE [ext].[MoeGETPRODUCTARTIFACTS]
END
GO

CREATE PROCEDURE [ext].[MoeGETPRODUCTARTIFACTS]
    @itemId            nvarchar(20)
AS
BEGIN

    SELECT
	   ArtifactId
      ,ItemId
      ,[MediaName]
	  ,FileType
	  ,MediaUrl
	  ,ISRETIREARTIFACT
	  ,RecId
	  ,LANGUAGEID
  FROM MoeRESOURCEARTIFACT
  WHERE ItemId = @itemId
 
END;
GO

GRANT EXECUTE ON [ext].[MoeGETPRODUCTARTIFACTS] TO [DeployExtensibilityRole];
GO
GRANT EXECUTE ON [ext].[MoeGETPRODUCTARTIFACTS] TO [UsersRole];
GO

------------------==============================

