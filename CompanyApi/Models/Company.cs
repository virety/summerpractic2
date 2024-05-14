namespace CompanyApi.Models
{
    public class Company
    {

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid BigBoss { get; set; }
        public string Country { get; set; }
    }
}