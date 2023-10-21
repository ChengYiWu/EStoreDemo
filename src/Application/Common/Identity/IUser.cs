namespace Application.Common.Identity;

public interface IUser
{
    /// <summary>
    /// 使用者 Id
    /// </summary>
    string Id { get; }

    /// <summary>
    /// 使用者名稱
    /// </summary>
    string UserName { get; }

    /// <summary>
    /// 使用者信箱
    /// </summary>
    string Email { get; }

    /// <summary>
    /// 使用者所屬角色
    /// </summary>
    public IEnumerable<IRole> Roles { get; }
}
