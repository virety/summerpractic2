using System.ComponentModel.DataAnnotations;

public class UserDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; }

    [Required]
    public string UserName { get; set; }

    public string Name { get; set; }
    public string PhoneNumber { get; set; }
}