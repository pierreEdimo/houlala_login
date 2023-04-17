using Microsoft.AspNetCore.Mvc;
using user_service.Dto;
using Microsoft.AspNetCore.Authorization;
using user_service.repositories;

namespace user_service.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[Produces("application/json")]
[Authorize]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _repository;


    public AuthController(IAuthRepository repository)
    {
        _repository = repository;
    }

    [HttpGet(Name = nameof(GetAllUsers))]
    public async Task<ActionResult<List<UserDto>>> GetAllUsers()
    {
        return await _repository.GetAllUsers();
    }

    [HttpGet("{token}")]
    [AllowAnonymous]
    public async Task<ActionResult<String>> ValidateToken(string token)
    {
        return await _repository.ValidateToken(token);
    }


    [HttpGet("{email}")]
    public async Task<ActionResult<UserDto>> GetUserByEmail(String email)
    {
        return await _repository.GetUserByEmail(email);
    }

    [HttpGet("{userId}")]
    [AllowAnonymous]
    public async Task<ActionResult<UserDto>> GetUserById(string userId)
    {
        return await _repository.GetUserById(userId);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [AllowAnonymous]
    public async Task<ActionResult<UserToken>> Register([FromBody] RegisterDto model)
    {
        return await _repository.Register(model);
    }

    [HttpPut("{email}")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult<UserToken>> EditSellerInfo([FromBody] SellerInfo info, string email)
    {
        return await _repository.EditSellerInfo(info, email);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [AllowAnonymous]
    public async Task<ActionResult<UserToken>> RenewPassword([FromBody] LoginDto login)
    {
        return await _repository.RenewPassword(login);
    }

    [HttpPut("{email}")]
    public async Task<ActionResult<UserToken>> EditUserInformations([FromBody] PersonalData model, String email)
    {
        return await _repository.EditUserInformations(model, email);
    }

    [HttpPut("{email}")]
    public async Task<ActionResult<UserToken>> EditUserEmail([FromBody] EditEmail newEmail, String email)
    {
        return await _repository.EditUserEmail(newEmail, email);
    }


    [HttpPut("{Email}")]
    public async Task<ActionResult<UserToken>> EditAddressInformations([FromBody] AdressData model, String email)
    {
        return await _repository.EditAddressInformations(model, email);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<UserToken>> Login([FromBody] LoginDto login)
    {
        return await _repository.Login(login);
    }
}
