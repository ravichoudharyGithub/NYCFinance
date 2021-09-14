using System;

namespace NYCFinance.Model
{
    [Serializable]
    public class Exemptions
    {
        public string BenefitName { get; set; }
        public string YearStarted { get; set; }
        public string CurrentPeriod { get; set; }
        public string CurrentAmount { get; set; }
        public string ProposedForNextPeriod { get; set; }
    }
}
