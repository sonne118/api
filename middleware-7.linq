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
  <Namespace>Microsoft.AspNetCore.Builder.Extensions</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

var app = WebApplication.Create();

//Use MapMiddleWare and MapWhenMiddlware directly
var whenOption = new MapWhenOptions
{
	Branch =
		context =>
			context.Response.WriteAsync($"MapWhenMiddleware| Path: {context.Request.Path} - Path Base: {context.Request.PathBase}"),
	Predicate = context => context.Request.Path.Value.Contains("hello")
};

app.UseMiddleware<MapWhenMiddleware>(whenOption);

var mapOption = new MapOptions
{
	Branch =
		context =>
			context.Response.WriteAsync($"MapMiddleware| Path: {context.Request.Path} - Path Base: {context.Request.PathBase}"),
	PathMatch = "/greetings"
};

app.UseMiddleware<MapMiddleware>(mapOption);

app.Run(context =>
{
	context.Response.Headers.Add("content-type", "text/html");
	return context.Response.WriteAsync(@"<a href=""/hello"">/hello</a> <a href=""/greetings"">/greetings</a>");
});

app.Run();