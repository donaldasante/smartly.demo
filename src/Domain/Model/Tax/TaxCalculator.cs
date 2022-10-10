using System.Collections.Generic;

namespace SmartlyDemo.RiotSPA.Domain.Model.Tax
{
    public class TaxCalculator
    {
        public string TaxYear { get; set; }
        public IEnumerable<Band> Bands { get; set; }
    }
}
