namespace user_service.Dto;

public class AddressDto
{
    public int Id { get; init; }
    public string? Street { get; init; }
    public string? Country { get; init; }
    public string? PoBox { get; init; }
    public string? City { get; init; }
    public string? HouseNumber { get; init; }
    public string? LastName { get; init; }
    public string? FirstName { get; init; }
}