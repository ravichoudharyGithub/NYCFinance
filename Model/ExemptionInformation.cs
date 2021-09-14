using System;

namespace NYCFinance.Model
{
    [Serializable]
    public class ExemptionInformation
    {
        public int Code { get; set; }
        public string Description { get; set; }
        public string ExemptValue { get; set; }
    }
}
