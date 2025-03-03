using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;

namespace StudyFlow.Application.ApplicationUsers.Commands.ForgotPassword;
public class ForgotPasswordComand : IRequest<Output>
{
    public string? Email { get; init; }
    public string? SoDienThoai { get; init; }
}
public class ForgotPasswordComandHandler : IRequestHandler<ForgotPasswordComand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    public ForgotPasswordComandHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }
    public async Task<Output> Handle(ForgotPasswordComand request, CancellationToken cancellationToken)
    {
        Output output = new Output();
        if((string.IsNullOrEmpty(request.Email)&&string.IsNullOrEmpty(request.SoDienThoai))||
            (request.Email!=null && request.SoDienThoai!=null))
        {
            throw new WrongInputException();
        }
        else
        {
            if (request.Email != null)
            {
                if (!IsValidEmail(request.Email)) throw new FormatException();
                output = await _identityService.ForgotPasswordByEmail(request.Email, "Thay đổi mật khẩu mới");
            }
            if(request.SoDienThoai!=null)
            {
                if (!IsValidPhoneNumber(request.SoDienThoai)) throw new FormatException();
                output = await _identityService.ForgotPasswordByPhone(request.SoDienThoai);
            }          
        }
        return output;
    }
    private bool IsValidEmail(string email)
    {
        var regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
        return regex.IsMatch(email);
    }
    private bool IsValidPhoneNumber(string phoneNumber)
    {
        // Kiểm tra số điện thoại có đúng định dạng 10-11 chữ số và bắt đầu bằng số 0 hay không
        var regex = new Regex(@"^0\d{9,10}$");
        return regex.IsMatch(phoneNumber);
    }
}
