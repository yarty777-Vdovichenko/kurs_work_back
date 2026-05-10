using kurswork_back.DTOs;
using kurswork_back.Models;
using kurswork_back.Repositories;
using kurswork_back.Services;
using Moq;

namespace kurswork_back.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IJwtService> _jwtService = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _service = new AuthService(
            _userRepo.Object,
            _jwtService.Object,
            _hasher.Object
        );
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsTokens()
    {
        var user = new User { Id = "1", Email = "test@gmail.com", PasswordHash = "hash", Role = "User", Name = "Test" };

        _userRepo.Setup(r => r.GetByEmailAsync("test@gmail.com")).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify("123456", "hash")).Returns(true);
        _jwtService.Setup(j => j.GenerateAccessToken(user)).Returns("access_token");
        _jwtService.Setup(j => j.GenerateRefreshToken()).Returns("refresh_token");

        var result = await _service.LoginAsync(new LoginDto { Email = "test@gmail.com", Password = "123456" });

        Assert.NotNull(result);
        Assert.Equal("access_token", result.AccessToken);
        Assert.Equal("refresh_token", result.RefreshToken);
    }

    [Fact]
    public async Task Login_UserNotFound_Throws()
    {
        _userRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        var ex = await Assert.ThrowsAsync<Exception>(() =>
            _service.LoginAsync(new LoginDto { Email = "no@gmail.com", Password = "123456" }));

        Assert.Equal("Неправильний пароль або ел. пошта", ex.Message);
    }

    [Fact]
    public async Task Login_WrongPassword_Throws()
    {
        var user = new User { Email = "test@gmail.com", PasswordHash = "hash" };
        _userRepo.Setup(r => r.GetByEmailAsync("test@gmail.com")).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify("wrongpass", "hash")).Returns(false);


        var ex = await Assert.ThrowsAsync<Exception>(() =>
            _service.LoginAsync(new LoginDto { Email = "test@gmail.com", Password = "wrongpass" }));

        Assert.Equal("Неправильний пароль або ел. пошта", ex.Message);
    }

    [Fact]
    public async Task Refresh_ValidToken_ReturnsNewTokens()
    {
        var user = new User
        {
            Id = "1",
            Email = "test@gmail.com",
            Role = "User",
            Name = "Test",
            RefreshToken = "valid_token",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1) // ще не прострочений
        };
        _userRepo.Setup(r => r.GetByRefreshTokenAsync("valid_token")).ReturnsAsync(user);
        _jwtService.Setup(j => j.GenerateAccessToken(user)).Returns("new_access");
        _jwtService.Setup(j => j.GenerateRefreshToken()).Returns("new_refresh");

        var result = await _service.RefreshAsync("valid_token");

        Assert.Equal("new_access", result.AccessToken);
    }
    [Fact]
    public async Task Refresh_ExpiredToken_Throws()
    {
        var user = new User
        {
            RefreshToken = "old_token",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1)
        };
        _userRepo.Setup(r => r.GetByRefreshTokenAsync("old_token")).ReturnsAsync(user);

        var ex = await Assert.ThrowsAsync<Exception>(() => _service.RefreshAsync("old_token"));
        Assert.Equal("Refresh token expired", ex.Message);
    }

    [Fact]
    public async Task Logout_ValidUser_ClearsToken()
    {
        var user = new User { Id = "1", RefreshToken = "some_token" };
        _userRepo.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);

        await _service.LogoutAsync("1");

        _userRepo.Verify(r => r.UpdateAsync(It.Is<User>(u => u.RefreshToken == null)), Times.Once);
    }
}
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly UserService _service;

    public UserServiceTests()
    {
        _service = new UserService(_userRepo.Object, _hasher.Object);
    }

    [Fact]
    public async Task GetById_ValidId_ReturnsUser()
    {
        _userRepo.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(new User { Id = "1", Name = "Test" });

        var result = await _service.GetByIdAsync("1");

        Assert.NotNull(result);
        Assert.Equal("1", result.Id);
    }

    [Fact]
    public async Task GetById_EmptyId_Throws()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _service.GetByIdAsync(""));
    }

    [Fact]
    public async Task Create_ValidData_CallsRepository()
    {
        _hasher.Setup(h => h.HashPassword(It.IsAny<string>())).Returns("hashed");
        var dto = new CreateUserDto { Name = "Test", Email = "t@t.com", Password = "123456", Role = "User" };

        await _service.CreateAsync(dto);

        _userRepo.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Once);
    }
    [Fact]
    public async Task Create_InvalidRole_Throws()
    {
        var dto = new CreateUserDto { Name = "Test", Email = "t@t.com", Password = "123456", Role = "SuperAdmin" };

        var ex = await Assert.ThrowsAsync<Exception>(() => _service.CreateAsync(dto));
        Assert.Equal("Є тільки 3 ролі:User,Meneger,Admin", ex.Message);
    }

    [Fact]
    public async Task Create_NullDto_Throws()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateAsync(null!));
    }

    [Fact]
    public async Task Update_ExistingUser_ReturnsTrue()
    {
        _userRepo.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(new User { Id = "1" });
        _hasher.Setup(h => h.HashPassword(It.IsAny<string>())).Returns("hashed");
        var dto = new CreateUserDto { Name = "New", Email = "new@t.com", Password = "123456", Role = "Admin" };

        var result = await _service.UpdateAsync("1", dto);

        Assert.True(result);
        _userRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task Update_UserNotFound_ReturnsFalse()
    {
        _userRepo.Setup(r => r.GetByIdAsync("999")).ReturnsAsync((User?)null);
        var dto = new CreateUserDto { Name = "X", Email = "x@t.com", Password = "123456", Role = "User" };

        var result = await _service.UpdateAsync("999", dto);

        Assert.False(result);
    }

    [Fact]
    public async Task Patch_OnlyName_UpdatesOnlyName()
    {
        var user = new User { Id = "1", Name = "Old", Email = "t@t.com", Role = "User" };
        _userRepo.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);

        var result = await _service.PatchAsync("1", new UpdateUserDto { Name = "New" });

        Assert.True(result);
        _userRepo.Verify(r => r.UpdateAsync(It.Is<User>(u => u.Name == "New")), Times.Once);
    }

    [Fact]
    public async Task Patch_EmailTakenByOther_Throws()
    {
        _userRepo.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(new User { Id = "1", Email = "old@t.com" });
        _userRepo.Setup(r => r.GetByEmailAsync("taken@t.com")).ReturnsAsync(new User { Id = "2" }); // інший юзер

        var ex = await Assert.ThrowsAsync<Exception>(() =>
            _service.PatchAsync("1", new UpdateUserDto { Email = "taken@t.com" }));

        Assert.Equal("Ел. пошта вже зайнята", ex.Message);
    }
}

