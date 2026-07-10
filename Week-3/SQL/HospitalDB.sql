--CREATE DATABASE HospitalDB;

USE HospitalDB;
GO 


DROP TABLE IF EXISTS User_Accounts;
DROP TABLE IF EXISTS Schedules;
DROP TABLE IF EXISTS Lab_Results;
DROP TABLE IF EXISTS Lab_Requests;
DROP TABLE IF EXISTS Lab_Tests;
DROP TABLE IF EXISTS Prescription_Details;
DROP TABLE IF EXISTS Inventory;
DROP TABLE IF EXISTS Medications;
DROP TABLE IF EXISTS Billing;
DROP TABLE IF EXISTS Admissions;
DROP TABLE IF EXISTS Rooms;
DROP TABLE IF EXISTS Medical_Records;
DROP TABLE IF EXISTS Appointments;
DROP TABLE IF EXISTS Patients;
DROP TABLE IF EXISTS Doctors;
DROP TABLE IF EXISTS Staff;
DROP TABLE IF EXISTS Departments;
GO


CREATE TABLE Departments (
    DepartmentID INT IDENTITY(1,1) PRIMARY KEY,
    DepartmentName VARCHAR(100) NOT NULL,
    LocationBlock VARCHAR(50) NOT NULL,
    ContactExtension VARCHAR(20) NULL
);

CREATE TABLE Rooms (
    RoomID INT IDENTITY(1,1) PRIMARY KEY,
    DepartmentID INT NOT NULL,
    RoomNumber VARCHAR(10) NOT NULL,
    RoomType VARCHAR(30) NOT NULL CHECK (RoomType IN ('ICU', 'Private', 'Semi-Private', 'General Ward')),
    Status VARCHAR(20) NOT NULL DEFAULT 'Available' CHECK (Status IN ('Available', 'Occupied', 'Maintenance')),
    CONSTRAINT FK_Rooms_Departments FOREIGN KEY (DepartmentID) REFERENCES Departments(DepartmentID)
);
GO

CREATE TABLE Staff (
    StaffID INT IDENTITY(1,1) PRIMARY KEY,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    Role VARCHAR(50) NOT NULL CHECK (Role IN ('Doctor', 'Nurse', 'Lab Tech', 'Pharmacist', 'Admin', 'Security')),
    DepartmentID INT NOT NULL,
    ContactNumber VARCHAR(20) NULL,
    Email VARCHAR(100) NULL,
    HireDate DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Staff_Departments FOREIGN KEY (DepartmentID) REFERENCES Departments(DepartmentID)
);

-- Doctors table inherits from Staff. DoctorID is BOTH the PK and the FK.
CREATE TABLE Doctors (
    DoctorID INT PRIMARY KEY,
    Specialty VARCHAR(100) NOT NULL,
    LicenseNumber VARCHAR(50) NOT NULL UNIQUE,
    CONSTRAINT FK_Doctors_Staff FOREIGN KEY (DoctorID) REFERENCES Staff(StaffID) ON DELETE CASCADE
);

CREATE TABLE Schedules (
    ScheduleID INT IDENTITY(1,1) PRIMARY KEY,
    StaffID INT NOT NULL,
    ShiftDate DATE NOT NULL,
    ShiftStartTime TIME NOT NULL,
    ShiftEndTime TIME NOT NULL,
    CONSTRAINT FK_Schedules_Staff FOREIGN KEY (StaffID) REFERENCES Staff(StaffID),
    CONSTRAINT CK_Shift_Times CHECK (ShiftEndTime > ShiftStartTime)
);

CREATE TABLE User_Accounts (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    StaffID INT NOT NULL UNIQUE,
    Username VARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    AccessLevel VARCHAR(30) NOT NULL CHECK (AccessLevel IN ('SystemAdmin', 'Clinician', 'BillingStaff', 'Registrar')),
    LastLogin DATETIME NULL,
    CONSTRAINT FK_UserAccounts_Staff FOREIGN KEY (StaffID) REFERENCES Staff(StaffID)
);
GO


