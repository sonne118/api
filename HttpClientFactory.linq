<Query Kind="Statements">
  <Output>DataGrids</Output>
  <NuGetReference>Microsoft.AspNetCore.Mvc</NuGetReference>
  <NuGetReference>System.Net.Http</NuGetReference>
  <NuGetReference>System.Net.Http.Json</NuGetReference>
  <NuGetReference>System.Net.Http.WinHttpHandler</NuGetReference>
  <Namespace>Microsoft.AspNetCore.Mvc</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

public class HomeController : Controller
{
	public Task<string> Bad()
	{
		var client = new HttpClient { BaseAddress = new Uri("https://localhost:5003") };

		return client.GetStringAsync($"/homes/{Guid.NewGuid()}");
	}

	public Task<string> Simple([FromServices] IHttpClientFactory factory)
	{
		var client = factory.CreateClient("simple");

		return client.GetStringAsync($"/homes/{Guid.NewGuid()}");
	}


	public Task<string> Typed([FromServices] CustomHttpClient client)
	{
		return client.GetHome();
	}


	public Task<Home> Shared([FromServices] ApiClient client)
	{
		return client.GetHome();
	}