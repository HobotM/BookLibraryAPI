using Microsoft.AspNetCore.Mvc;
using BookLibraryAPI.Models;
using System.Collections.Generic;
using System.Linq;
using BookLibraryAPI.Data;
using Microsoft.EntityFrameworkCore;
using BookLibraryAPI.Interfaces;
using AutoMapper;
using BookLibraryAPI.Dtos;
using Microsoft.Extensions.Caching.Memory;
using BookLibraryAPI.Helpers;


namespace BookLibraryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
{
    private readonly IBookRepository _repository;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    public BooksController(IBookRepository repository, IMapper mapper, IMemoryCache cache)
    {
        _repository = repository;
        _mapper = mapper;
        _cache = cache;

    }






[HttpGet]
[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
public async Task<ActionResult<PagedResponse<BookDto>>> GetAllBooks([FromQuery] BookQueryParameters queryParameters)
{
    var cacheKey = $"bookList_{queryParameters.PageNumber}_{queryParameters.PageSize}_{queryParameters.SearchTerm}_{queryParameters.OrderBy}_{queryParameters.Genre}_{queryParameters.Author}";
    if (!_cache.TryGetValue(cacheKey, out PagedResponse<BookDto> response))
    {

        var pagedBooks = await _repository.GetPagedBooksAsync(queryParameters);
        var booksDto = _mapper.Map<IEnumerable<BookDto>>(pagedBooks.Data);

        response = new PagedResponse<BookDto>
        {
            Data = booksDto,
            TotalCount = pagedBooks.TotalCount,
            PageNumber = queryParameters.PageNumber,
            PageSize = queryParameters.PageSize
        };

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(5));

        _cache.Set(cacheKey, response, cacheEntryOptions);
    }
  

    return Ok(response);
}






// GET: api/books/{id}
[HttpGet("{id}")]
[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
public async Task<ActionResult<BookDto>> GetBookById(int id)
{

    var cacheKey = $"book_{id}";
    if(!_cache.TryGetValue(cacheKey, out BookDto bookDto))
    {
        var book = await _repository.GetByIdAsync(id);

        if (book == null)
        {
            return NotFound();
        }

        bookDto = _mapper.Map<BookDto>(book);

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(5));
        _cache.Set(cacheKey, bookDto, cacheEntryOptions);
       
    }
    return Ok(bookDto);
}


// POST: api/books
[HttpPost]
public async Task<ActionResult<BookDto>> AddBook(BookForCreationDto bookForCreationDto)
{
    var book = _mapper.Map<Book>(bookForCreationDto);

    await _repository.AddAsync(book);
    await _repository.SaveAsync();

    var bookToReturn = _mapper.Map<BookDto>(book);

    _cache.Remove("bookList");
    return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, bookToReturn);
}



// PUT: api/books/{id}
[HttpPut("{id}")]
public async Task<IActionResult> UpdateBook(int id, BookDto bookDto)
{
    if (id != bookDto.Id)
    {
        return BadRequest();
    }

    var bookFromRepo = await _repository.GetByIdAsync(id);
    if (bookFromRepo == null)
    {
        return NotFound();
    }

    _mapper.Map(bookDto, bookFromRepo);

    _repository.Update(bookFromRepo);

    await _repository.SaveAsync();

      // Invalidate cache
    _cache.Remove($"book_{id}");
    _cache.Remove("bookList");

    return NoContent();
}




// DELETE: api/books/{id}
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteBook(int id)
{
    var book = await _repository.GetByIdAsync(id);
    if (book == null)
    {
        return NotFound();
    }

    _repository.Delete(book);
    await _repository.SaveAsync();

 // Invalidate cache
    _cache.Remove($"book_{id}");
    _cache.Remove("bookList");
    return NoContent();
}





    }
}
