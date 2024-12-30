namespace user_service.Dto;

public class CreateAddressDto
{
    public string? Street { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? PoBox { get; set; }
    public string? HouseNumber { get; set; }
}