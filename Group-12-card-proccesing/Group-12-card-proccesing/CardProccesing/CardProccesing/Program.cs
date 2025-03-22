namespace CardProccesing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //VisaCard card1 = new("1234567890123456", "John Doe", DateTime.Now.AddYears(2), "123");
            //VisaCard card2 = new("6543210987654321", "Jane Doe", DateTime.Now.AddYears(1), "321");

            //TransactionProcessor.DepositToCard(card1, 1000);
            //Console.WriteLine();
            //TransactionProcessor.DepositToCard(card2, 500);
            //Console.WriteLine();
            //TransactionProcessor.Transfer(card1, card2, 300);
            //Console.WriteLine();

            //Console.WriteLine($"Card 1 balance: {card1.Balance}");
            //Console.WriteLine($"Card 2 balance: {card2.Balance}");

            MasterCard card3 = new("1233367890123456000", "John Smith", DateTime.Now.AddYears(2), "9846");
            MasterCard card4 = new("1234000890123456000", "Jane Smith", DateTime.Now.AddYears(2), "5746");

            TransactionProcessor.DepositToCard(card3, 1000);
            Console.WriteLine();
            TransactionProcessor.DepositToCard(card4, 500);
            Console.WriteLine();
            TransactionProcessor.Transfer(card3, card4, 300);
            Console.WriteLine();

            Console.WriteLine($"Card 3 balance: {card3.Balance}");
            Console.WriteLine($"Card 4 balance: {card4.Balance}");

        }
    }
}
