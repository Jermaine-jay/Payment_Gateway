namespace Payment_Gateway.Models.Enums
{
    public enum UserType
    {
        User = 1,
        Admin,
        SuperAdmin,
        ThirdParty,
    }

    public static class UserTypeExtension
    {
        public static string? GetStringValue(this UserType userType)
        {
            return userType switch
            {
                UserType.User => "User",
                UserType.Admin => "Admin",
                UserType.SuperAdmin => "SuperAdmin",
                UserType.ThirdParty => "ThirdParty",
                _ => null
            };
        }
    }
}
