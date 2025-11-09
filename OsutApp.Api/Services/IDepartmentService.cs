using OsutApp.Api.DTOs;
using OsutApp.Api.Models;

namespace OsutApp.Api.Services;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync();
    Task<DepartmentDto?> GetDepartmentByIdAsync(Guid id);
    Task<IEnumerable<DepartmentDto>> GetDepartmentsByTypeAsync(DepartmentType type);
    Task<DepartmentDto> CreateDepartmentAsync(DepartmentDto departmentDto);
    Task<DepartmentDto?> UpdateDepartmentAsync(Guid id, DepartmentDto departmentDto);
    Task<bool> DeleteDepartmentAsync(Guid id);
}
