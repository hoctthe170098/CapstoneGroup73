using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Cosos.Commands.CreateCoSo;
using StudyFlow.Application.Cosos.Commands.EditCoSo;

namespace StudyFlow.Application.UnitTests.CoSo;
[TestFixture]
public class EditCoSoTest
{
    private Mock<IApplicationDbContext> _contextMock;
    private EditCoSoComandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _handler = new EditCoSoComandHandler(_contextMock.Object);
    }

    [Test]
    public void Handle_ShouldThrowException_WhenRequiredFieldsAreEmpty()
    {
        var command = new EditCoSoComand
        {
            Id = "",
            Ten = "",
            DiaChi = "",
            SoDienThoai = "",
            TrangThai = ""
        };

        Assert.ThrowsAsync<NotFoundDataException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_ShouldThrowException_WhenSoDienThoaiIsInvalid()
    {
        var command = new EditCoSoComand
        {
            Id = "1",
            Ten = "Hoa Hong",
            DiaChi = "Dia Chi Test",
            SoDienThoai = "1122334455",
            TrangThai = "open"
        };

        Assert.ThrowsAsync<FormatException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_ShouldThrowException_WhenInputLengthInvalid()
    {
        var command = new EditCoSoComand
        {
            Id = "1",
            Ten = new string('a', 31),
            DiaChi = new string('a', 201),
            SoDienThoai = new string('a', 12),
            TrangThai = "open"
        };

        Assert.ThrowsAsync<WrongInputException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_ShouldThrowException_WhenStatusInvalid()
    {
        var command = new EditCoSoComand
        {
            Id = "1",
            Ten = "Hoa Hong",
            DiaChi = "Dia Chi Test",
            SoDienThoai = "0122334455",
            TrangThai = "use"
        };

        Assert.ThrowsAsync<WrongInputException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
