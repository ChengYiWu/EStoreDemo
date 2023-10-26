using MediatR;

namespace Application.Orders.Comnmands.PlaceOrder;

public record PlaceOrderCommand(
    string ContactPhone,
    string ShippingAddress,
    string? Note,
    string CustomerId,
    List<OrderItemDTO> Items
) : IRequest<string>;

public record OrderItemDTO(
    int ProductItemId,
    int Quantity
);
