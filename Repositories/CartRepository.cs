using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ProjectEcomerceFinal.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartRepository(ApplicationDbContext db, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> AddItem(int bookId, int qty)
        {
            string userId = GetUserId();
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("user is not logged-in");
                }
                var cart = await GetCart(userId);
                if (cart is null)
                {
                    cart = new ShoppingCart
                    {
                        UserId = userId,
                    };
                    _dbContext.ShoppingCarts.Add(cart);
                }
                _dbContext.SaveChanges();

                //add details items
                var cartItem = _dbContext.CartDetails.FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.BookId == bookId);
                if (cartItem is not null)
                {
                    cartItem.Quantity += qty;
                }
                else
                {
                    cartItem = new CartDetail
                    {
                        BookId = bookId,
                        ShoppingCartId = cart.Id,
                        Quantity = qty
                    };
                    _dbContext.CartDetails.Add(cartItem);
                }
                _dbContext.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException("error");
            }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }
        public async Task<int> RemoveItem(int bookId)
        {
            string userId = GetUserId();
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("user is not logged-in");
                }
                var cart = await GetCart(userId);
                if (cart is null)
                {
                    throw new UnauthorizedAccessException("error");
                }
                _dbContext.SaveChanges();

                //add details items
                var cartItem = _dbContext.CartDetails.FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.BookId == bookId);
                if (cartItem is null)
                    throw new InvalidOperationException("Not items in cart");
                else if (cartItem.Quantity == 1)
                    _dbContext.CartDetails.Remove(cartItem);
                else
                    cartItem.Quantity = cartItem.Quantity - 1;
                _dbContext.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException("error");
            }

            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }

        public async Task<ShoppingCart> GetUserCart()
        {
            var userId = GetUserId();
            if (userId == null)
                throw new InvalidOperationException("Invalid userid");
            var shoppingCart = await _dbContext.ShoppingCarts
                                  .Include(a => a.CartDetails)
                                  .ThenInclude(a => a.Book)
                                  .ThenInclude(a => a.genre)
                                  .Where(a => a.UserId == userId).FirstOrDefaultAsync();
            return shoppingCart;

        }

        public async Task<ShoppingCart> GetCart(string userId)
        {
            var cart = await _dbContext.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);
            return cart;
        }

        private String GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(principal);
            return userId;
        }

        public async Task<int> GetCartItemCount(string userId = "")
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = GetUserId();
            }

            var data = await (from cart in _dbContext.ShoppingCarts
                              join details in _dbContext.CartDetails
                              on cart.Id equals details.ShoppingCartId
                              where cart.UserId == userId
                              select new { details.Id }
                              ).ToListAsync();
            return data.Count;
        }

        public async Task<bool> DoCheck()
        {
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    throw new Exception("user is not login");
                }
                var cart = await GetCart(userId);
                if (cart is null)
                {
                    throw new Exception("invalid cart");
                }

                var cartDetail = _dbContext.CartDetails.Include(a => a.Book).Where(a => a.ShoppingCartId == cart.Id).ToList();
                if (cartDetail.Count == 0)
                {
                    throw new Exception("Cart Is Empty");

                }
                var order = new Order
                {
                    UserId = userId,
                    CreateDate = DateTime.Now,
                    OrderStatusId = 1,

                };
                _dbContext.Orders.Add(order);
                _dbContext.SaveChanges();

                foreach (var item in cartDetail)
                {
                    var orderDetail = new OrderDetail
                    {
                        BookId = item.BookId,
                        OrderId = order.Id,
                        Quantity = item.Quantity,
                        UnitPrice = item.Book?.Price ?? 0

                    };
                    _dbContext.OrderDetails.Add(orderDetail);

                }

                _dbContext.CartDetails.RemoveRange(cartDetail);
                await _dbContext.SaveChangesAsync();


                transaction.Commit();
                return true;

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Gagal membuat order baru: " + ex.Message, ex);
            }
        }
    }
}
