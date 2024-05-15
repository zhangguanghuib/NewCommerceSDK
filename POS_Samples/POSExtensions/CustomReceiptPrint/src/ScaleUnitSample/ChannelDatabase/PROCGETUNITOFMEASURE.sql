
IF OBJECT_ID(N'[ext].[PROCGETUNITOFMEASURE]', N'P') IS NOT NULL
    DROP PROCEDURE [ext].[PROCGETUNITOFMEASURE]
GO

CREATE PROCEDURE [ext].[PROCGETUNITOFMEASURE]
    @tvp_QueryResultSettings [crt].[QUERYRESULTSETTINGSTABLETYPE] READONLY,    
    @bi_ChannelId            BIGINT,    
    @tvp_ProductIds          [crt].[RECORDIDTABLETYPE]    READONLY  
AS    
BEGIN    
    SET NOCOUNT ON;    
    
    -- Get the data area for the specified channel identifier.    
    DECLARE @nvc_DataAreaId NVARCHAR(4);    
    SELECT @nvc_DataAreaId = rct.INVENTLOCATIONDATAAREAID FROM [ax].RETAILCHANNELTABLE rct WHERE rct.RECID = @bi_ChannelId;    
    
    -- Retrieves the product specific unit of measure conversions.    
    SELECT    
        it.PRODUCT              AS PRODUCT,    
        it.ITEMID               AS ITEMID,    
        uomc.FROMUNITOFMEASURE  AS FROMUNITID,    
        uom1.SYMBOL             AS FROMUOMSYMBOL,    
        uomc.TOUNITOFMEASURE    AS TOUNITID,    
        uom2.SYMBOL             AS TOUOMSYMBOL,  
		uomc.FACTOR             AS FACTOR  
    FROM @tvp_ProductIds tvppi    
    INNER JOIN [ax].UNITOFMEASURECONVERSION uomc ON tvppi.RECID = uomc.PRODUCT    
    INNER JOIN [ax].UNITOFMEASURE uom1 ON uom1.RECID = uomc.FROMUNITOFMEASURE    
    INNER JOIN [ax].UNITOFMEASURE uom2 ON uom2.RECID = uomc.TOUNITOFMEASURE    
    INNER JOIN [ax].INVENTTABLE it ON it.PRODUCT = uomc.PRODUCT AND it.DATAAREAID = @nvc_DataAreaId    
    INNER JOIN [ax].INVENTTABLEMODULE itm ON (itm.ITEMID = it.ITEMID AND itm.MODULETYPE = 2 AND itm.DATAAREAID = @nvc_DataAreaId)    
        AND (uom1.SYMBOL = itm.UNITID OR uom2.SYMBOL = itm.UNITID)    
    WHERE (uom1.SYMBOL = itm.UNITID OR uom2.SYMBOL = itm.UNITID)    
    
    UNION ALL
    
    SELECT    
        [PRODUCT],    
        [ITEMID],    
        [FROMUNITID],    
        [FROMUOMSYMBOL],    
        [TOUNITID],    
        [TOUOMSYMBOL],  
		NULL FACTOR  
    FROM [crt].GETUNITOFMEASURECONVERSIONOPTIONS(@nvc_DataAreaId, @tvp_ProductIds)    
END;
GO

GRANT EXECUTE ON [ext].[PROCGETUNITOFMEASURE] TO [UsersRole];
GO

GRANT EXECUTE ON [ext].[PROCGETUNITOFMEASURE] TO [DeployExtensibilityRole];
GO