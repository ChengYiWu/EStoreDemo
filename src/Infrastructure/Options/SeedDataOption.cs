namespace Infrastructure.Options;

public class SeedDataOption
{
    public AdminOption Admin { get; set; }
}

public class AdminOption
{
    public string Email { get; set; }

    public string Password { get; set; }
}

