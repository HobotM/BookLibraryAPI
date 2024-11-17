using System;

namespace BookLibraryAPI.Helpers
{

public class BookQueryParameters
{
    private const int MaxPageSize = 50;
    private int _pageSize = 10;

    public int PageNumber {get;set;} = 1;

    public int PageSize{

        get => _pageSize;
        set => _pageSize = Math.Min(MaxPageSize, value);

    }

        public string? SearchTerm { get; set; }
        public string? OrderBy { get; set; }
        public string? Genre { get; set; }
        public string? Author { get; set; }


}

}