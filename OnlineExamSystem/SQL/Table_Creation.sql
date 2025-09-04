create database OnlineExamSystem

--Creating Admin Table

create table Admin(admin_id int identity(1,1) primary key,
					admin_name nvarchar(30) not null,
