namespace CardProccesing;

interface ICard
{
    string Number { get; }
    string HolderName { get; }
    DateTime ExpiryDate { get; }
    string CVV { get; }
    decimal Balance { get; }
    decimal WithdrawTax { get; }
    void Deposit(decimal amount);
    void Withdraw(decimal amount);
}
