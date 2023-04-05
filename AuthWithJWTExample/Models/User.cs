namespace AuthWithJWTExample.Models;

public class User
{
    public int Id { get; set; }
    public string? Nickname { get; set; }
    public string? Login { get; set; }
    public string? Password { get; set; }
    public int Status { get; set; }
}
