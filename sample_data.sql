-- =============================================
-- Sample Data Script for PRM Project Assignment
-- =============================================
-- This script inserts sample data for testing purposes
-- Note: Passwords are stored as plain text (no hashing for easier testing)
-- Default password for all test users: "Test@123"
-- =============================================

USE [CarRentalDb]; -- Update with your actual database name
GO

-- =============================================
-- INSERT STATIONS (20 stations across Vietnam)
-- =============================================
INSERT INTO dbo.Stations (Id, Name, Address, Latitude, Longitude)
VALUES
-- Ho Chi Minh City
(NEWID(), N'Trạm Bến Thành', N'123 Lê Lợi, Phường Bến Nghé, Quận 1, TP.HCM', 10.773855, 106.701128),
(NEWID(), N'Trạm Đồng Khởi', N'456 Đồng Khởi, Phường Bến Nghé, Quận 1, TP.HCM', 10.778785, 106.703879),
(NEWID(), N'Trạm Nguyễn Huệ', N'789 Nguyễn Huệ, Phường Bến Nghé, Quận 1, TP.HCM', 10.775901, 106.704159),
(NEWID(), N'Trạm Landmark 81', N'208 Nguyễn Hữu Cảnh, Phường 22, Quận Bình Thạnh, TP.HCM', 10.794817, 106.721843),
(NEWID(), N'Trạm Sân Bay Tân Sơn Nhất', N'Trường Sơn, Phường 2, Quận Tân Bình, TP.HCM', 10.818804, 106.651856),

-- Hanoi
(NEWID(), N'Trạm Hồ Gươm', N'123 Hàng Trống, Phường Hàng Trống, Quận Hoàn Kiếm, Hà Nội', 21.028511, 105.854167),
(NEWID(), N'Trạm Nhà Hát Lớn', N'1 Tràng Tiền, Phường Tràng Tiền, Quận Hoàn Kiếm, Hà Nội', 21.024830, 105.856597),
(NEWID(), N'Trạm Cầu Giấy', N'456 Cầu Giấy, Phường Quan Hoa, Quận Cầu Giấy, Hà Nội', 21.033170, 105.797838),
(NEWID(), N'Trạm Mỹ Đình', N'789 Mỹ Đình, Phường Mỹ Đình 2, Quận Nam Từ Liêm, Hà Nội', 21.021558, 105.768470),

-- Da Nang
(NEWID(), N'Trạm Bãi Biển Mỹ Khê', N'123 Hoàng Sa, Phường Mỹ An, Quận Ngũ Hành Sơn, Đà Nẵng', 16.051456, 108.245995),
(NEWID(), N'Trạm Sông Hàn', N'456 Bạch Đằng, Phường Hải Châu 1, Quận Hải Châu, Đà Nẵng', 16.066107, 108.222737),

-- Nha Trang
(NEWID(), N'Trạm Bãi Biển Trần Phú', N'789 Trần Phú, Phường Lộc Thọ, Thành phố Nha Trang', 12.238791, 109.196749),

-- Hue
(NEWID(), N'Trạm Cố Đô Huế', N'123 Lê Lợi, Phường Phú Hội, Thành phố Huế', 16.463713, 107.590866),

-- Can Tho
(NEWID(), N'Trạm Chợ Nổi Cái Răng', N'123 Nguyễn Văn Cừ, Phường An Khánh, Quận Ninh Kiều, Cần Thơ', 10.045161, 105.746857),

-- Hai Phong
(NEWID(), N'Trạm Cát Bà', N'789 Đường 1 Tháng 4, Phường Hạ Lý, Quận Hồng Bàng, Hải Phòng', 20.844912, 106.688084),

-- Vung Tau
(NEWID(), N'Trạm Bãi Sau', N'456 Trần Phú, Phường 5, Thành phố Vũng Tàu', 10.345906, 107.079231),

-- Da Lat
(NEWID(), N'Trạm Hồ Xuân Hương', N'123 Trần Phú, Phường 3, Thành phố Đà Lạt', 11.940419, 108.438797),

