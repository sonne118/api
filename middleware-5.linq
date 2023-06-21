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

app.Map("/hello", (IApplicationBuilder pp) =>
{
	//nested
	app.Map("/world", (IApplicationBuilder ppa) => ppa.Run(context =>
		context.Response.WriteAsync($"Path: {context.Request.Path} - Path Base: {context.Request.PathBase}")));

	pp.Run(context =>
		context.Response.WriteAsync($"Path: {context.Request.Path} - Path Base: {context.Request.PathBase}"));
});

app.Run(context =>
{
	context.Response.Headers.Add("Content-Type", "text/html");
	return context.Response.WriteAsync(@"<a href=""/hello"">/hello</a> <a href=""/hello/world"">/hello/world</a>");
});

app.Run();