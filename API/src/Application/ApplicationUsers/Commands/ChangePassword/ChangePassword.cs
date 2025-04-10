using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;

namespace StudyFlow.Application.ApplicationUsers.Commands.ChangePassword;
    public class ChangePasswordComand : IRequest<Output>
    {
        public string? token { get; init; }
        public string? oldPassword { get; init; }
        public string? newPassword { get; init; }
    }
    public class ChangePasswordComandHandler : IRequestHandler<ChangePasswordComand, Output>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        public ChangePasswordComandHandler(IApplicationDbContext context, IIdentityService identityService)
        {
            _context = context;
            _identityService = identityService;
        }

    public async Task<Output> Handle(ChangePasswordComand request, CancellationToken cancellationToken)
    {
        if(string.IsNullOrWhiteSpace(request.token)||string.IsNullOrWhiteSpace(request.oldPassword)
            ||string.IsNullOrWhiteSpace(request.newPassword)) throw new NotFoundDataException();
        else
        {
            if (!IsValidPassword(request.newPassword)) throw new FormatException();
            return await _identityService
                .ChangePassword(request.token,request.oldPassword, request.newPassword);
        }
    }
    private bool IsValidPassword(string password)
    {
        // Kiểm tra độ dài tối thiểu
        if (password.Length < 8)
        {
            return false;
        }

        // Kiểm tra chữ hoa, chữ thường, chữ số và ký tự đặc biệt
        if (!password.Any(char.IsUpper) || !password.Any(char.IsLower) 
            || !password.Any(char.IsDigit) || !password.Any(ch => !char.IsLetterOrDigit(ch)))
        {
            return false;
        }

        return true;
    }
}

