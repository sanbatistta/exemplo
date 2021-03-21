using Exemple.Domain.Commands;
using Exemple.Domain.Interfaces.Queries;
using Exemple.Infrastructure.Data.Resource;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace Exemple.Domain.Validations.Commands.Exemple {

    public class PostWeatherForecastCommandValidation: AbstractValidator<PostWeatherForecastCommand> {

        private readonly IWeatherForecastQuery _weatherForecastQuery;

        public PostWeatherForecastCommandValidation( IWeatherForecastQuery weatherForecastQuery ) {
            _weatherForecastQuery = weatherForecastQuery;

            #region [ Validations ]

            DateCantBeNull( );
            SummaryCantBeNull( );
            TemperatureCantBeNull( );
            TemperatureMustNotExist( );

            #endregion [ Validations ]
        }

        private Task<bool> TemperatureMustNotExists( int temperature, CancellationToken cancellationToken ) {
            return _weatherForecastQuery.NotExistsAsync( temperature, cancellationToken );
        }

        protected void DateCantBeNull( ) =>
                    RuleFor( x => x.Date )
                .NotEmpty( )
                .WithMessage( string.Format( Messages.CantBeNull, Names.Date ) );

        protected void TemperatureCantBeNull( ) =>
            RuleFor( x => x.TemperatureC )
                .NotEmpty( )
                .WithMessage( string.Format( Messages.CantBeNull, Names.TemperatureC ) );

        protected void SummaryCantBeNull( ) =>
            RuleFor( x => x.Summary )
                .NotEmpty( )
                .WithMessage( string.Format( Messages.CantBeNull, Names.Summary ) );

        protected void TemperatureMustNotExist( ) =>
            RuleFor( x => x.TemperatureC )
                .MustAsync( TemperatureMustNotExists )
                .WithMessage( Messages.TemperatureExist );
    }
}