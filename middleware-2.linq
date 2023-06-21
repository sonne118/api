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

//https://github.com/aspnet/Entropy/blob/master/samples/Builder.Filtering.Web/Startup.cs

app.Use(BufferAsync);
app.Run(async context =>
{
	context.Response.Headers.Add("Content-Type", "text/html");
	await context.Response.WriteAsync("<h1>This sample uses buffering</h1>");
	await context.Response.WriteAsync("<p>This allows all your Middlewares to write to Response.</p>");
});

async Task BufferAsync(HttpContext context, RequestDelegate next)
{
	var body = context.Response.Body;
	var buffer = new MemoryStream();
	context.Response.Body = buffer;

	try
	{
		await context.Response.WriteAsync("<html><body>");
		await next(context);
		await context.Response.WriteAsync("</body></html>");

		buffer.Position = 0;
		await buffer.CopyToAsync(body);
	}
	finally
	{
		context.Response.Body = body;
	}
}

app.Run();