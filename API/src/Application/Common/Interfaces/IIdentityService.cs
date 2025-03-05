using StudyFlow.Application.Common.Models;

namespace StudyFlow.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId);
    Task<bool> IsInRoleAsync(string userId, string role);
    Task<bool> AuthorizeAsync(string userId, string policyName);
    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);
    Task<Result> DeleteUserAsync(string userId);
    Task<string?> GenerateJwtToken(string username,string password);
    Task<Output> ForgotPasswordByEmail(string email, string title);
    Task<Output> ForgotPasswordByPhone(string phone);
    Task<Output> ChangePassword(string token, string oldPassword, string newPassword);
    Task<List<string>> GetRolesByUserId(string userId);
    Task<bool> AssignRoleAsync(string userId, string role);
}
