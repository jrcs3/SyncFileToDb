USE [InsecureDB]
GO

/****** Object:  Table [dbo].[EmployeeIdAndSsn]    Script Date: 12/8/2023 7:34:55 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EmployeeIdAndSsn](
	[EmployeeId] [nvarchar](11) NOT NULL,
	[ssn] [nvarchar](9) NULL
) ON [PRIMARY]
GO


