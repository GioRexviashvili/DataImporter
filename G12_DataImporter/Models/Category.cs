namespace G12_DataImporter.Models;

public sealed class Category
{
    public Category(string name, bool isActive)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        Name = name;
        IsActive = isActive;
    }

    public string Name { get; }
    public bool IsActive { get; }
    public ICollection<Product> Products { get; } = new List<Product>();

    public override string ToString() => $"{Name} {IsActive}";
}