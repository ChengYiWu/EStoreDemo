namespace Application.Roles.Queries.GetRoleList;

public class RoleListResponse
{
    public IEnumerable<RoleListItemDTO> Items { get; set; } = new List<RoleListItemDTO>();
}

public class RoleListItemDTO
{
    public string Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
