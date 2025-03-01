
using Microsoft.AspNetCore.Authorization;
using StudyFlow.Application.ApplicationUsers.Commands.ChangePassword;
using StudyFlow.Application.ApplicationUsers.Commands.ForgotPassword;
using StudyFlow.Application.ApplicationUsers.Commands.Login;
using StudyFlow.Application.Common.Models;
using StudyFlow.Domain.Constants;

namespace StudyFlow.Web.Endpoints;

public class ApplicationUsers : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(Login,"Login")
            .MapPost(ForgotPassword,"ForgotPassword")
            .MapPost(ChangePassword,"ChangePassword");
    }
    //[Authorize(Policy = Policies.CanPurge)]
    public async Task<Output> Login(ISender sender, LoginComand comand)
    {
        return await sender.Send(comand);
    }
    
    public async Task<Output> ForgotPassword(ISender sender, ForgotPasswordComand comand)
    {
        return await sender.Send(comand);
    }
    [Authorize(Policy = Policies.AuthenticatedUser)]
    public async Task<Output> ChangePassword(ISender sender, ChangePasswordComand comand)
    {
        return await sender.Send(comand);
    }
}