public class TarifServiceTests
{
    private readonly Mock<ITarifRepository> _tarifRepo = new();
    private readonly Mock<ISubscriberRepository> _subscriberRepo = new();
    private readonly TarifService _service;

    public TarifServiceTests()
    {
        _service = new TarifService(_tarifRepo.Object, _subscriberRepo.Object);
    }

    [Fact]
    public async Task GetById_ValidId_ReturnsTarif()
    {
        _tarifRepo.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(new Tarif { Id = "1" });

        var result = await _service.GetByIdAsync("1");

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetById_EmptyId_Throws()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _service.GetByIdAsync(""));
    }

    [Fact]
    public async Task Create_ValidTarif_CallsRepository()
    {
        await _service.CreateAsync(new Tarif { Id = "1" });

        _tarifRepo.Verify(r => r.CreateAsync(It.IsAny<Tarif>()), Times.Once);
    }

    [Fact]
    public async Task Create_NullTarif_Throws()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateAsync(null!));
    }

    [Fact]
    public async Task Delete_ValidIds_DeletesAndMigrates()
    {
        _tarifRepo.Setup(r => r.GetByIdAsync("old")).ReturnsAsync(new Tarif { Id = "old" });
        _tarifRepo.Setup(r => r.GetByIdAsync("new")).ReturnsAsync(new Tarif { Id = "new" });

        await _service.DeleteAsync("old", "new");

        _subscriberRepo.Verify(r => r.UpdateTarifForAllAsync("old", "new"), Times.Once);
        _tarifRepo.Verify(r => r.DeleteAsync("old"), Times.Once);
    }

    [Fact]
    public async Task Delete_SameIds_Throws()
    {
        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.DeleteAsync("same", "same"));

        Assert.Equal("Вони повинні відрізнятися", ex.Message);
    }

    [Fact]
    public async Task Delete_TarifNotFound_Throws()
    {
        _tarifRepo.Setup(r => r.GetByIdAsync("old")).ReturnsAsync((Tarif?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.DeleteAsync("old", "new"));
    }
    [Fact]
    public async Task Update_ExistingTarif_ReturnsTrue()
    {
        _tarifRepo.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(new Tarif { Id = "1" });

        var result = await _service.UpdateAsync("1", new Tarif());

        Assert.True(result);
        _tarifRepo.Verify(r => r.UpdateAsync(It.IsAny<Tarif>()), Times.Once);
    }

    [Fact]
    public async Task Update_TarifNotFound_ReturnsFalse()
    {
        _tarifRepo.Setup(r => r.GetByIdAsync("999")).ReturnsAsync((Tarif?)null);

        var result = await _service.UpdateAsync("999", new Tarif());

        Assert.False(result);
    }
}