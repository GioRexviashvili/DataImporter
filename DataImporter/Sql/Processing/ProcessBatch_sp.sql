create procedure ProcessBatch_sp @BatchId uniqueidentifier
as
begin
    set nocount on;

    begin try
        begin transaction;

        exec CleanUpImportErrors_sp @BatchId;

        exec CategoryIsActiveValidation_sp @BatchId;
        exec ProductIsActiveValidation_sp @BatchId;
        exec ProductCodeValidation_sp @BatchId;
        exec CategoryNameValidation_sp @BatchId;
        exec ProductNameValidation_sp @BatchId;
        exec PriceValidation_sp @BatchId;
        exec QuantityValidation_sp @BatchId;

        if object_id('tempdb..#Valid') is not null
            drop table #Valid;

        select
            s.Id as StageRowId,
            ltrim(rtrim(s.CategoryName)) as CategoryName,
            iif(try_convert(bit, ltrim(rtrim(s.CategoryIsActiveRaw))) = 1, 0, 1) as CategoryIsDeleted,
            ltrim(rtrim(s.ProductCode)) as ProductCode,
            ltrim(rtrim(s.ProductName)) as ProductName,
            try_convert(money, ltrim(rtrim(s.PriceRaw))) as Price,
            try_convert(int, ltrim(rtrim(s.QuantityRaw))) as Quantity,
            iif(try_convert(bit, ltrim(rtrim(s.ProductIsActiveRaw))) = 1, 0, 1) as ProductIsDeleted
        into #Valid
        from StagingTable s
        where s.BatchId = @BatchId
          and not exists (
            select 1
            from ImportErrors e
            where e.BatchId = @BatchId
              and e.StageRowId = s.Id
        );

        exec ProcessCategories_sp;
        exec ProcessProducts_sp;

        commit transaction;
    end try
    begin catch
        if @@trancount > 0
            rollback transaction;
        throw;
    end catch;

    declare @stagingRows int;
    declare @invalidRows int;
    declare @errorsCount int;

    select @stagingRows = count(*)
    from StagingTable
    where BatchId = @BatchId;

    select @invalidRows = count(distinct StageRowId)
    from ImportErrors
    where BatchId = @BatchId;

    select @errorsCount = count(*)
    from ImportErrors
    where BatchId = @BatchId;

    select @BatchId as BatchId,
           @stagingRows as StagingRows,
           (@stagingRows - @invalidRows) as ValidRows,
           @invalidRows as InvalidRows,
           @errorsCount as ErrorsCount;
end