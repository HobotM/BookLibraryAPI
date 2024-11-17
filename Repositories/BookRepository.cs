using BookLibraryAPI.Data;
using BookLibraryAPI.Interfaces;
using BookLibraryAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibraryAPI.Helpers;
using System.Linq;



namespace BookLibraryAPI.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly BookContext _context;

        public BookRepository(BookContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<Book> GetByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task AddAsync(Book book)
        {
            await _context.Books.AddAsync(book);
        }

        public void Update(Book book)
        {
            _context.Entry(book).State = EntityState.Modified;
        }

        public void Delete(Book book)
        {
            _context.Books.Remove(book);
        }

        public async Task<bool> SaveAsync()
        {
            return (await _context.SaveChangesAsync()) >= 0;
        }

        public async Task<PagedResult<Book>> GetPagedBooksAsync(BookQueryParameters queryParameters)
        {
            IQueryable<Book> query = _context.Books;

            //Filtering
            if(!string.IsNullOrEmpty(queryParameters.SearchTerm))
            {
                var searchTerm = queryParameters.SearchTerm.ToLower();
                query = query.Where(b => b.Title.ToLower().Contains(searchTerm)); 


            }
             if (!string.IsNullOrEmpty(queryParameters.Genre))
            {
                query = query.Where(b => b.Genre == queryParameters.Genre);
            }

            if (!string.IsNullOrEmpty(queryParameters.Author))
            {
                query = query.Where(b => b.Author == queryParameters.Author);
            }


             // Sorting
            if (!string.IsNullOrEmpty(queryParameters.OrderBy))
            {
                switch (queryParameters.OrderBy.ToLower())
                {
                    case "title":
                        query = query.OrderBy(b => b.Title);
                        break;
                    case "title_desc":
                        query = query.OrderByDescending(b => b.Title);
                        break;
                    case "author":
                        query = query.OrderBy(b => b.Author);
                        break;
                    case "author_desc":
                        query = query.OrderByDescending(b => b.Author);
                        break;
                    default:
                        query = query.OrderBy(b => b.Title);
                        break;
                }

                
            }
            // Total count before pagination
            var totalCount = await query.CountAsync();

             // Pagination
            var data = await query
                .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .ToListAsync();

            return new PagedResult<Book>
            {
                Data = data,
                TotalCount = totalCount
            };
        }
    }
}