-- Phu Quoc
(NEWID(), N'Trạm Bãi Dài', N'789 Đường Trần Hưng Đạo, Phường Dương Đông, Thành phố Phú Quốc', 10.289279, 103.984016),

-- Qui Nhon
(NEWID(), N'Trạm Bãi Xep', N'456 Nguyễn Huệ, Phường Ghềnh Ráng, Thành phố Qui Nhơn', 13.783416, 109.228408);
GO

-- Store station IDs in variables for later use (this is a simplified approach)
-- In practice, you'd query them or use specific GUIDs

-- =============================================
-- INSERT USERS (100 users: 5 Admins, 15 Staff, 80 Renters)
-- =============================================

-- Note: Passwords are stored as plain text (no hashing for easier testing)
-- Default password for all test users: "Test@123"

DECLARE @StationIds TABLE (Id UNIQUEIDENTIFIER, RowNum INT);
INSERT INTO @StationIds (Id, RowNum)
SELECT Id, ROW_NUMBER() OVER (ORDER BY Name) FROM dbo.Stations;

DECLARE @StaffIds TABLE (Id UNIQUEIDENTIFIER);
DECLARE @RenterIds TABLE (Id UNIQUEIDENTIFIER);

-- Admins (5)
INSERT INTO dbo.Users (Id, FullName, Email, PasswordHash, Role, Status, CreatedAt, IsVerified, DriverLicenseNumber, IDCardNumber)
VALUES
(NEWID(), N'Nguyễn Văn Admin', 'admin@prm.com', 'Test@123', 'Admin', 'Active', DATEADD(day, -365, GETUTCDATE()), 1, NULL, NULL),
(NEWID(), N'Trần Thị Quản Lý', 'admin2@prm.com', 'Test@123', 'Admin', 'Active', DATEADD(day, -300, GETUTCDATE()), 1, NULL, NULL),
(NEWID(), N'Lê Văn Quản Trị', 'admin3@prm.com', 'Test@123', 'Admin', 'Active', DATEADD(day, -250, GETUTCDATE()), 1, NULL, NULL),
(NEWID(), N'Phạm Thị Hệ Thống', 'admin4@prm.com', 'Test@123', 'Admin', 'Active', DATEADD(day, -200, GETUTCDATE()), 1, NULL, NULL),
(NEWID(), N'Hoàng Văn Điều Hành', 'admin5@prm.com', 'Test@123', 'Admin', 'Active', DATEADD(day, -150, GETUTCDATE()), 1, NULL, NULL);

