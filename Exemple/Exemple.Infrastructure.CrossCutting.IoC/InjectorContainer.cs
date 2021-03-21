using Exemple.Domain.Jobs;
using Harpy.IoC;
using Microsoft.Extensions.DependencyInjection;

namespace Exemple.Infrastructure.CrossCutting.IoC {

    public static class InjectorContainer {

        private static IServiceCollection AddUpdateJob( this IServiceCollection services ) {
            var updateJob = new WeatherForecastPersistedJob( );
            services.RaiseBackgroundJob( updateJob );

            return services;
        }

        public static IServiceCollection AddExemple( this IServiceCollection services, bool triggerJobs = true ) {
            services.AddRepositories( );
            services.AddQueries( );

            if ( triggerJobs )
                services.AddUpdateJob( );

            return services;
        }
    }
}