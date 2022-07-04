
using BooksiteAPI.Models.Order;

namespace BooksiteAPI.Services
{
    public interface IOrderService
    {
        Task<OrderOperationResDto> CreateOrderAsync(
            OrderCreateReqDto orderCreateReq, string email);
        public Task<OrderOperationResDto> GetOrderDetailsAsync(int orderId, string email);
    }
}
