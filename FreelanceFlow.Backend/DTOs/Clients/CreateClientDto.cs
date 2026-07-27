namespace FreelanceFlow.Backend.DTOs.Clients;

public class CreateClientDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Company { get; set; }
    public string Country { get; set; } = string.Empty;
    public int PaymentTermsDays { get; set; } = 14;
    public string? Notes { get; set; }
}