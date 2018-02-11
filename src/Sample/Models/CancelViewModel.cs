using System.ComponentModel.DataAnnotations;

namespace Sample.Models
{
    public class CancelViewModel
    {
        [Required]
        public long Id { get; set; }
    }
}