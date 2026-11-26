using LibraryManagementAPI.DTOs.Loans;
using LibraryManagementAPI.Filter;
using LibraryManagementAPI.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers;

[ServiceFilter(typeof(LogActionFilter))]
[ApiController]
[Route("api/[controller]")]
[Tags("Loans")]
public sealed class LoansController(ILoanService loanService) : WebApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ReadLoanDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ReadLoanDto>>> Get()
    {
        var result = await loanService.GetAllAsync();
        return result.IsError ? Problem(result.Errors) : Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ReadLoanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReadLoanDto>> GetById(int id)
    {
        var result = await loanService.GetByIdAsync(id);
        return result.IsError ? Problem(result.Errors) : Ok(result.Value);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ReadLoanDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ReadLoanDto>> Create(CreateLoanDto request)
    {
        var result = await loanService.CreateAsync(request);
        return result.IsError
            ? Problem(result.Errors)
            : CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ReadLoanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ReadLoanDto>> Update(int id, UpdateLoanDto request)
    {
        var result = await loanService.UpdateAsync(id, request);
        return result.IsError ? Problem(result.Errors) : Ok(result.Value);
    }

    [HttpPost("{id:int}/return")]
    [ProducesResponseType(typeof(ReadLoanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ReadLoanDto>> Return(int id)
    {
        var result = await loanService.ReturnAsync(id);
        return result.IsError ? Problem(result.Errors) : Ok(result.Value);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await loanService.DeleteAsync(id);
        return result.IsError ? Problem(result.Errors) : NoContent();
    }
}
