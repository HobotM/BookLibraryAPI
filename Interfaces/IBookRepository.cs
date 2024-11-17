using BookLibraryAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibraryAPI.Helpers;

namespace BookLibraryAPI.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllAsync();
        Task<Book> GetByIdAsync(int id);
        Task<PagedResult<Book>> GetPagedBooksAsync(BookQueryParameters queryParameters);

        Task AddAsync(Book book);
        void Update(Book book);
        void Delete(Book book);
        Task<bool> SaveAsync();
    }
}
