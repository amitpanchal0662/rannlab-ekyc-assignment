namespace ekyc_api.DTOs;

public class CreateCustomerRequest
{
    public string CustomerRefNo { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = string.Empty;

    public DateTime Dob { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string Mobile { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string EkycStatus { get; set; } = "pending";

    public string KycLevel { get; set; } = "basic";

    public int RiskScore { get; set; }

    public Guid OrgId { get; set; }

    public string OrgName { get; set; } = string.Empty;
}