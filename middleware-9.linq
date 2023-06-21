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
builder.Services.AddSingleton<Greeting>();
builder.Services.AddSingleton<Goodbye>();

var app = builder.Build();
app.UseMiddleware(typeof(TerminalMiddleware));
app.Run();

public class Greeting
{
	public string Greet() => "Good morning";
}

public class Goodbye
{
	public string Say() => "Goodbye";
}

public class TerminalMiddleware
{
	Greeting _greet;

	public TerminalMiddleware(RequestDelegate next, Greeting greet)
	{
		_greet = greet;
	}

	public async Task Invoke(HttpContext context, Goodbye goodbye)
	{
		await context.Response.WriteAsync($"{_greet.Greet()} {goodbye.Say()}");
	}
}
