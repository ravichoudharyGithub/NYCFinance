using System;

namespace NYCFinance.Model
{
    [Serializable]
    public class MarketValueHistory
    {
        public string TaxYear { get; set; }
        public string MarketValue { get; set; }
    }
}
