create procedure CategoryNameValidation_sp @BatchId uniqueidentifier
as
begin
    set nocount on;

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
end