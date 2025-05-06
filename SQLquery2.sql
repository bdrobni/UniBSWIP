use master
go

use diplomskidb
go

use dipwebapp
go

drop table mdlfile 
drop table article
drop table fileassociations
drop table tagassociations
drop table appuser
drop table authoredobj
drop database dipwebapp

create table appuser (
Id int not null primary key,
username nvarchar(50) not null,
email nvarchar(50) not null,
pass nvarchar(50) not null,
userrole nvarchar(10) not null
)

create table authoredobj(
Id int not null primary key,
title nvarchar(50) not null,
objdescription nvarchar(500),
objcontent nvarchar(max) not null,
createddate datetime not null,
authorid int not null,
filetype nvarchar(25) not null,
foreign key (authorid) references appuser(Id)
)


create table tag (
Id int not null primary key,
tagname nvarchar(25) not null,
tagdescription nvarchar(250) not null
)

create table tagassociations (
tagid int not null,
objectid int not null,
primary key (tagid, objectid),
foreign key (tagid) references tag (Id),
foreign key (objectid) references authoredobj (Id) 
)

create table fileassociations (
parentfileid int not null,
associatedid int not null,
primary key (parentfileid, associatedid),
foreign key (parentfileid) references authoredobj (Id),
foreign key (associatedid) references authoredobj (Id)
)

