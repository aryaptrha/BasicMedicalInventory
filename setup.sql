-- CREATE DATABASE
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'MedicalInventoryDb')
BEGIN
    CREATE DATABASE MedicalInventoryDb;
END
GO

USE MedicalInventoryDb;
GO

-- DROP TABLES IF THEY EXIST (FOR FRESH SETUP)
IF OBJECT_ID('ApprovalLogs', 'U') IS NOT NULL DROP TABLE ApprovalLogs;
IF OBJECT_ID('RequestDetails', 'U') IS NOT NULL DROP TABLE RequestDetails;
IF OBJECT_ID('Requests', 'U') IS NOT NULL DROP TABLE Requests;
IF OBJECT_ID('Medicines', 'U') IS NOT NULL DROP TABLE Medicines;
IF OBJECT_ID('Users', 'U') IS NOT NULL DROP TABLE Users;
IF OBJECT_ID('Roles', 'U') IS NOT NULL DROP TABLE Roles;
GO

-- CREATE TABLES
CREATE TABLE Roles (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    RoleName VARCHAR(100) NOT NULL
);

CREATE TABLE Users (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    FullName VARCHAR(150) NOT NULL,
    Email VARCHAR(200) NOT NULL UNIQUE,
    PasswordHash VARCHAR(500) NOT NULL,
    RoleId BIGINT NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY(RoleId) REFERENCES Roles(Id)
);

