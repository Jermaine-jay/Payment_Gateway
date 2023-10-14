namespace Payment_Gateway.BLL.Infrastructure.CacheServices
{
    public class RedisConfig
    {
        public string InstanceId { get; set; } = null!;
        public string Host { get; set; } = null!;
        public string IP { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int Port { get; set; }
    }
}
