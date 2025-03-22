namespace CardProccesing
{
    static class TransactionProcessor
    {
        public static void DepositToCard(ICard card, decimal amount)
        {
            if (card == null)
                throw new ArgumentNullException(nameof(card));
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive", nameof(amount));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Depositing {amount} to card {card.Number} ({card.GetType().Name})");
            card.Deposit(amount);
            Console.WriteLine("Deposit successful");
            Console.ResetColor();
        }

        public static void WithdrawFromCard(ICard card, decimal amount)
        {
            if (card == null)
                throw new ArgumentNullException(nameof(card));
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive", nameof(amount));

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Withdrawing {amount} from card {card.Number} ({card.GetType().Name})");
            card.Withdraw(amount);
            Console.WriteLine("Withdrawal successful");
            Console.ResetColor();
        }

        public static void Transfer(ICard source, ICard destination, decimal amount)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive", nameof(amount));

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Transferring {amount} from card {source.Number} ({source.GetType().Name}) to card {destination.Number} ({destination.GetType().Name})");
            source.Withdraw(amount);
            destination.Deposit(amount);
            Console.WriteLine("Transfer successful");
            Console.ResetColor();
        }
    }
}
