using Exemple.Domain.AggregateModels;
using Harpy.Domain.Interfaces.Queries;
using Harpy.Domain.Interfaces.Queries.Base;
using Myth.ValueObjects.QueryObjects;
using System.Threading;
using System.Threading.Tasks;

namespace Exemple.Domain.Interfaces.Queries {

    public interface IWeatherForecastQuery: IQuery {

        Task<bool> ExistsAsync( long id, CancellationToken cancellationToken );

        Task<bool> NotExistsAsync( int temperature, CancellationToken cancellationToken );

        ValueTask<WeatherForecast> GetAsync( long id, CancellationToken cancellationToken );

        Task<IPaginated<WeatherForecast>> GetAsync( Odata<WeatherForecast> odata, CancellationToken cancellationToken );
    }
}