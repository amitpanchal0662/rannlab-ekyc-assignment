namespace ekyc_api.DTOs;

public class UpdateCustomerRequest
{
    public string FirstName { get; set; } = string.Empty;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = string.Empty;

    public string Mobile { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string EkycStatus { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}