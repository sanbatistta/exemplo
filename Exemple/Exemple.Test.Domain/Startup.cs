using Exemple.Infrastructure.CrossCutting.IoC;
using Exemple.Infrastructure.Data.Context;
using Harpy.IoC;
using Harpy.IoC.SQLite;
using Harpy.Test.Domain;
using Harpy.Test.Domain.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Exemple.Test.Domain {

    public class Startup: StartupTestBase {

        public override void ConfigureServices( IServiceCollection services ) {
            base.ConfigureServices( services );

            services.AddDatabase(
                connection => connection.UseSqliteConnection( ),
                context => {
                    context.AddContext<ExempleContext>( );
                }
            );

            services.CreateTestDatabase<ExempleContext>( );

            services.AddExemple( triggerJobs: false );
        }
    }
}