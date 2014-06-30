
namespace ProsperAPIWrapper
{
    public class InvestResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public int ListingId { get; set; }
        public decimal RequestedAmount { get; set; }
        public decimal AmountInvested { get; set; }
    }
}
