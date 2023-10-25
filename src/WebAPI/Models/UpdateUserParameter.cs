namespace WebAPI.Models;

public class UpdateUserParameter
{
    public string Email { get; set; }
    public string UserName { get; set; }
    public string[]? RoleNames { get; set; }
}
