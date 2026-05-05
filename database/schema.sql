-- IntelliFlow — Azure SQL Database Schema
-- Managed by: Hassan Asif (Module 3) via Entity Framework Core migrations
-- Run this manually only for initial setup reference. EF Core handles actual migrations.

CREATE TABLE Tasks (
    Id            UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Topic         NVARCHAR(500)    NOT NULL,
    Status        NVARCHAR(50)     NOT NULL DEFAULT 'Pending',
    -- Status values: Pending | Researching | Summarizing | Reporting | Notifying | Completed | Failed
    RequestedBy   NVARCHAR(256)    NOT NULL,   -- email of requesting user
    CreatedAt     DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt     DATETIME2        NOT NULL DEFAULT GETUTCDATE()
);

CREATE TABLE AgentLogs (
    Id            UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TaskId        UNIQUEIDENTIFIER NOT NULL REFERENCES Tasks(Id),
    AgentName     NVARCHAR(100)    NOT NULL,   -- Research | Summarizer | Reporter | Notifier
    Status        NVARCHAR(50)     NOT NULL,   -- Started | Completed | Failed
    Message       NVARCHAR(MAX)    NULL,
    ExecutedAt    DATETIME2        NOT NULL DEFAULT GETUTCDATE()
);

CREATE TABLE Reports (
    Id            UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TaskId        UNIQUEIDENTIFIER NOT NULL REFERENCES Tasks(Id) UNIQUE,
    BlobUrl       NVARCHAR(2000)   NOT NULL,   -- Azure Blob Storage download URL
    OutputHash    NVARCHAR(64)     NOT NULL,   -- SHA-256 hex string (for blockchain)
    BlockchainTxHash NVARCHAR(66)  NULL,       -- Sepolia transaction hash (0x...)
    CreatedAt     DATETIME2        NOT NULL DEFAULT GETUTCDATE()
);

-- Index for common query patterns
CREATE INDEX IX_AgentLogs_TaskId ON AgentLogs(TaskId);
CREATE INDEX IX_Tasks_Status     ON Tasks(Status);
