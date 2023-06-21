<Query Kind="Statements">
  <Output>DataGrids</Output>
  <NuGetReference>App.Metrics.AspNetCore</NuGetReference>
  <NuGetReference>App.Metrics.AspNetCore.Core</NuGetReference>
  <NuGetReference>Microsoft.AspNetCore.Http</NuGetReference>
  <NuGetReference>Microsoft.AspNetCore.Http.Features</NuGetReference>
  <NuGetReference>Microsoft.AspNetCore.Mvc</NuGetReference>
  <NuGetReference>Microsoft.EntityFrameworkCore</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Configuration</NuGetReference>
  <NuGetReference>Microsoft.Extensions.DependencyInjection</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Http.Polly</NuGetReference>
  <NuGetReference>NUnitLite</NuGetReference>
  <NuGetReference>Swashbuckle.AspNetCore</NuGetReference>
  <NuGetReference>Swashbuckle.AspNetCore.SwaggerGen</NuGetReference>
  <NuGetReference>Swashbuckle.AspNetCore.SwaggerUI</NuGetReference>
  <NuGetReference>System.Threading.Tasks.Dataflow</NuGetReference>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.AspNetCore.Builder.Extensions</Namespace>
  <Namespace>Microsoft.AspNetCore.Http</Namespace>
  <Namespace>Microsoft.AspNetCore.Http.Features</Namespace>
  <Namespace>Microsoft.AspNetCore.Http.Features.Authentication</Namespace>
  <Namespace>Microsoft.AspNetCore.WebUtilities</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection.Extensions</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>Microsoft.Extensions.ObjectPool</Namespace>
  <Namespace>Microsoft.Extensions.Options</Namespace>
  <Namespace>Microsoft.Extensions.Primitives</Namespace>
  <Namespace>Microsoft.Net.Http.Headers</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
  <RuntimeVersion>6.0</RuntimeVersion>
</Query>

var builder = WebApplication.CreateBuilder();
builder.Services.AddSingleton<Greeter>();
builder.Services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, GreeterUpdaterService>();

var app = builder.Build();

//app.UseSwagger();
//app.UseSwaggerUI(s => s.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1"));

app.MapGet("/", () => "Hello World!");
app.Run(context =>
{
	var greet = context.RequestServices.GetService<Greeter>();

	return context.Response.WriteAsync($"Please reload page (greeting updated every 1 second in the background) {greet}");
});

app.Run();

public class GreeterUpdaterService : IHostedService, IDisposable
{
	Greeter _greeter;
	readonly ILogger<GreeterUpdaterService> _logger;

	Timer _timer;

	public GreeterUpdaterService(ILogger<GreeterUpdaterService> logger, Greeter greeter)
	{
		_logger = logger;
		_greeter = greeter;
	}

	private void DoWork(object state)
	{
		_greeter.Counter++;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation($"{nameof(GreeterUpdaterService)} running.");

		_timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation($"{nameof(GreeterUpdaterService)} is stopping.");

		_timer?.Change(Timeout.Infinite, 0);

		return Task.CompletedTask;
	}

	public void Dispose()
	{
		_timer?.Dispose();
	}
}

public class Greeter
{
	public int Counter { get; set; }

	public override string ToString() => $"Hello world {Counter}";
}

