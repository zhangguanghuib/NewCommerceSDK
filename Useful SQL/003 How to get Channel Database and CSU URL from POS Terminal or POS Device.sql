--DECLARE @featureClassNamePattern NVARCHAR(100) = '%Dynamics.AX.Application.RetailDeliveryModeConsistencyFeature%';
--Dynamics.AX.Application.RetailUnifiedReturnUXImprovementFeature
DECLARE @featureClassNamePattern NVARCHAR(100) = 'Dynamics.AX.Application.RetailUnifiedReturnsFeature';
DECLARE @featureStateRecId BIGINT;

SELECT @featureStateRecId = recid FROM dbo.featuremanagementstate WHERE NAME LIKE @featureClassNamePattern

SELECT * FROM dbo.featuremanagementstate WHERE recid = @featureStateRecId
SELECT enabledate, modifieddatetime, modifiedby, * FROM dbo.featuremanagementmetadata WHERE  featurestate = @featureStateRecId

UPDATE [dbo].[featuremanagementmetadata]
SET    enabledate = '1900-01-01 00:00:00.000',
       modifieddatetime = Getdate(),
       modifiedby = 'Testing'
WHERE  featurestate = @featureStateRecId

UPDATE [dbo].featuremanagementstate
SET    isenabled = 0
WHERE  recid = @featureStateRecId

SELECT * FROM dbo.featuremanagementstate WHERE recid = @featureStateRecId
SELECT enabledate, modifieddatetime, modifiedby, * FROM dbo.featuremanagementmetadata WHERE  featurestate = @featureStateRecId



SELECT * FROM dbo.featuremanagementstate WHERE NAME LIKE 'Dynamics.AX.Application.RetailUnifiedReturnsFeature'
