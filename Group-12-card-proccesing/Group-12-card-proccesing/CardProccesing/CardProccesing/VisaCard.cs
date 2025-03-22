namespace CardProccesing
{
    class VisaCard : BaseCard
    {
        public VisaCard(string number, string holderName, DateTime expiryDate, string cvv)
            : base(number, holderName, expiryDate, cvv)
        {
        }

        public override decimal WithdrawTax => 0.01m;
    }
}
