create table Products(
                         ID int primary key identity(1, 1),
                         CategoryID int not null,
                         Code varchar(10) not null unique,
                         Name nvarchar(100) not null,
                         Price money not null,
                         Quantity int not null,
                         Description nvarchar(500) null,
                         IsDeleted bit not null default(0),
                         CreateDate datetime not null default(getdate()),
                         UpdateDate datetime null,
                         foreign key (CategoryID) references Categories(ID)
)