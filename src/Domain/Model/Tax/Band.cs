namespace SmartlyDemo.RiotSPA.Domain.Model.Tax
{
    public class Band
    {
        public int Id { get; set; }
        public decimal Minimum { get; set; }
        public decimal Maximum { get; set; }
        public decimal TaxRate { get; set; }
        public bool NoMaximumLimit { get; set; } = false;
    }
}
