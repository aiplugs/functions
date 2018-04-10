using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;

namespace Aiplugs.Functions.Core
{
    public class JobCreationViewModel
    {
        [Required]
        public string Name { get; set; }
        public JObject Parameters { get; set; }
    }
}