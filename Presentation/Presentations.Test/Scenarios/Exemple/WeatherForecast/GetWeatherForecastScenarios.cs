using Harpy.Domain.Models;
using Harpy.Template;
using Microsoft.AspNetCore.Mvc.Testing;
using Myth.Extensions;
using Presentations.Api.Application.ViewModels.Exemple;
using Presentations.Test.Scenarios.Exemple.WeatherForecast.Base;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace Presentations.Test.Scenarios.Exemple.WeatherForecast {

    public class GetWeatherForecastScenarios: WeatherForecastScenarios {

        public GetWeatherForecastScenarios( WebApplicationFactory<Startup> factory, ITestOutputHelper output )
            : base( factory, output ) {
        }

        [Fact]
        public async void Get_all_weather_forecast_ok( ) {
            var response = await _client
                .RequestAsync( $"{_apiUrl}/{_endpointUrl}", CancellationToken.None )
                .ThenGet<Paginated<WeatherForecastViewModel>>( );

            Assert.NotNull( response );
        }

        [Fact]
        public async void Get_first_weather_forecast_ok( ) {
            var response = await _client
                .RequestAsync( $"{_apiUrl}/{_endpointUrl}/1", CancellationToken.None )
                .ThenGet<WeatherForecastViewModel>( );

            Assert.NotNull( response );
        }
    }
}