create procedure QuantityValidation_sp @BatchId uniqueidentifier
as
begin
    set nocount on;

    insert into dbo.ImportErrors
        (BatchId, StageRowId, LineNumber, FieldName, RawValue, Reason)
    select s.BatchId,
           s.Id,
           s.LineNumber,
           N'QuantityRaw',
           s.QuantityRaw,
           N'Quantity is not valid: cast to int failed.'
    from dbo.StagingTable s
    where s.BatchId = @BatchId
      and try_convert(int, ltrim(rtrim(s.QuantityRaw))) is null;
end