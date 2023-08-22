using Payment_Gateway.BLL.Infrastructure.CacheServices;
using Payment_Gateway.BLL.Infrastructure.EmailSender;
using Payment_Gateway.BLL.Infrastructure.jwt;
using Payment_Gateway.BLL.Infrastructure.Paystack;

namespace Payment_Gateway.BLL.Infrastructure
{
    public class Settings
    {
        public PaystackConfig PaystackConfig { get; set; } = null!;
        public JwtConfig JwtConfig { get; set; } = null!;
        public RedisConfig redisConfig { get; set; } = null!;
        public ZeroBounceConfig ZeroBounceConfig { get; set; } = null!;
        public EmailSenderOptions EmailSenderOptions { get; set; } = null!;
    }
}
