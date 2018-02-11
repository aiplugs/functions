using System.Reflection;
using Aiplugs.Functions.Core;

namespace Sample.Models
{
    public class SampleProcedure : IProcedure
    {
        private string _name;
        public SampleProcedure(string name)
        {
            _name = name;
        }
        public MethodInfo CreateMethod()
        {
            return typeof(SampleFunctions).GetMethod(_name, BindingFlags.Public | BindingFlags.Static);
        }
    }
}