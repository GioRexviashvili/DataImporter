create procedure ProductIsActiveValidation_sp @BatchId uniqueidentifier
as
begin 
    set nocount on;

    delete
    from ImportErrors
    where BatchId = @BatchId
      and FieldName = N'ProductIsActiveRaw';

    insert into ImportErrors
    (BatchId, StageRowId, LineNumber, FieldName, RawValue, Reason)
    select s.BatchId,
           s.Id,
           s.LineNumber,
           N'ProductIsActiveRaw',
           s.ProductIsActiveRaw,
           N'Expected bit value'
    from StagingTable s
    where s.BatchId = @BatchId
      and try_convert(bit, ltrim(rtrim(s.ProductIsActiveRaw))) is null
end