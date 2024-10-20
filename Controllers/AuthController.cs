using Microsoft.AspNetCore.Mvc;
using user_service.Dto;
using Microsoft.AspNetCore.Authorization;
using user_service.repositories;
using Microsoft.AspNetCore.Identity;
using user_service.Model;

namespace user_service.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[Produces("application/json")]
[Authorize]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _repository;

    private readonly UserManager<UserEntity> _userManager;



    public AuthController(IAuthRepository repository, UserManager<UserEntity> userManager)
    {
        _repository = repository;
        _userManager = userManager;
    }

    [HttpGet(Name = nameof(GetAllUsers))]
    public async Task<ActionResult<List<UserDto>>> GetAllUsers()
    {
        return await _repository.GetAllUsers();
    }

    [HttpGet]
    public async Task<ActionResult<UserDto>> GetConnectedUser()
    {
        return await _repository.GetConnectedUser();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [AllowAnonymous]
    public async Task<ActionResult<UserToken>> Register([FromBody] RegisterDto model)
    {
        return await _repository.Register(model);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<UserToken>> Login([FromBody] LoginDto login)
    {
        return await _repository.Login(login);
    }
}
