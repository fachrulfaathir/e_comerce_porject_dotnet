
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace ProjectEcomerceFinal.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _db;
        public HomeRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<Book>> DisplayBooks(string sTerm = "", int genreId = 0)
        {
            sTerm = (sTerm ?? "").Trim().ToLower();

            IQueryable<Book> query = from book in _db.Books
                                     join genre in _db.Genres on book.GenreId equals genre.Id
                                     select new Book
                                     {
                                         Id = book.Id,
                                         Image = book.Image,
                                         AuthorName = book.AuthorName,
                                         BookName = book.BookName,
                                         GenreId = book.GenreId,
                                         Price = book.Price,
                                         GenreNames = genre.GenreName
                                     };

            // Filter pencarian (jika sTerm tidak kosong)
            if (!string.IsNullOrWhiteSpace(sTerm))
            {
                query = query.Where(b => b.BookName.ToLower().StartsWith(sTerm) ||
                                         b.AuthorName.ToLower().StartsWith(sTerm));
            }

            // Filter berdasarkan genre (jika genreId > 0)
            if (genreId > 0)
            {
                query = query.Where(b => b.GenreId == genreId);
            }

            // Eksekusi query ke database secara async
            return await query.ToListAsync();
        }
    }
}
