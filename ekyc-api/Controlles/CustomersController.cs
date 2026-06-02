using Microsoft.AspNetCore.Mvc;
using ekyc_api.Repositories;
using ekyc_api.DTOs;

namespace ekyc_api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerRepository _repository;

    public CustomersController(ICustomerRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var customer = await _repository.GetByIdAsync(id);

        if (customer == null)
            return NotFound();

        return Ok(customer);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCustomerRequest request)
    {
        var id = await _repository.CreateAsync(request);

        return CreatedAtAction(
            nameof(Get),
            new { id },
            new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
    Guid id,
    UpdateCustomerRequest request)
    {
        var updated = await _repository.UpdateAsync(id, request);

        if (!updated)
            return NotFound();

        return Ok(new
        {
            Message = "Customer updated successfully"
        });
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _repository.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return Ok(new
        {
            Message = "Customer deleted successfully"
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomers(
    [FromQuery] CustomerListRequest request)
    {
        var result =
            await _repository.GetCustomersAsync(request);

        return Ok(result);
    }
}