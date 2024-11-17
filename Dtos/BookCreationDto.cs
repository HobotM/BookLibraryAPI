namespace BookLibraryAPI.Dtos
{
    public class BookForCreationDto
    {
        // Do not include Id
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public int Year { get; set; }
    }
}
