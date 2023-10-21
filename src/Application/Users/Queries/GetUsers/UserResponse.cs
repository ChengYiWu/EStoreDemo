namespace Application.Users.Queries.GetUsers;

public class UserResponse
{
    public string Id { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public IList<RoleResponse> Roles { get; set; } = new List<RoleResponse>();
}

public class RoleResponse
{
    public string Name { get; set; }
}
