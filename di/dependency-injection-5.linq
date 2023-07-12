<Query Kind="Program">
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
  <Namespace>System.Net.Http</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
</Query>


class Program
{
	private static IServiceProvider serviceProvider;

	static void Main(string[] args)
	{
		ConfigureServices();

		var orderManager = serviceProvider.GetService<IOrderManager>();
		var order = CreateOrder();

		var content = orderManager.Transmit(order);
		Console.WriteLine(content);
	}

	private static void ConfigureServices()
	{
		var services = new ServiceCollection();

		services.AddTransient<IOrderSender, HttpOrderSender>();
		services.AddTransient<IOrderManager, OrderManager>();

		serviceProvider = services.BuildServiceProvider();
	}

	private static Order CreateOrder()
	{
		return new Order
		{
			CustomerId = "12345",
			Date = new DateTime(),
			TotalAmount = 145,
			Items = new System.Collections.Generic.List<OrderItem>
				{
					new OrderItem {
						ItemId = "99999",
						Quantity = 1,
						Price = 145
					}
				}
		};
	}
}


public interface IOrderManager
{
	public Task<string> Transmit(Order order);
}

public class OrderManager : IOrderManager
{
	private IOrderSender orderSender;

	public OrderManager(IOrderSender sender)
	{
		orderSender = sender;
	}

	public async Task<string> Transmit(Order order)
	{
		return await orderSender.Send(order);
	}
}


public interface IOrderSender
{
	Task<string> Send(Order order);
}

public class HttpOrderSender : IOrderSender
{
	private static readonly HttpClient httpClient = new HttpClient();

	public async Task<string> Send(Order order)
	{
		var jsonOrder = System.Text.Json.JsonSerializer.Serialize<Order>(order);
		var stringContent = new StringContent(jsonOrder, UnicodeEncoding.UTF8, "application/json");

		//This statement calls a not existing URL. This is just an example...
		var response = await httpClient.GetStringAsync("https://www.microsoft.com/");

		return response;
	}
}


public class Order
{
	public string CustomerId { get; set; }
	public DateTime Date { get; set; }
	public decimal TotalAmount { get; set; }
	public List<OrderItem> Items { get; set; }

	public Order()
	{
		Items = new List<OrderItem>();
	}
}

public class OrderItem
{
	public string ItemId { get; set; }
	public decimal Quantity { get; set; }
	public decimal Price { get; set; }
}
