using System;

namespace NYCFinance.Model
{
    [Serializable]
    public class NoticesOfPropertyValue
    {
        public string Term { get; set; }
        public string Date { get; set; }
        public string Other { get; set; }
    }
}
