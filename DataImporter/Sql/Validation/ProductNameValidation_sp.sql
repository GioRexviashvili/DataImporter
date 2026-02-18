create procedure ProductNameValidation_sp @BatchId uniqueidentifier
as
begin
    set nocount on;

    insert into dbo.ImportErrors
        (BatchId, StageRowId, LineNumber, FieldName, RawValue, Reason)
    select s.BatchId,
           s.Id,
           s.LineNumber,
           N'ProductName',
           left(ltrim(rtrim(s.ProductName)), 100),
           N'ProductName is not valid: empty or more than 100 characters long.'
    from dbo.StagingTable s
    where s.BatchId = @BatchId
      and (
        ltrim(rtrim(s.ProductName)) = N''
            or len(ltrim(rtrim(s.ProductName))) > 100
        );
end