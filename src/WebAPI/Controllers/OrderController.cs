using Application.Common.Models;
using Application.Orders.Commands.ChangeOrderToShippedStatus;
using Application.Orders.Commands.ChengeOrderToCancelledStstus;
using Application.Orders.Comnmands.PlaceOrder;
using Application.Orders.Queries.GetOrder;
using Application.Orders.Queries.GetOrders;
using Application.Orders.Queries.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ISender _sender;

        public OrderController(ISender sender)
        {
            _sender = sender;
        }

        [Authorize]
        [HttpGet]
        public async Task<PaginatedList<OrderResponse>> GetOrders([FromQuery] GetOrdersQuery query, CancellationToken cancellationToken)
        {
            return await _sender.Send(query, cancellationToken);
        }

        [Authorize]
        [HttpGet]
        [Route("{orderNo}")]
        public async Task<OrderResponse> GetOrder(string orderNo, CancellationToken cancellationToken)
        {
            return await _sender.Send(new GetOrderQuery(orderNo), cancellationToken);
        }

        /// <summary>
        /// 建立訂單 API
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<string> PlaceOrder(PlaceOrderCommand command, CancellationToken cancellationToken)
        {
            return await _sender.Send(command, cancellationToken);
        }

        /// <summary>
        /// 變更訂單狀態為已出貨 API
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        [Route("{orderNo}/shipped")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<bool> ChangeOrderStatusToShipped(string orderNo, CancellationToken cancellationToken)
        {
            return await _sender.Send(new ChangeOrderToShippedStatusCommand(orderNo), cancellationToken);
        }

        /// <summary>
        /// 變更訂單狀態為已取消 API
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        [Route("{orderNo}/cancelled")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<bool> ChangeOrderStatusToCancelled(string orderNo, ChangeOrderToCancelledStatusCommand command, CancellationToken cancellationToken)
        {
            command.OrderNo = orderNo;

            return await _sender.Send(command, cancellationToken);
        }
    }
}
