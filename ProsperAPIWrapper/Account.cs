namespace ProsperAPIWrapper
{
    public class Account
    {
        public decimal AvailableCashBalance { get; set; }
        public decimal OutstandingPrincipalOnActiveNotes { get; set; }
        public decimal PendingInvestmentsPrimaryMkt { get; set; }
        public decimal PendingInvestmentsSecondaryMkt { get; set; }
        public decimal PendingQuickInvestOrders { get; set; }
        public decimal TotalAccountValue { get; set; }
        public decimal TotalAmountInvestedOnActiveNotes { get; set; }
        public decimal TotalPrincipalReceivedOnActiveNotes { get; set; }
    }
}
