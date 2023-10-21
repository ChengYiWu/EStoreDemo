using Application.Common.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class ApplicationUser : IdentityUser, IUser
{
    public IEnumerable<IRole> Roles { get; set; } = new List<IRole>();
}