CREATE TABLE Patients (
    PatientID INT IDENTITY(1,1) PRIMARY KEY,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    DateOfBirth DATE NOT NULL,
    Insurance VARCHAR (100) NULL,
    Gender VARCHAR(10) NOT NULL CHECK (Gender IN ('Male', 'Female')),
    Email VARCHAR(100) NULL,
    ContactNumber VARCHAR(20) NULL,
    EmergencyContactName VARCHAR(100) NULL,
    EmergencyContactPhone VARCHAR(20) NULL,
    BloodType VARCHAR(5) NOT NULL CHECK (BloodType IN ('A+', 'A-', 'B+', 'B-', 'AB+', 'AB-', 'O+', 'O-')),
    Address VARCHAR(200) NULL,
    Allergies VARCHAR(200) NULL
);

CREATE TABLE Appointments (
    AppointmentID INT IDENTITY(1,1) PRIMARY KEY,
    PatientID INT NOT NULL,
    DoctorID INT NOT NULL,
    AppointmentDate DATE NOT NULL,
    AppointmentTime TIME NOT NULL,
    ReasonForVisit VARCHAR(100) NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'Scheduled' CHECK (Status IN ('Scheduled', 'Completed', 'Cancelled', 'No-Show')),
    CONSTRAINT FK_Appointments_Patients FOREIGN KEY (PatientID) REFERENCES Patients(PatientID),
    CONSTRAINT FK_Appointments_Doctors FOREIGN KEY (DoctorID) REFERENCES Doctors(DoctorID)
);

CREATE TABLE Medical_Records (
    RecordID INT IDENTITY(1,1) PRIMARY KEY,
    PatientID INT NOT NULL,
    DoctorID INT NOT NULL,
    AppointmentID INT NULL, -- Nullable for Emergency/Walk-ins
    VisitDate DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    Symptoms VARCHAR(500) NULL,
    Diagnosis VARCHAR(500) NULL,
    TreatmentPlan VARCHAR(500) NULL,
    ClinicalNotes VARCHAR(500) NOT NULL,
    CONSTRAINT FK_MedicalRecords_Patients FOREIGN KEY (PatientID) REFERENCES Patients(PatientID),
    CONSTRAINT FK_MedicalRecords_Doctors FOREIGN KEY (DoctorID) REFERENCES Doctors(DoctorID),
    CONSTRAINT FK_MedicalRecords_Appointments FOREIGN KEY (AppointmentID) REFERENCES Appointments(AppointmentID)
);
GO

CREATE TABLE Admissions (
    AdmissionID INT IDENTITY(1,1) PRIMARY KEY,
    PatientID INT NOT NULL,
    RoomID INT NOT NULL,
    AttendingDoctorID INT NOT NULL,
    AdmissionDate DATETIME NOT NULL DEFAULT GETDATE(),
    DischargeDate DATETIME NULL, -- Nullable while patient is in bed
    AdmissionReason VARCHAR(300) NOT NULL,
    DischargeNotes VARCHAR(300) NULL,
    CONSTRAINT FK_Admissions_Patients FOREIGN KEY (PatientID) REFERENCES Patients(PatientID),
    CONSTRAINT FK_Admissions_Rooms FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID),
    CONSTRAINT FK_Admissions_Doctors FOREIGN KEY (AttendingDoctorID) REFERENCES Doctors(DoctorID),
    CONSTRAINT CK_Discharge_Date CHECK (DischargeDate IS NULL OR DischargeDate >= AdmissionDate)
);
GO

CREATE TABLE Medications (
    MedicationID INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    GenericName VARCHAR(100) NULL,
    BrandName VARCHAR(100) NULL,
    DosageForm VARCHAR(30) NOT NULL,
    Strength VARCHAR(20) NOT NULL,
    UnitPrice NUMERIC(8,2) NOT NULL CHECK (UnitPrice >= 0)
);

