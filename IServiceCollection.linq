<Query Kind="Program">
  <Output>DataGrids</Output>
  <NuGetReference>Microsoft.Extensions.Configuration</NuGetReference>
  <NuGetReference>Microsoft.Extensions.DependencyInjection</NuGetReference>
  <NuGetReference>Microsoft.Extensions.DependencyInjection.Abstractions</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Options</NuGetReference>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
</Query>

void Main()
{
	
}

static IServiceCollection AddPostgres(this IServiceCollection services, IConfiguration configuration)
{
	services.Configure<PostgresOptions>(configuration.GetRequiredSection(OptionsSectionName));
	var postgresOptions = configuration.GetOptions<PostgresOptions>(OptionsSectionName);
	services.AddDbContext<MySpotDbContext>(x => x.UseNpgsql(postgresOptions.ConnectionString));
	services.AddScoped<IWeeklyParkingSpotRepository, PostgresWeeklyParkingSpotRepository>();
	services.AddScoped<IUserRepository, PostgresUserRepository>();
	services.AddHostedService<DatabaseInitializer>();
	services.AddScoped<IUnitOfWork, PostgresUnitOfWork>();

	services.TryDecorate(typeof(ICommandHandler<>), typeof(UnitOfWorkCommandHandlerDecorator<>));

	// EF Core + Npgsql issue
	AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

	return services;
}
