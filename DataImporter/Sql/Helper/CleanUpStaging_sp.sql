create procedure dbo.CleanUpStaging_sp @BatchId uniqueidentifier
as
begin
    set nocount on;

    delete
    from dbo.StagingTable
    where BatchId = @BatchId;
end