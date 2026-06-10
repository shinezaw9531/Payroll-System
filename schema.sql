-- Create Database
CREATE DATABASE PayrollDb;
GO
USE PayrollDb;
GO

-- 1. Instructors Table
CREATE TABLE Instructors (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(256) NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE()
);

-- 2. Classes Table
CREATE TABLE Classes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    InstructorId INT NOT NULL,
    ClassName NVARCHAR(100) NOT NULL,
    ClassType NVARCHAR(50) NOT NULL,
    FixedFee DECIMAL(18,2) NOT NULL,
    PayoutPerBooking DECIMAL(18,2) NOT NULL,
    Status NVARCHAR(20) NOT NULL, -- 'Completed', 'Cancelled'
    StartTime DATETIME2 NOT NULL,
    FOREIGN KEY (InstructorId) REFERENCES Instructors(Id)
);

-- 3. Bookings Table
CREATE TABLE Bookings (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ClassId INT NOT NULL,
    ParticipantName NVARCHAR(100) NOT NULL,
    Status NVARCHAR(20) NOT NULL, -- 'Completed', 'Cancelled', 'NoShow'
    FOREIGN KEY (ClassId) REFERENCES Classes(Id)
);

-- 4. Sales Table
CREATE TABLE Sales (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    InstructorId INT NOT NULL,
    ItemType NVARCHAR(20) NOT NULL, -- 'Membership', 'Package'
    Amount DECIMAL(18,2) NOT NULL,
    IsRefunded BIT NOT NULL DEFAULT 0,
    SaleDate DATETIME2 NOT NULL,
    FOREIGN KEY (InstructorId) REFERENCES Instructors(Id)
);

-- 5. Studio Configurations Table
CREATE TABLE StudioConfigurations (
    KeyName NVARCHAR(50) PRIMARY KEY,
    Value NVARCHAR(100) NOT NULL
);

-- 6. Payroll Summaries Table (Audit-ready Ledger)
CREATE TABLE PayrollSummaries (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    InstructorId INT NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    ClassEarnings DECIMAL(18,2) NOT NULL,
    Commission DECIMAL(18,2) NOT NULL,
    Bonus DECIMAL(18,2) NOT NULL,
    Adjustment DECIMAL(18,2) NOT NULL,
    FinalPayout DECIMAL(18,2) NOT NULL,
    GeneratedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (InstructorId) REFERENCES Instructors(Id),
    CONSTRAINT UQ_Instructor_Period UNIQUE (InstructorId, StartDate, EndDate)
);

-- Initialize Global Studio No-Show Rule
INSERT INTO StudioConfigurations (KeyName, Value) VALUES ('NoShowPayoutPolicy', 'PayInstructor');