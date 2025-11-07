using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ProjectEcomerceFinal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _homeRepository;

        public HomeController(ILogger<HomeController> logger, IHomeRepository homeRepository)
        {
            _logger = logger;
            _homeRepository = homeRepository;
        }

        public async Task<IActionResult> Index(string sTerm = "", int genreId = 0)
        {
            IEnumerable<Book> books = await _homeRepository.GetBooks(sTerm, genreId);
            IEnumerable<Genre> genre = await _homeRepository.Genres();

            BookDisplayModel displayModelBook = new BookDisplayModel()
            {
                Books = books,
                Genres = genre,
                GenreId = genreId,
                STerm = sTerm
            };
            return View(displayModelBook);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
