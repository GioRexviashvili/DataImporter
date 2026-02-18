create procedure ProductCodeValidation_sp @BatchId uniqueidentifier
as
begin
    set nocount on;

    -- check for empty or too long codes
    insert into ImportErrors
        (BatchId, StageRowId, LineNumber, FieldName, RawValue, Reason)
    select s.BatchId,
           s.Id,
           s.LineNumber,
           N'ProductCode',
           s.ProductCode,
           N'ProductCode is not valid: empty or more than 10 characters long.'
    from StagingTable s
    where s.BatchId = @BatchId
      and (ltrim(rtrim(s.ProductCode)) = N''
        or len(ltrim(rtrim(s.ProductCode))) > 10);

    -- check for duplicates
    insert into ImportErrors
        (BatchId, StageRowId, LineNumber, FieldName, RawValue, Reason)
    select s.BatchId,
           s.Id,
           s.LineNumber,
           N'ProductCode',
           s.ProductCode,
           N'ProductCode is not unique.'
    from StagingTable s
             join (select ltrim(rtrim(ProductCode)) as CodeTrim
                   from StagingTable
                   where BatchId = @BatchId
                     and ltrim(rtrim(ProductCode)) <> N''
                     and len(ltrim(rtrim(ProductCode))) <= 10
                   group by ltrim(rtrim(ProductCode))
                   having count(*) > 1) d
                  on ltrim(rtrim(s.ProductCode)) = d.CodeTrim
    where s.BatchId = @BatchId;
end