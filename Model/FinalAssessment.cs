using System;

namespace NYCFinance.Model
{
    [Serializable]
    public class FinalAssessment
    {
        public string FinalAssessmentRoll { get; set; }
        public string TaxableStatusDate { get; set; }
        public string OwnerName { get; set; }
        public string PropertyAddres { get; set; }
        public string BillingNameAndAddress { get; set; }
        public string TaxClass { get; set; }
        public string BuildingClass { get; set; }
    }
}
