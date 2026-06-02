namespace ekyc_api.DTOs;

public class CustomerListResponse
{
    public List<CustomerSummaryDto> Data { get; set; } = [];

    public string? NextCursor { get; set; }

    public bool HasMore { get; set; }

    public long ExecutionTimeMs { get; set; }
}