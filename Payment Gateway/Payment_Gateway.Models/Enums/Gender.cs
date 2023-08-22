#nullable disable

namespace Payment_Gateway.Models.Enums
{
    public enum Gender
    {
        Male = 1,
        Female,
        Others
    }

    public static class GenderExtension
    {
        public static string GetStringValue(this Gender gender)
        {
            return gender switch
            {
                Gender.Male => "Male",
                Gender.Female => "Female",
                Gender.Others => "Others",
                _ => null
            };
        }

    }
}