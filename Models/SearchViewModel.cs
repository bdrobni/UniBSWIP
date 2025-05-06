using Microsoft.EntityFrameworkCore.Storage;

namespace dipwebapp.Models
{
    public class SearchViewModel
    {
        public string? SelectedAuthor {  get; set; }
        public string? SearchBoxContent { get; set; }
        public string? SortOption { get; set; }
        public string? SelectedFileType { get; set; }
        public List<ObjectBO>? AllObjects { get; set; }
        public List<ObjectBO>? SelectedObjects { get; set; }
        public List<TagBO>? Tags { get; set; }
    }
}
