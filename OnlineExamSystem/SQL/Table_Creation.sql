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

insert into Admin(admin_name, phone, address) values ('Admin_Rahul', '9876543210', 'New York, USA')

insert into Users(email, password, role, reference_Id) values ('admin123@gmail.com', 'Admin@123', 'admin', 1)


select * from Admin
select * from Users
select * from Student
select * from courses
select * from Levels

drop table Levels
drop table courses