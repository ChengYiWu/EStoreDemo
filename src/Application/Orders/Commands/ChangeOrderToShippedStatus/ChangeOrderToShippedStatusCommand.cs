using MediatR;

namespace Application.Orders.Commands.ChangeOrderToShippedStatus;

public record ChangeOrderToShippedStatusCommand(
    string OrderNo
) : IRequest<bool>;
