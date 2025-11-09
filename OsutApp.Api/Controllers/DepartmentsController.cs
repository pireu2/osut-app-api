using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OsutApp.Api.DTOs;
using OsutApp.Api.Models;
using OsutApp.Api.Services;
using System.Security.Claims;

namespace OsutApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepartmentsController(
    IDepartmentService departmentService,
    IUserService userService) : BaseController
{
    private readonly IDepartmentService _departmentService = departmentService;
    private readonly IUserService _userService = userService;

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
    public async Task<IActionResult> CreateDepartment([FromBody] DepartmentDto request)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = await _userService.IsUserAdminAsync(currentUserId.ToString());

            if (!isAdmin)
            {
                return Forbid("Only administrators can create departments");
            }

            var department = await _departmentService.CreateDepartmentAsync(request);

            return CreatedAtAction(nameof(GetDepartment), new { id = department.Id }, department);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateDepartment(Guid id, [FromBody] DepartmentDto request)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = await _userService.IsUserAdminAsync(currentUserId.ToString());

            if (!isAdmin)
            {
                return Forbid("Only administrators can update departments");
            }

            var department = await _departmentService.UpdateDepartmentAsync(id, request);

            if (department == null)
            {
                return NotFound();
            }

            return Ok(department);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
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
        try
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = await _userService.IsUserAdminAsync(currentUserId.ToString());

            if (!isAdmin)
            {
                return Forbid("Only administrators can delete departments");
            }

            var success = await _departmentService.DeleteDepartmentAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }
}