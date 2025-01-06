using Microsoft.AspNetCore.Mvc;
using user_service.Dto;

namespace user_service.repositories;

public interface IAuthRepository
{
    Task<ActionResult<List<UserDto>>> GetAllUsers();
    Task<ActionResult<UserDto>> GetConnectedUser();
    Task<ActionResult<UserToken>> Register(RegisterDto model);
    Task<ActionResult<UserToken>> Login(LoginDto login);
    Task<ActionResult<UserToken>> EditUserInfos(EditUserDto model);
}