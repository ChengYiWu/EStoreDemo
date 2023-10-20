namespace Application.Common.Identity;

public interface IUser
{
    string Id { get; }

    string UserName { get; }

    string Email { get; }
}
