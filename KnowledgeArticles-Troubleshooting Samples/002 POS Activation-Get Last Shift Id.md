## How to get the latest shift id when POS activation?

1. In CSU side  GetLatestNumberSequence.sql<br/>
   
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

4. Some code screenshot:
   ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/faf34183-343b-4192-b885-a6d6cff4feb1)<br/>
   ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/4d3ffb56-dbef-4498-b288-1a19bb18e0b8)<br/>
   ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/daeaae2f-b476-44b5-bc11-0d112d45d572)<br/>
   ![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/06933c48-f3cc-4b12-8cbb-00a98dc05854)






