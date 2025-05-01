# 1. SQL Script to disable the feature "Pricing Management"<br/>
```sql
select T.RecId, T.DisplayName, T.enabledate, T.FeatureState, * from dbo.FEATUREMANAGEMENTMETADATA as T where T.displayname like '%Pricing management%'

select * from dbo.FeatureManagementState where RecId in ( 5637178354,5637178358,5637181026);

DECLARE @featureStateRecId BIGINT = 5637188827;

UPDATE [dbo].[featuremanagementmetadata]
SET enabledate = '1900-01-01 00:00:00.000',
    modifieddatetime = Getdate(),
    modifiedby = 'Testing'
WHERE featurestate = @featureStateRecId;

UPDATE [dbo].[featuremanagementstate]
SET isenabled = 0
WHERE recid = @featureStateRecId;

SELECT * FROM dbo.featuremanagementstate WHERE recid = @featureStateRecId;

SELECT enabledate, modifieddatetime, modifiedby, * FROM dbo.featuremanagementmetadata WHERE featurestate = @featureStateRecId;
```
# 2. You should check if the feature is really disabled:<br/>
![image](https://github.com/user-attachments/assets/2f5bff87-edc3-4bf6-938e-6ac96c835304)








  