CREATE TABLE Inventory (
    InventoryID INT IDENTITY(1,1) PRIMARY KEY,
    MedicationID INT NOT NULL,
    BatchNumber VARCHAR(50) NOT NULL,
    StockQuantity INT NOT NULL DEFAULT 0 CHECK (StockQuantity >= 0),
    ExpiryDate DATE NOT NULL,
    SupplierName VARCHAR(100) NOT NULL,
    CONSTRAINT FK_Inventory_Medications FOREIGN KEY (MedicationID) REFERENCES Medications(MedicationID)
);

CREATE TABLE Prescription_Details (
    PrescriptionID INT IDENTITY(1,1) PRIMARY KEY,
    RecordID INT NOT NULL,
    MedicationID INT NOT NULL,
    DosageInstructions VARCHAR(300) NOT NULL,
    Duration INT NOT NULL CHECK (Duration > 0), -- Days
    QuantityDispensed INT NOT NULL CHECK (QuantityDispensed >= 0),
    CONSTRAINT FK_Prescriptions_MedicalRecords FOREIGN KEY (RecordID) REFERENCES Medical_Records(RecordID),
    CONSTRAINT FK_Prescriptions_Medications FOREIGN KEY (MedicationID) REFERENCES Medications(MedicationID)
);
GO

CREATE TABLE Lab_Tests (
    TestID INT IDENTITY(1,1) PRIMARY KEY,
    TestCategory VARCHAR(50) NOT NULL CHECK (TestCategory IN ('Bloodwork', 'Radiology', 'Pathology', 'Cardiology')),
    Cost NUMERIC(8,2) NOT NULL CHECK (Cost >= 0)
);

CREATE TABLE Lab_Requests (
    RequestID INT IDENTITY(1,1) PRIMARY KEY,
    PatientID INT NOT NULL,
    DoctorID INT NOT NULL,
    TestID INT NOT NULL,
    RequestDate DATETIME NOT NULL DEFAULT GETDATE(),
    Status VARCHAR(20) NOT NULL DEFAULT 'Requested' CHECK (Status IN ('Requested', 'Sample Collected', 'Processing', 'Completed')),
    CONSTRAINT FK_LabRequests_Patients FOREIGN KEY (PatientID) REFERENCES Patients(PatientID),
    CONSTRAINT FK_LabRequests_Doctors FOREIGN KEY (DoctorID) REFERENCES Doctors(DoctorID),
    CONSTRAINT FK_LabRequests_LabTests FOREIGN KEY (TestID) REFERENCES Lab_Tests(TestID)
);

CREATE TABLE Lab_Results (
    ResultID INT IDENTITY(1,1) PRIMARY KEY,
    RequestID INT NOT NULL UNIQUE, -- 1-to-1 relationship with its request
    LabTechnicianID INT NOT NULL,
    ResultValue VARCHAR(20) NOT NULL,
    ResultDate DATETIME NOT NULL DEFAULT GETDATE(),
    PathologistNotes VARCHAR(300) NULL,
    CONSTRAINT FK_LabResults_LabRequests FOREIGN KEY (RequestID) REFERENCES Lab_Requests(RequestID),
    CONSTRAINT FK_LabResults_Staff FOREIGN KEY (LabTechnicianID) REFERENCES Staff(StaffID)
);
GO

CREATE TABLE Billing (
    BillingID INT IDENTITY(1,1) PRIMARY KEY,
    PatientID INT NOT NULL,
    AppointmentID INT NULL,
    AdmissionID INT NULL,
    InvoiceDate DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    TotalAmount NUMERIC(10,2) NOT NULL DEFAULT 0.00 CHECK (TotalAmount >= 0),
    InsuranceCoverageAmount NUMERIC(10,2) NOT NULL DEFAULT 0.00 CHECK (InsuranceCoverageAmount >= 0),
    PatientResponsibilityAmount NUMERIC(10,2) NOT NULL DEFAULT 0.00 CHECK (PatientResponsibilityAmount >= 0),
    PaymentStatus VARCHAR(20) NOT NULL DEFAULT 'Unpaid' CHECK (PaymentStatus IN ('Paid', 'Partial', 'Unpaid', 'Disputed')),
    CONSTRAINT FK_Billing_Patients FOREIGN KEY (PatientID) REFERENCES Patients(PatientID),
    CONSTRAINT FK_Billing_Appointments FOREIGN KEY (AppointmentID) REFERENCES Appointments(AppointmentID),
    CONSTRAINT FK_Billing_Admissions FOREIGN KEY (AdmissionID) REFERENCES Admissions(AdmissionID));

