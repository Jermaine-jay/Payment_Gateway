using System.Text;

namespace Payment_Gateway.Models.Utilities
{
    public static class AccountNumberGenerator
    {
        public static string GenerateRandomNumber()
        {
            StringBuilder chars = new StringBuilder();
            Random randomFirst = new Random();
            long minValue = 20;
            long maxValue = 29;
            int randomNumber = randomFirst.Next((int)minValue, (int)maxValue);
            chars.Append(randomNumber.ToString());


            Random randomnext = new Random();
            long minValue2 = 10000000;
            long maxValue2 = 99999999;
            int randomNumber2 = randomnext.Next((int)minValue2, (int)maxValue2);
            chars.Append(randomNumber2.ToString());
            return chars.ToString();
        }


    }
}
