USE [AxDB]
GO

/****** Object:  StoredProcedure [ext].[VALIDATEADDRESSExt]    Script Date: 6/21/2023 7:29:45 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [ext].[VALIDATEADDRESSExt]
    @COUNTRYREGIONID    NVARCHAR(10),
    @STATEPROVINCEID    NVARCHAR(60) = NULL,
    @COUNTYID           NVARCHAR(60) = NULL,
    @CITY               NVARCHAR(60) = NULL,
    @DISTRICT           NVARCHAR(60) = NULL,
    @ZIPPOSTALCODE      NVARCHAR(60) = NULL
AS
BEGIN
SET NOCOUNT ON

DECLARE @i_ReturnCode               INT;
DECLARE @i_TransactionIsOurs        INT;
DECLARE @i_Error                    INT;
DECLARE @b_ValidateCity             BIT;
DECLARE @b_ValidateDistrict         BIT;
DECLARE @b_ValidateZipCode          BIT;
DECLARE @b_DisableValidateCounty    BIT  = 0;

SELECT 
    @b_ValidateCity     = VALIDATECITY, 
    @b_ValidateDistrict = VALIDATEDISTRICT, 
    @b_ValidateZipCode  = VALIDATEZIPCODE 
FROM [ax].LOGISTICSADDRESSPARAMETERS
WHERE [KEY] = 0

SELECT 
     @b_DisableValidateCounty  = DISABLECOUNTYVALIDATION
FROM [ext].[LOGISTICSADDRESSPARAMETERSExt]
WHERE [KEY] = 0


-- CountryId is mandatory according to Ax schema
IF (@COUNTRYREGIONID IS NULL)
BEGIN
    SET @i_ReturnCode           = 1 ; -- invalid countryid
    GOTO exit_label;
END

IF NOT EXISTS
(
    SELECT 1
    FROM [ax].LOGISTICSADDRESSCOUNTRYREGION LCNTRY
    WHERE LCNTRY.COUNTRYREGIONID = @COUNTRYREGIONID
)
BEGIN
    SET @i_ReturnCode           = 1 ; -- invalid country/region
    GOTO exit_label;
END

-- Validate State, if exists
IF (@STATEPROVINCEID IS NOT NULL)
BEGIN
    IF NOT EXISTS
    (
        SELECT 1
        FROM [ax].LOGISTICSADDRESSSTATE LSTATE
        WHERE
            LSTATE.STATEID = @STATEPROVINCEID AND
            LSTATE.COUNTRYREGIONID = @COUNTRYREGIONID
    )
    BEGIN
        SET @i_ReturnCode           = 2 ; -- invalid stateid
        GOTO exit_label;
    END
END

-- Validate County, if exists
IF (@COUNTYID IS NOT NULL) and (@b_DisableValidateCounty = 0)
BEGIN
    IF NOT EXISTS
    (
        SELECT 1
        FROM [ax].LOGISTICSADDRESSCOUNTY LCNTY
        WHERE
            LCNTY.COUNTYID = @COUNTYID AND
            LCNTY.COUNTRYREGIONID = @COUNTRYREGIONID AND
            (@STATEPROVINCEID IS NULL OR LCNTY.STATEID = @STATEPROVINCEID)
    )
    BEGIN
        SET @i_ReturnCode           = 3 ; -- invalid countyid
        GOTO exit_label;
    END
END

-- Validate City, if exists
IF (@CITY IS NOT NULL) AND (@b_ValidateCity != 0)
BEGIN
    IF NOT EXISTS
    (
        SELECT 1
        FROM [ax].LOGISTICSADDRESSCITY LCITY
        INNER JOIN [ax].LOGISTICSADDRESSCOUNTRYREGION LCNTRY ON LCNTRY.COUNTRYREGIONID = LCITY.COUNTRYREGIONID
        WHERE
            LCITY.NAME = @CITY
            AND LCITY.COUNTRYREGIONID = LCNTRY.COUNTRYREGIONID
            AND (@STATEPROVINCEID IS NULL OR LCITY.STATEID = @STATEPROVINCEID)
            AND (@COUNTYID IS NULL OR LCITY.COUNTYID = '' OR LCITY.COUNTYID = @COUNTYID)
    )
    BEGIN
        SET @i_ReturnCode           = 4 ; -- invalid city
        GOTO exit_label;
    END
END

-- Validate District, if exists
IF (@DISTRICT IS NOT NULL) AND (@b_ValidateDistrict != 0)
BEGIN
    IF NOT EXISTS
    (
        SELECT 1
        FROM [ax].LOGISTICSADDRESSDISTRICT LDIST
        LEFT OUTER JOIN [ax].LOGISTICSADDRESSCITY LCITY ON LCITY.RECID = LDIST.CITY
        WHERE
            LDIST.NAME = @DISTRICT
            AND (@CITY IS NULL OR LCITY.NAME = @CITY )
            AND (@COUNTYID IS NULL OR LCITY.COUNTYID = '' OR LCITY.COUNTYID = @COUNTYID)
            AND (@STATEPROVINCEID IS NULL OR LCITY.STATEID = @STATEPROVINCEID)
            AND (LCITY.COUNTRYREGIONID = @COUNTRYREGIONID)
    )
    BEGIN
        SET @i_ReturnCode           = 5 ; -- invalid district
        GOTO exit_label;
    END
END

-- Validate ZipCode, if exists
IF (@ZIPPOSTALCODE IS NOT NULL) AND ((@b_ValidateZipCode IS NULL) OR (@b_ValidateZipCode != 0))
BEGIN
    IF NOT EXISTS
    (
        SELECT 1
        FROM [ax].LOGISTICSADDRESSZIPCODE LZIP
        WHERE
            LZIP.ZIPCODE = @ZIPPOSTALCODE
            AND LZIP.COUNTRYREGIONID = @COUNTRYREGIONID
            AND (@STATEPROVINCEID IS NULL OR LZIP.STATE = @STATEPROVINCEID)
    )
    BEGIN
        SET @i_ReturnCode           = 6 ; -- invalid zipcode
        GOTO exit_label;
    END
END

SET @i_ReturnCode = 0;

exit_label:

    RETURN @i_ReturnCode;
END;
GO


