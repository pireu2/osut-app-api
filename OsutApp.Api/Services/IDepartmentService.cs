using OsutApp.Api.Models;

namespace OsutApp.Api.Services;

public interface IDepartmentService
{
    Task<IEnumerable<Department>> GetAllDepartmentsAsync();
    Task<Department?> GetDepartmentByIdAsync(Guid id);
    Task<IEnumerable<Department>> GetDepartmentsByTypeAsync(DepartmentType type);
    Task<Department> CreateDepartmentAsync(string name, string? description, DepartmentType type, string coordinatorId);
    Task<Department?> UpdateDepartmentAsync(Guid id, string? name, string? description, DepartmentType? type, string? coordinatorId);
    Task<bool> DeleteDepartmentAsync(Guid id);
}
