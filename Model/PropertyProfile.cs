using System;

namespace NYCFinance.Model
{
    [Serializable]
    public class PropertyProfile
    {
        public string BuildingClass { get; set; }
        public string TaxClass { get; set; }
        public string UnusedSCRIECredit { get; set; }
        public string UnusedDRIECredit { get; set; }
        public string RefundAvailable { get; set; }
        public string Overpaymentamount { get; set; }
    }
}
