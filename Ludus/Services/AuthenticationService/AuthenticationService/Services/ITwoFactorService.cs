using System.Text;
namespace AuthenticationService.Services
{
    public interface ITwoFactorService
    {
        Task<bool> SendTwoFactorCodeAsync(string email);

        Task<bool> VerifyTwoFactorCodeAsync(string email, string code);
    }
}
