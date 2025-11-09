using OsutApp.Api.Models;

namespace OsutApp.Api.Repositories;

public interface IDepartmentRepository
{
    Task<Department?> GetByIdAsync(Guid id);
    Task<IEnumerable<Department>> GetAllAsync();
    Task<IEnumerable<Department>> GetByTypeAsync(DepartmentType type);
    Task AddAsync(Department department);
    Task UpdateAsync(Department department);
    Task DeleteAsync(Department department);
}