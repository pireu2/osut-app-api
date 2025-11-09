using OsutApp.Api.Models;
using OsutApp.Api.Repositories;

namespace OsutApp.Api.Services;

public class DepartmentService(IDepartmentRepository departmentRepository, IUserRepository userRepository) : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository = departmentRepository;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
    {
        return await _departmentRepository.GetAllAsync();
    }

    public async Task<Department?> GetDepartmentByIdAsync(Guid id)
    {
        return await _departmentRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Department>> GetDepartmentsByTypeAsync(DepartmentType type)
    {
        return await _departmentRepository.GetByTypeAsync(type);
    }

    public async Task<Department> CreateDepartmentAsync(string name, string? description, DepartmentType type, string coordinatorId)
    {
        var coordinator = await _userRepository.GetByIdAsync(coordinatorId);

        if (coordinator == null)
        {
            throw new ArgumentException("Invalid coordinator ID");
        }

        var department = new Department
        {
            Name = name,
            Description = description,
            Type = type,
            CoordinatorId = coordinatorId
        };

        await _departmentRepository.AddAsync(department);
        return department;
    }

    public async Task<Department?> UpdateDepartmentAsync(Guid id, string? name, string? description, DepartmentType? type, string? coordinatorId)
    {
        var department = await _departmentRepository.GetByIdAsync(id);

        if (department == null)
        {
            return null;
        }

        if (name != null)
        {
            department.Name = name;
        }

        if (description != null)
        {
            department.Description = description;
        }

        if (type.HasValue)
        {
            department.Type = type.Value;
        }

        if (!string.IsNullOrEmpty(coordinatorId))
        {
            var coordinator = await _userRepository.GetByIdAsync(coordinatorId);
            if (coordinator == null)
            {
                throw new ArgumentException("Invalid coordinator ID");
            }
            department.CoordinatorId = coordinatorId;
        }

        await _departmentRepository.UpdateAsync(department);
        return department;
    }

    public async Task<bool> DeleteDepartmentAsync(Guid id)
    {
        var department = await _departmentRepository.GetByIdAsync(id);

        if (department == null)
        {
            return false;
        }

        await _departmentRepository.DeleteAsync(department);
        return true;
    }
}