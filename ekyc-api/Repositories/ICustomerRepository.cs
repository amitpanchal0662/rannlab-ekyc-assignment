using ekyc_api.DTOs;
using ekyc_api.Models;

namespace ekyc_api.Repositories;

public interface ICustomerRepository
{
    Task<CustomerEkyc?> GetByIdAsync(Guid id);

    Task<Guid> CreateAsync(CreateCustomerRequest request);

    Task<bool> UpdateAsync(Guid id, UpdateCustomerRequest request);


    Task<bool> DeleteAsync(Guid id);

    Task<CustomerListResponse> GetCustomersAsync(
    CustomerListRequest request);
}