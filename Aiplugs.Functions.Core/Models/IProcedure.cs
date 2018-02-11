using System.Reflection;

namespace Aiplugs.Functions.Core
{
    public interface IProcedure
    {
        MethodInfo CreateMethod();
    }
}