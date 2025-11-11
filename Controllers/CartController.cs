using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProjectEcomerceFinal.Controllers
{
    [Authorize] 
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepository;

        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }
        public async Task<IActionResult> AddItem(int bookId, int qty=1, int redirect=0 )
        {
            var cartCount = await _cartRepository.AddItem(bookId, qty);
            if (redirect == 0)
            {
                return Ok(cartCount);
            }
            return RedirectToAction("GetUserCart");
        }
        public async Task<IActionResult> RemoveItem(int bookId)
        {
            var cartCount = await _cartRepository.RemoveItem(bookId);
            return RedirectToAction("GetUserCart");
        }
        public async  Task<IActionResult> GetUserCart()
        {
            var car = await _cartRepository.GetUserCart();
            return View(car);
        }
        public async Task<IActionResult> GetTotalItenInCart()
        {
            int cartItem = await _cartRepository.GetCartItemCount();
            return Ok(cartItem);
        }
        public IActionResult ChekoutKeranjang()
        {
            return View();
        }
        public IActionResult OrderSuccess()
        {
            return View();
        }
        public IActionResult OrderFailure()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChekoutKeranjang(CheckoutModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var paymentResult = await _cartRepository.DoCheck(model);
            if (string.IsNullOrEmpty(paymentResult))
            {
                Console.WriteLine("Payment Result dari Midtrans:");
                Console.WriteLine(paymentResult);
            }

            // Deserialize JSON agar bisa ambil token / URL Midtrans
            var json = JsonSerializer.Deserialize<JsonElement>(paymentResult);
            var redirectUrl = json.GetProperty("redirect_url").GetString();

            // arahkan user ke halaman pembayaran Midtrans
            return Redirect(redirectUrl);
        }
    }
    
}
