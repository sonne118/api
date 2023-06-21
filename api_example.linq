<Query Kind="Program">
  <Output>DataGrids</Output>
  <NuGetReference>App.Metrics.AspNetCore</NuGetReference>
  <NuGetReference>App.Metrics.AspNetCore.Core</NuGetReference>
  <NuGetReference>Microsoft.AspNetCore.Http</NuGetReference>
  <NuGetReference>Microsoft.AspNetCore.Http.Features</NuGetReference>
  <NuGetReference>Microsoft.AspNetCore.Mvc</NuGetReference>
  <NuGetReference>Microsoft.EntityFrameworkCore</NuGetReference>
  <NuGetReference>Microsoft.EntityFrameworkCore.Abstractions</NuGetReference>
  <NuGetReference>Microsoft.EntityFrameworkCore.InMemory</NuGetReference>
  <NuGetReference>Microsoft.EntityFrameworkCore.Relational</NuGetReference>
  <NuGetReference>Microsoft.EntityFrameworkCore.Sqlite.Core</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Configuration</NuGetReference>
  <NuGetReference>Microsoft.Extensions.DependencyInjection</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Http.Polly</NuGetReference>
  <NuGetReference>NUnitLite</NuGetReference>
  <NuGetReference>Swashbuckle</NuGetReference>
  <NuGetReference>Swashbuckle.AspNetCore</NuGetReference>
  <NuGetReference>Swashbuckle.AspNetCore.Swagger</NuGetReference>
  <NuGetReference>System.Threading.Tasks.Dataflow</NuGetReference>
  <Namespace>LINQPad.ObjectModel</Namespace>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.AspNetCore.Http</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>Microsoft.OpenApi.Models</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>



void Main()
{
	// Reference https://docs.microsoft.com/en-us/learn/modules/build-web-api-minimal-api
	var builder = WebApplication.CreateBuilder();
	builder.Services.AddEndpointsApiExplorer();
	builder.Services.AddSwaggerGen(s => s.SwaggerDoc("v1", new OpenApiInfo { Title = "Todo API", Description = "Keep track of your tasks" }));
	builder.Services.AddDbContext<PizzaDb>(options => options.UseInMemoryDatabase("items"));
	var app = builder.Build();
	
	//app.UseSwagger();
	//app.UseSwaggerUI(s => s.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1"));
	
	app.MapGet("/", () => "Hello World!");

	app.MapGet("/pizzas", async (PizzaDb db) => await db.Pizzas.ToListAsync());
	app.MapGet("/pizza/{id}", async (PizzaDb db, int id) => await db.Pizzas.FindAsync(id));

	app.MapPost("/pizza", async (PizzaDb db, Pizza pizza) =>
	{
		await db.Pizzas.AddAsync(pizza);
		await db.SaveChangesAsync();
		return Results.Created($"/pizza/{pizza.Id}", pizza);
	});
	
	app.MapPut("/pizza/{id}", async (PizzaDb db, Pizza updatePizza, int id) => {
		var pizza = await db.Pizzas.FindAsync(id);
		if (pizza is null) return Results.NotFound();
		pizza.Name = updatePizza.Name;
		pizza.Description = updatePizza.Description;
		await db.SaveChangesAsync();
		return Results.NoContent();
	});
	
	app.MapDelete("/pizza/{id}", async (PizzaDb db, int id) => {
		var pizza = await db.Pizzas.FindAsync(id);
		if (pizza is null) return Results.NotFound();
		db.Pizzas.Remove(pizza);
		await db.SaveChangesAsync();
		return Results.Ok();
	});

	app.Run();
}

// You can define other methods, fields, classes and namespaces here
public class Pizza
{
	public int Id { get; set; }
	public string? Name { get; set; }
	public string? Description { get; set; }
}

class PizzaDb : DbContext 
{
	public PizzaDb(DbContextOptions options) : base(options) { }
	public DbSet<Pizza> Pizzas { get; set; }
}