-- Staff (15)
INSERT INTO dbo.Users (Id, FullName, Email, PasswordHash, Role, Status, CreatedAt, IsVerified, DriverLicenseNumber, IDCardNumber)
VALUES
(NEWID(), N'Võ Thị Nhân Viên 1', 'staff1@prm.com', 'Test@123', 'Staff', 'Active', DATEADD(day, -180, GETUTCDATE()), 1, NULL, NULL),
(NEWID(), N'Đặng Văn Nhân Viên 2', 'staff2@prm.com', 'Test@123', 'Staff', 'Active', DATEADD(day, -170, GETUTCDATE()), 1, NULL, NULL),
(NEWID(), N'Bùi Thị Nhân Viên 3', 'staff3@prm.com', 'Test@123', 'Staff', 'Active', DATEADD(day, -160, GETUTCDATE()), 1, NULL, NULL),
(NEWID(), N'Đỗ Văn Nhân Viên 4', 'staff4@prm.com', 'Test@123', 'Staff', 'Active', DATEADD(day, -150, GETUTCDATE()), 1, NULL, NULL),
(NEWID(), N'Hồ Thị Nhân Viên 5', 'staff5@prm.com', 'Test@123', 'Staff', 'Active', DATEADD(day, -140, GETUTCDATE()), 1, NULL, NULL),
(NEWID(), N'Ngô Văn Nhân Viên 6', 'staff6@prm.com', 'Test@123', 'Staff', 'Active', DATEADD(day, -130, GETUTCDATE()), 1, NULL, NULL),
(NEWID(), N'Dương Thị Nhân Viên 7', 'staff7@prm.com', 'Test@123', 'Staff', 'Active', DATEADD(day, -120, GETUTCDATE()), 1, NULL, NULL),
(NEWID(), N'Lý Văn Nhân Viên 8', 'staff8@prm.com', 'Test@123', 'Staff', 'Active', DATEADD(day, -110, GETUTCDATE()), 1, NULL, NULL),
(NEWID(), N'Vũ Thị Nhân Viên 9', 'staff9@prm.com', 'Test@123', 'Staff', 'Active', DATEADD(day, -100, GETUTCDATE()), 1, NULL, NULL),
(NEWID(), N'Phan Văn Nhân Viên 10', 'staff10@prm.com', 'Test@123', 'Staff', 'Active', DATEADD(day, -90, GETUTCDATE()), 1, NULL, NULL),
(NEWID(), N'Trương Thị Nhân Viên 11', 'staff11@prm.com', 'Test@123', 'Staff', 'Active', DATEADD(day, -80, GETUTCDATE()), 1, NULL, NULL),
(NEWID(), N'Lưu Văn Nhân Viên 12', 'staff12@prm.com', 'Test@123', 'Staff', 'Active', DATEADD(day, -70, GETUTCDATE()), 1, NULL, NULL),
(NEWID(), N'Vương Thị Nhân Viên 13', 'staff13@prm.com', 'Test@123', 'Staff', 'Active', DATEADD(day, -60, GETUTCDATE()), 1, NULL, NULL),
(NEWID(), N'Tôn Văn Nhân Viên 14', 'staff14@prm.com', 'Test@123', 'Staff', 'Active', DATEADD(day, -50, GETUTCDATE()), 1, NULL, NULL),
(NEWID(), N'Thái Thị Nhân Viên 15', 'staff15@prm.com', 'Test@123', 'Staff', 'Active', DATEADD(day, -40, GETUTCDATE()), 1, NULL, NULL);

-- Store staff IDs
INSERT INTO @StaffIds (Id)
SELECT Id FROM dbo.Users WHERE Role = 'Staff';

-- Renters (80 users)
DECLARE @Counter INT = 1;
DECLARE @RenterNames TABLE (
    FullName NVARCHAR(100),
    Email VARCHAR(255),
    DriverLicense VARCHAR(50),
    IDCard VARCHAR(50)
);

-- Generate renter data
WHILE @Counter <= 80
BEGIN
    INSERT INTO @RenterNames (FullName, Email, DriverLicense, IDCard)
    VALUES 
    (N'Khách hàng ' + CAST(@Counter AS NVARCHAR), 'renter' + CAST(@Counter AS VARCHAR) + '@test.com', 
     'DL' + RIGHT('00000' + CAST(@Counter AS VARCHAR), 5), 
     'ID' + RIGHT('000000000' + CAST(@Counter AS VARCHAR), 9));
    
    SET @Counter = @Counter + 1;
END;

INSERT INTO dbo.Users (Id, FullName, Email, PasswordHash, Role, Status, CreatedAt, IsVerified, DriverLicenseNumber, IDCardNumber)
SELECT 
    NEWID(),
    FullName,
    Email,
    'Test@123',
    'Renter',
    CASE WHEN ROW_NUMBER() OVER (ORDER BY Email) % 10 = 0 THEN 'Inactive' ELSE 'Active' END,
    DATEADD(day, -RAND(CHECKSUM(NEWID())) * 200, GETUTCDATE()),
    CASE WHEN ROW_NUMBER() OVER (ORDER BY Email) % 5 = 0 THEN 0 ELSE 1 END,
    DriverLicense,
    IDCard
FROM @RenterNames;

-- Store renter IDs
INSERT INTO @RenterIds (Id)
SELECT Id FROM dbo.Users WHERE Role = 'Renter';

GO

-- =============================================
-- INSERT VEHICLES (100 vehicles)
-- =============================================

