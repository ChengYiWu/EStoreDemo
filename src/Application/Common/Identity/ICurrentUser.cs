namespace Application.Common.Identity;

public interface ICurrentUser
{
    string GetCurrentUserId();

    IUser GetCurrentUser();
}
