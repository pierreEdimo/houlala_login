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
    private readonly UserManager<User>? _userManager;
    private readonly SignInManager<User>? _signInManager;
    private readonly IConfiguration? _configuration;
    private readonly IMapper _mapper;
    private readonly UserDbContext? _context;


    public AuthRepository(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration configuration,
        IMapper mapper,
        UserDbContext context
    )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _mapper = mapper;
        _context = context;
    }

    public async Task<ActionResult<List<UserDto>>> GetAllUsers()
    {
        await using var context = new UserDbContext();
        var users = await _userManager!.Users.ToListAsync();
        return _mapper!.Map<List<UserDto>>(users);
    }

    public async Task<ActionResult<string>> ValidateToken(string token)
    {
        var validator = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidAudience = _configuration!["JwtIssuer"],
            ValidIssuer = _configuration["JwtIssuer"],
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"])),
            RequireExpirationTime = false,
        };

        var principal = await validator.ValidateTokenAsync(token, validationParameters);
        if (principal.IsValid)
        {
            return "Validated";
        }
        else
        {
            return "NotValidated";
        }
    }

    public async Task<ActionResult<UserDto>> GetUserByEmail(string email)
    {
        var user = await _userManager!.FindByEmailAsync(email);
        if (user == null)
            throw new LoginException("Utilisateur n'a pas ete retrouve", (int)HttpStatusCode.NotFound);
        return _mapper!.Map<UserDto>(user);
    }

    public async Task<ActionResult<UserDto>> GetUserById(string userId)
    {
        var user = await _userManager!.FindByIdAsync(userId);
        if (user == null)
            throw new LoginException("Utilisateur n'a pas ete retrouve", (int)HttpStatusCode.NotFound);
        return _mapper!.Map<UserDto>(user);
    }

    public async Task<ActionResult<UserToken>> Register(RegisterDto model)
    {
        var user = new User
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
        var result = await _userManager!.CreateAsync(user, model.PassWord);
        if (!result.Succeeded)
            throw new LoginException(result.Errors, (int)HttpStatusCode.BadRequest);
        return GenerateJwtToken(model!.Email!, user);
    }

    public async Task<ActionResult<UserToken>> RenewPassword(LoginDto model)
    {
        var user = await _userManager!.FindByEmailAsync(model.Email);
        if (user == null)
            throw new LoginException("L'utilisateur n'a pas ete retrouve", (int)HttpStatusCode.NotFound);
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, model.PassWord);
        if (!result.Succeeded) throw new LoginException(result.Errors, (int)HttpStatusCode.BadRequest);
        await _signInManager!.PasswordSignInAsync(user, model.PassWord, false, false);
        return GenerateJwtToken(model.Email!, user);
    }

    public async Task<ActionResult<UserToken>> EditUserInformations(PersonalData model, string email)
    {
        var user = await _userManager!.FindByEmailAsync(email);
        if (user == null)
            throw new LoginException("L'utilisateur n'a pas ete retrouve", (int)HttpStatusCode.NotFound);
        user.UserName = model.UserName;
        user.PhoneNumber = model.PhoneNumber;
        user.LastName = model.LastName;
        user.FirstName = model.FirstName;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new LoginException(result.Errors, (int)HttpStatusCode.BadRequest);
        return GenerateJwtToken(user.Email!, user);
    }

    public async Task<ActionResult<UserToken>> EditUserEmail(EditEmail newEmail, string email)
    {
        var user = await _userManager!.FindByEmailAsync(email);
        if (user == null)
            throw new LoginException("L'utilisateur n'a pas ete retrouve", (int)HttpStatusCode.NotFound);
        user.Email = newEmail.Email;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new LoginException(result.Errors, (int)HttpStatusCode.BadRequest);
        return GenerateJwtToken(user.Email!, user);
    }

    public async Task<ActionResult<UserToken>> EditAddressInformations(AdressData model, string email)
    {
        var user = await _userManager!.FindByEmailAsync(email);
        if (user == null)
            throw new LoginException("L'utilisateur n'a pas ete retrouve", (int)HttpStatusCode.NotFound);
        user.StreetName = model.StreetName;
        user.City = model.City;
        user.Country = model.Country;
        user.PoBox = model.PoBox;
        user.HouseNumber = model.HouseNumber;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new LoginException(result.Errors, (int)HttpStatusCode.BadRequest);
        return GenerateJwtToken(user.Email!, user);
    }

    public async Task<ActionResult<UserToken>> Login(LoginDto login)
    {
        var user = await _userManager!.FindByEmailAsync(login.Email);
        if (user == null)
            throw new LoginException("L'utilisateur n'a pas ete retrouve", (int)HttpStatusCode.NotFound);
        var result = await _signInManager!.PasswordSignInAsync(user, login.PassWord, false, false);
        if (!result.Succeeded)
            throw new LoginException("Connection invalide", (int)HttpStatusCode.BadRequest);
        return GenerateJwtToken(login.Email!, user);
    }

    public async Task<ActionResult<UserToken>> EditSellerInfo(SellerInfo info)
    {
        var user = await _userManager!.FindByEmailAsync(info.Email);
        if (user == null)
            throw new LoginException("L'utilisateur n'a pas ete retrouve", (int)HttpStatusCode.NotFound);
        user.UserName = info.UserName;
        user.Email = info.Email;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new LoginException(result.Errors, (int)HttpStatusCode.BadRequest);
        return GenerateJwtToken(user.Email!, user);
    }

    private UserToken GenerateJwtToken(String email, User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.NameId, user.Id)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration!["JwtKey"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _configuration["JwtIssuer"],
            _configuration["JwtIssuer"],
            claims,
            signingCredentials: credentials
        );

        var loggedUserDto = _mapper!.Map<UserDto>(user);

        return new UserToken()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            UserName = loggedUserDto.UserName,
            Email = loggedUserDto.Email,
            UserId = loggedUserDto.Id
        };
    }
}