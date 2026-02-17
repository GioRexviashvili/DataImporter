namespace DataImporter.Models;

public class Categories
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public ICollection<Products> Products { get; set; } = new List<Products>();
}