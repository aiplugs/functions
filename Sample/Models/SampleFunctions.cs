using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Aiplugs.Functions;
using Aiplugs.Functions.Core;

namespace Sample.Models
{
    public class SampleFunctions
    {
        public static void HelloWorld(IContext context)
        {
            context.Logger.LogInfo("Hello, World!");
        }
        public static void LongRun(IContext context)
        {
            context.Logger.LogInfo("Start!");
            for(var i = 0; i < 10; i++)
            {
                context.Progress.Report(10 * i);
                context.Logger.LogInfo(string.Join("", Enumerable.Range(0, i).Select(_ => "=")) + ">");
                Task.Delay(TimeSpan.FromSeconds(3)).Wait(context.CancellationToken);
            }
            context.Logger.LogInfo("Finish!");
        }
        public static void WillFail(IContext context)
        {
            context.Errors.Add(new Error("SampleError", "An error has occurred. "));
        }
        public static void ThrowException(IContext context)
        {
            throw new ApplicationException("An error has occurred. ");
        }
    }
}