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

builder.Services.AddSingleton(x => new SingletonDate());
builder.Services.AddTransient(x => new TransientDate());
builder.Services.AddScoped(x => new ScopedDate());

builder.Services.AddScoped(x => new MyDependency());

var app = builder.Build();

app.Use(async(context, next) =>
{
	var scoped = context.RequestServices.GetService<MyDependency>();

	await context.Response.WriteAsync($"scoped: {scoped.WriteMessage("Hiyyaaaa")}\n");
	 await next.Invoke();
});

//app.Run();

app.Use(async (context, next) =>
{
	var single = context.RequestServices.GetService<SingletonDate>();
	var scoped = context.RequestServices.GetService<ScopedDate>();
	var transient = context.RequestServices.GetService<TransientDate>();

	await context.Response.WriteAsync("Open this page in two tabs \n");
	await context.Response.WriteAsync("Keep refreshing and you will see the three different DI behaviors\n");
	await context.Response.WriteAsync("----------------------------------\n");
	await context.Response.WriteAsync($"Singleton : {single.Date.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}\n");
	await context.Response.WriteAsync($"Scoped: {scoped.Date.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}\n");
	await context.Response.WriteAsync($"Transient: {transient.Date.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}\n");
	await next.Invoke();
});

app.Run(async (context) =>
{
	await Task.Delay(100);//delay for 100 ms

	var single = context.RequestServices.GetService<SingletonDate>();
	var scoped = context.RequestServices.GetService<ScopedDate>();
	var transient = context.RequestServices.GetService<TransientDate>();

	await context.Response.WriteAsync("----------------------------------\n");
	await context.Response.WriteAsync($"Singleton : {single.Date.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}\n");
	await context.Response.WriteAsync($"Scoped: {scoped.Date.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}\n");
	await context.Response.WriteAsync($"Transient: {transient.Date.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}\n");
});

app.Run();

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

public interface IMyDependency
{
	string WriteMessage(string message);
}

public class MyDependency : IMyDependency
{
	public MyDependency() => WriteMessage("Hi Hi");


	public string WriteMessage(string message)
	{
		Console.WriteLine($"MyDependency.WriteMessage Message: {message}");
		return message;
	}
}