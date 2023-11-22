using System.ComponentModel.DataAnnotations;

namespace MAUIAndroidReleaseTool.Models
{
    public class ReleaseModel
    {
        [Required]
        public string Path { get; set; }

        [Required]
        public string Framework { get; set; }

        [Required]
        public string Runtime { get; set; }

        [Required]
        public string Password { get; set; }

        public string Trimmed { get; set; }

        [Required]
        public string KeystorePath { get; set; }
    }
}
