using Microsoft.EntityFrameworkCore;
using OsutApp.Api.Data;
using OsutApp.Api.Models;

namespace OsutApp.Api.Repositories;

public class DepartmentRepository(ApplicationDbContext context) : IDepartmentRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Department?> GetByIdAsync(Guid id)
    {
        return await _context.Departments
            .Include(d => d.Coordinator)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<IEnumerable<Department>> GetAllAsync()
    {
        return await _context.Departments
            .Include(d => d.Coordinator)
            .ToListAsync();
    }

    public async Task<IEnumerable<Department>> GetByTypeAsync(DepartmentType type)
    {
        return await _context.Departments
            .Include(d => d.Coordinator)
            .Where(d => d.Type == type)
            .ToListAsync();
    }

    public async Task AddAsync(Department department)
    {
        await _context.Departments.AddAsync(department);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Department department)
    {
        _context.Departments.Update(department);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Department department)
    {
        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();
    }
}