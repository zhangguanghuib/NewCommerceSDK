USE [AxDB]
GO

/****** Object:  View [ext].[CHANNELTENDERTYPEVIEW]    Script Date: 6/23/2023 6:06:48 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [ext].[CHANNELTENDERTYPEVIEW] AS
(
    SELECT DISTINCT
        rsttt.CHANNEL,
        rsttt.RECID,
        rsttt.TENDERTYPEID,
        rsttt.NAME,
        ro.OPERATIONNAME,
        rsttt.POSOPERATION AS OPERATIONID,
        rsttt.CHANGETENDERID,
        rsttt.CONNECTORNAME AS CONNECTORID,
        rsttt.HIDECARDINPUTDETAILSINPOS,
        rsttt.[FUNCTION],
        IIF(LEN(rsttt.GIFTCARDITEMID) = 0 AND rsttt.TENDERTYPEID = ttgc.TENDERTYPEID, rtp.GIFTCARDITEM, rsttt.GIFTCARDITEMID) AS GIFTCARDITEMID,
        rsttt.OPENDRAWER,
        rsttt.SIGCAPENABLED AS USESIGNATURECAPTUREDEVICE,
        rsttt.SIGCAPMINAMOUNT AS MINIMUMSIGNATURECAPTUREAMOUNT,
        rsttt.ALLOWOVERTENDER,
        rsttt.ALLOWUNDERTENDER,
        rsttt.MAXIMUMOVERTENDERAMOUNT,
        rsttt.UNDERTENDERAMOUNT AS MAXIMUMUNDERTENDERAMOUNT,
        rsttt.MAXIMUMAMOUNTALLOWED AS MAXIMUMAMOUNTPERTRANSACTION,
        rsttt.MAXIMUMAMOUNTENTERED AS MAXIMUMAMOUNTPERLINE,
        rsttt.MINIMUMAMOUNTALLOWED AS MINIMUMAMOUNTPERTRANSACTION,
        rsttt.MINIMUMAMOUNTENTERED AS MINIMUMAMOUNTPERLINE,
        rsttt.MINIMUMCHANGEAMOUNT AS ABOVEMINIMUMCHANGEAMOUNT,
        rsttt.ABOVEMINIMUMTENDERID AS ABOVEMINIMUMCHANGETENDERTYTPEID,
        rsttt.MAXCOUNTINGDIFFERENCE,
        rsttt.MAXRECOUNT,
        rsttt.ROUNDINGMETHOD,
        rsttt.ROUNDING AS ROUNDOFF,
        rsttt.COUNTINGREQUIRED,
        rsttt.TAKENTOBANK,
        rsttt.TAKENTOSAFE,
        rsttt.CASHDRAWERLIMITENABLED,
        rsttt.CASHDRAWERLIMIT,
        rsttt.CHANGELINEONRECEIPT,
        rsttt.GIFTCARDCASHOUTTHRESHOLD,
        rsttt.RESTRICTRETURNSWITHOUTRECEIPT,
        IIF(ISNULL(td.RECID, 0) > 0, 1, 0) AS HASTENDERDISCOUNT,
        rct.CARDTYPEID,
        rsttt.USEFORDECLARESTARTAMOUNT,
		rstttext.DISPLAYORDER as DISPLAYORDER
    FROM [ax].RETAILSTORETENDERTYPETABLE rsttt
	    INNER JOIN [ext].RETAILSTORETENDERTYPETABLE rstttext
		ON rsttt.DATAAREAID = rstttext.DATAAREAID
		AND rsttt.CHANNEL = rstttext.CHANNEL
		AND rsttt.TENDERTYPEID = rstttext.TENDERTYPEID
        INNER JOIN [ax].RETAILCHANNELTABLE rct
        ON rsttt.CHANNEL = rct.RECID
        AND rsttt.DATAAREAID = rct.INVENTLOCATIONDATAAREAID -- disregard RetailStoreTenderType table records which are not part of the referenced store's inventlocationDataAreaId (i.e. referenced store's company)
        INNER JOIN [ax].RETAILPARAMETERS rtp
        ON rtp.DATAAREAID = rsttt.DATAAREAID
    LEFT OUTER JOIN [ax].RETAILOPERATIONS ro ON ro.OPERATIONID = rsttt.POSOPERATION
    LEFT JOIN
        (SELECT tenderDiscount.RECID, tenderDiscount.DATAAREAID, tt.TENDERTYPEID
            FROM [ax].RETAILTENDERDISCOUNT tenderDiscount
            JOIN [ax].RETAILTENDERTYPETABLE tt ON tenderDiscount.RETAILTENDERTYPE = tt.RECID
            WHERE tenderDiscount.STATUS = 1
        ) td ON td.DATAAREAID = rsttt.DATAAREAID
                    AND td.TENDERTYPEID = rsttt.TENDERTYPEID
    LEFT JOIN
        (SELECT rsttt.TENDERTYPEID, rsttt.GIFTCARDITEMID, rsttt.CHANNEL
            FROM [ax].RETAILSTORETENDERTYPETABLE rsttt
            INNER JOIN [ax]. RETAILCHANNELTABLE rct
            ON rsttt.CHANNEL = rct.RECID
            AND rsttt.DATAAREAID = rct.INVENTLOCATIONDATAAREAID
            INNER JOIN [ax].RETAILSTORETENDERTYPECARDTABLE rsttct
            ON rsttct.TENDERTYPEID = rsttt.TENDERTYPEID AND rsttct.CHANNEL = rct.RECID
            INNER JOIN [ax].RETAILTENDERTYPECARDTABLE rttct
            ON rttct.CARDTYPEID = rsttct.CARDTYPEID AND rttct.CARDTYPES = 7 -- RetailCardTypesBase::GiftCard
            WHERE LEN(rsttt.CONNECTORNAME) = 0
            )ttgc ON rsttt.CHANNEL = ttgc.CHANNEL
    AND IIF(LEN(rsttt.GIFTCARDITEMID) = 0 AND rsttt.TENDERTYPEID = ttgc.TENDERTYPEID, rtp.GIFTCARDITEM, rsttt.GIFTCARDITEMID) != ''
)
GO

