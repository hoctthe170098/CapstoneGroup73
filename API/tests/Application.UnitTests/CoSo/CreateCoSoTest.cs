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
using StudyFlow.Application.Cosos.Commands.CreateCoSo;
using StudyFlow.Application.Phongs.Commands.CreatePhong;

namespace StudyFlow.Application.UnitTests.CoSo;
[TestFixture]
public class CreateCoSoTest
{
    private Mock<IApplicationDbContext> _contextMock;
    private CreateCoSoComandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _handler = new CreateCoSoComandHandler(_contextMock.Object);
    }

    [Test]
    public void Handle_ShouldThrowException_WhenRequiredFieldsAreEmpty()
    {
        var command = new CreateCoSoComand
        {
            Ten = "",
            DiaChi = "",
            SoDienThoai = ""
        };

        Assert.ThrowsAsync<NotFoundDataException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_ShouldThrowException_WhenSoDienThoaiIsInvalid()
    {
        var command = new CreateCoSoComand
        {
            Ten = "Van Cao",
            DiaChi = "New Dia Chi",
            SoDienThoai = "1123456789"
        };

        Assert.ThrowsAsync<FormatException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_ShouldThrowException_WhenInputLengthInvalid()
    {
        var command = new CreateCoSoComand
        {
            Ten = new string ('a', 31),
            DiaChi = new string('a', 201),
            SoDienThoai = new string('a', 12)
        };

        Assert.ThrowsAsync<WrongInputException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
