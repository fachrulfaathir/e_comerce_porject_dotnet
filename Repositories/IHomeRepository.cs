namespace ProjectEcomerceFinal.Repositories
{
    public interface IHomeRepository
    {
       Task<IEnumerable<Book>> DisplayBooks(string sTerm = "", int genreId = 0);

    }
}