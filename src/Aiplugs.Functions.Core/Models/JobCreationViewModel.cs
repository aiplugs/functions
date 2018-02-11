using System.ComponentModel.DataAnnotations;

namespace Aiplugs.Functions.Core
{
    public class JobCreationViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}