using System;
using System.Data;
using Aiplugs.Functions.Core.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Aiplugs.Functions.Core
{
    public static class ServiceCollectionsExtensions
    {
        public static IServiceCollection AddAiplugsFunctions(this IServiceCollection services, Func<OptionsBuilder, OptionsBuilder> options)
        {
            var opts = options(new OptionsBuilder());
            services.AddTransient<IMigration>(provider => opts.MigrationFactory(provider.GetRequiredService<IDbConnection>()));
            services.AddTransient<IJobRepository>(provider => opts.JobRepositoryFactory(provider.GetRequiredService<IDbConnection>()));
            services.AddSingleton<IJobRegistory, JobRegistory>();
            services.AddTransient<IJobService, JobService>();
            return services;
        }
    }
}