using Application.Users.Queries.GetUsers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Queries.GetUser;

public record GetUserQuery(
    string Id
) : IRequest<UserResponse>;
