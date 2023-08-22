using Payment_Gateway.Models.Entities;
using Payment_Gateway.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Payment_Gateway.Shared.DataTransferObjects.Requests
{
    public class LoginRequest
    {
        [Required]
        public string UserNameOrEmail { get; set; }

        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class TwoFactorLoginRequest
    {
        public ApplicationUser ApplicationUser { get; set; }
        public string Token { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string UserId { get; set; }
        public string Token { get; set; }

        [Required, DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
        
        [Required, DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }


    public class ValidateTokenRequest
    {
        public string UserId { get; set; }
        public string Token { get; set; }
    }


    public class AddUserToRoleRequest
    {
        public string UserName { get; set; }
        public string? Role { get; set; }
    }

    public class VerifyAccountRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string Token { get; set; }

    }

    public class UserRegistrationRequest
    {
        [Required]
        public string Firstname { get; set; }

        [Required]
        public string LastName { get; set; }

        public string? OtherName { get; set; }


		[Required, DataType(DataType.Password)]
		public string Password { get; set; }

        [Required]
        [EmailAddress(ErrorMessage ="Invalid Email Address")]
        public string Email { get; set; }

        [Phone]
		public string PhoneNumber { get; set; }

        [Required]
        public string UserName { get; set; }

    }

    public class ResetPasswordRequest
    {
        [Required]
        public string Token { get; set; }

        [Required, DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required,DataType(DataType.Password), Compare(nameof(NewPassword))]
        public string ConfirmPassword { get; set; }
    }

    public class UpdateRecoveryMailRequest
    {
        [Required]
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        public string Email { get; set; }
    }

    public class ChangeEmailRequest
    {
        [Required]
        public string NewEmail { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }
    }

    public class ChangeEmailRequestDto
    {
        [Required]
        public string NewEmail { get; set; }

        [Required]
        public string RecoveryEmail { get; set; }
    }
    public class ForgotPasswordRequestDto
    {
	    [Required, DataType(DataType.EmailAddress)]
	    public string Email { get; set; }
    }
}

