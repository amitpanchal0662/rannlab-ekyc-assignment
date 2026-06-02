namespace ekyc_api.DTOs;

public class CustomerListRequest
{
    public int PageSize { get; set; } = 50;

    public Guid? OrgId { get; set; }

    public string? Status { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedFrom { get; set; }

    public DateTime? CreatedTo { get; set; }

    public int? RiskMin { get; set; }

    public int? RiskMax { get; set; }

    public string? Q { get; set; }

    public DateTime? CursorCreatedAt { get; set; }

    public Guid? CursorId { get; set; }
}