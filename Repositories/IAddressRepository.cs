using Microsoft.AspNetCore.Mvc;
using user_service.Dto;

namespace user_service.repositories;

public interface IAddressRepository
{
    Task<ActionResult<List<AddressDto>>> GetUsersAddresses(string userId);

    Task<ActionResult<AddressDto>> CreateAddress(CreateAddressDto createAddressDto, string userId);

    Task<IActionResult> EditAddress(int id, CreateAddressDto createAddressDto);

    Task<IActionResult> DeleteAddress(int id);
}