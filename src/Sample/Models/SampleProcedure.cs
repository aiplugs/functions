using System.Reflection;
using Aiplugs.Functions.Core;

namespace Sample.Models
{
    public class SampleProcedure : IProcedure
    {
        private string _name;
        public string InstanceMessage { get; } = "This is instance method"; 
        public SampleProcedure(string name)
        {
            _name = name;
        }
        public void InstanceMethod(IContext context)
        {
            context.Logger.LogInfo(InstanceMessage);            
        }
        public MethodInfo CreateMethod()
        {
            return typeof(SampleFunctions).GetMethod(_name, BindingFlags.Public | BindingFlags.Static) ?? this.GetType().GetMethod(_name);
        }
    }
}