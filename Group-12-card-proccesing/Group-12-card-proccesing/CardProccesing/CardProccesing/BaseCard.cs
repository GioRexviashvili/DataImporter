namespace CardProccesing;

abstract class BaseCard : ICard
{
    public BaseCard(string number, string holderName, DateTime expiryDate, string cvv)
    {
        ValidateParameters(number, holderName, expiryDate, cvv);

        Number = number;
        HolderName = holderName;
        ExpiryDate = expiryDate;
        CVV = cvv;
    }

    public string Number { get; }
    public string HolderName { get; }
    public DateTime ExpiryDate { get; }
    public string CVV { get; }
    public decimal Balance { get; private set; }
    public abstract decimal WithdrawTax { get; }
    protected virtual int ValidNumberLength => 16;
    protected virtual int ValidCVVLength => 3;


    public void Deposit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive", nameof(amount));
        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive", nameof(amount));

        amount = amount + GetTaxAmount(amount);
        if (Balance < amount)
            throw new InvalidOperationException("Insufficient funds");

        Balance -= amount;
    }

    protected decimal GetTaxAmount(decimal amount)
    {
        decimal taxAmount = amount * WithdrawTax;
        return taxAmount;
    }

    protected void ValidateParameters(string number, string holderName, DateTime expiryDate, string cvv)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("Card number cannot be empty", nameof(number));
        if (string.IsNullOrWhiteSpace(holderName))
            throw new ArgumentException("Holder name cannot be empty", nameof(holderName));
        if (string.IsNullOrWhiteSpace(cvv))
            throw new ArgumentException("CVV cannot be empty", nameof(cvv));
        if (expiryDate < DateTime.Now)
            throw new ArgumentException("Expiry date must be in the future", nameof(expiryDate));
        ValidateCardNumber(number);
        ValidateCVVLength(cvv);
    }

    protected virtual void ValidateCVVLength(string cvv)
    {
        if (cvv.Length != ValidCVVLength)
            throw new ArgumentException($"CV number must be {ValidCVVLength} digits", nameof(cvv));
    }

    protected virtual void ValidateCardNumber(string number)
    {
        if (number.Length != ValidNumberLength)
            throw new ArgumentException($"Card number must be {ValidNumberLength} digits", nameof(number));
    }
}