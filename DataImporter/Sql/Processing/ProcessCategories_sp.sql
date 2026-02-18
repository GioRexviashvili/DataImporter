create procedure ProcessCategories_sp
as
begin
    set nocount on;

    -- insert new categories
    insert into Categories (Name, IsDeleted)
    select x.CategoryName,
           x.CategoryIsDeleted
    from (select v.CategoryName,
                 max(v.CategoryIsDeleted) as CategoryIsDeleted
          from #Valid v
          group by v.CategoryName) x
    where not exists (select 1
                      from Categories c
                      where c.Name = x.CategoryName);

    -- update categories only if IsDeleted changed
    update c
    set c.IsDeleted  = x.CategoryIsDeleted,
        c.UpdateDate = getdate()
    from Categories c
             join (select v.CategoryName,
                          max(v.CategoryIsDeleted) as CategoryIsDeleted
                   from #Valid v
                   group by v.CategoryName) x on x.CategoryName = c.Name
    where c.IsDeleted != x.CategoryIsDeleted;
end