--create database OnlineExamSystem

--use OnlineExamSystem

----Creating Admin Table

--create table Admin(admin_id int identity(1,1) primary key,
--					admin_name nvarchar(30) not null,
--					phone nvarchar(20) not null,
--					address nvarchar(200) not null)

----Creating Student Table

--create table Student(stu_id int identity(101,1) primary key,
--						stu_name nvarchar(20) not null,
--						mobile nvarchar(20) not null,
--						city nvarchar(30) not null,
--						State nvarchar(20) not null,
--						DOB date not null,
--						Qualification nvarchar(20) not null,
--						Completion nvarchar(20) not null)

----Creating User Table

--create table Users(user_Id int identity(1,1) primary key,
--					email nvarchar(30) not null,
--					password nvarchar(40) not null,
--					role nvarchar(25) default('student') not null,
--					reference_Id int not null)

----Creating Table Courses

--create table courses(course_Id int identity(1,1) primary key,
--						course_name nvarchar(100) not null,
--						status bit default 1)

----Creating table Levels

--Create table Levels(level_id int identity(1,1) primary key,
--					course_id int foreign key references courses(course_id),
--					level_number int not null,
--					level_name nvarchar(30) not null,
--					passing_marks int default 20 not null,
--					tot_ques int not null,
--					duration int default 5 not null)

----Create Table Questions

--create table Questions(QuestionId int identity(1,1) primary key,
--						CourseId int foreign key references courses(course_id),
--						LevelNumber int foreign key references Levels(Level_id),
--						QuestionText nvarchar(max) not null,
--						OptionA nvarchar(200) not null,
--						OptionB nvarchar(200) not null,
--						OptionC nvarchar(200) not null,
--						OptionD nvarchar(200) not null,
--						Answer nvarchar(1) not null check (Answer in('a','b','c','d')),
--						Marks int not null default 1,
--						Status bit not null default 1)

----Create table ExamAttempts

--create table ExamAttempts(attempt_id int identity(1,1) primary key,
--							user_id int foreign key references Users(user_id),
--							course_id int foreign key references courses(course_id),
--							level_number int not null,
--							total_questions int not null,
--							correct_answers int not null,
--							score int not null,
--							total_time int not null,
--							time_taken int not null,
--							is_passed bit not null)

----Creating table UserAnswers

--create table UserAnswers(answer_id int identity(1,1) primary key,
--							attempt_id int foreign key references ExamAttempts(attempt_id),
--							question_id int foreign key references Questions(QuestionId),
--							selected_option nvarchar(1) check (selected_option in('A','B','C','D')),
--							is_correct bit not null)

----Creating table StudentProgress

--create table StudentProgress(progress_id int identity(1,1) primary key,
--								user_id int foreign key references Users(user_id),
--								course_id int foreign key references courses(course_id),
--								highest_level_passed int default 0,
--								is_completed bit default 0)

----Creating Table Exam Reports

--create table ExamReports(report_id int identity(1,1) primary key,
--							attempt_id int foreign key references ExamAttempts(attempt_id),
--							user_id int foreign key references Users(user_id),
--							course_id int foreign key references courses(course_id),
--							level_number int not null,
--							total_marks int not null,
--							passing_marks int not null,
--							score int not null,
--							is_passed bit not null,
--							total_time int not null,
--							time_taken int not null)



--insert into Admin(admin_name, phone, address) values ('Admin_Rahul', '9876543210', 'New York, USA')

--insert into Users(email, password, role, reference_Id) values ('admin123@gmail.com', 'Admin@123', 'admin', 1)


--select * from Admin
--select * from Users
--select * from Student
--select * from courses
--select * from Levels
--select * from Questions
--select * from ExamAttempts
--select * from UserAnswers
--select * from StudentProgress
--select * from ExamReports

--drop table ExamReports
--drop table StudentProgress
--drop table UserAnswers
--drop table ExamAttempts
--drop table Questions
--drop table Levels
--drop table courses


-- Create Database
IF DB_ID('OnlineExamSystem') IS NOT NULL
    DROP DATABASE OnlineExamSystem;
GO

CREATE DATABASE OnlineExamSystem;
GO

USE OnlineExamSystem;
GO

---------------------------------------------------
-- DROP TABLES if they exist (in reverse dependency order)
---------------------------------------------------
IF OBJECT_ID('ExamReports', 'U') IS NOT NULL DROP TABLE ExamReports;
IF OBJECT_ID('StudentProgress', 'U') IS NOT NULL DROP TABLE StudentProgress;
IF OBJECT_ID('UserAnswers', 'U') IS NOT NULL DROP TABLE UserAnswers;
IF OBJECT_ID('ExamAttempts', 'U') IS NOT NULL DROP TABLE ExamAttempts;
IF OBJECT_ID('Questions', 'U') IS NOT NULL DROP TABLE Questions;
IF OBJECT_ID('Levels', 'U') IS NOT NULL DROP TABLE Levels;
IF OBJECT_ID('Courses', 'U') IS NOT NULL DROP TABLE Courses;
IF OBJECT_ID('Users', 'U') IS NOT NULL DROP TABLE Users;
IF OBJECT_ID('Student', 'U') IS NOT NULL DROP TABLE Student;
IF OBJECT_ID('Admin', 'U') IS NOT NULL DROP TABLE Admin;
GO

