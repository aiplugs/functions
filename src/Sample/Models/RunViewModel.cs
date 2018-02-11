using System.ComponentModel.DataAnnotations;

namespace Sample.Models
{
    public class RunViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}