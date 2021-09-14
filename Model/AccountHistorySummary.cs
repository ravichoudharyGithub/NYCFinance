using System;

namespace NYCFinance.Model
{
    [Serializable]
    public class AccountHistorySummary
    {
        public int Year { get; set; }
        public int Period { get; set; }
        public string ChargeType { get; set; }
        public string OriginalDueDate { get; set; }
        // Interest Begin/Process Date
        public string InterestBegin { get; set; }
        public float? Charge { get; set; }
        public float? Paid { get; set; }
        public float? Balance { get; set; }
    }
}
