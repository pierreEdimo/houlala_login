using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using user_service.Dto;
using user_service.repositories;

namespace user_service.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class AddressesController(IAuthRepository authRepository, IAddressRepository addressRepository)
{
    [HttpGet]
    public async Task<ActionResult<List<AddressDto>>> GetUsersAddresses()
    {
        var connectedUser = authRepository.GetConnectedUser().Result.Value;

        return await addressRepository.GetUsersAddresses(connectedUser!.Id!);
    }

    [HttpPost]
    public async Task<ActionResult<AddressDto>> CreateAddress([FromBody] CreateAddressDto createAddressDto)
    {
        var connectedUser = authRepository.GetConnectedUser().Result.Value;

        return await addressRepository.CreateAddress(createAddressDto, connectedUser!.Id!);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditAddress([FromBody] CreateAddressDto addressDto, int id)
    {
        return await addressRepository.EditAddress(id, addressDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAddress(int id)
    {
        return await addressRepository.DeleteAddress(id);
    }
}