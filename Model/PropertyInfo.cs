using System;

namespace NYCFinance.Model
{
    [Serializable]
    public class PropertyInfo
    {
        public string PropertyOwner { get; set; }
        public string TaxYear { get; set; }
        public string LotGrouping { get; set; }
        public string PropertyAddress { get; set; }
        public string TaxClass { get; set; }
        public string BuildingClass { get; set; }
        public string CondoDevelopment { get; set; }
        public string CondoSuffix { get; set; }
    }
}
