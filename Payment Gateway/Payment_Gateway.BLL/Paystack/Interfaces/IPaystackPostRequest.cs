namespace Payment_Gateway.BLL.Paystack.Interfaces
{
    public interface IPaystackPostRequest
    {
        Task<HttpResponseMessage> PostRequest<T>(string url, T? request) where T : class;
        Task<HttpResponseMessage> GetRequest(string apiUrl);
    }
}
