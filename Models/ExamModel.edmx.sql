
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 12/13/2019 12:01:03
-- Generated from EDMX file: C:\Users\NCTCPW10JYT\source\repos\OnlineExam\Models\ExamModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Exam1];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_AdminQuesCategory]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[QuesCategories] DROP CONSTRAINT [FK_AdminQuesCategory];
GO
IF OBJECT_ID(N'[dbo].[FK_QuesCategoryQuestion]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Questions] DROP CONSTRAINT [FK_QuesCategoryQuestion];
GO
IF OBJECT_ID(N'[dbo].[FK_StudentSetExam]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Reports] DROP CONSTRAINT [FK_StudentSetExam];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Admins]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Admins];
GO
IF OBJECT_ID(N'[dbo].[Exam]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Exam];
GO
IF OBJECT_ID(N'[dbo].[Profile]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Profile];
GO
IF OBJECT_ID(N'[dbo].[QuesCategories]', 'U') IS NOT NULL
    DROP TABLE [dbo].[QuesCategories];
GO
IF OBJECT_ID(N'[dbo].[Questions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Questions];
GO
IF OBJECT_ID(N'[dbo].[Reports]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Reports];
GO
IF OBJECT_ID(N'[dbo].[Students]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Students];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Admins'
CREATE TABLE [dbo].[Admins] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Password] nvarchar(50)  NOT NULL,
    [Email] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'Questions'
CREATE TABLE [dbo].[Questions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Text] nvarchar(max)  NOT NULL,
    [OPA] nvarchar(max)  NOT NULL,
    [OPB] nvarchar(max)  NOT NULL,
    [OPC] nvarchar(max)  NOT NULL,
    [OPD] nvarchar(max)  NOT NULL,
    [COP] nvarchar(max)  NOT NULL,
    [QuesCategoryId] int  NOT NULL
);
GO

-- Creating table 'QuesCategories'
CREATE TABLE [dbo].[QuesCategories] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [AdminId] int  NOT NULL,
    [ExamQuestion] int  NULL,
    [TotalQuestion] int  NULL
);
GO

-- Creating table 'Reports'
CREATE TABLE [dbo].[Reports] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Date] datetime  NOT NULL,
    [ExamName] nvarchar(50)  NOT NULL,
    [Score] int  NOT NULL,
    [StudentId] int  NOT NULL,
    [StudentName] nvarchar(50)  NOT NULL,
    [Email] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'Students'
CREATE TABLE [dbo].[Students] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FirstName] nvarchar(50)  NOT NULL,
    [LastName] nvarchar(50)  NOT NULL,
    [PhoneNumber] int  NOT NULL,
    [Email] nvarchar(50)  NOT NULL,
    [Password] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'Exams'
CREATE TABLE [dbo].[Exams] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Category] nvarchar(50)  NOT NULL,
    [NoOfQues] int  NOT NULL,
    [CheckBox] bit  NOT NULL,
    [ProfileId] int  NOT NULL
);
GO

-- Creating table 'Profiles'
CREATE TABLE [dbo].[Profiles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [Status] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Admins'
ALTER TABLE [dbo].[Admins]
ADD CONSTRAINT [PK_Admins]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Questions'
ALTER TABLE [dbo].[Questions]
ADD CONSTRAINT [PK_Questions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'QuesCategories'
ALTER TABLE [dbo].[QuesCategories]
ADD CONSTRAINT [PK_QuesCategories]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Reports'
ALTER TABLE [dbo].[Reports]
ADD CONSTRAINT [PK_Reports]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Students'
ALTER TABLE [dbo].[Students]
ADD CONSTRAINT [PK_Students]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Exams'
ALTER TABLE [dbo].[Exams]
ADD CONSTRAINT [PK_Exams]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Profiles'
ALTER TABLE [dbo].[Profiles]
ADD CONSTRAINT [PK_Profiles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [QuesCategoryId] in table 'Questions'
ALTER TABLE [dbo].[Questions]
ADD CONSTRAINT [FK_QuesCategoryQuestion]
    FOREIGN KEY ([QuesCategoryId])
    REFERENCES [dbo].[QuesCategories]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_QuesCategoryQuestion'
CREATE INDEX [IX_FK_QuesCategoryQuestion]
ON [dbo].[Questions]
    ([QuesCategoryId]);
GO

-- Creating foreign key on [AdminId] in table 'QuesCategories'
ALTER TABLE [dbo].[QuesCategories]
ADD CONSTRAINT [FK_AdminQuesCategory]
    FOREIGN KEY ([AdminId])
    REFERENCES [dbo].[Admins]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AdminQuesCategory'
CREATE INDEX [IX_FK_AdminQuesCategory]
ON [dbo].[QuesCategories]
    ([AdminId]);
GO

-- Creating foreign key on [StudentId] in table 'Reports'
ALTER TABLE [dbo].[Reports]
ADD CONSTRAINT [FK_StudentSetExam]
    FOREIGN KEY ([StudentId])
    REFERENCES [dbo].[Students]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_StudentSetExam'
CREATE INDEX [IX_FK_StudentSetExam]
ON [dbo].[Reports]
    ([StudentId]);
GO

-- Creating foreign key on [ProfileId] in table 'Exams'
ALTER TABLE [dbo].[Exams]
ADD CONSTRAINT [FK_ProfileExam]
    FOREIGN KEY ([ProfileId])
    REFERENCES [dbo].[Profiles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProfileExam'
CREATE INDEX [IX_FK_ProfileExam]
ON [dbo].[Exams]
    ([ProfileId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------