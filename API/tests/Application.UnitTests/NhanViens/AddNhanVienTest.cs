using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using StudyFlow.Application.Common.Exceptions;
using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Application.Common.Models;
using StudyFlow.Application.NhanViens.Command.CreateNhanVien;
using StudyFlow.Domain.Constants;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Application.UnitTests.NhanViens;
[TestFixture]
public class AddNhanVienTest
{
    private Mock<IApplicationDbContext> _contextMock;
    private Mock<IIdentityService> _identityServiceMock;
    private CreateNhanVienCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _identityServiceMock = new Mock<IIdentityService>();
        _handler = new CreateNhanVienCommandHandler(_contextMock.Object, _identityServiceMock.Object);
    }

    [Test]
    public void Handle_ShouldThrowException_WhenRequiredFieldsAreEmpty()
    {
        var command = new CreateNhanVienCommand
        {
            Code = "",
            Ten = "",
            GioiTinh = "",
            DiaChi = "",
            NgaySinh = "",
            Email = "",
            SoDienThoai = "",
            CoSoId = "",
            Role = ""
        };

        Assert.ThrowsAsync<NotFoundDataException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_ShouldThrowException_WhenRoleIsInvalid()
    {
        var command = new CreateNhanVienCommand
        {
            Code = "123",
            Ten = "Test",
            GioiTinh = "nam",
            DiaChi = "Test Address",
            NgaySinh = "1990-01-01",
            Email = "test@example.com",
            SoDienThoai = "0123456789",
            CoSoId = "0CD95BFC-9D86-4D9D-900A-5683CBF93BC3D",
            Role = "InvalidRole"
        };

        Assert.ThrowsAsync<WrongInputException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public async Task Handle_ShouldCreateNhanVien_WhenDataIsValid()
    {
        var command = new CreateNhanVienCommand
        {
            Code = "123111",
            Ten = "Nguyen Van A",
            GioiTinh = "nam",
            DiaChi = "123 Test Street",
            NgaySinh = "1990-01-01",
            Email = "mytr123@gmail.com",
            SoDienThoai = "01234900125",
            CoSoId = "0CD95BFC-9D86-4D9D-900A-5683CBF93BC3D",
            Role = Roles.CampusManager
        };

        //_contextMock.Setup(x => x.CoSos.AnyAsync(It.IsAny<Expression<Func<CoSo, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _contextMock.Setup(x => x.NhanViens.AnyAsync(It.IsAny<Expression<Func<NhanVien, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _identityServiceMock
            .Setup(x => x.GenerateUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((Result.Success(), "user123"));

        var result = await _handler.Handle(command, CancellationToken.None);

        _contextMock.Verify(x => x.NhanViens.Add(It.IsAny<NhanVien>()), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _identityServiceMock.Verify(x => x.AssignRoleAsync("user123", Roles.CampusManager), Times.Once);

        Assert.IsFalse(result.isError);
        Assert.That(result.code, Is.EqualTo(200));
        Assert.That(result.message, Is.EqualTo("Tạo nhân viên mới thành công"));
    }

    [Test]
    public void Handle_ShouldThrowException_WhenEmailIsInvalid()
    {
        var command = new CreateNhanVienCommand
        {
            Code = "123",
            Ten = "Test",
            GioiTinh = "nam",
            DiaChi = "Test Address",
            NgaySinh = "1990-01-01",
            Email = "testexample.com",
            SoDienThoai = "0123456789",
            CoSoId = "0CD95BFC-9D86-4D9D-900A-5683CBF93BC3D",
            Role = Roles.CampusManager
        };

        Assert.ThrowsAsync<FormatException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_ShouldThrowException_WhenInputExceedCValidLength()
    {
        var command = new CreateNhanVienCommand
        {
            Code = new string('a', 21),
            Ten = new string('a', 51),
            GioiTinh = "nam",
            DiaChi = new string('a', 201),
            NgaySinh = "1990-01-01",
            Email = "test@example.com",
            SoDienThoai = "0123456789",
            CoSoId = "0CD95BFC-9D86-4D9D-900A-5683CBF93BC3D",
            Role = Roles.CampusManager
        };

        Assert.ThrowsAsync<WrongInputException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_ShouldThrowException_WhenGenderInvalid()
    {
        var command = new CreateNhanVienCommand
        {
            Code = "123",
            Ten = "Test",
            GioiTinh = "Male",
            DiaChi = "Test Address",
            NgaySinh = "1990-01-01",
            Email = "test@example.com",
            SoDienThoai = "0123456789",
            CoSoId = "0CD95BFC-9D86-4D9D-900A-5683CBF93BC3D",
            Role = Roles.CampusManager
        };

        Assert.ThrowsAsync<WrongInputException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_ShouldThrowException_WhenPhoneInvalid()
    {
        var command = new CreateNhanVienCommand
        {
            Code = "123",
            Ten = "Test",
            GioiTinh = "nam",
            DiaChi = "Test Address",
            NgaySinh = "1990-01-01",
            Email = "test@example.com",
            SoDienThoai = "1123456789",
            CoSoId = "0CD95BFC-9D86-4D9D-900A-5683CBF93BC3D",
            Role = Roles.CampusManager
        };

        Assert.ThrowsAsync<FormatException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_ShouldThrowException_WhenDobInvalid()
    {
        var command = new CreateNhanVienCommand
        {
            Code = "123",
            Ten = "Test",
            GioiTinh = "nam",
            DiaChi = "Test Address",
            NgaySinh = "01-01-1991",
            Email = "test@example.com",
            SoDienThoai = "0123456789",
            CoSoId = "0CD95BFC-9D86-4D9D-900A-5683CBF93BC3D",
            Role = Roles.CampusManager
        };

        Assert.ThrowsAsync<FormatException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_ShouldThrowException_WhenDobIsUnder18()
    {
        var command = new CreateNhanVienCommand
        {
            Code = "123",
            Ten = "Test",
            GioiTinh = "nam",
            DiaChi = "Test Address",
            NgaySinh = "2021-01-01",
            Email = "test@example.com",
            SoDienThoai = "0123456789",
            CoSoId = "0CD95BFC-9D86-4D9D-900A-5683CBF93BC3D",
            Role = Roles.CampusManager
        };

        Assert.ThrowsAsync<WrongInputException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_ShouldThrowException_WhenPhoneOrEmailDuplicate()
    {
        var command = new CreateNhanVienCommand
        {
            Code = "123",
            Ten = "Test",
            GioiTinh = "nam",
            DiaChi = "Test Address",
            NgaySinh = "1991-01-01",
            Email = "itssocool7102@gmail.com", // Email đã tồn tại
            SoDienThoai = "0123456789",
            CoSoId = "0CD95BFC-9D86-4D9D-900A-5683CBF93BC3D",
            Role = Roles.CampusManager
        };

        Assert.ThrowsAsync<WrongInputException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
