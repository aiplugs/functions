using System.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Aiplugs.Functions.Core
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAiplugsFunctions<TParams>(this IApplicationBuilder app)
        {
            var services = app.ApplicationServices;
            var lifetime = services.GetRequiredService<IApplicationLifetime>();
            var migration = services.GetRequiredService<IMigration>();
            var jobService = services.GetRequiredService<IJobService>();
            var procedureResolver = services.GetRequiredService<IProcedureResolver>();
            var contextFactory = services.GetRequiredService<IContextFactory<TParams>>();
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("Aiplugs.Functions");

            if (migration.NeedMigrate())
                migration.Migrate();

            var runner = new JobRunner<TParams>(jobService, procedureResolver, contextFactory, logger);

            lifetime.ApplicationStopping.Register(() => runner.NotifyStop());
            lifetime.ApplicationStopped.Register(() => runner.Dispose());

            runner.Start();

            return app;
        }
    }
}