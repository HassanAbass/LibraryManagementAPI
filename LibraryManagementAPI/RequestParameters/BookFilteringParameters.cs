using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.RequestParameters
{
    public class BookFilteringParameters
    {
        [property: FromQuery(Name = "title")]
        public string? Title { set; get; }

        [property: FromQuery(Name = "categoryId")]
        public int? CategoryId { set; get; }    
        [property: FromQuery(Name = "authorId")]
        public int? AuthorId { set; get; }
    }
}
