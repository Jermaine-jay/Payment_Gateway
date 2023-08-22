using System.Security.Cryptography;

namespace Payment_Gateway.Models.Extensions
{
    public class GenerateApiKey
    {
        public static string GenerateAnApiKey()
        {
            var rng = new RNGCryptoServiceProvider();
            var bytes = new byte[32];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
