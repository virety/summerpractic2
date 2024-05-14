namespace CompanyApi.DTOs;

public class CreateCompanyDto
{
    public string Name { get; set; }
    public string Country { get; set; }
    public Guid BigBoss { get; set; }
}