-- ==========================================
-- POPULATE MOCK DATA (SQL SERVER T-SQL)
-- ==========================================

INSERT INTO Departments (DepartmentName, LocationBlock, ContactExtension) VALUES 
('Emergency Room', 'Block A, Floor 1', '101'),
('Pediatrics', 'Block B, Floor 2', '202'),
('Cardiology', 'Block C, Floor 3', '303'),
('General Ward', 'Block A, Floor 2', '102');

INSERT INTO Rooms (DepartmentID, RoomNumber, RoomType, Status) VALUES 
(1, 'ER-101', 'ICU', 'Available'),
(1, 'ER-102', 'ICU', 'Occupied'),
(2, 'PEDS-201', 'Semi-Private', 'Available'),
(3, 'CARD-301', 'Private', 'Occupied'),
(4, 'WARD-401', 'General Ward', 'Available');

INSERT INTO Staff (FirstName, LastName, Role, DepartmentID, ContactNumber, Email, IsActive) VALUES 
('Alice', 'Smith', 'Doctor', 3, '555-0101', 'alice.smith@hospital.com', 1),     
('Bob', 'Jones', 'Doctor', 2, '555-0102', 'bob.jones@hospital.com', 1),       
('Charlie', 'Brown', 'Doctor', 1, '555-0103', 'charlie.brown@hospital.com', 1), 
('Diana', 'Prince', 'Nurse', 1, '555-0201', 'diana.p@hospital.com', 1),       
('Evan', 'Wright', 'Lab Tech', 1, '555-0301', 'evan.w@hospital.com', 1),      
('Fiona', 'Gallagher', 'Pharmacist', 4, '555-0401', 'fiona.g@hospital.com', 1),
('George', 'Costanza', 'Admin', 4, '555-0501', 'george.c@hospital.com', 1);   

INSERT INTO Doctors (DoctorID, Specialty, LicenseNumber) VALUES 
(1, 'Cardiology', 'LIC-12345'),
(2, 'Pediatrics', 'LIC-67890'),
(3, 'Emergency Medicine', 'LIC-11223');

INSERT INTO Schedules (StaffID, ShiftDate, ShiftStartTime, ShiftEndTime) VALUES
(1, '2026-07-03', '08:00:00', '16:00:00'),
(2, '2026-07-03', '06:00:00', '10:00:00'),
(4, '2026-07-03', '07:00:00', '19:00:00'),
(5, '2026-07-03', '09:00:00', '17:00:00');

INSERT INTO User_Accounts (StaffID, Username, PasswordHash, AccessLevel, LastLogin) VALUES 
(1, 'asmith', 'hashed_pwd_123', 'Clinician', GETDATE()),
(4, 'dprince', 'hashed_pwd_456', 'Clinician', GETDATE()),
(7, 'gcostanza', 'hashed_pwd_789', 'SystemAdmin', GETDATE());

INSERT INTO Patients (FirstName, LastName, DateOfBirth, Insurance, Gender, Email, ContactNumber, EmergencyContactName, EmergencyContactPhone, BloodType, Address, Allergies) VALUES 
('John', 'Doe', '1985-05-12', 'Blue Cross', 'Male', 'john.doe@gmail.com', '555-9001', 'Jane Doe', '555-9002', 'A+', '123 Main St, Springfield', 'Peanuts'),
('Mary', 'Jane', '1992-10-22', 'Cigna', 'Female', 'mary.j@gmail.com', '555-8001', 'Peter Parker', '555-8002', 'O-', '456 Oak Rd, Riverdale', 'Penicillin'),
('Timmy', 'Turner', '2018-03-15', 'Medicaid', 'Male', 'timmy@turner.com', '555-7001', 'Vicky Turner', '555-7002', 'B+', '789 Maple Dr', NULL);

