create procedure dbo.CleanUpImportErrors_sp @BatchId uniqueidentifier
as
begin
    set nocount on;

    delete
    from dbo.ImportErrors
    where BatchId = @BatchId;
end