create table StagingTable
(
    Id                  int primary key identity (1,1),
    BatchId             uniqueidentifier not null,
    LineNumber          int              not null,
    CategoryName        nvarchar(100)    not null,
    CategoryIsActiveRaw varchar(10)      not null,
    ProductCode         varchar(10)      not null,
    ProductName         nvarchar(100)    not null,
    PriceRaw            varchar(50)      not null,
    QuantityRaw         varchar(50)      not null,
    ProductIsActiveRaw  varchar(10)      not null,
    DateInserted        datetime         not null default getdate(),

    constraint UQ_StagingTable_BatchID_LineNumber unique (BatchId, LineNumber),

    Index IX_StagingTable_BatchId (BatchId),
    Index IX_StagingTable_CategoryName (BatchId, CategoryName), --we will need this for the join to check if CategoryIsActive is changed
    Index IX_StagingTable_ProductCode (BatchId, ProductCode)    -- we will need this for the join to check if any product attribute is changed
)