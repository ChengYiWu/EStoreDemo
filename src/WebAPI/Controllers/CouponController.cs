using Application.Coupons.Commands.CreateCoupon;
using Application.Coupons.Commands.DeleteCoupon;
using Application.Coupons.Commands.UpdateCoupon;
using Application.Coupons.Queries.GetCoupon;
using Application.Coupons.Queries.GetCouponList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/coupons")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ISender _sender;

        public CouponController(ISender sender)
        {
            _sender = sender;
        }

        [Authorize]
        [HttpGet]
        [Route("list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<CouponListReponse> GetCouponList(CancellationToken cancellationToken)
        {
            return await _sender.Send(new GetCouponListQuery(), cancellationToken);
        }

        [Authorize]
        [HttpGet]
        [Route("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<CouponResponse> GetCoupon(int id, CancellationToken cancellationToken)
        {
            return await _sender.Send(new GetCouponQuery(id), cancellationToken);
        }

        /// <summary>
        /// 建立優惠券 API
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<int> CreateCoupon(CreateCouponCommand command, CancellationToken cancellationToken)
        {
            return await _sender.Send(command, cancellationToken);
        }

        /// <summary>
        /// 修改優惠券 API
        /// </summary>
        /// <param name="id"></param>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        [Route("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<bool> UpdateCoupon(int id, UpdateCouponCommand command, CancellationToken cancellationToken)
        {
            command.Id = id;
            return await _sender.Send(command, cancellationToken);
        }

        [Authorize]
        [HttpDelete]
        [Route("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<bool> DeleteCoupon(int id, CancellationToken cancellationToken)
        {
            return await _sender.Send(new DeleteCouponCommand(id), cancellationToken);
        }
    }
}
