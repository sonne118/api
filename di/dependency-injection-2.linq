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
  <NuGetReference>System.Threading.Tasks.Dataflow</NuGetReference>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.AspNetCore.Http</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

var builder = WebApplication.CreateBuilder();

builder.Services.AddSingleton<SingletonDate>();
builder.Services.AddTransient<TransientDate>();
builder.Services.AddScoped<ScopedDate>();
builder.Services.AddTransient<DateProvider>();

var app = builder.Build();

app.Use(async (context, next) =>
{
	var dateProvider = context.RequestServices.GetService<DateProvider>();
	var single = dateProvider.Singleton;
	var scoped = dateProvider.Scoped;
	var transient = dateProvider.Transient; ;

	await context.Response.WriteAsync("Open this page in two tabs \n");
	await context.Response.WriteAsync("Keep refreshing and you will see the three different DI behaviors\n");
	await context.Response.WriteAsync("----------------------------------\n");
	await context.Response.WriteAsync($"Singleton : {single.Date.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}\n");
	await context.Response.WriteAsync($"Scoped: {scoped.Date.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}\n");
	await context.Response.WriteAsync($"Transient: {transient.Date.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}\n");
	await next(context);
});

app.Run(async (context) =>
{
	await Task.Delay(100);//delay for 100 ms

	var dateProvider = context.RequestServices.GetService<DateProvider>();
	var single = dateProvider.Singleton;
	var scoped = dateProvider.Scoped;
	var transient = dateProvider.Transient; ;

	await context.Response.WriteAsync("----------------------------------\n");
	await context.Response.WriteAsync($"Singleton : {single.Date.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}\n");
	await context.Response.WriteAsync($"Scoped: {scoped.Date.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}\n");
	await context.Response.WriteAsync($"Transient: {transient.Date.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}\n");
});

app.Run();

public class DateProvider
{
	readonly IServiceProvider _provider;

	public SingletonDate Singleton => _provider.GetService<SingletonDate>();

	public ScopedDate Scoped => _provider.GetService<ScopedDate>();

	public TransientDate Transient => _provider.GetService<TransientDate>();

	public DateProvider(IServiceProvider provider)
	{
		_provider = provider;
	}
}

public class SingletonDate
{
	public DateTime Date { get; set; } = DateTime.Now;
}

public class TransientDate
{
	public DateTime Date { get; set; } = DateTime.Now;
}

public class ScopedDate
{
	public DateTime Date { get; set; } = DateTime.Now;
}