using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using user_service.Dto;
using user_service.repositories;

namespace user_service.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[Produces("application/json")]
[Authorize]
public class AuthController(IAuthRepository repository)
    : ControllerBase
{
    [HttpGet(Name = nameof(GetAllUsers))]
    public async Task<ActionResult<List<UserDto>>> GetAllUsers()
    {
        return await repository.GetAllUsers();
    }

    [HttpGet]
    public async Task<ActionResult<UserDto>> GetConnectedUser()
    {
        return await repository.GetConnectedUser();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [AllowAnonymous]
    public async Task<ActionResult<UserToken>> Register([FromBody] RegisterDto model)
    {
        return await repository.Register(model);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<UserToken>> Login([FromBody] LoginDto login)
    {
        return await repository.Login(login);
    }

    [HttpPut]
    public async Task<ActionResult<UserToken>> EditUserInfo([FromBody] EditUserDto model)
    {
        return await repository.EditUserInfos(model);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserToken>> EditAddressId(int id)
    {
        return await repository.EditDeliveryAddressId(id);
    }
}