create table ImportErrors
(
    Id         int identity (1,1) primary key,
    BatchId    uniqueidentifier not null,
    StageRowId int,
    LineNumber int              not null,
    FieldName  nvarchar(100)    not null,
    RawValue   nvarchar(100)    not null,
    Reason     nvarchar(500)    not null,
    CreatedAt  datetime         not null default getdate(),
    constraint FK_ImportErrors_StagingTable foreign key (stageRowId) references StagingTable (Id) on delete cascade,

    index IX_ImportErrors_BatchId (BatchId),
    index IX_ImportErrors_StageRowId (StageRowId),
    index IX_ImportErrors_Batch_Line (BatchId, LineNumber)
)