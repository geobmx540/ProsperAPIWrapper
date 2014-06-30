using System;

namespace ProsperAPI
{
    public class Investment
    {
        public string InvestmentKey { get; set; }
        public string MemberKey { get; set; }
        public int ListingNumber { get; set; }
        public int? LoanNumber { get; set; }
        public DateTime InvestmentDate { get; set; }
        public decimal AmountInvested { get; set; }
        public int ListingStatus { get; set; }
        public string ListingStatusDescription { get; set; }
    }
}
