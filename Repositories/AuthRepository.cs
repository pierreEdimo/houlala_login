using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using user_service.DbContext;
using user_service.Dto;
using user_service.exception;
using user_service.Model;

namespace user_service.repositories;

public class AuthRepository(
    UserManager<UserEntity> userManager,
    SignInManager<UserEntity> signInManager,
    IConfiguration configuration,
    IMapper mapper,
    IHttpContextAccessor httpContext)
    : IAuthRepository
{
    private readonly IConfiguration? _configuration = configuration;
    private readonly IHttpContextAccessor? _httpContext = httpContext;
    private readonly SignInManager<UserEntity>? _signInManager = signInManager;
    private readonly UserManager<UserEntity>? _userManager = userManager;


    public async Task<ActionResult<List<UserDto>>> GetAllUsers()
    {
        await using var context = new UserDbContext();
        var users = await _userManager!.Users.ToListAsync();
        return mapper.Map<List<UserDto>>(users);
    }

    public async Task<ActionResult<UserToken>> Register(RegisterDto model)
    {
        var user = new UserEntity
        {
            UserName = model.UserName,
            Email = model.Email,
        };
        var result = await _userManager!.CreateAsync(user, model.PassWord!);
        if (!result.Succeeded)
            throw new LoginException(result.Errors, (int)HttpStatusCode.BadRequest);
        return GenerateJwtToken(model.Email!, user);
    }

    public async Task<ActionResult<UserDto>> GetConnectedUser()

    {
        var user = await _getCurrentUser();
        return mapper.Map<UserDto>(user);
    }


    public async Task<ActionResult<UserToken>> Login(LoginDto login)
    {
        var user = await _userManager!.FindByEmailAsync(login.Email!);
        if (user == null)
            throw new LoginException("L'utilisateur n'a pas ete retrouve", (int)HttpStatusCode.NotFound);
        var result = await _signInManager!.PasswordSignInAsync(user, login.PassWord!, false, false);
        if (!result.Succeeded)
            throw new LoginException("Connection invalide", (int)HttpStatusCode.BadRequest);
        return GenerateJwtToken(login.Email!, user);
    }

    public async Task<ActionResult<UserToken>> EditUserInfos(EditUserDto model)
    {
        var user = await _getCurrentUser();

        if (model.UserName != null) user.UserName = model.UserName;

        if (model.LastName != null) user.LastName = model.LastName;

        if (model.FirstName != null) user.FirstName = model.FirstName;

        if (model.PhoneNumber != null) user.PhoneNumber = model.PhoneNumber;

        if (model.Email != null) user.Email = model.Email;

        var result = await _userManager!.UpdateAsync(user);

        if (!result.Succeeded) throw new LoginException(result.Errors, (int)HttpStatusCode.BadRequest);

        return GenerateJwtToken(model.Email!, user);
    }

    private UserToken GenerateJwtToken(string email, UserEntity user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.NameId, user.Id)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration!["JwtKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _configuration["JwtIssuer"],
            _configuration["JwtIssuer"],
            claims,
            signingCredentials: credentials
        );

        return new UserToken
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token)
        };
    }

    private async Task<UserEntity> _getCurrentUser()
    {
        var email = _httpContext!.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        if (email == null)
            throw new LoginException("Email not found", (int)HttpStatusCode.BadRequest);
        var user = await _userManager!.FindByEmailAsync(email);
        if (user == null)
            throw new LoginException("No user found", (int)HttpStatusCode.NotFound);
        return user;
    }
}