using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using user_service.Dto;
using user_service.Model;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using user_service.DbContext;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using user_service.exception;

namespace user_service.repositories;

public class AuthRepository : IAuthRepository
{
    private readonly UserManager<UserEntity>? _userManager;
    private readonly SignInManager<UserEntity>? _signInManager;
    private readonly IConfiguration? _configuration;
    private readonly IMapper _mapper;
    private readonly UserDbContext? _context;
    private readonly IHttpContextAccessor? _httpContext;


    public AuthRepository(
        UserManager<UserEntity> userManager,
        SignInManager<UserEntity> signInManager,
        IConfiguration configuration,
        IMapper mapper,
        UserDbContext context,
        IHttpContextAccessor httpContext
    )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _mapper = mapper;
        _context = context;
        _httpContext = httpContext;
    }

    public async Task<ActionResult<List<UserDto>>> GetAllUsers()
    {
        await using var context = new UserDbContext();
        var users = await _userManager!.Users.ToListAsync();
        return _mapper!.Map<List<UserDto>>(users);
    }

    public async Task<ActionResult<UserToken>> Register(RegisterDto model)
    {
        var user = new UserEntity
        {
            UserName = model.UserName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            City = model.City,
            Country = model.Country,
            HouseNumber = model.HouseNumber,
            LastName = model.LastName,
            FirstName = model.FirstName,
            StreetName = model.StreetName,
            PoBox = model.PoBox
        };
        var result = await _userManager!.CreateAsync(user, model.PassWord!);
        if (!result.Succeeded)
            throw new LoginException(result.Errors, (int)HttpStatusCode.BadRequest);
        return GenerateJwtToken(model!.Email!, user);
    }

    public async Task<ActionResult<UserDto>> GetConnectedUser()

    {
        var email = _httpContext!.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        if (email == null)
            throw new LoginException("Email not found", (int)HttpStatusCode.BadRequest);
        UserEntity? user = await _userManager!.FindByEmailAsync(email);
        if (user == null)
            throw new LoginException("No user found", (int)HttpStatusCode.NotFound);
        return _mapper.Map<UserDto>(user);
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

    private UserToken GenerateJwtToken(String email, UserEntity user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.NameId, user.Id)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration!["JwtKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _configuration["JwtIssuer"],
            audience: _configuration["JwtIssuer"],
            claims: claims,
            signingCredentials: credentials
        );

        return new UserToken()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
        };
    }
}