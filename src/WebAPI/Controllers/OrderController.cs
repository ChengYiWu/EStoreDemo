using Application.Orders.Commands.ChangeOrderToShippedStatus;
using Application.Orders.Comnmands.PlaceOrder;
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

        [Authorize]
        [HttpPost]
        [Route("{orderNo}/shipped")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<bool> ChangeOrderStatusToShipped(string orderNo, CancellationToken cancellationToken)
        {
            return await _sender.Send(new ChangeOrderToShippedStatusCommand(orderNo), cancellationToken);
        }

    }
}
