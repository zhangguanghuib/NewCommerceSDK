
IF OBJECT_ID(N'[ext].[PARTYPUBLICPOSTALADDRESSVIEW]', N'V') IS NOT NULL
    DROP VIEW [ext].[[PARTYPUBLICPOSTALADDRESSVIEW]
GO

CREATE VIEW [ext].[PARTYPUBLICPOSTALADDRESSVIEW]
AS
    SELECT
        dp.RECID AS PARTYRECID,
        dp.NAME AS STORENAME,
        dp.PARTYNUMBER,
        dpl.RECID AS DIRPARTYLOCATIONRECID,
        dpl.PARTY,
        dpl.ATTENTIONTOADDRESSLINE,
        lpa.RECID,
        lpa.[ADDRESS],
        lpa.BUILDINGCOMPLIMENT,
        lpa.CITY,
        lpa.CITYRECID,
        lpa.DISTRICT,
        lpa.DISTRICTNAME,
        lpa.ISPRIVATE,
        lpa.LATITUDE,
        lpa.LOCATION,
        lpa.LONGITUDE,
        lpa.POSTBOX,
        lpa.STREET,
        lpa.STREETNUMBER,
        lpa.ZIPCODE,
        lpa.ZIPCODERECID,
        lpa.MODIFIEDDATETIME,
        lpa.COUNTRYREGIONID,
        lpa.[STATE],
        lpa.COUNTY
    FROM [ax].DIRPARTYTABLE dp
    LEFT JOIN [ax].DIRPARTYLOCATION dpl ON dpl.PARTY = dp.RECID --AND dpl.ISPRIMARY = 1
    OUTER APPLY
    (
        SELECT TOP 1
            RECID,
            [ADDRESS],
            BUILDINGCOMPLIMENT,
            CITY,
            CITYRECID,
            DISTRICT,
            DISTRICTNAME,
            ISPRIVATE,
            LATITUDE,
            [LOCATION],
            LONGITUDE,
            POSTBOX,
            STREET,
            STREETNUMBER,
            ZIPCODE,
            ZIPCODERECID,
            MODIFIEDDATETIME,
            COUNTRYREGIONID,
            [STATE],
            COUNTY
        FROM [ax].LOGISTICSPOSTALADDRESS
        WHERE LOCATION = dpl.LOCATION
            AND ISPRIVATE = 0
            AND GETUTCDATE() BETWEEN VALIDFROM AND VALIDTO
        ORDER BY VALIDFROM DESC
    ) lpa
GO

GRANT SELECT ON [ext].[PARTYPUBLICPOSTALADDRESSVIEW] TO [UsersRole];
GO


IF OBJECT_ID(N'[ext].[ORGUNITADDRESSVIEW]', N'V') IS NOT NULL
    DROP VIEW [ext].[ORGUNITADDRESSVIEW]
GO
CREATE VIEW [ext].[ORGUNITADDRESSVIEW] AS
(
    SELECT 
        rct.RECID AS [CHANNELID],
        rst.TAXGROUP,
        addr.RECID AS [RECID], -- POSTALADDRESSRECID, the RecId of Address class
        addr.BUILDINGCOMPLIMENT,
        addr.CITY,
        addr.COUNTRYREGIONID, -- Three letters country / region code.
        lacr.ISOCODE, -- Two letters country / region code.
        addr.COUNTY,
        lac.NAME AS [COUNTYNAME],
        addr.DISTRICTNAME,
        1 AS [ISPRIMARY], -- only select primary address
        addr.ISPRIVATE,
        addr.POSTBOX,
        addr.STATE,
        las.NAME AS [STATENAME],
        addr.STREET,
        addr.STREETNUMBER,
        addr.ZIPCODE,
        addr.ADDRESS, --Address.FullAddresss
        addr.PARTYRECID AS [DIRPARTYTABLERECID],
        addr.DIRPARTYLOCATIONRECID,
        addr.ATTENTIONTOADDRESSLINE,
        addr.PARTYNUMBER,
        addr.STORENAME,
        addr.LATITUDE,
        addr.LONGITUDE
    FROM [ax].RETAILCHANNELTABLE rct    
    LEFT OUTER JOIN [ax].RETAILSTORETABLE rst ON rct.RECID = rst.RECID
    -- ADDRESS INFORMATION
    LEFT OUTER JOIN [ext].PARTYPUBLICPOSTALADDRESSVIEW addr ON addr.PARTYRECID = rct.OMOPERATINGUNITID
    LEFT OUTER JOIN [ax].LOGISTICSADDRESSCOUNTRYREGION lacr ON addr.COUNTRYREGIONID = lacr.COUNTRYREGIONID
    --state name
    LEFT OUTER JOIN [ax].LOGISTICSADDRESSSTATE las ON addr.COUNTRYREGIONID = las.COUNTRYREGIONID
                AND addr.STATE = las.STATEID
    --county name
    LEFT OUTER JOIN [ax].LOGISTICSADDRESSCOUNTY lac ON addr.COUNTRYREGIONID = lac.COUNTRYREGIONID
                AND addr.STATE = lac.STATEID
                AND addr.COUNTY = lac.COUNTYID
)

GO

GRANT SELECT ON [ext].[ORGUNITADDRESSVIEW] TO [UsersRole];
GO
