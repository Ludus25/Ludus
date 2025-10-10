using AuthenticationService.Entities;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationService.Services
{
    public class TwoFactorService : ITwoFactorService
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;

        public TwoFactorService(
            UserManager<User> userManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<bool> SendTwoFactorCodeAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !user.TwoFactorEnabled)
                return false;


            var code = await _userManager.GenerateTwoFactorTokenAsync(
                user, TokenOptions.DefaultEmailProvider);


            string subject = "Your Two-Factor Authentication Code";
            string body = $"Your verification code is: <strong>{code}</strong>.";

            await _emailSender.SendEmailAsync(user.Email, subject, body);
            return true;
        }

        public async Task<bool> VerifyTwoFactorCodeAsync(string email, string code)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !user.TwoFactorEnabled)
                return false;

            return await _userManager.VerifyTwoFactorTokenAsync(
                user, TokenOptions.DefaultEmailProvider, code);
        }
    }
}
