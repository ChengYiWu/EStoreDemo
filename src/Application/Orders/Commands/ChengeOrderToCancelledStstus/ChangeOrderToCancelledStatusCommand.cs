using MediatR;

namespace Application.Orders.Commands.ChengeOrderToCancelledStstus;

public class ChangeOrderToCancelledStatusCommand : IRequest<bool>
{
    public string OrderNo { get; set; }

    public string Reason { get; set; }
}
