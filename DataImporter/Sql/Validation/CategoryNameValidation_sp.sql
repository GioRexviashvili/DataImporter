create procedure CategoryNameValidation_sp @BatchId uniqueidentifier
as
begin
    set nocount on;

    -- check for empty or too long codes
    insert into dbo.ImportErrors
        (BatchId, StageRowId, LineNumber, FieldName, RawValue, Reason)
    select s.BatchId,
           s.Id,
           s.LineNumber,
           N'CategoryName',
           left(ltrim(rtrim(s.CategoryName)), 100),
           N'CategoryName is not valid: empty or more than 100 characters long.'
    from dbo.StagingTable s
    where s.BatchId = @BatchId
      and (
        ltrim(rtrim(s.CategoryName)) = N''
            or len(ltrim(rtrim(s.CategoryName))) > 100
        );

    -- check for duplicates
    insert into ImportErrors
        (BatchId, StageRowId, LineNumber, FieldName, RawValue, Reason)
    select s.BatchId,
           s.Id,
           s.LineNumber,
           N'CategoryName',
           s.CategoryName,
           N'CategoryName is not unique.'
    from StagingTable s
             join (select ltrim(rtrim(CategoryName)) as Name
                   from StagingTable
                   where BatchId = @BatchId
                     and ltrim(rtrim(CategoryName)) <> N''
                     and len(ltrim(rtrim(CategoryName))) <= 100
                   group by ltrim(rtrim(CategoryName))
                   having count(*) > 1) d
                  on ltrim(rtrim(s.CategoryName)) = d.Name
    where s.BatchId = @BatchId;
end