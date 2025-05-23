using EmployeeListAPI.Application.Interfaces;
using EmployeeListAPI.Domain.Entities;
using EmployeeListAPI.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EmployeeListAPI.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees
                .Where(e => e.IsActive)
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .ToListAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == id && e.IsActive);
        }

        public async Task<Employee?> GetEmployeeByEmailAsync(string email)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.Email == email && e.IsActive);
        }

        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            // Using stored procedure for bonus points
            var parameters = new[]
            {
                new SqlParameter("@FirstName", employee.FirstName),
                new SqlParameter("@LastName", employee.LastName),
                new SqlParameter("@Email", employee.Email),
                new SqlParameter("@PhoneNumber", employee.PhoneNumber ?? (object)DBNull.Value),
                new SqlParameter("@Department", employee.Department),
                new SqlParameter("@Position", employee.Position),
                new SqlParameter("@Salary", employee.Salary),
                new SqlParameter("@HireDate", employee.HireDate)
            };

            var result = await _context.Database
                .SqlQueryRaw<int>("EXEC sp_CreateEmployee @FirstName, @LastName, @Email, @PhoneNumber, @Department, @Position, @Salary, @HireDate", parameters)
                .ToListAsync();

            var newEmployeeId = result.FirstOrDefault();
            return await GetEmployeeByIdAsync(newEmployeeId) ?? employee;
        }

        public async Task<Employee> UpdateEmployeeAsync(Employee employee)
        {
            employee.UpdatedAt = DateTime.UtcNow;

            // Using stored procedure for bonus points
            var parameters = new[]
            {
                new SqlParameter("@Id", employee.Id),
                new SqlParameter("@FirstName", employee.FirstName),
                new SqlParameter("@LastName", employee.LastName),
                new SqlParameter("@Email", employee.Email),
                new SqlParameter("@PhoneNumber", employee.PhoneNumber ?? (object)DBNull.Value),
                new SqlParameter("@Department", employee.Department),
                new SqlParameter("@Position", employee.Position),
                new SqlParameter("@Salary", employee.Salary),
                new SqlParameter("@HireDate", employee.HireDate),
                new SqlParameter("@IsActive", employee.IsActive)
            };

            await _context.Database
                .ExecuteSqlRawAsync("EXEC sp_UpdateEmployee @Id, @FirstName, @LastName, @Email, @PhoneNumber, @Department, @Position, @Salary, @HireDate, @IsActive", parameters);

            return await GetEmployeeByIdAsync(employee.Id) ?? employee;
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            // Soft delete using stored procedure
            var parameter = new SqlParameter("@Id", id);
            var result = await _context.Database
                .ExecuteSqlRawAsync("EXEC sp_DeleteEmployee @Id", parameter);

            return result > 0;
        }

        public async Task<bool> EmployeeExistsAsync(int id)
        {
            return await _context.Employees
                .AnyAsync(e => e.Id == id && e.IsActive);
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            var query = _context.Employees.Where(e => e.Email == email && e.IsActive);

            if (excludeId.HasValue)
            {
                query = query.Where(e => e.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
