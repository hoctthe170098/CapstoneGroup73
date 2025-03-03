using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;

namespace StudyFlow.Application.ApplicationUsers.Commands.Login;
public class LoginComand :IRequest<Output>
{
    public string? UserName { get; init; }
    public string? Password { get; init; }
}
public class LoginComandHandler : IRequestHandler<LoginComand, Output>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    public LoginComandHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<Output> Handle(LoginComand request, CancellationToken cancellationToken)
    {
        if (request.UserName == null|| request.Password==null)
        {
            throw new NotFoundDataException();
        }
        Output output = new Output();
        var token = await _identityService.GenerateJwtToken(request.UserName, request.Password);
        if(token == null)
        {
            output.isError = true;
            output.code = 404;
            output.message = "Tên đăng nhập hoặc mật khẩu không đúng";
        }
        else
        {
            output.isError = false;
            output.data = token;
            output.code = 200;
            output.message = "Đăng nhập thành công";
        }
        return output;
    }
}
