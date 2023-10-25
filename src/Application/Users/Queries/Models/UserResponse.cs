namespace Application.Users.Queries.GetUsers;

public class UserResponse
{
    public string Id { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public IList<RoleDTO> Roles { get; set; } = new List<RoleDTO>();
}

public class RoleDTO
{
    public string Name { get; set; }
}
