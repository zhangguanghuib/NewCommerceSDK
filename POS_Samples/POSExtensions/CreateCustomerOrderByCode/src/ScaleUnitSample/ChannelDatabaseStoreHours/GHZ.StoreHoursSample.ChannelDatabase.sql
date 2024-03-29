IF (SELECT OBJECT_ID('[ext].[CONTOSORETAILSTOREHOURSTABLE]')) IS NULL 
BEGIN
    CREATE TABLE [ext].[CONTOSORETAILSTOREHOURSTABLE](
        [RECID] [bigint]  IDENTITY(1, 1) NOT NULL,
        [DAY] [int] NOT NULL DEFAULT ((0)),
        [OPENTIME] [int] NOT NULL DEFAULT ((0)),
        [CLOSINGTIME] [int] NOT NULL DEFAULT ((0)),
        [RETAILSTORETABLE] [bigint] NOT NULL DEFAULT ((0)),
    CONSTRAINT [I_CONTOSORETAILSTOREHOURSTABLE_RECID] PRIMARY KEY CLUSTERED 
    (
        [RECID] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]

    ALTER TABLE [ext].[CONTOSORETAILSTOREHOURSTABLE]  WITH CHECK ADD CHECK  (([RECID]<>(0)))
END
GO

GRANT SELECT, INSERT, UPDATE, DELETE ON OBJECT::[ext].[CONTOSORETAILSTOREHOURSTABLE] TO [DataSyncUsersRole]
GO

-- Create the custom extension view that is accessed by CRT to query the custom fields.

IF (SELECT OBJECT_ID('[ext].[CONTOSORETAILSTOREHOURSVIEW]')) IS NOT NULL
    DROP VIEW [ext].[CONTOSORETAILSTOREHOURSVIEW]
GO

CREATE VIEW [ext].[CONTOSORETAILSTOREHOURSVIEW] AS
(
    SELECT
        sdht.DAY, 
        sdht.OPENTIME,
        sdht.CLOSINGTIME,
        sdht.RECID,
        rst.STORENUMBER
    FROM [ext].[CONTOSORETAILSTOREHOURSTABLE] sdht
    INNER JOIN [ax].RETAILSTORETABLE rst ON rst.RECID = sdht.RETAILSTORETABLE
)
GO

GRANT SELECT ON OBJECT::[ext].[CONTOSORETAILSTOREHOURSVIEW] TO [UsersRole];
GO

GRANT SELECT ON OBJECT::[ext].[CONTOSORETAILSTOREHOURSVIEW] TO [DeployExtensibilityRole];
GO

-- Create the custom extension stored procedure that is accessed by CRT to update the custom table.

IF OBJECT_ID(N'[ext].[UPDATESTOREDAYHOURS]', N'P') IS NOT NULL
    DROP PROCEDURE [ext].[UPDATESTOREDAYHOURS]
GO

CREATE PROCEDURE [ext].[UPDATESTOREDAYHOURS]
    @bi_Id          BIGINT,
    @i_Day          INT,
    @i_OpenTime     INT,
    @i_ClosingTime  INT
AS
BEGIN
    SET NOCOUNT ON

    UPDATE ext.CONTOSORETAILSTOREHOURSTABLE SET
        DAY = @i_Day,
        OPENTIME = @i_OpenTime,
        CLOSINGTIME = @i_ClosingTime
    WHERE
        RECID = @bi_Id
END;
GO

GRANT EXECUTE ON [ext].[UPDATESTOREDAYHOURS] TO [UsersRole];
GO

GRANT EXECUTE ON [ext].[UPDATESTOREDAYHOURS] TO [DeployExtensibilityRole];
GO
