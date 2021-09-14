using System;

namespace NYCFinance.Model
{
    [Serializable]
    public class AssessmentInformation
    {
        public string Description { get; set; }
        public string Land { get; set; }
        public string Total { get; set; }
    }
}
