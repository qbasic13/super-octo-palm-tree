using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BooksiteAPI.Data;
using BooksiteAPI.Services;
using BooksiteAPI.Models.Order;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BooksiteAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _order;

        public OrderController(IOrderService order)
        {
            _order = order;
        }

        [Authorize(Roles = "verified,admin")]
        [HttpGet]
        public async Task<ActionResult<OrderOperationResDto>>
            GetUserOrder(int orderId)
        {
            var userEmailClaim = HttpContext.User.Claims
                .Where(c => c.Type == ClaimTypes.Email).FirstOrDefault();
            if (userEmailClaim == null || userEmailClaim.Value.Length == 0)
                return new OrderOperationResDto()
                {
                    IsSuccess = false,
                    Status = "no_creator_email",
                    Message = "Didn't get user email"
                };

            return await _order.GetOrderDetailsAsync(
                orderId, userEmailClaim.Value);
        }

        [Authorize(Roles = "verified,admin")]
        [HttpGet("all")]
        public async Task<ActionResult<OrdersResDto>>
            GetUserOrders(string email)
        {
            var userEmailClaim = HttpContext.User.Claims
                .Where(c => c.Type == ClaimTypes.Email).FirstOrDefault();
            if (userEmailClaim == null || email != userEmailClaim.Value) 
                return new OrdersResDto()
            {
                IsSuccess = false,
                Status = "unauthorized",
                Message = "Unauthorized"
            };

            return await _order.GetOrdersAsync(email);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("admin")]
        public async Task<ActionResult<OrdersResDto>>
            GetAdminOrders()
        {
            return await _order.GetOrdersAsync(" ", true);
        }

        [Authorize(Roles = "verified, admin")]
        [HttpPost("status")]
        public async Task<ActionResult<OrderOperationResDto>>
            SetOrderStatus(int orderId, string newStatus)
        {
            var userEmailClaim = HttpContext.User.Claims
                .Where(c => c.Type == ClaimTypes.Email).FirstOrDefault();
            if (userEmailClaim == null || userEmailClaim.Value.Length == 0)
                return new OrderOperationResDto()
                {
                    IsSuccess = false,
                    Status = "no_editor_email",
                    Message = "Didn't get editor email"
                };
            return await _order.ChangeOrderStatusAsync(orderId, 
                newStatus, userEmailClaim.Value);
        }

        [Authorize(Roles = "verified,admin")]
        [HttpPost("create")]
        public async Task<ActionResult<OrderOperationResDto>>
            CreateOrder(OrderCreateReqDto createReq)
        {
            var userEmailClaim = HttpContext.User.Claims
                .Where(c => c.Type == ClaimTypes.Email).FirstOrDefault();
            if (userEmailClaim == null || userEmailClaim.Value.Length == 0)
                return new OrderOperationResDto()
                {
                    IsSuccess = false,
                    Status = "no_creator_email",
                    Message = "Didn't get creator email"
                };

            return await _order.CreateOrderAsync(
                createReq, userEmailClaim.Value);
        }
    }
}
