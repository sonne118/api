<Query Kind="Program">
  <Output>DataGrids</Output>
  <NuGetReference>Microsoft.Extensions.Http.Polly</NuGetReference>
  <NuGetReference>Polly</NuGetReference>
  <NuGetReference>Polly.Caching.Distributed</NuGetReference>
  <NuGetReference>Polly.Caching.Memory</NuGetReference>
  <NuGetReference>Polly.Caching.Serialization.Json</NuGetReference>
  <NuGetReference>Polly.Contrib.WaitAndRetry</NuGetReference>
  <NuGetReference>Polly.Extensions.Http</NuGetReference>
  <NuGetReference>Polly-Signed</NuGetReference>
  <Namespace>Microsoft.AspNetCore.Http</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Polly</Namespace>
  <Namespace>Polly.CircuitBreaker</Namespace>
  <Namespace>Polly.Fallback</Namespace>
  <Namespace>Polly.Timeout</Namespace>
  <Namespace>Polly.Wrap</Namespace>
  <Namespace>System.Collections.Concurrent</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

namespace UserRegService.Resilient
{
	public class ResilientHttpClient : IResilientHttpClient
	{

		static CircuitBreakerPolicy<HttpResponseMessage> _circuitBreakerPolicy;
		static Policy<HttpResponseMessage> _retryPolicy;
		static FallbackPolicy<HttpResponseMessage> _fallbackPolicy;
		static FallbackPolicy<HttpResponseMessage> _fallbackCircuitBreakerPolicy;
		static TimeoutPolicy<HttpResponseMessage> _timeoutPolicy;
		HttpClient _client;

		public ResilientHttpClient(HttpClient client, CircuitBreakerPolicy<HttpResponseMessage> circuitBreakerPolicy)
		{
			_client = client;
			_client.DefaultRequestHeaders.Accept.Clear();
			_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			//circuit breaker policy injected as defined in the Startup class   
			_circuitBreakerPolicy = circuitBreakerPolicy;

			//Defining retry policy
			_retryPolicy = Policy.HandleResult<HttpResponseMessage>(x =>
			{
				var result = !x.IsSuccessStatusCode;
				return result;
			})
			.Or<TimeoutException>()
			//Retry 3 times and for each retry wait for 3 seconds
			.WaitAndRetry(3, sleepDuration => TimeSpan.FromSeconds(3));


			_fallbackCircuitBreakerPolicy = Policy<HttpResponseMessage>
				.Handle<BrokenCircuitException>()
				.Fallback(new HttpResponseMessage(HttpStatusCode.OK)
				{
					Content = new StringContent("Please try again later[Circuit breaker is Open]")
				}
				);


			_fallbackPolicy = Policy.HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.InternalServerError)
				.Or<TimeoutRejectedException>()
				.Fallback(new HttpResponseMessage(HttpStatusCode.OK)
				{
					Content = new StringContent("Some error occured")
				});

			_timeoutPolicy = Policy.Timeout<HttpResponseMessage>(1);
		}


		//To do HTTP Get request
		public HttpResponseMessage Get(string uri)
		{
			//Invoke ExecuteWithRetryandCircuitBreaker method that wraps the code with retry and circuit breaker policies
			return ExecuteWithRetryandCircuitBreaker(uri, () =>
			{
				try
				{
					var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
					var response = _client.SendAsync(requestMessage).Result;
					return response;

				}
				catch (Exception ex)
				{
					//Handle exception and return InternalServerError as response code
					HttpResponseMessage res = new HttpResponseMessage();
					res.StatusCode = HttpStatusCode.InternalServerError;
					return res;
				}
			});
		}

		//To do HTTP POST request
		public HttpResponseMessage Post<T>(string uri, T item)
		{
			//Invoke ExecuteWithRetryandCircuitBreaker method that wraps the code with retry and circuit breaker policies
			return ExecuteWithRetryandCircuitBreaker(uri, () =>
			{
				try
				{
					var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

					requestMessage.Content = new StringContent(JsonConvert.SerializeObject(item), System.Text.Encoding.UTF8, "application/json");

					var response = _client.SendAsync(requestMessage).Result;

					return response;

				}
				catch (Exception ex)
				{
					//Handle exception and return InternalServerError as response code
					HttpResponseMessage res = new HttpResponseMessage();
					res.StatusCode = HttpStatusCode.InternalServerError;
					return res;
				}
			});
		}

		//To do HTTP PUT request
		public HttpResponseMessage Put<T>(string uri, T item)
		{
			//Invoke ExecuteWithRetryandCircuitBreaker method that wraps the code with retry and circuit breaker policies
			return ExecuteWithRetryandCircuitBreaker(uri, () =>
			{
				try
				{
					var requestMessage = new HttpRequestMessage(HttpMethod.Put, uri);

					requestMessage.Content = new StringContent(JsonConvert.SerializeObject(item), System.Text.Encoding.UTF8, "application/json");

					var response = _client.SendAsync(requestMessage).Result;

					return response;
				}
				catch (Exception ex)
				{
					//Handle exception and return InternalServerError as response code
					HttpResponseMessage res = new HttpResponseMessage();
					res.StatusCode = HttpStatusCode.InternalServerError;
					return res;
				}

			});
		}

		//To do HTTP DELETE request
		public HttpResponseMessage Delete(string uri)
		{
			//Invoke ExecuteWithRetryandCircuitBreaker method that wraps the code with retry and circuit breaker policies
			return ExecuteWithRetryandCircuitBreaker(uri, () =>
			{
				try
				{
					var requestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

					var response = _client.SendAsync(requestMessage).Result;

					return response;

				}
				catch (Exception ex)
				{
					//Handle exception and return InternalServerError as response code
					HttpResponseMessage res = new HttpResponseMessage();
					res.StatusCode = HttpStatusCode.InternalServerError;
					return res;
				}
			});

		}


		//Wrap function body in Retry and Circuit breaker policies
		public HttpResponseMessage ExecuteWithRetryandCircuitBreaker(string uri, Func<HttpResponseMessage> func)
		{


			//      PolicyWrap<HttpResponseMessage> resiliencePolicyWrap = Policy.Wrap(_retryPolicy, _circuitBreakerPolicy);
			//    PolicyWrap<HttpResponseMessage> fallbackPolicyWrap = _fallbackForAnyExceptionPolicy.Wrap(resiliencePolicyWrap);
			//  var res= fallbackPolicyWrap.Execute(() => func());
			// var res = _fallbackPolicy.Execute(() =>_retryPolicy.Wrap(_circuitBreakerPolicy).Execute(() => func()));

			PolicyWrap<HttpResponseMessage> resiliencePolicyWrap = Policy.Wrap(_timeoutPolicy, _retryPolicy, _circuitBreakerPolicy);

			PolicyWrap<HttpResponseMessage> fallbackPolicyWrap = _fallbackPolicy.Wrap(_fallbackCircuitBreakerPolicy.Wrap(resiliencePolicyWrap));

			var res = fallbackPolicyWrap.Execute(() => func());
			return res;
			//var res = _fallbackPolicy.Execute(() => _retryPolicy.Execute(() => func()));
			//return res;
		}

	}

	public interface IResilientHttpClient
	{
		HttpResponseMessage Get(string uri);

		HttpResponseMessage Post<T>(string uri, T item);

		HttpResponseMessage Delete(string uri);

		HttpResponseMessage Put<T>(string uri, T item);
	}
}
