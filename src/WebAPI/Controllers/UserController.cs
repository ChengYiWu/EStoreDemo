using Application.Users.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ISender _sender;

        public UserController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IEnumerable<UserResponse>> GetUsers([FromQuery] GetUsersQuery query, CancellationToken cancellationToken)
        {
            return await _sender.Send(query, cancellationToken);
        }
    }
}
