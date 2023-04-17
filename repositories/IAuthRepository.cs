using Microsoft.AspNetCore.Mvc;
using user_service.Dto;

namespace user_service.repositories;

public interface IAuthRepository
{
    Task<ActionResult<List<UserDto>>> GetAllUsers();
    Task<ActionResult<string>> ValidateToken(string token);
    Task<ActionResult<UserDto>> GetUserByEmail(string email);
    Task<ActionResult<UserDto>> GetUserById(string userId);
    Task<ActionResult<UserToken>> Register(RegisterDto model);
    Task<ActionResult<UserToken>> RenewPassword(LoginDto model);
    Task<ActionResult<UserToken>> EditUserInformations(PersonalData model, string email);
    Task<ActionResult<UserToken>> EditUserEmail(EditEmail newEmail, string email);
    Task<ActionResult<UserToken>> EditAddressInformations(AdressData model, string email);
    Task<ActionResult<UserToken>> Login(LoginDto login);
    Task<ActionResult<UserToken>> EditSellerInfo(SellerInfo info);
}