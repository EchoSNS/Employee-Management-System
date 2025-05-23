-- Create Employee Stored Procedure
CREATE OR ALTER PROCEDURE sp_CreateEmployee
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @Email NVARCHAR(255),
    @PhoneNumber NVARCHAR(20) = NULL,
    @Department NVARCHAR(100),
    @Position NVARCHAR(100),
    @Salary DECIMAL(18,2),
    @HireDate DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @NewEmployeeId INT;
    
    INSERT INTO Employees (
        FirstName, 
        LastName, 
        Email, 
        PhoneNumber, 
        Department, 
        Position, 
        Salary, 
        HireDate, 
        IsActive, 
        CreatedAt
    )
    VALUES (
        @FirstName,
        @LastName,
        @Email,
        @PhoneNumber,
        @Department,
        @Position,
        @Salary,
        @HireDate,
        1,
        GETUTCDATE()
    );
    
    SET @NewEmployeeId = SCOPE_IDENTITY();
    
    SELECT @NewEmployeeId AS NewEmployeeId;
END
GO

-- Update Employee Stored Procedure
CREATE OR ALTER PROCEDURE sp_UpdateEmployee
    @Id INT,
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @Email NVARCHAR(255),
    @PhoneNumber NVARCHAR(20) = NULL,
    @Department NVARCHAR(100),
    @Position NVARCHAR(100),
    @Salary DECIMAL(18,2),
    @HireDate DATETIME2,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Employees 
    SET 
        FirstName = @FirstName,
        LastName = @LastName,
        Email = @Email,
        PhoneNumber = @PhoneNumber,
        Department = @Department,
        Position = @Position,
        Salary = @Salary,
        HireDate = @HireDate,
        IsActive = @IsActive,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id AND IsActive = 1;
    
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- Delete Employee Stored Procedure (Soft Delete)
CREATE OR ALTER PROCEDURE sp_DeleteEmployee
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Employees 
    SET 
        IsActive = 0,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id AND IsActive = 1;
    
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- Get All Active Employees Stored Procedure
CREATE OR ALTER PROCEDURE sp_GetAllActiveEmployees
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        FirstName,
        LastName,
        CONCAT(FirstName, ' ', LastName) AS FullName,
        Email,
        PhoneNumber,
        Department,
        Position,
        Salary,
        HireDate,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM Employees
    WHERE IsActive = 1
    ORDER BY LastName, FirstName;
END
GO

-- Get Employee By Id Stored Procedure
CREATE OR ALTER PROCEDURE sp_GetEmployeeById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        FirstName,
        LastName,
        CONCAT(FirstName, ' ', LastName) AS FullName,
        Email,
        PhoneNumber,
        Department,
        Position,
        Salary,
        HireDate,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM Employees
    WHERE Id = @Id AND IsActive = 1;
END
GO

-- Check if Email Exists Stored Procedure
CREATE OR ALTER PROCEDURE sp_CheckEmailExists
    @Email NVARCHAR(255),
    @ExcludeId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Count INT;
    
    IF @ExcludeId IS NULL
    BEGIN
        SELECT @Count = COUNT(*)
        FROM Employees
        WHERE Email = @Email AND IsActive = 1;
    END
    ELSE
    BEGIN
        SELECT @Count = COUNT(*)
        FROM Employees
        WHERE Email = @Email AND IsActive = 1 AND Id != @ExcludeId;
    END
    
    SELECT @Count AS EmailExists;
END
GO