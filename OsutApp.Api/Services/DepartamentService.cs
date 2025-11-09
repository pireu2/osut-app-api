using AutoMapper;
using OsutApp.Api.DTOs;
using OsutApp.Api.Models;
using OsutApp.Api.Repositories;

namespace OsutApp.Api.Services;

public class DepartmentService(IDepartmentRepository departmentRepository, IUserRepository userRepository, IMapper mapper) : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository = departmentRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync()
    {
        var departments = await _departmentRepository.GetAllAsync();

        return _mapper.Map<IEnumerable<DepartmentDto>>(departments);
    }

    public async Task<DepartmentDto?> GetDepartmentByIdAsync(Guid id)
    {
        var department = await _departmentRepository.GetByIdAsync(id);

        return department != null ? _mapper.Map<DepartmentDto>(department) : null;
    }

    public async Task<IEnumerable<DepartmentDto>> GetDepartmentsByTypeAsync(DepartmentType type)
    {
        var departments = await _departmentRepository.GetByTypeAsync(type);

        return _mapper.Map<IEnumerable<DepartmentDto>>(departments);
    }

    public async Task<DepartmentDto> CreateDepartmentAsync(DepartmentDto departmentDto)
    {
        var coordinator = await _userRepository.GetByIdAsync(departmentDto.CoordinatorId);

        if (coordinator == null)
        {
            throw new ArgumentException("Invalid coordinator ID");
        }

        var department = new Department
        {
            Name = departmentDto.Name,
            Description = departmentDto.Description,
            Type = Enum.Parse<DepartmentType>(departmentDto.Type),
            CoordinatorId = departmentDto.CoordinatorId
        };

        await _departmentRepository.AddAsync(department);

        return _mapper.Map<DepartmentDto>(department);
    }

    public async Task<DepartmentDto?> UpdateDepartmentAsync(Guid id, DepartmentDto departmentDto)
    {
        var department = await _departmentRepository.GetByIdAsync(id);

        if (department == null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(departmentDto.Name))
        {
            department.Name = departmentDto.Name;
        }

        if (departmentDto.Description != null)
        {
            department.Description = departmentDto.Description;
        }

        if (!string.IsNullOrEmpty(departmentDto.Type))
        {
            department.Type = Enum.Parse<DepartmentType>(departmentDto.Type);
        }

        if (!string.IsNullOrEmpty(departmentDto.CoordinatorId))
        {
            var coordinator = await _userRepository.GetByIdAsync(departmentDto.CoordinatorId);
            if (coordinator == null)
            {
                throw new ArgumentException("Invalid coordinator ID");
            }
            department.CoordinatorId = departmentDto.CoordinatorId;
        }

        await _departmentRepository.UpdateAsync(department);

        return _mapper.Map<DepartmentDto>(department);
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