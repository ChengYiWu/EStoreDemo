using Application.Common.Models;
using Application.Users.Commands.ChangeUserPassword;
using Application.Users.Commands.CreateUser;
using Application.Users.Commands.DeleteUser;
using Application.Users.Commands.UpdateUser;
using Application.Users.Queries.ValidUserEmail;
using Application.Users.Queries.GetUser;
using Application.Users.Queries.GetUserList;
using Application.Users.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

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

        [Authorize]
        [HttpGet]
        [Route("list")]
        public async Task<IEnumerable<UserForListResponse>> GetUserList(CancellationToken cancellationToken)
        {
            return await _sender.Send(new GetUserListQuery(), cancellationToken);
        }

        [Authorize]
        [HttpPost]
        [Route("validEmail")]
        public async Task<bool> ValidUserEmail(ValidUserEmailQuery query, CancellationToken cancellationToken)
        {
            return await _sender.Send(query, cancellationToken);
        }

        [Authorize]
        [HttpGet]
        public async Task<PaginatedList<UserResponse>> GetUsers([FromQuery] GetUsersQuery query, CancellationToken cancellationToken)
        {
            return await _sender.Send(query, cancellationToken);
        }

        [Authorize]
        [HttpGet]
        [Route("{id}")]
        public async Task<UserResponse> GetUser(string id, CancellationToken cancellationToken)
        {
            return await _sender.Send(new GetUserQuery(id), cancellationToken);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<string> CreateUser(CreateUserCommand command, CancellationToken cancellationToken)
        {
            return await _sender.Send(command, cancellationToken);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateUser(string id, UpdateUserParameter parameter, CancellationToken cancellationToken)
        {
            var command = new UpdateUserCommand(
                id,
                parameter.UserName,
                parameter.RoleNames
            );

            await _sender.Send(command, cancellationToken);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id, CancellationToken cancellationToken)
        {
            await _sender.Send(new DeleteCommand(id), cancellationToken);
            return Ok();
        }

        [Authorize]
        [HttpPut]
        [Route("{id}/password")]
        public async Task<IActionResult> UpdatePassword(string id, ChangeUserPasswordParameter parameter, CancellationToken cancellationToken)
        {
            var command = new ChangeUserPasswordCommand(
                id,
                parameter.OldPassword,
                parameter.NewPassword
            );

            await _sender.Send(command, cancellationToken);
            return Ok();
        }
    }
}
