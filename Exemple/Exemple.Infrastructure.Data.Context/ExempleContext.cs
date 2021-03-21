using Exemple.Domain.AggregateModels;
using Exemple.Infrastructure.Data.Context.Mappings;
using Harpy.Context;
using Harpy.IoC.Options;
using Microsoft.EntityFrameworkCore;

namespace Exemple.Infrastructure.Data.Context {

    public class ExempleContext: BaseContext {
        public DbSet<WeatherForecast> WeatherForecasts { get; private set; }

        public ExempleContext( DbContextOptions options, Parameters parameters = null ) : base( options ) {
        }

        protected override void OnModelCreating( ModelBuilder modelBuilder ) {
            modelBuilder
                .ApplyConfiguration( new WeatherForecastMap( ) );

            base.OnModelCreating( modelBuilder );
        }
    }
}