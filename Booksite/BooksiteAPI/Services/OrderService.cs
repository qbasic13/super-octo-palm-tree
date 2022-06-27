using BooksiteAPI.Data;
using BooksiteAPI.Models;
using BooksiteAPI.Models.Order;
using Microsoft.EntityFrameworkCore;

namespace BooksiteAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly BooksiteContext _context;
        private readonly IBookService _book;
        private readonly IMailService _mail;

        public OrderService(BooksiteContext context,
            IBookService book, IMailService mail)
        {
            _context = context;
            _book = book;
            _mail = mail;
        }

        public async Task<OrderOperationResDto> CreateOrderAsync(
            OrderCreateReqDto orderCreateReq, string email)
        {
            if (_context == null || _book == null)
                return new OrderOperationResDto()
                {
                    IsSuccess = false,
                    Status = "server_error"
                };

            if (orderCreateReq.OrderItems == null
                || orderCreateReq.OrderItems.Length == 0)
                return new OrderOperationResDto()
                {
                    IsSuccess = false,
                    Status = "empty_order_items",
                    Message = "Can't create empty order"
                };

            var user = _context.Users.Where(
                u => u.UEmail == email).SingleOrDefault();
            if (user == null)
                return new OrderOperationResDto()
                {
                    IsSuccess = false,
                    Status = "user_doesnt_exist",
                    Message = "User doesn't exist"
                };

            var createdStatus = _context.OrderStatuses.Where(
                os => os.OsName == "created").SingleOrDefault();
            if (createdStatus == null)
                return new OrderOperationResDto()
                {
                    IsSuccess = false,
                    Status = "server_error",
                };

            int orderId = -1;
            //BEGIN TRANSACTION
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var newOrder = new Order()
                    {
                        OCreatorNavigation = user,
                        OCreationDt = DateTime.Now,
                        OTotalPrice = 0,
                        OStatusNavigation = createdStatus,
                    };

                    foreach (var item in orderCreateReq.OrderItems)
                    {
                        if (item == null || item.Isbn == null
                            || item.Quantity == 0)
                        {
                            transaction.Rollback();
                            return new OrderOperationResDto()
                            {
                                IsSuccess = false,
                                Status = "bad_order_item",
                                Message = "Incorrect order item"
                            };
                        }

                        var book = _context.Books.Where(
                            b => b.BIsbn == item.Isbn).SingleOrDefault();
                        if (book == null || book.BQuantity < item.Quantity)
                        {
                            transaction.Rollback();
                            return new OrderOperationResDto()
                            {
                                IsSuccess = false,
                                Status = "book_not_available",
                                Message = "Book not available for order"
                            };
                        }

                        decimal price = item.Quantity * book.BPrice ?? 0;
                        var m2mOrdersBooks = new M2mOrdersBook()
                        {
                            M2mobBIsbnNavigation = book,
                            M2mobO = newOrder,
                            M2mobPrice = price,
                        };

                        //updating or writing
                        _context.Books.Update(book);
                        book.BQuantity -= item.Quantity;
                        _context.M2mOrdersBooks.Add(m2mOrdersBooks);
                        newOrder.OTotalPrice += price;
                    }

                    _context.Orders.Add(newOrder);

                    //COMMIT TRANSACTION
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    orderId = newOrder.OId;
                }
                catch
                {
                    transaction.Rollback();
                    return new OrderOperationResDto()
                    {
                        IsSuccess = false,
                        Status = "transaction_error",
                        Message = "Failed to create order"
                    };
                }
            }

            if (orderId == -1)
                return new OrderOperationResDto()
                {
                    IsSuccess = true,
                    Status = "order_id_lost",
                    Message = "Order created, but can't get id"
                };

            return new OrderOperationResDto()
            {
                IsSuccess = true,
                Status = "success",
                Message = "Order created successfully"
            };
        }

        public async Task<OrderOperationResDto> 
            GetOrderDetailsAsync(int orderId, string email)
        {
            if (_context == null)
                return new OrderOperationResDto()
                {
                    IsSuccess = false,
                    Status = "server_error"
                };

            var creator = await _context.Users.Where(
                u => u.UEmail == email).FirstOrDefaultAsync();
            if(creator == null)
                return new OrderOperationResDto()
                {
                    IsSuccess = false,
                    Status = "unauthorized"
                };

            var m2mOrdersBooks = await _context.M2mOrdersBooks
                .Include(o => o.M2mobO).Include(b => b.M2mobBIsbnNavigation)
                .Include(os => os.M2mobO.OStatusNavigation)
                .Include(g => g.M2mobBIsbnNavigation.BGenreNavigation)
                .Where(m2mob => m2mob.M2mobOId == orderId 
                && m2mob.M2mobO.OCreator == creator.UId).ToArrayAsync();
            if (!m2mOrdersBooks.Any())
                return new OrderOperationResDto()
                {
                    IsSuccess = false,
                    Status = "not_found",
                    Message = "Transaction not found"
                };


            var orderBooks = new List<BookDetailsDto>();
            foreach (var m2mobBook in m2mOrdersBooks)
            {
                orderBooks.Add(new BookDetailsDto()
                {
                    Isbn = m2mobBook.M2mobBIsbn,
                    Title = m2mobBook.M2mobBIsbnNavigation.BTitle,
                    Author = m2mobBook.M2mobBIsbnNavigation.BAuthor,
                    Genre = m2mobBook.M2mobBIsbnNavigation.BGenreNavigation.GName,
                    PublishYear = m2mobBook.M2mobBIsbnNavigation.BPublishYear,
                    Quantity = m2mobBook.M2mobBIsbnNavigation.BQuantity,
                    Price = m2mobBook.M2mobPrice ?? 0,
                    CoverFile = m2mobBook.M2mobBIsbnNavigation.BCoverFile
                });
            }

            var order = m2mOrdersBooks.First().M2mobO;
            var orderDto = new OrderDto()
            {
                Id = orderId,
                UserEmail = creator.UEmail,
                UserPhone = creator.UPhone,
                UserFirstName = creator.UFirstName,
                UserLastName = creator.ULastName,
                UserMiddleName = creator.UMiddleName,
                Books = orderBooks.ToArray(),
                Status = order.OStatusNavigation.OsName,
                TotalPrice = order.OTotalPrice,
                CreatedDate = order.OCreationDt,
                CompletionDate = order.OCompletionDt
            };

            if (orderDto != null)
                return new OrderOperationResDto()
                {
                    IsSuccess = true,
                    Status = "success",
                    Message = "Successfully got order data",
                    Order = orderDto
                };

            return new OrderOperationResDto()
            {
                IsSuccess = false,
                Status = "server_error"
            };
        }
    }
}
