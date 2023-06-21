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
  <Namespace>Microsoft.AspNetCore.Http</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

var app = WebApplication.Create();
app.UseMiddleware<TerminalMiddleware>();
app.Run();

//A middleware class in ASP.NET Core is simply a class that 
// - Take a constructor with RequestDelegate
// - implements Invoke method taking HttpContext and returning Task
//If you take a look at this code, it cannot be any simpler.
//This is a terminal middleware. It does not invoke the subsequent middleware. It just returns its own response and that's it.

public class TerminalMiddleware
{
	public TerminalMiddleware(RequestDelegate next)
	{
		//We are not using the parameter next in this middleware since this middleware is terminal
	}

	public async Task Invoke(HttpContext context)
	{
		await context.Response.WriteAsync("Hello world");
	}
}
