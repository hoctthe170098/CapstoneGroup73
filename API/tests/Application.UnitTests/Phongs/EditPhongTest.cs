using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Phongs.Commands.CreatePhong;
using StudyFlow.Application.Phongs.Commands.EditPhong;

namespace StudyFlow.Application.UnitTests.Phongs;
[TestFixture]
public class EditPhongTest
{
    private Mock<IApplicationDbContext> _contextMock;
    private Mock<IIdentityService> _identityServiceMock;
    private Mock<IHttpContextAccessor> _httpContextAccessor;
    private EditPhongCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _identityServiceMock = new Mock<IIdentityService>();
        _httpContextAccessor = new Mock<IHttpContextAccessor>();
        _handler = new EditPhongCommandHandler(_contextMock.Object, _identityServiceMock.Object, _httpContextAccessor.Object);
    }

    [Test]
    public void Handle_ShouldThrowException_WhenRequiredFieldsAreEmpty()
    {
        var command = new EditPhongCommand
        {
            Id = 1,
            Ten = " ",
            TrangThai ="use"
        };

        Assert.ThrowsAsync<WrongInputException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_ShouldThrowException_WhenRoomNameExceedValidLength()
    {
        var command = new EditPhongCommand
        {
            Id = 1,
            Ten = "AL-01",
            TrangThai = " "
        };

        Assert.ThrowsAsync<WrongInputException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_ShouldThrowException_WhenStatusIsInvalid()
    {
        var command = new EditPhongCommand
        {
            Id = 1,
            Ten = "AL-01",
            TrangThai = "ooppeenn"
        };

        Assert.ThrowsAsync<WrongInputException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