DECLARE @VehicleCounter INT = 1;
DECLARE @TotalStations INT;
SELECT @TotalStations = COUNT(*) FROM dbo.Stations;

WHILE @VehicleCounter <= 100
BEGIN
    DECLARE @StationId UNIQUEIDENTIFIER;
    DECLARE @StationIndex INT = ((@VehicleCounter - 1) % @TotalStations) + 1;
    
    SELECT @StationId = Id 
    FROM dbo.Stations 
    ORDER BY Name
    OFFSET (@StationIndex - 1) ROWS
    FETCH NEXT 1 ROWS ONLY;
    
    DECLARE @VehicleType NVARCHAR(50) = CASE 
        WHEN @VehicleCounter % 3 = 0 THEN 'Car'
        WHEN @VehicleCounter % 3 = 1 THEN 'Scooter'
        ELSE 'Other'
    END;
    
    DECLARE @VehicleStatus NVARCHAR(50) = CASE 
        WHEN @VehicleCounter % 4 = 0 THEN 'Maintenance'
        WHEN @VehicleCounter % 4 = 1 THEN 'InUse'
        WHEN @VehicleCounter % 4 = 2 THEN 'Booked'
        ELSE 'Available'
    END;
    
    DECLARE @BatteryLevel INT = 20 + (ABS(CHECKSUM(NEWID())) % 80);
    DECLARE @PlateNumber NVARCHAR(50) = '30' + RIGHT('000' + CAST(@VehicleCounter AS VARCHAR), 3) + '-AB';
    DECLARE @ImageUrl NVARCHAR(1000) = CONCAT('/images/vehicles/', REPLACE(@PlateNumber, '-', ''), '.jpg');
    
    INSERT INTO dbo.Vehicles (Id, StationId, PlateNumber, Type, BatteryLevel, Status, ImageUrl)
    VALUES (NEWID(), @StationId, @PlateNumber, @VehicleType, @BatteryLevel, @VehicleStatus, @ImageUrl);
    
    SET @VehicleCounter = @VehicleCounter + 1;
END;

GO

-- =============================================
-- INSERT VEHICLE HISTORIES (5-10 records per vehicle)
-- =============================================

DECLARE @VehicleId UNIQUEIDENTIFIER;
DECLARE @VehicleCursor CURSOR;

SET @VehicleCursor = CURSOR FOR
SELECT Id FROM dbo.Vehicles;

OPEN @VehicleCursor;
FETCH NEXT FROM @VehicleCursor INTO @VehicleId;

WHILE @@FETCH_STATUS = 0
BEGIN
    DECLARE @HistoryCount INT = 5 + (ABS(CHECKSUM(NEWID())) % 6); -- 5-10 records
    DECLARE @HistoryCounter INT = 0;
    
    WHILE @HistoryCounter < @HistoryCount
    BEGIN
        DECLARE @HistoryBattery INT = 10 + (ABS(CHECKSUM(NEWID())) % 90);
        DECLARE @HistoryDate DATETIME = DATEADD(day, -(@HistoryCount - @HistoryCounter) * 2, GETUTCDATE());
        DECLARE @ConditionNote NVARCHAR(4000) = CASE 
            WHEN @HistoryCounter % 3 = 0 THEN N'Tình trạng tốt, không có vấn đề'
            WHEN @HistoryCounter % 3 = 1 THEN N'Cần kiểm tra định kỳ'
            ELSE NULL
        END;
        
        INSERT INTO dbo.VehicleHistories (Id, VehicleId, TimeStamp, BatteryLevel, ConditionNote)
        VALUES (NEWID(), @VehicleId, @HistoryDate, @HistoryBattery, @ConditionNote);
        
        SET @HistoryCounter = @HistoryCounter + 1;
    END;
    
    FETCH NEXT FROM @VehicleCursor INTO @VehicleId;
END;

CLOSE @VehicleCursor;
DEALLOCATE @VehicleCursor;

GO

-- =============================================
-- INSERT RENTALS (200 rentals)
-- =============================================