INSERT INTO Appointments (PatientID, DoctorID, AppointmentDate, AppointmentTime, ReasonForVisit, Status) VALUES 
(1, 1, '2026-07-01', '09:30:00', 'Chest pain follow-up', 'Completed'),
(3, 2, '2026-07-05', '14:00:00', 'Routine pediatric checkup', 'Scheduled'),
(2, 1, '2026-07-02', '10:00:00', 'Arrhythmia consultation', 'Cancelled');

INSERT INTO Medical_Records (PatientID, DoctorID, AppointmentID, VisitDate, Symptoms, Diagnosis, TreatmentPlan, ClinicalNotes) VALUES 
(1, 1, 1, '2026-07-01', 'Mild chest tightness during exercise', 'Stable Angina', 'Prescribed Beta-Blockers. Low sodium diet.', 'Patient responding well to lifestyle adjustments.'),
(2, 3, NULL, '2026-07-02', 'Acute appendicitis symptoms, severe lower right quadrant pain', 'Appendicitis', 'Emergency Appendectomy scheduled immediately', 'ER walk-in. Transferred to surgery floor.');

INSERT INTO Admissions (PatientID, RoomID, AttendingDoctorID, AdmissionDate, DischargeDate, AdmissionReason, DischargeNotes) VALUES 
(2, 4, 3, '2026-07-02 02:15:00', NULL, 'Post-op appendectomy recovery', NULL),
(1, 2, 1, '2026-06-25 10:00:00', '2026-06-28 14:00:00', 'Observation for cardiovascular monitoring', 'Discharged safely with prescription updates.');

INSERT INTO Medications (Name, GenericName, BrandName, DosageForm, Strength, UnitPrice) VALUES 
('Metoprolol', 'Metoprolol Succinate', 'Toprol XL', 'Tablet', '50mg', 0.75),
('Amoxicillin', 'Amoxicillin Trihydrate', 'Amoxil', 'Capsule', '500mg', 0.40),
('Ibuprofen', 'Ibuprofen', 'Advil', 'Tablet', '400mg', 0.15);

INSERT INTO Inventory (MedicationID, BatchNumber, StockQuantity, ExpiryDate, SupplierName) VALUES 
(1, 'BATCH-M12', 500, '2028-12-01', 'PharmaCorp Inc'),
(2, 'BATCH-A99', 1200, '2027-06-15', 'Global Meds Dist'),
(3, 'BATCH-I44', 2000, '2029-01-20', 'PharmaCorp Inc');

INSERT INTO Prescription_Details (RecordID, MedicationID, DosageInstructions, Duration, QuantityDispensed) VALUES 
(1, 1, 'Take 1 tablet daily in the morning', 30, 30),
(2, 3, 'Take 1 tablet every 6 hours as needed for pain', 7, 28);

INSERT INTO Lab_Tests (TestCategory, Cost) VALUES 
('Bloodwork', 45.00),
('Bloodwork', 60.00),
('Radiology', 120.00);

INSERT INTO Lab_Requests (PatientID, DoctorID, TestID, RequestDate, Status) VALUES 
(1, 1, 2, '2026-07-01 09:45:00', 'Completed'),
(2, 3, 1, '2026-07-02 01:00:00', 'Processing');
GO

INSERT INTO Lab_Results (RequestID, LabTechnicianID, ResultValue, ResultDate, PathologistNotes) VALUES 
(1, 5, 'Elevated LDL', '2026-07-01 13:00:00', 'Slightly elevated LDL cholesterol. Recommend dietary control and lifestyle follow-up.'),
(2, 5, 'Normal WBC', '2026-07-02 04:30:00', 'White blood cell count is within healthy boundaries. No acute infection indicated.');

INSERT INTO Billing (PatientID, AppointmentID, AdmissionID, InvoiceDate, TotalAmount, InsuranceCoverageAmount, PatientResponsibilityAmount, PaymentStatus) VALUES 
(1, 1, NULL, '2026-07-01', 105.00, 80.00, 25.00, 'Paid'),
(2, NULL, 1, '2026-07-02', 2450.00, 2000.00, 450.00, 'Unpaid');
GO

