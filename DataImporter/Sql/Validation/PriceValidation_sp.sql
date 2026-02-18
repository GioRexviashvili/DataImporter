create procedure PriceValidation_sp @BatchId uniqueidentifier
as
begin
    set nocount on;

    insert into dbo.ImportErrors
        (BatchId, StageRowId, LineNumber, FieldName, RawValue, Reason)
    select s.BatchId,
           s.Id,
           s.LineNumber,
           N'PriceRaw',
           s.PriceRaw,
           N'Price is not valid: cast to money failed.'
    from dbo.StagingTable s
    where s.BatchId = @BatchId
      and try_convert(money, ltrim(rtrim(s.PriceRaw))) is null;
end