DECLARE @RentalCounter INT = 1;
DECLARE @RentalRenterIds TABLE (Id UNIQUEIDENTIFIER);
DECLARE @RentalStaffIds TABLE (Id UNIQUEIDENTIFIER);
DECLARE @RentalVehicleIds TABLE (Id UNIQUEIDENTIFIER);

INSERT INTO @RentalRenterIds (Id)
SELECT Id FROM dbo.Users WHERE Role = 'Renter' AND Status = 'Active';

INSERT INTO @RentalStaffIds (Id)
SELECT Id FROM dbo.Users WHERE Role = 'Staff' AND Status = 'Active';

INSERT INTO @RentalVehicleIds (Id)
SELECT Id FROM dbo.Vehicles WHERE Status != 'Maintenance';

DECLARE @TotalRenters INT;
DECLARE @TotalStaff INT;
DECLARE @TotalVehicles INT;

SELECT @TotalRenters = COUNT(*) FROM @RentalRenterIds;
SELECT @TotalStaff = COUNT(*) FROM @RentalStaffIds;
SELECT @TotalVehicles = COUNT(*) FROM @RentalVehicleIds;

WHILE @RentalCounter <= 200
BEGIN
    DECLARE @RenterId UNIQUEIDENTIFIER;
    DECLARE @StaffId UNIQUEIDENTIFIER;
    DECLARE @VehicleId UNIQUEIDENTIFIER;
    DECLARE @StationId UNIQUEIDENTIFIER;
    
    -- Get random renter
    SELECT @RenterId = Id 
    FROM @RentalRenterIds
    ORDER BY NEWID()
    OFFSET ((@RentalCounter - 1) % @TotalRenters) ROWS
    FETCH NEXT 1 ROWS ONLY;
    
    -- Get random staff
    SELECT @StaffId = Id 
    FROM @RentalStaffIds
    ORDER BY NEWID()
    OFFSET ((@RentalCounter - 1) % @TotalStaff) ROWS
    FETCH NEXT 1 ROWS ONLY;
    
    -- Get random vehicle
    SELECT @VehicleId = Id 
    FROM @RentalVehicleIds
    ORDER BY NEWID()
    OFFSET ((@RentalCounter - 1) % @TotalVehicles) ROWS
    FETCH NEXT 1 ROWS ONLY;
    
    -- Get vehicle's station
    SELECT @StationId = StationId FROM dbo.Vehicles WHERE Id = @VehicleId;
    
    DECLARE @StartTime DATETIME = DATEADD(day, -ABS(CHECKSUM(NEWID())) % 180, GETUTCDATE());
    DECLARE @EndTime DATETIME = NULL;
    DECLARE @TotalCost DECIMAL(10,2) = 0;
    DECLARE @RentalStatus NVARCHAR(50);
    
    -- Determine rental status
    IF @RentalCounter % 4 = 0
    BEGIN
        SET @RentalStatus = 'Cancelled';
        SET @TotalCost = 0;
    END
    ELSE IF @RentalCounter % 4 = 1
    BEGIN
        SET @RentalStatus = 'Booked';
        SET @TotalCost = CAST(50 + (ABS(CHECKSUM(NEWID())) % 200) AS DECIMAL(10,2));
    END
    ELSE IF @RentalCounter % 4 = 2
    BEGIN
        SET @RentalStatus = 'Ongoing';
        SET @StartTime = DATEADD(hour, -ABS(CHECKSUM(NEWID())) % 72, GETUTCDATE());
        SET @TotalCost = CAST(100 + (ABS(CHECKSUM(NEWID())) % 300) AS DECIMAL(10,2));
    END
    ELSE
    BEGIN
        SET @RentalStatus = 'Completed';
        SET @EndTime = DATEADD(hour, 1 + ABS(CHECKSUM(NEWID())) % 24, @StartTime);
        SET @TotalCost = CAST(150 + (ABS(CHECKSUM(NEWID())) % 500) AS DECIMAL(10,2));
    END;
    
    INSERT INTO dbo.Rentals (Id, VehicleId, RenterId, StationId, StaffId, StartTime, EndTime, TotalCost, Status)
    VALUES (NEWID(), @VehicleId, @RenterId, @StationId, @StaffId, @StartTime, @EndTime, @TotalCost, @RentalStatus);
    
    SET @RentalCounter = @RentalCounter + 1;
