using Aiplugs.Functions;
using Newtonsoft.Json.Linq;

namespace Sample.Models
{
    public interface IContext : IContext<JObject>
    {}
}