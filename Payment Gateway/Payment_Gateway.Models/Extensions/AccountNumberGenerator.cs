using System.Text;

namespace Payment_Gateway.Models.Extensions
{
    public static class AccountNumberGenerator
    {
        public static string GenerateRandomNumber()
        {
            Random randomFirst = new Random();
            long minValue = 20;
            long maxValue = 29;
            int randomNumber = randomFirst.Next((int)minValue, (int)maxValue);

            StringBuilder chars = new StringBuilder(randomNumber.ToString(), 10);


            Random randomnext = new Random();
            long minValue2 = 10000000;
            long maxValue2 = 99999999;
            int randomNumber2 = randomnext.Next((int)minValue2, (int)maxValue2);
            chars.AppendLine(randomNumber2.ToString());
            return chars.ToString();
        }


    }
}
