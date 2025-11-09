using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OsutApp.Api.Models;
using OsutApp.Api.Services;

namespace OsutApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepartmentsController(
    IDepartmentService departmentService) : ControllerBase
{
    private readonly IDepartmentService _departmentService = departmentService;

    [HttpGet]
    public async Task<IActionResult> GetAllDepartments()
    {
        var departments = await _departmentService.GetAllDepartmentsAsync();
        return Ok(departments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDepartment(Guid id)
    {
        var department = await _departmentService.GetDepartmentByIdAsync(id);
        if (department == null)
        {
            return NotFound();
        }
        return Ok(department);
    }

    [HttpGet("type/{type}")]
    public async Task<IActionResult> GetDepartmentsByType(DepartmentType type)
    {
        var departments = await _departmentService.GetDepartmentsByTypeAsync(type);
        return Ok(departments);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentRequest request)
    {
        try
        {
            var department = await _departmentService.CreateDepartmentAsync(
                request.Name,
                request.Description,
                request.Type,
                request.CoordinatorId);

            return CreatedAtAction(nameof(GetDepartment), new { id = department.Id }, department);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateDepartment(Guid id, [FromBody] UpdateDepartmentRequest request)
    {
        try
        {
            var department = await _departmentService.UpdateDepartmentAsync(
                id,
                request.Name,
                request.Description,
                request.Type,
                request.CoordinatorId);

            if (department == null)
            {
                return NotFound();
            }

            return Ok(department);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteDepartment(Guid id)
    {
        var success = await _departmentService.DeleteDepartmentAsync(id);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}

public class CreateDepartmentRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required DepartmentType Type { get; set; }
    public required string CoordinatorId { get; set; }
}

public class UpdateDepartmentRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DepartmentType? Type { get; set; }
    public string? CoordinatorId { get; set; }
}