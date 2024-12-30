using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using user_service.DbContext;
using user_service.Dto;
using user_service.Model;

namespace user_service.repositories;

public class AddressRepository(UserDbContext userDbContext, IMapper mapper) : IAddressRepository
{
    public async Task<ActionResult<List<AddressDto>>> GetUsersAddresses(string userId)
    {
        var addresses = await userDbContext.Addresses!.Where(address => address.UserId == userId).ToListAsync();
        return mapper.Map<List<AddressDto>>(addresses);
    }

    public async Task<ActionResult<AddressDto>> CreateAddress(CreateAddressDto createAddressDto, string userId)
    {
        var newAddress = mapper.Map<Address>(createAddressDto);
        newAddress.UserId = userId;

        userDbContext.Add((object)newAddress);

        await userDbContext.SaveChangesAsync();

        var addressDto = mapper.Map<AddressDto>(newAddress);

        return new CreatedResult("GetAddress", addressDto);
    }

    public async Task<IActionResult> EditAddress(int id, CreateAddressDto createAddressDto)
    {
        var existingAddress = await userDbContext.Addresses!.FirstOrDefaultAsync(address => address.Id == id);

        if (existingAddress == null)
            return new NotFoundResult();

        existingAddress = mapper.Map(createAddressDto, existingAddress);

        await userDbContext.SaveChangesAsync();

        return new NoContentResult();
    }

    public async Task<IActionResult> DeleteAddress(int id)
    {
        var existingAdddress = await userDbContext.Addresses!.FirstOrDefaultAsync(address => address.Id == id);

        if (existingAdddress == null)
            return new NotFoundResult();

        userDbContext.Remove(existingAdddress);

        await userDbContext.SaveChangesAsync();

        return new NoContentResult();
    }
}