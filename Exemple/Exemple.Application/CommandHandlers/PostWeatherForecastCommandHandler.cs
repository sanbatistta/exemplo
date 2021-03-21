﻿using Exemple.Domain.AggregateModels;
using Exemple.Domain.Commands;
using Exemple.Domain.Events;
using Exemple.Domain.Interfaces.Repositories;
using FluentValidation;
using Harpy.Application.CommandHandlers;
using Harpy.Domain.Interfaces.Mediator;
using System.Threading;
using System.Threading.Tasks;

namespace Exemple.Application.CommandHandlers {

    public class PostWeatherForecastCommandHandler: CommandHandler<PostWeatherForecastCommand, WeatherForecast> {

        private readonly IWeatherForecastRepository _weatherForecastRepository;

        public PostWeatherForecastCommandHandler(
            IMediatorHandler bus,
            IValidator<PostWeatherForecastCommand> validator,
            IWeatherForecastRepository weatherForecastRepository )
            : base( bus, validator ) {
            _weatherForecastRepository = weatherForecastRepository;
        }

        public override async Task<WeatherForecast> Handle( PostWeatherForecastCommand command, CancellationToken cancellationToken ) {
            await EnsureIsValidAsync( command, cancellationToken );

            var weatherForecast = new WeatherForecast(
                command.Date,
                command.TemperatureC,
                command.Summary
                );

            await _weatherForecastRepository.AddAsync( weatherForecast, cancellationToken );

            await _weatherForecastRepository.SaveChangesAsync( cancellationToken );

            var @event = new WeatherForecastCreatedEvent( weatherForecast.WeatherForecastId );

            await _bus.RaiseEventAsync( @event, cancellationToken );

            return weatherForecast;
        }
    }
}