using System.Net;

namespace Payment_Gateway.API.Extensions
{

	public class ServiceResponse<T> where T : class
	{	
		public T? Data { get; set; }
	
		public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
	
		public string? Message { get; set; }
		public bool Success { get; set; }
	}

	public class ServiceResponse
	{
	
		public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
		
		public string? Message { get; set; }
		public bool Success { get; set; }
	}
}
