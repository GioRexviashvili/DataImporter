create procedure ProcessBatch_sp @BatchId uniqueidentifier
as
begin
    set nocount on;

    begin try
        begin transaction;

        -- clean previous errors
        exec CleanUpImportErrors_sp @BatchId;

        -- run all validations
        exec CategoryIsActiveValidation_sp @BatchId;
        exec ProductIsActiveValidation_sp @BatchId;

        exec ProductCodeValidation_sp @BatchId;

        exec CategoryNameValidation_sp @BatchId;
        exec ProductNameValidation_sp @BatchId;

        exec PriceValidation_sp @BatchId;
        exec QuantityValidation_sp @BatchId;

        -- 3) build temp valid rows table
        exec CreateTempValidTable_sp @BatchId;

        -- 4) process valid data into final tables
        exec ProcessCategories_sp;
        exec ProcessProducts_sp;

        commit transaction;
    end try
    begin catch
        if @@trancount > 0
            rollback transaction;

        throw;
    end catch;

    -- 5) return summary for the app
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

    select @BatchId                      as BatchId,
           @stagingRows                  as StagingRows,
           (@stagingRows - @invalidRows) as ValidRows,
           @invalidRows                  as InvalidRows,
           @errorsCount                  as ErrorsCount;
end