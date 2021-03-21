using Exemple.Domain.Interfaces.Repositories;
using Exemple.Domain.Jobs;
using Harpy.Application.JobHandlers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Exemple.Application.JobHandlers {

    public class WeatherForecastUpdatedHandler: JobHandler<WeatherForecastUpdatedJob> {

        private readonly IWeatherForecastRepository _weatherForecastRepository;

        public WeatherForecastUpdatedHandler( IWeatherForecastRepository weatherForecastRepository ) {
            _weatherForecastRepository = weatherForecastRepository;
        }

        public override async Task Handle( WeatherForecastUpdatedJob notification, CancellationToken cancellationToken ) {
            var weatherForeacast = await _weatherForecastRepository.FindAsync( cancellationToken, notification.WeatherForecastId );

            weatherForeacast.UpdateDate( DateTime.Now );

            await _weatherForecastRepository.UpdateAsync( weatherForeacast, cancellationToken );

            await _weatherForecastRepository.SaveChangesAsync( cancellationToken );
        }
    }
}