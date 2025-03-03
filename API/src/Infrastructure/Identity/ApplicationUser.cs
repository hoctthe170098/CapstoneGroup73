using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public bool IsActive { get; set; } = true;
}
