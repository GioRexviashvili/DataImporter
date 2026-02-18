create procedure dbo.GetBatchErrors_sp @BatchId uniqueidentifier
as
begin
    set nocount on;

    select e.Id,
           e.BatchId,
           e.StageRowId,
           e.LineNumber,
           e.FieldName,
           e.RawValue,
           e.Reason,
           e.CreatedAt
    from dbo.ImportErrors e
    where e.BatchId = @BatchId
    order by e.LineNumber
end