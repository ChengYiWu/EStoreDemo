namespace WebAPI.Models;

public class ChangeUserPasswordParameter
{
    public string OldPassword { get; set; }

    public string NewPassword { get; set; }
}
