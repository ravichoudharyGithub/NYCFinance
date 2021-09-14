using System;

namespace NYCFinance.Model
{
    [Serializable]
    public class AccountHistoryDetails
    {
        public int Year { get; set; }
        public int Period { get; set; }
        public string ChargeType { get; set; }
        public string AccountId { get; set; }
        public string OriginalDueDate { get; set; }
        // Interest Begin/Process Date
        public string InterestBegin { get; set; }
        public string TransactionType { get; set; }
        public string ActionType { get; set; }
        public string Reason { get; set; }
        public string Payment { get; set; }
        public string PaymentDate { get; set; }
        public float? AmountDue { get; set; }

    }
}
