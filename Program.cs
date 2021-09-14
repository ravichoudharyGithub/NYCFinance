using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NYCFinance
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new NYCFinance();
            // test.NYCFinanceForFetchPropertyDetails("sancho street");
            test.NYCFinanceForFetchPropertyInfo("86-72", "sancho street");
        }
    }
}
