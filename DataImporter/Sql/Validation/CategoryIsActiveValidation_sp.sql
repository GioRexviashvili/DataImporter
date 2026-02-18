create procedure CategoryIsActiveValidation_sp @BatchId uniqueidentifier
as
begin
    set nocount on;

    delete
    from ImportErrors
    where BatchId = @BatchId
      and FieldName = N'CategoryIsActiveRaw';

    insert into ImportErrors
        (BatchId, StageRowId, LineNumber, FieldName, RawValue, Reason)
    select s.BatchId,
           s.Id,
           s.LineNumber,
           N'CategoryIsActiveRaw',
           s.CategoryIsActiveRaw,
           N'Expected bit value'
    from StagingTable s
    where s.BatchId = @BatchId
      and try_convert(bit, ltrim(rtrim(s.CategoryIsActiveRaw))) is null
end