CREATE TABLE Medicines (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    MedicineName VARCHAR(200) NOT NULL,
    StockQty INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    ExpiredDate DATE NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

CREATE TABLE Requests (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    RequestNumber VARCHAR(50) NOT NULL,
    UserId BIGINT NOT NULL,
    Status VARCHAR(50) NOT NULL,
    RequestDate DATETIME DEFAULT GETDATE(),
    AdminApprovedAt DATETIME NULL,
    DistributionApprovedAt DATETIME NULL,
    DeliveredAt DATETIME NULL,
    FOREIGN KEY(UserId) REFERENCES Users(Id)
);

CREATE TABLE RequestDetails (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    RequestId BIGINT NOT NULL,
    MedicineId BIGINT NOT NULL,
    Qty INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    FOREIGN KEY(RequestId) REFERENCES Requests(Id),
    FOREIGN KEY(MedicineId) REFERENCES Medicines(Id)
);

CREATE TABLE ApprovalLogs (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    RequestId BIGINT NOT NULL,
    ActionBy BIGINT NOT NULL,
    ActionType VARCHAR(50) NOT NULL,
    Remarks VARCHAR(500),
    ActionDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY(RequestId) REFERENCES Requests(Id),
    FOREIGN KEY(ActionBy) REFERENCES Users(Id)
);
GO

-- SEED ROLES
INSERT INTO Roles (RoleName) VALUES ('Admin');
INSERT INTO Roles (RoleName) VALUES ('User Distribution');
INSERT INTO Roles (RoleName) VALUES ('External User');
GO

-- SEED USERS (Password for all: password123)
-- Hash calculation: SHA256 of 'password123'
INSERT INTO Users (FullName, Email, PasswordHash, RoleId, IsActive, CreatedAt)
VALUES ('Ahmad Admin', 'admin@abc.com', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 1, 1, GETDATE());

INSERT INTO Users (FullName, Email, PasswordHash, RoleId, IsActive, CreatedAt)
VALUES ('Dian Distribusi', 'dist@abc.com', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 2, 1, GETDATE());

INSERT INTO Users (FullName, Email, PasswordHash, RoleId, IsActive, CreatedAt)
VALUES ('Eko External', 'external@gmail.com', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 3, 1, GETDATE());
GO

-- SEED MEDICINES
-- Simulated current date: 2026-06-16
INSERT INTO Medicines (MedicineName, StockQty, Price, ExpiredDate, CreatedAt)
VALUES ('Paracetamol 500mg', 120, 15000.00, '2026-12-10', DATEADD(day, -30, GETDATE()));

INSERT INTO Medicines (MedicineName, StockQty, Price, ExpiredDate, CreatedAt)
VALUES ('Amoxicillin 250mg', 5, 32000.00, '2026-06-20', DATEADD(day, -20, GETDATE())); -- Low & Near Expired

INSERT INTO Medicines (MedicineName, StockQty, Price, ExpiredDate, CreatedAt)
VALUES ('Ibuprofen 400mg', 8, 25000.00, '2026-08-15', DATEADD(day, -15, GETDATE())); -- Low & Safe

INSERT INTO Medicines (MedicineName, StockQty, Price, ExpiredDate, CreatedAt)
VALUES ('Cetirizine 10mg', 150, 18000.00, '2026-06-18', DATEADD(day, -10, GETDATE())); -- Normal & Near Expired

INSERT INTO Medicines (MedicineName, StockQty, Price, ExpiredDate, CreatedAt)
VALUES ('Metformin 500mg', 0, 22000.00, '2027-02-28', DATEADD(day, -40, GETDATE())); -- Out of stock & Safe

INSERT INTO Medicines (MedicineName, StockQty, Price, ExpiredDate, CreatedAt)
VALUES ('Amlodipine 5mg', 210, 10000.00, '2026-06-22', DATEADD(day, -12, GETDATE())); -- Normal & Near Expired

INSERT INTO Medicines (MedicineName, StockQty, Price, ExpiredDate, CreatedAt)
VALUES ('Vitamin C 1000mg', 85, 12000.00, '2026-10-01', DATEADD(day, -5, GETDATE()));
GO

-- SEED REQUESTS
INSERT INTO Requests (RequestNumber, UserId, Status, RequestDate, AdminApprovedAt, DistributionApprovedAt, DeliveredAt)
VALUES ('REQ-20260610-001', 3, 'Delivered', DATEADD(day, -6, GETDATE()), DATEADD(day, -5, GETDATE()), DATEADD(day, -5, GETDATE()), DATEADD(day, -4, GETDATE()));

INSERT INTO Requests (RequestNumber, UserId, Status, RequestDate, AdminApprovedAt)
VALUES ('REQ-20260614-001', 3, 'Approved By Admin', DATEADD(day, -2, GETDATE()), DATEADD(day, -1, GETDATE()));

INSERT INTO Requests (RequestNumber, UserId, Status, RequestDate)
VALUES ('REQ-20260615-001', 3, 'Submitted', DATEADD(day, -1, GETDATE()));
GO

-- SEED REQUEST DETAILS
-- Request 1 Details
INSERT INTO RequestDetails (RequestId, MedicineId, Qty, Price) VALUES (1, 1, 10, 15000.00);
INSERT INTO RequestDetails (RequestId, MedicineId, Qty, Price) VALUES (1, 3, 2, 25000.00);
-- Request 2 Details
INSERT INTO RequestDetails (RequestId, MedicineId, Qty, Price) VALUES (2, 1, 15, 15000.00);
INSERT INTO RequestDetails (RequestId, MedicineId, Qty, Price) VALUES (2, 7, 5, 12000.00);
-- Request 3 Details
INSERT INTO RequestDetails (RequestId, MedicineId, Qty, Price) VALUES (3, 3, 3, 25000.00);
GO

-- SEED APPROVAL LOGS
-- Request 1 logs
INSERT INTO ApprovalLogs (RequestId, ActionBy, ActionType, Remarks, ActionDate) 
VALUES (1, 3, 'Submit', 'Permintaan rutin bulanan', DATEADD(day, -6, GETDATE()));
INSERT INTO ApprovalLogs (RequestId, ActionBy, ActionType, Remarks, ActionDate) 
VALUES (1, 1, 'Admin Approved', 'Dokumen lengkap dan valid', DATEADD(day, -5, GETDATE()));
INSERT INTO ApprovalLogs (RequestId, ActionBy, ActionType, Remarks, ActionDate) 
VALUES (1, 2, 'Distribution Approved', 'Stok tersedia di gudang', DATEADD(day, -5, GETDATE()));
INSERT INTO ApprovalLogs (RequestId, ActionBy, ActionType, Remarks, ActionDate) 
VALUES (1, 2, 'Delivered', 'Paket dikirim kurir internal', DATEADD(day, -4, GETDATE()));

-- Request 2 logs
INSERT INTO ApprovalLogs (RequestId, ActionBy, ActionType, Remarks, ActionDate) 
VALUES (2, 3, 'Submit', 'Kebutuhan klinik satelit', DATEADD(day, -2, GETDATE()));
INSERT INTO ApprovalLogs (RequestId, ActionBy, ActionType, Remarks, ActionDate) 
VALUES (2, 1, 'Admin Approved', 'Approved', DATEADD(day, -1, GETDATE()));

-- Request 3 logs
INSERT INTO ApprovalLogs (RequestId, ActionBy, ActionType, Remarks, ActionDate) 
VALUES (3, 3, 'Submit', 'Stok ibuprofen menipis', DATEADD(day, -1, GETDATE()));
GO

-- ==========================================
-- STORED PROCEDURES & TYPE DEFINITIONS
-- ==========================================

-- Drop existing procedures if they exist
IF OBJECT_ID('sp_GetUserByEmail', 'P') IS NOT NULL DROP PROCEDURE sp_GetUserByEmail;
IF OBJECT_ID('sp_GetAllMedicines', 'P') IS NOT NULL DROP PROCEDURE sp_GetAllMedicines;
IF OBJECT_ID('sp_GetMedicineById', 'P') IS NOT NULL DROP PROCEDURE sp_GetMedicineById;
IF OBJECT_ID('sp_CreateMedicine', 'P') IS NOT NULL DROP PROCEDURE sp_CreateMedicine;
IF OBJECT_ID('sp_UpdateMedicine', 'P') IS NOT NULL DROP PROCEDURE sp_UpdateMedicine;
IF OBJECT_ID('sp_DeleteMedicine', 'P') IS NOT NULL DROP PROCEDURE sp_DeleteMedicine;
IF OBJECT_ID('sp_GetAllRequests', 'P') IS NOT NULL DROP PROCEDURE sp_GetAllRequests;
IF OBJECT_ID('sp_GetRequestDetailsByRequestId', 'P') IS NOT NULL DROP PROCEDURE sp_GetRequestDetailsByRequestId;
IF OBJECT_ID('sp_GetRequestById', 'P') IS NOT NULL DROP PROCEDURE sp_GetRequestById;
IF OBJECT_ID('sp_UpdateRequestStatus', 'P') IS NOT NULL DROP PROCEDURE sp_UpdateRequestStatus;
IF OBJECT_ID('sp_UpdateRequestStatusAndDeductStock', 'P') IS NOT NULL DROP PROCEDURE sp_UpdateRequestStatusAndDeductStock;
IF OBJECT_ID('sp_GetAllApprovalLogs', 'P') IS NOT NULL DROP PROCEDURE sp_GetAllApprovalLogs;
IF OBJECT_ID('sp_GetApprovalLogsByRequestId', 'P') IS NOT NULL DROP PROCEDURE sp_GetApprovalLogsByRequestId;
GO

-- Drop procedure dependent on RequestDetailType before dropping type
IF OBJECT_ID('sp_CreateRequest', 'P') IS NOT NULL DROP PROCEDURE sp_CreateRequest;
GO

-- Drop and recreate User-Defined Table Type
IF TYPE_ID('RequestDetailType') IS NOT NULL DROP TYPE RequestDetailType;
GO

CREATE TYPE RequestDetailType AS TABLE (
    MedicineId BIGINT,
    Qty INT,
    Price DECIMAL(18,2)
);
GO

-- 1. Authentication
CREATE PROCEDURE sp_GetUserByEmail
    @Email VARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT u.Id, u.FullName, u.Email, u.PasswordHash, u.RoleId, r.RoleName, u.IsActive, u.CreatedAt
    FROM Users u
    INNER JOIN Roles r ON u.RoleId = r.Id
    WHERE LOWER(u.Email) = LOWER(@Email) AND u.IsActive = 1;
END
GO

-- 2. Medicine CRUD
CREATE PROCEDURE sp_GetAllMedicines
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, MedicineName, StockQty, Price, ExpiredDate, CreatedAt, UpdatedAt 
    FROM Medicines 
    ORDER BY MedicineName;
END
GO

CREATE PROCEDURE sp_GetMedicineById
    @Id BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, MedicineName, StockQty, Price, ExpiredDate, CreatedAt, UpdatedAt 
    FROM Medicines 
    WHERE Id = @Id;
END
GO

CREATE PROCEDURE sp_CreateMedicine
    @MedicineName VARCHAR(200),
    @StockQty INT,
    @Price DECIMAL(18,2),
    @ExpiredDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Medicines (MedicineName, StockQty, Price, ExpiredDate, CreatedAt)
    VALUES (@MedicineName, @StockQty, @Price, @ExpiredDate, GETUTCDATE());
END
GO

CREATE PROCEDURE sp_UpdateMedicine
    @Id BIGINT,
    @MedicineName VARCHAR(200),
    @StockQty INT,
    @Price DECIMAL(18,2),
    @ExpiredDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Medicines
    SET MedicineName = @MedicineName, 
        StockQty = @StockQty, 
        Price = @Price, 
        ExpiredDate = @ExpiredDate, 
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
END
GO

CREATE PROCEDURE sp_DeleteMedicine
    @Id BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Medicines WHERE Id = @Id;
END
GO

-- 3. Request Operations
CREATE PROCEDURE sp_GetAllRequests
AS
BEGIN
    SET NOCOUNT ON;
    SELECT r.Id, r.RequestNumber, r.UserId, u.FullName AS UserFullName, r.Status, r.RequestDate,
           r.AdminApprovedAt, r.DistributionApprovedAt, r.DeliveredAt
    FROM Requests r
    INNER JOIN Users u ON r.UserId = u.Id
    ORDER BY r.RequestDate DESC;
END
GO

CREATE PROCEDURE sp_GetRequestDetailsByRequestId
    @RequestId BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT d.Id, d.RequestId, d.MedicineId, m.MedicineName, d.Qty, d.Price
    FROM RequestDetails d
    INNER JOIN Medicines m ON d.MedicineId = m.Id
    WHERE d.RequestId = @RequestId;
END
GO

CREATE PROCEDURE sp_GetRequestById
    @Id BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT r.Id, r.RequestNumber, r.UserId, u.FullName AS UserFullName, r.Status, r.RequestDate,
           r.AdminApprovedAt, r.DistributionApprovedAt, r.DeliveredAt
    FROM Requests r
    INNER JOIN Users u ON r.UserId = u.Id
    WHERE r.Id = @Id;
END
GO

CREATE PROCEDURE sp_CreateRequest
    @RequestNumber VARCHAR(50),
    @UserId BIGINT,
    @Status VARCHAR(50),
    @ActionBy BIGINT,
    @ActionType VARCHAR(50),
    @Remarks VARCHAR(500),
    @Details RequestDetailType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @RequestId BIGINT;
        
        -- Insert Request Header
        INSERT INTO Requests (RequestNumber, UserId, Status, RequestDate)
        VALUES (@RequestNumber, @UserId, @Status, GETUTCDATE());
        
        SET @RequestId = SCOPE_IDENTITY();
        
        -- Insert Request Details
        INSERT INTO RequestDetails (RequestId, MedicineId, Qty, Price)
        SELECT @RequestId, MedicineId, Qty, Price
        FROM @Details;
        
        -- Insert Initial Log
        INSERT INTO ApprovalLogs (RequestId, ActionBy, ActionType, Remarks, ActionDate)
        VALUES (@RequestId, @ActionBy, @ActionType, @Remarks, GETUTCDATE());
        
        COMMIT TRANSACTION;
        
        SELECT @RequestId AS RequestId;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

CREATE PROCEDURE sp_UpdateRequestStatus
    @RequestId BIGINT,
    @Status VARCHAR(50),
    @AdminApprovedAt DATETIME,
    @DistributionApprovedAt DATETIME,
    @DeliveredAt DATETIME,
    @ActionBy BIGINT,
    @ActionType VARCHAR(50),
    @Remarks VARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        -- Update Status & Approvals
        UPDATE Requests
        SET Status = @Status,
            AdminApprovedAt = COALESCE(@AdminApprovedAt, AdminApprovedAt),
            DistributionApprovedAt = COALESCE(@DistributionApprovedAt, DistributionApprovedAt),
            DeliveredAt = COALESCE(@DeliveredAt, DeliveredAt)
        WHERE Id = @RequestId;

        -- Log Action
        INSERT INTO ApprovalLogs (RequestId, ActionBy, ActionType, Remarks, ActionDate)
        VALUES (@RequestId, @ActionBy, @ActionType, @Remarks, GETUTCDATE());

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

CREATE PROCEDURE sp_UpdateRequestStatusAndDeductStock
    @RequestId BIGINT,
    @Status VARCHAR(50),
    @DistributionApprovedAt DATETIME,
    @ActionBy BIGINT,
    @ActionType VARCHAR(50),
    @Remarks VARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        -- 1. Validate stocks beforehand
        DECLARE @FailedMedicineId BIGINT;
        DECLARE @FailedMedicineName VARCHAR(200);
        DECLARE @StockQty INT;
        DECLARE @RequestedQty INT;

        SELECT TOP 1 
            @FailedMedicineId = rd.MedicineId,
            @FailedMedicineName = COALESCE(m.MedicineName, 'Unknown Medicine'),
            @StockQty = COALESCE(m.StockQty, 0),
            @RequestedQty = rd.Qty
        FROM RequestDetails rd
        LEFT JOIN Medicines m ON rd.MedicineId = m.Id
        WHERE rd.RequestId = @RequestId AND (m.Id IS NULL OR m.StockQty < rd.Qty);

        IF @FailedMedicineId IS NOT NULL
        BEGIN
            DECLARE @ErrMsg VARCHAR(300);
            IF @FailedMedicineName = 'Unknown Medicine'
            BEGIN
                SET @ErrMsg = 'Medicine ID ' + CAST(@FailedMedicineId AS VARCHAR(20)) + ' not found.';
            END
            ELSE
            BEGIN
                SET @ErrMsg = 'Insufficient stock for ' + @FailedMedicineName + '. Required: ' + CAST(@RequestedQty AS VARCHAR(20)) + ', Available: ' + CAST(@StockQty AS VARCHAR(20)) + '.';
            END
            ;THROW 50000, @ErrMsg, 1;
        END

        -- 2. Update Request Status
        UPDATE Requests
        SET Status = @Status,
            AdminApprovedAt = CASE WHEN @Status = 'Approved By Admin' THEN GETUTCDATE() ELSE AdminApprovedAt END,
            DistributionApprovedAt = @DistributionApprovedAt
        WHERE Id = @RequestId;

        -- 3. Deduct Stocks
        UPDATE m
        SET m.StockQty = m.StockQty - rd.Qty
        FROM Medicines m
        INNER JOIN RequestDetails rd ON m.Id = rd.MedicineId
        WHERE rd.RequestId = @RequestId;

        -- 4. Log Action
        INSERT INTO ApprovalLogs (RequestId, ActionBy, ActionType, Remarks, ActionDate)
        VALUES (@RequestId, @ActionBy, @ActionType, @Remarks, GETUTCDATE());

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

CREATE PROCEDURE sp_GetAllApprovalLogs
AS
BEGIN
    SET NOCOUNT ON;
    SELECT l.Id, l.RequestId, req.RequestNumber, l.ActionBy, u.FullName AS ActionByName, r.RoleName AS ActionByRole,
           l.ActionType, l.Remarks, l.ActionDate
    FROM ApprovalLogs l
    INNER JOIN Requests req ON l.RequestId = req.Id
    INNER JOIN Users u ON l.ActionBy = u.Id
    INNER JOIN Roles r ON u.RoleId = r.Id
    ORDER BY l.ActionDate DESC;
END
GO

CREATE PROCEDURE sp_GetApprovalLogsByRequestId
    @RequestId BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT l.Id, l.RequestId, req.RequestNumber, l.ActionBy, u.FullName AS ActionByName, r.RoleName AS ActionByRole,
           l.ActionType, l.Remarks, l.ActionDate
    FROM ApprovalLogs l
    INNER JOIN Requests req ON l.RequestId = req.Id
    INNER JOIN Users u ON l.ActionBy = u.Id
    INNER JOIN Roles r ON u.RoleId = r.Id
    WHERE l.RequestId = @RequestId
    ORDER BY l.ActionDate ASC;
END
GO

