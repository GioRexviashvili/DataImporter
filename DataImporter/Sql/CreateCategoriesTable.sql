create table Categories(
                           ID int primary key identity(1, 1),
                           Name nvarchar(100) not null unique,
                           Description nvarchar(500) null,
                           IsDeleted bit not null default(0),
                           CreateDate datetime not null default(getdate()),
                           UpdateDate datetime null
)