END;

GO

-- =============================================
-- INSERT PAYMENTS (for completed rentals only)
-- =============================================

INSERT INTO dbo.Payments (Id, RentalId, Amount, PaymentMethod, PaidTime)
SELECT 
    NEWID(),
    r.Id,
    r.TotalCost,
    CASE 
        WHEN ABS(CHECKSUM(NEWID())) % 3 = 0 THEN 'Cash'
        WHEN ABS(CHECKSUM(NEWID())) % 3 = 1 THEN 'Card'
        ELSE 'EWallet'
    END,
    ISNULL(r.EndTime, DATEADD(minute, 30, r.StartTime))
FROM dbo.Rentals r
WHERE r.Status = 'Completed' OR r.Status = 'Ongoing';

GO

-- =============================================
-- INSERT SOME REFRESH TOKENS (for active users)
-- =============================================

DECLARE @TokenUserId UNIQUEIDENTIFIER;
DECLARE @TokenCursor CURSOR;

SET @TokenCursor = CURSOR FOR
SELECT Id FROM dbo.Users WHERE Status = 'Active'
ORDER BY NEWID()
OFFSET 0 ROWS
FETCH NEXT 20 ROWS ONLY;

OPEN @TokenCursor;
FETCH NEXT FROM @TokenCursor INTO @TokenUserId;

WHILE @@FETCH_STATUS = 0
BEGIN
    INSERT INTO dbo.RefreshTokens (Id, Token, UserId, Expires)
    VALUES (
        NEWID(),
        CONVERT(NVARCHAR(MAX), NEWID()) + CONVERT(NVARCHAR(MAX), NEWID()), -- Simulated token
        @TokenUserId,
        DATEADD(day, 7, GETUTCDATE())
    );
    
    FETCH NEXT FROM @TokenCursor INTO @TokenUserId;
END;

CLOSE @TokenCursor;
DEALLOCATE @TokenCursor;

GO

-- =============================================
-- SUMMARY
-- =============================================
PRINT 'Sample data insertion completed!';
PRINT '';
PRINT 'Summary:';
PRINT '--------';
PRINT 'Stations: ' + CAST((SELECT COUNT(*) FROM dbo.Stations) AS VARCHAR) + ' records';
PRINT 'Users: ' + CAST((SELECT COUNT(*) FROM dbo.Users) AS VARCHAR) + ' records';
PRINT '  - Admins: ' + CAST((SELECT COUNT(*) FROM dbo.Users WHERE Role = 'Admin') AS VARCHAR);
PRINT '  - Staff: ' + CAST((SELECT COUNT(*) FROM dbo.Users WHERE Role = 'Staff') AS VARCHAR);
PRINT '  - Renters: ' + CAST((SELECT COUNT(*) FROM dbo.Users WHERE Role = 'Renter') AS VARCHAR);
PRINT 'Vehicles: ' + CAST((SELECT COUNT(*) FROM dbo.Vehicles) AS VARCHAR) + ' records';
PRINT 'Vehicle Histories: ' + CAST((SELECT COUNT(*) FROM dbo.VehicleHistories) AS VARCHAR) + ' records';
PRINT 'Rentals: ' + CAST((SELECT COUNT(*) FROM dbo.Rentals) AS VARCHAR) + ' records';
PRINT 'Payments: ' + CAST((SELECT COUNT(*) FROM dbo.Payments) AS VARCHAR) + ' records';
PRINT 'Refresh Tokens: ' + CAST((SELECT COUNT(*) FROM dbo.RefreshTokens) AS VARCHAR) + ' records';
PRINT '';
PRINT 'All users have password: Test@123';
PRINT 'Passwords are stored as plain text (no hashing)';
GO

