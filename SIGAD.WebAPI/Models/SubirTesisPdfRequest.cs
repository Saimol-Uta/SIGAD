namespace SIGAD.WebAPI.Models
{
    public class SubirTesisPdfRequest
    {
        public IFormFile File { get; set; }
        public string Dto { get; set; }
    }
}