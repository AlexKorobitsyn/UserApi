//Controllers.UsersController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using UserApi.Models;
using UserApi.Models.DTO;
using UserApi.Services;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateDto dto)
    {
        try
        {
            var user = await _userService.CreateUserAsync(dto);
            return CreatedAtAction(nameof(GetUserAdmin), new { login = user.Login }, user);
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or ArgumentException)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
    [HttpPut("{login}")]
    public async Task<IActionResult> UpdateUser(
        string login,
        [FromBody] UserUpdateDto dto)
    {
        try
        {
            await _userService.UpdateUserInfoAsync(login, dto);
            return NoContent();
        }
        catch (Exception ex) when (ex is KeyNotFoundException or UnauthorizedAccessException)
        {
            return ex is KeyNotFoundException ? NotFound() : Forbid();
        }
    }
    [HttpPatch("{login}/password")]
    public async Task<IActionResult> ChangePassword(
        string login,
        [FromBody] ChangePasswordDto dto)
    {
        try
        {
            await _userService.ChangePasswordAsync(login, dto.NewPassword);
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }
    [HttpPatch("{oldLogin}/login")]
    public async Task<IActionResult> ChangeLogin(
        string oldLogin,
        [FromBody] ChangeLoginDto dto)
    {
        try
        {
            await _userService.ChangeLoginAsync(oldLogin, dto.NewLogin);
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }
    [HttpGet("active")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllActiveUsers()
    {
        var users = await _userService.GetAllActiveUsersAsync();
        return Ok(users);
    }
    [HttpGet("{login}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserAdmin(string login)
    {
        try
        {
            var user = await _userService.GetUserByLoginAsync(login);
            return Ok(new
            {
                user.Name,
                user.Gender,
                user.Birthday,
                IsActive = user.RevokedOn == null
            });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetPersonalInfo([FromQuery] string password)
    {
        try
        {
            var user = await _userService.GetPersonalInfoAsync(password);
            return Ok(new
            {
                user.Name,
                user.Gender,
                user.Birthday,
                user.Login
            });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [HttpGet("older-than/{age}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsersOlderThan(int age)
    {
        var users = await _userService.GetUsersOlderThanAsync(age);
        return Ok(users);
    }

    [HttpDelete("{login}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(
        string login,
        [FromQuery] bool softDelete = true)
    {
        try
        {
            if (softDelete)
                await _userService.SoftDeleteAsync(login);
            else
                await _userService.DeleteAsync(login);

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    //Восстановление
    [HttpPatch("{login}/restore")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RestoreUser(string login)
    {
        try
        {
            await _userService.RestoreUserAsync(login);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    private IActionResult HandleException(Exception ex)
    {
        return ex switch
        {
            KeyNotFoundException => NotFound(),
            UnauthorizedAccessException => Forbid(),
            ArgumentException => BadRequest(new { Error = ex.Message }),
            _ => StatusCode(500)
        };
    }
}

public class ChangePasswordDto
{
    [Required] public string NewPassword { get; set; }
}

public class ChangeLoginDto
{
    [Required] public string NewLogin { get; set; }
}