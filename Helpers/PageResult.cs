using System.Collections.Generic;

namespace BookLibraryAPI.Helpers
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int TotalCount { get; set; }
    }
}
