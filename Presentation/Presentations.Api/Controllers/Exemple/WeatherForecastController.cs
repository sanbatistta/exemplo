using AutoMapper;
using Exemple.Domain.AggregateModels;
using Exemple.Domain.Commands;
using Exemple.Domain.Interfaces.Queries;
using Exemple.Domain.Jobs;
using Harpy.Domain.Interfaces.Mediator;
using Harpy.Domain.Interfaces.Queries;
using Harpy.Presentation.Controller;
using Harpy.Transaction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Myth.ValueObjects.OdataObjects;
using Myth.ViewModels.Errors;
using NSwag.Annotations;
using Presentations.Api.Application.ViewModels.Exemple;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Presentations.Api.Controllers.Exemple {

    [AllowAnonymous]
    [Route( "api/Exemple/" )]
    [OpenApiTags( "Exemple" )]
    public class WeatherForecastController: ApiController {

        private readonly IWeatherForecastQuery _weatherForecastQuery;

        public WeatherForecastController(
                IMediatorHandler mediator,
                IMapper mapper,
                IWeatherForecastQuery weatherForecastQuery )
                : base( mediator, mapper ) {
            _weatherForecastQuery = weatherForecastQuery;
        }

        [HttpGet( "[controller]" )]
        [OpenApiOperation( "Get all weather forecast",
            "Return all forecasts save in database. \n" +
            "At this endpoint, operations similar to the ODATA standard can be used. \n" +
            "Filter example:\n" +
            "- temperatureC gt 10 ( temperature in celsius greater than 10 )\n" +
            "- temperatureC le -10:int ( celsius temperature less than or equal to - 10 )\n" +
            "- summary eq Hot ( Summary equals hot )\n" +
            "                      \n" +
            "Sorting examples:     \n" +
            "- summary              \n" +
            "- temperatureC desc   \n" +
            "- temperatureF asc \n " +
            "\n" +
            "Paging examples: \n" +
            "- PageNumber: 2 \n" +
            "- PageSize: 2" )]
        [ProducesResponseType( typeof( IPaginated<WeatherForecastViewModel> ), StatusCodes.Status200OK )]
        public async Task<IActionResult> GetAsync( Odata<WeatherForecastViewModel, WeatherForecast> odata, CancellationToken cancellationToken ) {
            var weatherForecasts = await _weatherForecastQuery.GetAsync( odata.Build( _mapper ), cancellationToken );
            var result = _mapper.Map<IPaginated<WeatherForecastViewModel>>( weatherForecasts );
            return Response( result );
        }

        [HttpGet( "[controller]/{id}" )]
        [OpenApiOperation( "Get single weather forecast", "Return forecast by id" )]
        [ProducesResponseType( typeof( WeatherForecastViewModel ), StatusCodes.Status200OK )]
        [ProducesResponseType( typeof( ValidationResponse ), StatusCodes.Status404NotFound )]
        public async Task<IActionResult> GetAsync( [FromRoute] long id, CancellationToken cancellationToken ) {
            var weatherForecasts = await _weatherForecastQuery.GetAsync( id, cancellationToken );
            var result = _mapper.Map<WeatherForecastViewModel>( weatherForecasts );
            return Response( result );
        }

        [HttpPost( "[controller]" )]
        [OpenApiOperation( "Post weather forecast", "Create a new forecast" )]
        [ProducesResponseType( typeof( WeatherForecastViewModel ), StatusCodes.Status200OK )]
        [ProducesResponseType( typeof( ValidationResponse ), StatusCodes.Status404NotFound )]
        public async Task<IActionResult> PostAsync( [FromBody] PostWeatherForecastViewModel postWeatherForecast, CancellationToken cancellationToken ) {
            var command = _mapper.Map<PostWeatherForecastCommand>( postWeatherForecast );
            var result = await _mediator.SendCommandAsync<PostWeatherForecastCommand, WeatherForecast>( command, cancellationToken );
            var response = _mapper.Map<WeatherForecast, WeatherForecastViewModel>( result );

            var simpleJob = new WeatherForecastUpdatedJob( response.WeatherForecastId );
            var jobId = _mediator.RaiseJob( simpleJob );

            var continuedJob = new WeatherForecastContinuedJob( response.WeatherForecastId, jobId );
            _mediator.RaiseJob( continuedJob );

            return ResponseRedirect( "Get", new { id = response?.WeatherForecastId } );
        }

        [HttpPost( "[controller]/WithTransaction" )]
        [OpenApiOperation( "Post weather forecast using transaction",
            "Create a new forecast \n" +
            "## SQLite does not support multiple transactions, to use transactions do not use persistent jobs \n" +
            "## Uncomment the '.UseTransaction( )' line at Startup" )]
        [ProducesResponseType( typeof( WeatherForecastViewModel ), StatusCodes.Status200OK )]
        [ProducesResponseType( StatusCodes.Status404NotFound )]
        public async Task<IActionResult> PostAsync( [FromBody] PostWeatherForecastViewModel postWeatherForecast, CancellationToken cancellationToken, [FromServices] DbTransaction dbTransaction ) {
            // SQLite does not support multiple transactions, to use transactions do not use persistent jobs
            // Uncomment the '.UseTransaction( )' line at Startup
            await using var transaction = new Transactions( dbTransaction );
            var command = _mapper.Map<PostWeatherForecastCommand>( postWeatherForecast );
            var result = await _mediator.SendCommandAsync<PostWeatherForecastCommand, WeatherForecast>( command, cancellationToken );
            var response = _mapper.Map<WeatherForecast, WeatherForecastViewModel>( result );

            await transaction.CommitAsync( );

            var simpleJob = new WeatherForecastUpdatedJob( response.WeatherForecastId );
            var jobId = _mediator.RaiseJob( simpleJob );

            var continuedJob = new WeatherForecastContinuedJob( response.WeatherForecastId, jobId );
            _mediator.RaiseJob( continuedJob );

            return ResponseRedirect( "Get", new { id = response?.WeatherForecastId } );
        }
    }
}