/* Drop Foreign Key Constraints */

IF EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = object_id(N'[FK_b_g]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1) 
ALTER TABLE [books] DROP CONSTRAINT [FK_b_g]
GO

IF EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = object_id(N'[FK_m2mob_b]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1) 
ALTER TABLE [m2m_orders_books] DROP CONSTRAINT [FK_m2mob_b]
GO

IF EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = object_id(N'[FK_m2mob_o]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1) 
ALTER TABLE [m2m_orders_books] DROP CONSTRAINT [FK_m2mob_o]
GO

IF EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = object_id(N'[FK_m2m_uut_u]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1) 
ALTER TABLE [m2m_users_user_types] DROP CONSTRAINT [FK_m2m_uut_u]
GO

IF EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = object_id(N'[FK_m2muut_ut]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1) 
ALTER TABLE [m2m_users_user_types] DROP CONSTRAINT [FK_m2muut_ut]
GO

IF EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = object_id(N'[FK_o_os]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1) 
ALTER TABLE [orders] DROP CONSTRAINT [FK_o_os]
GO

IF EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = object_id(N'[FK_o_u]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1) 
ALTER TABLE [orders] DROP CONSTRAINT [FK_o_u]
GO

IF EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = object_id(N'[FK_rs_u]') AND OBJECTPROPERTY(id, N'IsForeignKey') = 1) 
ALTER TABLE [refresh_sessions] DROP CONSTRAINT [FK_rs_u]
GO

/* Drop Tables */

IF EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = object_id(N'[books]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1) 
DROP TABLE [books]
GO

IF EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = object_id(N'[genres]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1) 
DROP TABLE [genres]
GO

IF EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = object_id(N'[m2m_orders_books]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1) 
DROP TABLE [m2m_orders_books]
GO

IF EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = object_id(N'[m2m_users_user_types]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1) 
DROP TABLE [m2m_users_user_types]
GO

IF EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = object_id(N'[order_statuses]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1) 
DROP TABLE [order_statuses]
GO

IF EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = object_id(N'[orders]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1) 
DROP TABLE [orders]
GO

IF EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = object_id(N'[refresh_sessions]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1) 
DROP TABLE [refresh_sessions]
GO

IF EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = object_id(N'[user_types]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1) 
DROP TABLE [user_types]
GO

IF EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = object_id(N'[users]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1) 
DROP TABLE [users]
GO

/* Create Tables */

CREATE TABLE [books]
(
	[b_isbn] nvarchar(20) NOT NULL,
	[b_genre] int NOT NULL,
	[b_title] nvarchar(150) NOT NULL,
	[b_author] nvarchar(150) NULL,
	[b_publish_year] int NOT NULL,
	[b_quantity] int NOT NULL,
	[b_price] money NULL,
	[b_cover_file] nvarchar(150) NOT NULL
)
GO

CREATE TABLE [genres]
(
	[g_id] int NOT NULL IDENTITY(1,1),
	[g_name] nvarchar(50) NOT NULL
)
GO

CREATE TABLE [m2m_orders_books]
(
	[m2mob_o_id] int NOT NULL,
	[m2mob_b_isbn] nvarchar(20) NOT NULL,
	[m2mob_price] money NULL
)
GO

CREATE TABLE [m2m_users_user_types]
(
	[m2muut_u_id] int NOT NULL,
	[m2muut_ut_id] int NOT NULL
)
GO

CREATE TABLE [order_statuses]
(
	[os_id] int NOT NULL,
	[os_name] nvarchar(50) NOT NULL
)
GO

CREATE TABLE [orders]
(
	[o_id] int NOT NULL IDENTITY(1,1),
	[o_status] int NOT NULL,
	[o_creator] int NOT NULL,
	[o_total_price] money NOT NULL,
	[o_creation_dt] smalldatetime NOT NULL,
	[o_completion_dt] smalldatetime NULL
)
GO

CREATE TABLE [refresh_sessions]
(
	[rs_id] bigint NOT NULL,
	[rs_user_id] int NOT NULL,
	[rs_refresh_token] uniqueidentifier NOT NULL,
	[rs_fingerprint] nvarchar(150) NOT NULL,
	[rs_expires_in] datetime2(7) NOT NULL,
	[rs_created_at] datetime2(7) NOT NULL
)
GO

CREATE TABLE [user_types]
(
	[ut_id] int NOT NULL,
	[ut_name] nvarchar(50) NOT NULL
)
GO

CREATE TABLE [users]
(
	[u_id] int NOT NULL IDENTITY(1,1),
	[u_email] nvarchar(150) NOT NULL,
	[u_password] nchar(128) NOT NULL,
	[u_last_name] nvarchar(50) NULL,
	[u_first_name] nvarchar(50) NOT NULL,
	[u_middle_name] nvarchar(50) NULL,
	[u_phone] nvarchar(16) NOT NULL,
	[u_register_dt] smalldatetime NOT NULL,
	[u_profile_file] nvarchar(150) NULL
)
GO

/* Create Primary Keys, Indexes, Uniques, Checks */

ALTER TABLE [books] 
 ADD CONSTRAINT [PK_b]
	PRIMARY KEY CLUSTERED ([b_isbn] ASC)
GO

CREATE NONCLUSTERED INDEX [IXFK_b_g] 
 ON [books] ([b_genre] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_b_title] 
 ON [books] ([b_title] ASC)
GO

ALTER TABLE [genres] 
 ADD CONSTRAINT [PK_g]
	PRIMARY KEY CLUSTERED ([g_id] ASC)
GO

ALTER TABLE [genres] 
 ADD CONSTRAINT [UNQ_g_name] UNIQUE NONCLUSTERED ([g_name] ASC)
GO

ALTER TABLE [m2m_orders_books] 
 ADD CONSTRAINT [PK_m2mob]
	PRIMARY KEY CLUSTERED ([m2mob_o_id] ASC,[m2mob_b_isbn] ASC)
GO

CREATE NONCLUSTERED INDEX [IXFK_m2mob_b] 
 ON [m2m_orders_books] ([m2mob_b_isbn] ASC)
GO

CREATE NONCLUSTERED INDEX [IXFK_m2mob_o] 
 ON [m2m_orders_books] ([m2mob_o_id] ASC)
GO

ALTER TABLE [m2m_users_user_types] 
 ADD CONSTRAINT [PK_m2muut]
	PRIMARY KEY CLUSTERED ([m2muut_u_id] ASC,[m2muut_ut_id] ASC)
GO

CREATE NONCLUSTERED INDEX [IXFK_m2muut_u] 
 ON [m2m_users_user_types] ([m2muut_u_id] ASC)
GO

CREATE NONCLUSTERED INDEX [IXFK_m2muut_ut] 
 ON [m2m_users_user_types] ([m2muut_ut_id] ASC)
GO

ALTER TABLE [order_statuses] 
 ADD CONSTRAINT [PK_os]
	PRIMARY KEY CLUSTERED ([os_id] ASC)
GO

ALTER TABLE [order_statuses] 
 ADD CONSTRAINT [UNQ_os_name] UNIQUE NONCLUSTERED ([os_name] ASC)
GO

ALTER TABLE [orders] 
 ADD CONSTRAINT [PK_o]
	PRIMARY KEY CLUSTERED ([o_id] ASC)
GO

CREATE NONCLUSTERED INDEX [IXFK_o_os] 
 ON [orders] ([o_status] ASC)
GO

CREATE NONCLUSTERED INDEX [IXFK_o_u] 
 ON [orders] ([o_creator] ASC)
GO

ALTER TABLE [refresh_sessions] 
 ADD CONSTRAINT [PK_rs]
	PRIMARY KEY CLUSTERED ([rs_id] ASC)
GO

CREATE NONCLUSTERED INDEX [IXFK_rs_u] 
 ON [refresh_sessions] ([rs_user_id] ASC)
GO

ALTER TABLE [user_types] 
 ADD CONSTRAINT [PK_ut]
	PRIMARY KEY CLUSTERED ([ut_id] ASC)
GO

ALTER TABLE [user_types] 
 ADD CONSTRAINT [UNQ_ut_name] UNIQUE NONCLUSTERED ([ut_name] ASC)
GO

ALTER TABLE [users] 
 ADD CONSTRAINT [PK_u]
	PRIMARY KEY CLUSTERED ([u_id] ASC)
GO

ALTER TABLE [users] 
 ADD CONSTRAINT [UNQ_u_email] UNIQUE NONCLUSTERED ([u_email] ASC)
GO

/* Create Foreign Key Constraints */

ALTER TABLE [books] ADD CONSTRAINT [FK_b_g]
	FOREIGN KEY ([b_genre]) REFERENCES [genres] ([g_id]) ON DELETE No Action ON UPDATE No Action
GO

ALTER TABLE [m2m_orders_books] ADD CONSTRAINT [FK_m2mob_b]
	FOREIGN KEY ([m2mob_b_isbn]) REFERENCES [books] ([b_isbn]) ON DELETE No Action ON UPDATE No Action
GO

ALTER TABLE [m2m_orders_books] ADD CONSTRAINT [FK_m2mob_o]
	FOREIGN KEY ([m2mob_o_id]) REFERENCES [orders] ([o_id]) ON DELETE No Action ON UPDATE No Action
GO

ALTER TABLE [m2m_users_user_types] ADD CONSTRAINT [FK_m2m_uut_u]
	FOREIGN KEY ([m2muut_u_id]) REFERENCES [users] ([u_id]) ON DELETE No Action ON UPDATE No Action
GO

ALTER TABLE [m2m_users_user_types] ADD CONSTRAINT [FK_m2muut_ut]
	FOREIGN KEY ([m2muut_ut_id]) REFERENCES [user_types] ([ut_id]) ON DELETE No Action ON UPDATE No Action
GO

ALTER TABLE [orders] ADD CONSTRAINT [FK_o_os]
	FOREIGN KEY ([o_status]) REFERENCES [order_statuses] ([os_id]) ON DELETE No Action ON UPDATE No Action
GO

ALTER TABLE [orders] ADD CONSTRAINT [FK_o_u]
	FOREIGN KEY ([o_creator]) REFERENCES [users] ([u_id]) ON DELETE No Action ON UPDATE No Action
GO

ALTER TABLE [refresh_sessions] ADD CONSTRAINT [FK_rs_u]
	FOREIGN KEY ([rs_user_id]) REFERENCES [users] ([u_id]) ON DELETE No Action ON UPDATE No Action
GO