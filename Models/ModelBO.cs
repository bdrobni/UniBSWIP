namespace dipwebapp.Models
{
    public class ModelBO : ObjectBO
    {
        public string? Filename { get; set; }
        public IFormFile File { get; set; }
    }
}
