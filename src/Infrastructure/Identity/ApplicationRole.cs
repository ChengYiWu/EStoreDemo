using Application.Common.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class ApplicationRole : IdentityRole, IRole
{
    public ApplicationRole(): base()
    {
    }

    public ApplicationRole(string roleName) : base(roleName)
    {
    }
}
