## This feature is going to provide support display order on tender types for safe drop or bank drop, the current behavior on POS has limitation because tender table can only be sorted by RecId,  this is not friendly for customer.
### 1. How it works
#### 1)In HQ side, set the dislay order for store tender types
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/58fe220f-9291-4752-bc63-9866965ecbb0)
#### 2)Run this download distribution job and make download session applied
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/872c1732-e947-4a49-bdf1-dc43dcc7c489)
#### 3)On POS, go to safe drop or bank drop, you will find the tender type display order will be adjusted
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/efe09b42-5ff5-4739-8609-0de3804ced24)
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/54c232ab-336e-4431-9544-d235508c7033)
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/f823f09e-9c91-428b-9720-503bac3547b6)

### 2. How it gets implemented
#### In HQ  side:
##### 1) Create a EDT based in Real type:
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/d16f9d57-4c39-403f-8def-2be5d4d49e21)
##### 2) Add the EDT to the table extension of RetailStoreTenderType
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/712f9264-ac42-4e94-9920-ed3b86099504)
##### 3) Add this field on the form
![image](https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/82b729a8-5e9c-4875-a026-301717ab3a39)

#### CDX side:
#### 1) Create all the field for the new table:
<img width="1300" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/58ba5f5e-5424-4270-b98d-2faeaf5c8f2e">
Important:
In case you see the SysPick Dialog in the right,  just skip that, you need manually input the field name and save it.<br/>

##### 2) Create scheduler subjob:<br/>
<img width="1103" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/2aad63ac-4c21-4419-830c-7da91e8b67c9"><br/>
####  3) Create scheduler job:<br/>
<img width="878" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/42c4a9b4-e838-41d6-8c51-6ccc7942ee74"><br/>
####  4) Create distribution scheduler:<br/>
<img width="827" alt="image" src="https://github.com/zhangguanghuib/NewCommerceSDK/assets/14832260/52cca225-db0b-43d9-9bc0-9967840b070c"><br/>

#### Retail server side:
##### 1) Create extension table to store dispay order
```sql
create TABLE [ext].[RETAILSTORETENDERTYPETABLE](
	[CHANNEL] [bigint] NOT NULL,
	[TENDERTYPEID] [nvarchar](10) NOT NULL,
	[DATAAREAID] [nvarchar](4) NOT NULL,
	[DISPLAYORDER] [numeric](32, 6) NOT NULL default 0,
 CONSTRAINT [I_-1697922944_-828827534] PRIMARY KEY CLUSTERED 
(
	[CHANNEL] ASC,
	[TENDERTYPEID] ASC,
	[DATAAREAID] ASC
)
```
#### 2) Create the [ext].[CHANNELTENDERTYPEVIEW], this comes the OOB view [crt].[CHANNELTENDERTYPEVIEW], but add table join with [ext].RETAILSTORETENDERTYPETABLE so that it support tender type display order.
```sql
....
rsttt.USEFORDECLARESTARTAMOUNT,
rstttext.DISPLAYORDER as DISPLAYORDER
FROM [ax].RETAILSTORETENDERTYPETABLE rsttt
    INNER JOIN [ext].RETAILSTORETENDERTYPETABLE rstttext
	ON rsttt.DATAAREAID = rstttext.DATAAREAID
	AND rsttt.CHANNEL = rstttext.CHANNEL
	AND rsttt.TENDERTYPEID = rstttext.TENDERTYPEID
INNER JOIN [ax].RETAILCHANNELTABLE rct
...
```
#### 3）Overide GetChannelTenderTypesDataRequest to make it order by OperationId and then Display Order:
```csharp
query.From = ChannelTenderTypeViewName;
query.DatabaseSchema = "ext";
query.Where = whereClause;
query.IsQueryByPrimaryKey = false;
// query.OrderBy = RecIdColumn;

SortColumn sortColumnOperationId = new SortColumn("OPERATIONID", false);
SortColumn sortColumnDisplayOrder = new SortColumn("DISPLAYORDER", false);
query.OrderBy = new SortingInfo(sortColumnOperationId, sortColumnDisplayOrder).ToString();

using (DatabaseContext databaseContext = new DatabaseContext(request.RequestContext, DatabaseConnectionMode.IsReadOnly | ReadFromReplicaIfEnabled(request.RequestContext)))
{
    result = await databaseContext.ReadEntityAsync<TenderType>(query).ConfigureAwait(false);
}

```

