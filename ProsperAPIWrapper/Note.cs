using System;

namespace ProsperAPIWrapper
{
    public class Note
    {
        public int AgeInMonths { get; set; }
        public decimal AmountParticipation { get; set; }
        public decimal BorrowerRate { get; set; }
        public decimal DaysPastDue { get; set; }
        public decimal DebtSaleProceedsReceived { get; set; }
        public decimal GroupLeaderReward { get; set; }
        public decimal InterestPaid { get; set; }
        public bool IsSold { get; set; }
        public decimal LateFees { get; set; }
        public int ListingNumber { get; set; }
        public string LoanNoteID { get; set; }
        public int LoanNumber { get; set; }
        public decimal NextPaymentDueAmount { get; set; }
        public DateTime? NextPaymentDueDate { get; set; }
        public int? NoteDefaultReason { get; set; }
        public string NoteDefaultReasonDescription { get; set; }
        public int NoteStatus { get; set; }
        public string NoteStatusDescription { get; set; }
        public DateTime OriginationDate { get; set; }
        public decimal PlatformFeesPaid { get; set; }
        public decimal PlatformProceedsGrossReceived { get; set; }
        public decimal PrincipalBalance{ get; set; }
        public decimal PrincipalRepaid { get; set; }
        public decimal ProsperFees { get; set; }
        public string ProsperRating { get; set; }
        public decimal ServiceFees { get; set; }
        public int Term { get; set; }
        public decimal TotalAmountBorrowed { get; set; }
    }
}
