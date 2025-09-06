create database OnlineExamSystem

use OnlineExamSystem

--Creating Admin Table

create table Admin(admin_id int identity(1,1) primary key,
					admin_name nvarchar(30) not null,
					phone nvarchar(20) not null,
					address nvarchar(200) not null)

--Creating Student Table

create table Student(stu_id int identity(101,1) primary key,
						stu_name nvarchar(20) not null,
						mobile nvarchar(20) not null,
						city nvarchar(30) not null,
						State nvarchar(20) not null,
						DOB date not null,
						Qualification nvarchar(20) not null,
						Completion nvarchar(20) not null)

--Creating User Table

create table Users(user_Id int identity(1,1) primary key,
					email nvarchar(30) not null,
					password nvarchar(40) not null,
					role nvarchar(25) default('student') not null,
					reference_Id int not null)

--Creating Table Courses

create table courses(course_Id int identity(1,1) primary key,
						course_name nvarchar(100) not null,
						status bit default 1)

--Creating table Levels

Create table Levels(level_id int identity(1,1) primary key,
					course_id int foreign key references courses(course_id),
					level_number int not null,
					level_name nvarchar(30) not null,
					passing_marks int default 20 not null,
					tot_ques int not null,
					duration int default 5 not null)

--Create Table Questions

create table Questions(QuestionId int identity(1,1) primary key,
						CourseId int foreign key references courses(course_id),
						LevelNumber int foreign key references Levels(level_id),
						QuestionText nvarchar(max) not null,
						OptionA nvarchar(200) not null,
						OptionB nvarchar(200) not null,
						OptionC nvarchar(200) not null,
						OptionD nvarchar(200) not null,
						Answer nvarchar(1) not null check (Answer in('a','b','c','d')),
						Marks int not null default 1,
						Status bit not null default 1)

--Create table ExamAttempts

create table ExamAttempts(attempt_id int identity(1,1) primary key,
							user_id int foreign key references Users(user_id),
							course_id int foreign key references courses(course_id),
							level_number int not null,
							total_questions int not null,
							correct_answers int not null,
							score int not null,
							total_time int not null,
							time_taken int not null,
							is_passed bit not null)

--Creating table UserAnswers

create table UserAnswers(answer_id int identity(1,1) primary key,
							attempt_id int foreign key references ExamAttempts(attempt_id),
							question_id int foreign key references Questions(QuestionId),
							selected_option nvarchar(1) check (selected_option in('A','B','C','D')),
							is_correct bit not null)

--Creating table StudentProgress

create table StudentProgress(progress_id int identity(1,1) primary key,
								user_id int foreign key references Users(user_id),
								course_id int foreign key references courses(course_id),
								highest_level_passed int default 0,
								is_completed bit default 0)

--Creating Table Exam Reports

create table ExamReports(report_id int identity(1,1) primary key,
							attempt_id int foreign key references ExamAttempts(attempt_id),
							user_id int foreign key references Users(user_id),
							course_id int foreign key references courses(course_id),
							level_number int not null,
							total_marks int not null,
							passing_marks int not null,
							score int not null,
							is_passed bit not null,
							total_time int not null,
							time_taken int not null)



insert into Admin(admin_name, phone, address) values ('Admin_Rahul', '9876543210', 'New York, USA')

insert into Users(email, password, role, reference_Id) values ('admin123@gmail.com', 'Admin@123', 'admin', 1)


select * from Admin
select * from Users
select * from Student
select * from courses
select * from Levels
select * from Questions

drop table ExamReports
drop table StudentProgress
drop table UserAnswers
drop table ExamAttempts
drop table Questions
drop table Levels
drop table courses

