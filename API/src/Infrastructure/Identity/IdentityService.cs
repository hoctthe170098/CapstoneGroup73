using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using StudyFlow.Domain.Constants;
using System.Net.Mail;
using System.Net;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio.Exceptions;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;

namespace StudyFlow.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;
    private readonly IConfiguration _configuration;
    private readonly IApplicationDbContext _context;
    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService,
        IConfiguration configuration, IApplicationDbContext context)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
        _configuration = configuration;
        _context = context;
    }

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await _userManager.Users.FirstAsync(u => u.Id == userId);

        return user.UserName;
    }

    public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = userName,
        };                  

        var result = await _userManager.CreateAsync(user, password);

        return (result.ToApplicationResult(), user.Id);
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        return user != null ? await DeleteUserAsync(user) : Result.Success();
    }

    public async Task<Result> DeleteUserAsync(ApplicationUser user)
    {
        var result = await _userManager.DeleteAsync(user);

        return result.ToApplicationResult();
    }

    public async Task<string?> GenerateJwtToken(string username,string password)
    {
        var user = await  _userManager.FindByNameAsync(username);
        if (user != null &&user.IsActive==true && await _userManager.CheckPasswordAsync(user, password))
        {
            var roles = await _userManager.GetRolesAsync(user);
            IdentityOptions _options = new IdentityOptions();
            var secretKey = _configuration["Jwt:SecretKey"] ?? throw new ArgumentNullException(nameof(_configuration), "Jwt:SecretKey is missing in configuration.");
            var issuer = _configuration["Jwt:Issuer"] ?? throw new ArgumentNullException(nameof(_configuration), "Jwt:Issuer is missing in configuration.");
            var audience = _configuration["Jwt:Audience"] ?? throw new ArgumentNullException(nameof(_configuration), "Jwt:Audience is missing in configuration.");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, username)
    };
            if (!roles.Contains(Roles.Administrator))
            {
                var staff = _context.NhanViens.FirstOrDefault(s => s.UserId == user.Id);
                var teacher = _context.GiaoViens.FirstOrDefault(s => s.UserId == user.Id);
                var student = _context.HocSinhs.FirstOrDefault(s => s.UserId == user.Id);
                if (staff != null)
                {
                    claims.Add(new Claim(ClaimTypes.Locality, staff.Coso.Id.ToString()));
                }
                else if (teacher != null)
                {
                    claims.Add(new Claim(ClaimTypes.Locality, teacher.Coso.Id.ToString()));
                }
                else if (student != null)
                {
                    claims.Add(new Claim(ClaimTypes.Locality, student.Coso.Id.ToString()));
                }
            }
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.Now.AddMinutes(120), // Token hết hạn sau 2 giờ
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        else return null;

    }
    public async Task<Output> ForgotPasswordByEmail(string email, string title)
    {
        Output output = new Output();
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            output.isError = false;
            output.message = "Email không khớp, vui lòng thử lại!";
            return output;
        };
        var newPassword = GenerateRandomPassword();
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user,token, newPassword);
        if (result.Succeeded)
        {
            try
            {
                await SendEmail(email, title, $"Mật khẩu mới của bạn là: {newPassword}");
                output.isError = false;
                output.message = "Đã gửi mật khẩu mới vào email";
                return output;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi gửi email. Có thể log lỗi.
                output.isError = true;
                output.message = "Lỗi gửi email"+ ex.Message;
                return output;
            }
        }
        else
        {
            output.isError = true;
            output.message = "Lỗi đặt lại mật khẩu:" +  string.Join(", ", result.Errors.Select(e => e.Description));
            return output;
        }
    }
    private string GenerateRandomPassword(int length = 8)
    {
        const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
        const string digitChars = "0123456789";
        const string specialChars = "!@#$%^&*()_+";

        var random = new Random();
        var password = new StringBuilder();

        // Đảm bảo có ít nhất 1 ký tự từ mỗi loại
        password.Append(uppercaseChars[random.Next(uppercaseChars.Length)]);
        password.Append(lowercaseChars[random.Next(lowercaseChars.Length)]);
        password.Append(digitChars[random.Next(digitChars.Length)]);
        password.Append(specialChars[random.Next(specialChars.Length)]);

        // Thêm các ký tự ngẫu nhiên còn lại
        string allChars = uppercaseChars + lowercaseChars + digitChars + specialChars;
        for (int i = password.Length; i < length; i++)
        {
            password.Append(allChars[random.Next(allChars.Length)]);
        }

        // Trộn chuỗi để tăng tính ngẫu nhiên
        return new string(password.ToString().OrderBy(s => random.Next()).ToArray());
    }
    private async Task SendEmail(string toEmail, string subject, string body)
    {
        var smtpServer = _configuration["Email:SmtpServer"];
        var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
        var smtpUsername = _configuration["Email:SmtpUsername"];
        var smtpPassword = _configuration["Email:SmtpPassword"];
        if (string.IsNullOrEmpty(smtpUsername))
        {
            smtpUsername = "trantrunghoc46@gmail.com";
        }
        using (var client = new SmtpClient(smtpServer, smtpPort))
        {
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);


            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpUsername),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
    public async Task<Output> ForgotPasswordByPhone(string phone)
    {
        Output output = new Output();
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phone);
        if (user == null)
        {
            output.isError = true;
            output.message = "Không tìm thấy người dùng với số điện thoại này.";
            return output;
        }
        string formattedPhoneNumber = FormatPhoneNumber(phone);
        try
        {
            // Tạo mã đặt lại mật khẩu ngẫu nhiên
            var resetCode = GenerateRandomPassword();

            // Gửi mã đặt lại mật khẩu qua SMS
            await SendSms(formattedPhoneNumber, $"Mật khẩu mới của bạn là: {resetCode}");

            // Lưu mã đặt lại mật khẩu vào cơ sở dữ liệu hoặc bộ nhớ cache cùng với số điện thoại của người dùng
            // ... (Triển khai logic lưu trữ mã đặt lại mật khẩu)

            output.isError = false;
            output.message = "Mã đặt lại mật khẩu đã được gửi qua SMS.";
            return output;
        }
        catch (TwilioException ex)
        {
            // Xử lý lỗi Twilio
            output.isError = true;
            output.message = $"Lỗi Twilio: {ex.Message}";
            return output;
        }
        catch (Exception ex)
        {
            // Xử lý các lỗi khác
            output.isError = true;
            output.message = $"Lỗi: {ex.Message}";
            return output;
        }
    }
    private string FormatPhoneNumber(string phoneNumber)
    {
        // Thêm mã quốc gia (+84) nếu cần thiết
        if (!phoneNumber.StartsWith("+84"))
        {
            if (phoneNumber.StartsWith("0"))
            {
                phoneNumber = "+84" + phoneNumber.Substring(1);
            }
            else
            {
                phoneNumber = "+84" + phoneNumber;
            }
        }

        return phoneNumber;
    }
    private async Task SendSms(string toPhoneNumber, string message)
    {
        var accountSid = _configuration["Twilio:AccountSid"];
        var authToken = _configuration["Twilio:AuthToken"];
        var from = _configuration["Twilio:FromPhoneNumber"];

        TwilioClient.Init(accountSid, authToken);

        // Tạo CreateMessageOptions
        var messageOptions = new CreateMessageOptions(new PhoneNumber(toPhoneNumber))
        {
            From = new PhoneNumber(from),
            Body = message
        };

        await MessageResource.CreateAsync(messageOptions);
    }
    public async Task<Output> ChangePassword(string token, string oldPassword, string newPassword)
    {
        Output output = new Output();
        try
        {
            // 1. Xác thực mã thông báo JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = _configuration["Jwt:SecretKey"] ?? throw 
                new ArgumentNullException(nameof(_configuration)
                , "Jwt:SecretKey is missing in configuration.");
            var key = Encoding.ASCII.GetBytes(secretKey);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var username = jwtToken.Claims.First(x => x.Type == ClaimTypes.Name).Value;
            var user = await _userManager.FindByNameAsync(username);
            // 2. Tìm người dùng
            if (user == null)
            {
                output.isError = true;
                output.message = "Không tìm thấy người dùng.";
                return output;
            }
            // 3. Thay đổi mật khẩu
            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (result.Succeeded)
            {
                output.isError = false;
                output.message = "Đổi mật khẩu thành công.";
                return output;
            }
            else
            {
                output.isError = true;
                output.message = "Đổi mật khẩu thất bại:" + string.Join(", ", result.Errors.Select(e => e.Description));
                return output;
            }
        }
        catch (Exception ex)
        {
            output.isError = true;
            output.message = "Lỗi: " + ex.Message;
            return output;
        }
    }

    public async Task<List<string>> GetRolesByUserId(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return new List<string>(); // Trả về empty nếu ko tìm thấy user
        var roles = await _userManager.GetRolesAsync(user);
        return roles.ToList();
    }

    public async Task<bool> AssignRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var validRoles = new List<string> { "Administrator", "Student", "LearningManager", "Teacher", "CampusManager" }; 
        if (!validRoles.Contains(roleName))
        {
            throw new Exception($"Vai trò '{roleName}' không hợp lệ.");
        }

        var result = await _userManager.AddToRoleAsync(user, roleName);
        return result.Succeeded;
    }

    private string genUsername(string name, string code)
    {
        string username = "";
        name = RemoveDiacritics(name).ToLower(); 
        string[] chuoi = name.Split(' ');

        for (int i = 0; i < chuoi.Length - 1; i++)
        {
            username += chuoi[i][0]; 
        }

        username = username + chuoi[chuoi.Length - 1] + code; 
        return username;
    }

    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
    private async Task SendAccountInfoEmail(string email, string name, string username, string password)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return;
        }

        string subject = "Thông tin tài khoản đăng nhập";

        string body = $@"
    <html>
    <body>
        <p>Xin chào <b>{name}</b>,</p>
        <p>Tài khoản của bạn đã được tạo thành công. Dưới đây là thông tin đăng nhập của bạn:</p>
        <p><b>Username:</b> {username}</p>
        <p><b>Password:</b> {password}</p>
        <p style='color:red;'><i>Vui lòng đăng nhập và đổi mật khẩu ngay sau khi đăng nhập.</i></p>
        <p>Trân trọng,</p>
        <p><b>Đội ngũ StudyFlow</b></p>
    </body>
    </html>
    ";

        await SendEmail(email, subject, body);
    }

    public async Task<(Result Result, string UserId)> GenerateUser(string name, string code, string email)
    {
        string username = genUsername(name, code);
        string password = GenerateRandomPassword();

        var user = new ApplicationUser
        {
            UserName = username,
            Email = email
        };

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await SendAccountInfoEmail(email, name, username, password);
        }

        return (result.ToApplicationResult(), user.Id);
    }

    public async Task<bool> IsUserActiveAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user != null && user.IsActive;
    }

    public Guid GetCampusId(string token)
    {
        throw new NotImplementedException();
    }
}
