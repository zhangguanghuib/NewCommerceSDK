--------------------------------------------------------------------------------------------------------------------
---------------------------------------CONTOSORETAILSTAFFSUGGESTIONS -----------------------------------------------
-------------------------------------------------------------------------------------------------------------------- 
IF (SELECT OBJECT_ID('[ext].[CONTOSORETAILSTAFFSUGGESTIONS]')) IS NOT NULL  
BEGIN
    DROP TABLE [EXT].[CONTOSORETAILSTAFFSUGGESTIONS]
END

CREATE TABLE [ext].CONTOSORETAILSTAFFSUGGESTIONS(
	[SUGGESTIONID] [int] NOT NULL,
	[STOREID] [nvarchar](10) NOT NULL,
	[STAFF] [nvarchar](25) NOT NULL,
	[TERMINALID] [nvarchar](10) NOT NULL,
	[SUGGETION] [nvarchar](255) NOT NULL,
	[DATAAREAID] [nvarchar](4) NOT NULL,
	[DATELOGGED] [date] NOT NULL,
	[ROWVERSION] [timestamp] NOT NULL,
	[REPLICATIONCOUNTERFROMORIGIN] [int] IDENTITY(1,1) NOT NULL,
	 CONSTRAINT [PK_EXT_CONTOSORETAILSTAFFSUGGESTIONS] PRIMARY KEY CLUSTERED 
(
	[SUGGESTIONID] ASC,
	[STOREID] ASC,
	[TERMINALID] ASC,
	[DATAAREAID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

GRANT INSERT, DELETE, UPDATE, SELECT ON OBJECT::[ext].[CONTOSORETAILSTAFFSUGGESTIONS] TO [DataSyncUsersRole];
GO

