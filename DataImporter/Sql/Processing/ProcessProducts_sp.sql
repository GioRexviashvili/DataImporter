create procedure ProcessProducts_sp
as
begin
    set nocount on;

    -- insert new products
    insert into Products (CategoryID, Code, Name, Price, Quantity, IsDeleted)
    select c.ID,
           v.ProductCode,
           v.ProductName,
           v.Price,
           v.Quantity,
           v.ProductIsDeleted
    from #Valid v
             join Categories c
                  on c.Name = v.CategoryName
    where not exists (select 1
                      from Products p
                      where p.Code = v.ProductCode);

    -- update changed products
    update p
    set p.CategoryID = c.ID,
        p.Name       = v.ProductName,
        p.Price      = v.Price,
        p.Quantity   = v.Quantity,
        p.IsDeleted  = v.ProductIsDeleted,
        p.UpdateDate = getdate()
    from Products p
             join #Valid v
                  on v.ProductCode = p.Code
             join Categories c
                  on c.Name = v.CategoryName
    where p.CategoryID != c.ID
       or p.Name != v.ProductName
       or p.Price != v.Price
       or p.Quantity != v.Quantity
       or p.IsDeleted != v.ProductIsDeleted;
end