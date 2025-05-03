using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Phongs.Commands.CreatePhong;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.UnitTests.Phongs;
[TestFixture]
public class CreatePhongTest
{
    private Mock<IApplicationDbContext> _contextMock;
    private Mock<IIdentityService> _identityServiceMock;
    private Mock<IHttpContextAccessor> _httpContextAccessor;
    private CreatePhongCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _identityServiceMock = new Mock<IIdentityService>();
        _httpContextAccessor = new Mock<IHttpContextAccessor>();
        _handler = new CreatePhongCommandHandler(_contextMock.Object, _identityServiceMock.Object, _httpContextAccessor.Object);
    }

    [Test]
    public void Handle_ShouldThrowException_WhenRequiredFieldsAreEmpty()
    {
        var command = new CreatePhongCommand
        {
            Ten = ""
        };

        Assert.ThrowsAsync<WrongInputException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_ShouldThrowException_WhenRoomNameExceedValidLength()
    {
        var command = new CreatePhongCommand
        {
            Ten = new string('a', 21)
        };

        Assert.ThrowsAsync<WrongInputException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_ShouldThrowException_WhenRoomNameDuplicate()
    {
        var command = new CreatePhongCommand
        {
            Ten = "AL-403"
        };

        var fakeToken = "fake-token";
        var fakeCoSoId = Guid.Parse("9bceb7b0-1ef2-4cab-bcb2-a765db8bad3f");

        var context = new DefaultHttpContext();
        context.Request.Headers["Authorization"] = $"Bearer {fakeToken}";
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

        _identityServiceMock.Setup(x => x.GetCampusId(fakeToken)).Returns(fakeCoSoId);

        Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }

}
