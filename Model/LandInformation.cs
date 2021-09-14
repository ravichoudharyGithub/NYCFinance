using System;

namespace NYCFinance.Model
{
    [Serializable]
    public class LandInformation
    {
        public string Lot_Frontage_Feet { get; set; }
        public string Lot_Depth_Feet { get; set; }
        public string LandArea_sqft { get; set; }
        public string Regular_Irregular { get; set; }
        public string Corner { get; set; }
        public string NumberOfBuildings { get; set; }
        public string Building_Frontag_Feet { get; set; }
        public string Building_Depth_Feet { get; set; }
        public string Stories { get; set; }
        public string Extension { get; set; }
    }
}