---------------------------------------------------
-- Creating Tables
---------------------------------------------------

-- Admin Table
CREATE TABLE Admin(
    admin_id INT IDENTITY(1,1) PRIMARY KEY,
    admin_name NVARCHAR(30) NOT NULL,
    phone NVARCHAR(20) NOT NULL,
    address NVARCHAR(200) NOT NULL
);

-- Student Table
CREATE TABLE Student(
    stu_id INT IDENTITY(101,1) PRIMARY KEY,
    stu_name NVARCHAR(20) NOT NULL,
    mobile NVARCHAR(20) NOT NULL,
    city NVARCHAR(30) NOT NULL,
    state NVARCHAR(20) NOT NULL,
    DOB DATE NOT NULL,
    Qualification NVARCHAR(20) NOT NULL,
    Completion NVARCHAR(20) NOT NULL
);

-- Users Table
CREATE TABLE Users(
    user_Id INT IDENTITY(1,1) PRIMARY KEY,
    email NVARCHAR(30) NOT NULL,
    password NVARCHAR(40) NOT NULL,
    role NVARCHAR(25) NOT NULL DEFAULT('student'),
    reference_Id INT NOT NULL
);

-- Courses Table
CREATE TABLE Courses(
    course_Id INT IDENTITY(1,1) PRIMARY KEY,
    course_name NVARCHAR(100) NOT NULL,
    status BIT DEFAULT 1
);

-- Levels Table
CREATE TABLE Levels(
    level_id INT IDENTITY(1,1) PRIMARY KEY,
    course_id INT FOREIGN KEY REFERENCES Courses(course_id),
    level_number INT NOT NULL,
    level_name NVARCHAR(30) NOT NULL,
    passing_marks INT NOT NULL DEFAULT 20,
    tot_ques INT NOT NULL,
    duration INT NOT NULL DEFAULT 5
);

-- Questions Table
CREATE TABLE Questions(
    QuestionId INT IDENTITY(1,1) PRIMARY KEY,
    CourseId INT FOREIGN KEY REFERENCES Courses(course_id),
    LevelNumber INT FOREIGN KEY REFERENCES Levels(level_id),
    QuestionText NVARCHAR(MAX) NOT NULL,
    OptionA NVARCHAR(200) NOT NULL,
    OptionB NVARCHAR(200) NOT NULL,
    OptionC NVARCHAR(200) NOT NULL,
    OptionD NVARCHAR(200) NOT NULL,
    Answer NVARCHAR(1) NOT NULL CHECK (Answer IN('A','B','C','D')),
    Marks INT NOT NULL DEFAULT 1,
    Status BIT NOT NULL DEFAULT 1
);

-- ExamAttempts Table
CREATE TABLE ExamAttempts(
    attempt_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT FOREIGN KEY REFERENCES Users(user_id),
    course_id INT FOREIGN KEY REFERENCES Courses(course_id),
    level_number INT NOT NULL,
    total_questions INT NOT NULL,
    correct_answers INT NOT NULL,
    score INT NOT NULL,
    total_time INT NOT NULL,
    time_taken INT NOT NULL,
    is_passed BIT NOT NULL
);

-- UserAnswers Table
CREATE TABLE UserAnswers(
    answer_id INT IDENTITY(1,1) PRIMARY KEY,
    attempt_id INT FOREIGN KEY REFERENCES ExamAttempts(attempt_id),
    question_id INT FOREIGN KEY REFERENCES Questions(QuestionId),
    selected_option NVARCHAR(1) CHECK (selected_option IN('A','B','C','D')),
    is_correct BIT NOT NULL
);

-- StudentProgress Table
CREATE TABLE StudentProgress(
    progress_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT FOREIGN KEY REFERENCES Users(user_id),
    course_id INT FOREIGN KEY REFERENCES Courses(course_id),
    highest_level_passed INT DEFAULT 0,
    is_completed BIT DEFAULT 0
);

-- ExamReports Table
CREATE TABLE ExamReports(
    report_id INT IDENTITY(1,1) PRIMARY KEY,
    attempt_id INT FOREIGN KEY REFERENCES ExamAttempts(attempt_id),
    user_id INT FOREIGN KEY REFERENCES Users(user_id),
    course_id INT FOREIGN KEY REFERENCES Courses(course_id),
    level_number INT NOT NULL,
    total_marks INT NOT NULL,
    passing_marks INT NOT NULL,
    score INT NOT NULL,
    is_passed BIT NOT NULL,
    total_time INT NOT NULL,
    time_taken INT NOT NULL
);

---------------------------------------------------
-- Insert Default Data
---------------------------------------------------
INSERT INTO Admin(admin_name, phone, address) 
VALUES ('Admin_Rahul', '9876543210', 'New York, USA');

INSERT INTO Users(email, password, role, reference_Id) 
VALUES ('admin123@gmail.com', 'Admin@123', 'admin', 1);

---------------------------------------------------
-- Select Data
---------------------------------------------------
SELECT * FROM Admin;
SELECT * FROM Users;
SELECT * FROM Student;
SELECT * FROM Courses;
SELECT * FROM Levels;
SELECT * FROM Questions;
SELECT * FROM ExamAttempts;
SELECT * FROM UserAnswers;
SELECT * FROM StudentProgress;
SELECT * FROM ExamReports;

