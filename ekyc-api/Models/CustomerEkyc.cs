namespace ekyc_api.Models;

public class CustomerEkyc
{
    public Guid Id { get; set; }
    public string CustomerRefNo { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string EkycStatus { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}