<Query Kind="Program">
  <Output>DataGrids</Output>
  <NuGetReference>Humanizer</NuGetReference>
  <NuGetReference>Microsoft.AspNetCore.Http</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Logging.ApplicationInsights</NuGetReference>
  <Namespace>Humanizer</Namespace>
  <Namespace>Microsoft.AspNetCore.Http</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>MySpot.Core.Exceptions</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>


 //app.UseMiddleware<ExceptionMiddleware>();

internal sealed class ExceptionMiddleware : IMiddleware
{
	private readonly ILogger<ExceptionMiddleware> _logger;

	public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
	{
		_logger = logger;
	}

	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		try
		{
			await next(context);
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, exception.Message);
			await HandleExceptionAsync(exception, context);
		}
	}

	private async Task HandleExceptionAsync(Exception exception, HttpContext context)
	{
		var (statusCode, error) = exception switch
		{
			CustomException => (StatusCodes.Status400BadRequest,
				new Error(exception.GetType().Name.Underscore().Replace("_exception", string.Empty), exception.Message)),
			_ => (StatusCodes.Status500InternalServerError, new Error("error", "There was an error."))
		};

		context.Response.StatusCode = statusCode;
		await context.Response.WriteAsJsonAsync(error);
	}

	private record Error(string Code, string Reason);


}

public abstract class CustomException : Exception
{
	protected CustomException(string message) : base(message)
	{
	}
}