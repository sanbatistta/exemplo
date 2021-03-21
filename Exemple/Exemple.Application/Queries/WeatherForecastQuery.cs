using Exemple.Domain.AggregateModels;
using Exemple.Domain.Interfaces.Queries;
using Exemple.Domain.Interfaces.Repositories;
using Exemple.Domain.Specifications;
using Harpy.Domain.Interfaces.Queries;
using Harpy.Domain.Specifications;
using Harpy.Domain.Specifications.Odata;
using Myth.ValueObjects.QueryObjects;
using System.Threading;
using System.Threading.Tasks;

namespace Exemple.Application.Queries {

    public class WeatherForecastQuery: IWeatherForecastQuery {

        private readonly IWeatherForecastRepository _weatherForecastRepository;

        public WeatherForecastQuery( IWeatherForecastRepository weatherForecastRepository ) {
            _weatherForecastRepository = weatherForecastRepository;
        }

        public Task<bool> ExistsAsync( long id, CancellationToken cancellationToken ) {
            var spec = SpecBuilder<WeatherForecast>.Create( )
                .WithId( id );

            return _weatherForecastRepository.AnyAsync( spec, cancellationToken );
        }

        public Task<bool> NotExistsAsync( int temperature, CancellationToken cancellationToken ) {
            var spec = SpecBuilder<WeatherForecast>.Create( )
                .WithNotTemperature( temperature );

            return _weatherForecastRepository.AnyAsync( spec, cancellationToken );
        }

        public ValueTask<WeatherForecast> GetAsync( long id, CancellationToken cancellationToken ) {
            return _weatherForecastRepository.FindAsync( cancellationToken, id );
        }

        public Task<IPaginated<WeatherForecast>> GetAsync( Odata<WeatherForecast> odata, CancellationToken cancellationToken ) {
            var spec = SpecBuilder<WeatherForecast>.Create( )
                .Odata( odata );

            return _weatherForecastRepository.SearchPaginatedAsync( spec, cancellationToken );
        }
    }
}