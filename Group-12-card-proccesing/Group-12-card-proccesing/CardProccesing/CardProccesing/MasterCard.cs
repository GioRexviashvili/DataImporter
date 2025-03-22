namespace CardProccesing
{
    class MasterCard : BaseCard
    {
        public MasterCard(string number, string holderName, DateTime expiryDate, string cvv)
            : base(number, holderName, expiryDate, cvv)
        {
        }

        protected override int ValidNumberLength => 19;

        protected override int ValidCVVLength => 4;

        public override decimal WithdrawTax => 0.05m;
    }
}
