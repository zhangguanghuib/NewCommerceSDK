## How to get the latest shift id when POS activation?

1. In CSU side  GetLatestNumberSequence.sql
   
```sql
exec [crt].GETLATESTNUMBERSEQUENCE @bi_ChannelId=5637144592,@nvc_TerminalId=N'HOUSTON-39'
```

```sql
-- Get max shift id
WITH SHIFTNUMSEQ(SHIFTID)
AS
(
	SELECT MAX(BATCHID) AS 'SHIFTID' FROM ax.RETAILPOSBATCHTABLE AS BT
	WHERE BT.CHANNEL = @bi_ChannelId AND BT.TERMINALID = @nvc_TerminalId

	UNION

	SELECT MAX(SHIFTID) FROM crt.RETAILSHIFTSTAGINGTABLE AS SHFT
	WHERE SHFT.CHANNEL = @bi_ChannelId AND SHFT.TERMINALID = @nvc_TerminalId
)
SELECT TOP 1 MAX(SHIFTID) AS SHIFTID FROM SHIFTNUMSEQ;
```

2.  In X++ side,  RetailPOSSeedData.getSeedDataInMap<br/>

<img width="1154" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/8b79cba7-2a40-415a-b318-3470d0e85486">

3. Run P-Job, and then go to this form:<br/>
   ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/1b3f8ba6-4623-474f-ba1c-385256385696)


