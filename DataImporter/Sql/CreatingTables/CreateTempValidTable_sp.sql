create procedure CreateTempValidTable_sp @BatchId uniqueidentifier
as
begin
    set nocount on;

    if object_id('tempdb..#Valid') is not null
        drop table #Valid;

    select s.Id                                                                 as StageRowId,
           ltrim(rtrim(s.CategoryName))                                         as CategoryName,
           iif(try_convert(bit, ltrim(rtrim(s.CategoryIsActiveRaw))) = 1, 0, 1) as CategoryIsDeleted,
           ltrim(rtrim(s.ProductCode))                                          as ProductCode,
           ltrim(rtrim(s.ProductName))                                          as ProductName,
           try_convert(money, ltrim(rtrim(s.PriceRaw)))                         as Price,
           try_convert(int, ltrim(rtrim(s.QuantityRaw)))                        as Quantity,
           iif(try_convert(bit, ltrim(rtrim(s.ProductIsActiveRaw))) = 1, 0, 1)  as ProductIsDeleted
    into #Valid
    from StagingTable s
    where s.BatchId = @BatchId
      and not exists (select 1
                      from ImportErrors e
                      where e.BatchId = @BatchId
                        and e.StageRowId = s.Id);
end
go