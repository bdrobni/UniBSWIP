using System.Web.Mvc;

namespace dipwebapp.Models
{
    public class ObjectBO
    {
        private readonly IWebHostEnvironment _environment;
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public UserBO Author { get; set; }
        public UserBO? CurrentUser { get; set; } 
        public string Filetype { get; set; }
        public string? Description {  get; set; }
        [AllowHtml]
        public string ObjContent { get; set; }
        public IEnumerable<TagBO>? Tags { get; set; }
        public IEnumerable<TagBO>? AllTags { get; set; }
        public IEnumerable<ObjectBO>? Objects { get; set; }
        public IEnumerable<ObjectBO>? AttachedObjects { get; set; }
        public IEnumerable<ObjectBO>? Images { get; set; }
        public int Tempid {  get; set; }
        public string WebRootString { get; set; }

        public string ImagePath(string filename)
        {
            string filePath = Path.Combine(WebRootString, "images", filename);
            return filePath;
        }
    }
}
