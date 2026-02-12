namespace G12_DataImporter.Models;

public sealed class Product
{
    public Product(string code, string name, decimal price, int quantity, bool isActive)
    {
        ArgumentException.ThrowIfNullOrEmpty(code);
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentOutOfRangeException.ThrowIfNegative(price, nameof(price));
        ArgumentOutOfRangeException.ThrowIfNegative(quantity, nameof(quantity));

        Code = code;
        Name = name;
        Price = price;
        Quantity = quantity;
        IsActive = isActive;
    }

    public string Code { get; }
    public string Name { get; }
    public decimal Price { get; }
    public int Quantity { get; }
    public bool IsActive { get; }

    public override string ToString() => $"{Code} {Name} {Price} {Quantity} {IsActive}";
}