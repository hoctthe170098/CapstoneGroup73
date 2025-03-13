using System.Linq;
using Ardalis.GuardClauses;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception, "Exception occurred: {Message}", exception.Message);
        Output output = new Output
        {
            isError = true,
            errors = new[] { exception.Message }
        };
        switch (exception)
        {
            case NotFoundDataException:
                output.code = 404;
                output.message = "Thiếu dữ liệu đầu vào, vui lòng nhập đầy đủ";
                break;
            case WrongInputException:
                output.code = 404;
                output.message = "Dữ liệu nhập sai,vui lòng nhập lại!";
                break;
            case FormatException:
                output.code = 404;
                output.message = "Định dạng dữ liệu không hợp lệ!";
                break;
            case NotFoundIDException:
                output.code = 404;
                output.message = "Dữ liệu không tồn tại!";
                break;
            case InvalidCodeException:
                output.code = 500;
                output.message = "Mã code đã tồn tại!";
                break;
            default:
                output.code = 500;
                output.message =  string.Join("; ", new[] { exception.Message });
                break;
        }
        httpContext.Response.StatusCode = StatusCodes.Status200OK;
        await httpContext.Response.WriteAsJsonAsync(output, cancellationToken);
        return true;
    }
}
