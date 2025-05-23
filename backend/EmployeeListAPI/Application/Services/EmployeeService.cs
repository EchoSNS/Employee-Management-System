using EmployeeListAPI.Application.DTOs.Employee;
using EmployeeListAPI.Application.Interfaces;
using EmployeeListAPI.Domain.Entities;
using EmployeeManagement.Domain.Common;

namespace EmployeeListAPI.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<ServiceResult<IEnumerable<EmployeeListDTO>>> GetAllEmployeesAsync()
        {
            try
            {
                var employees = await _employeeRepository.GetAllEmployeesAsync();
                var employeeDtos = employees.Select(MapToEmployeeListDto);

                return ServiceResult<IEnumerable<EmployeeListDTO>>.Success(employeeDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<EmployeeListDTO>>.Failure($"Error retrieving employees: {ex.Message}");
            }
        }

        public async Task<ServiceResult<EmployeeDTO>> GetEmployeeByIdAsync(int id)
        {
            try
            {
                var employee = await _employeeRepository.GetEmployeeByIdAsync(id);

                if (employee == null)
                {
                    return ServiceResult<EmployeeDTO>.Failure("Employee not found");
                }

                var employeeDto = MapToEmployeeDto(employee);
                return ServiceResult<EmployeeDTO>.Success(employeeDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<EmployeeDTO>.Failure($"Error retrieving employee: {ex.Message}");
            }
        }

        public async Task<ServiceResult<EmployeeDTO>> CreateEmployeeAsync(CreateEmployeeDTO createEmployeeDto)
        {
            try
            {
                // Validate email uniqueness
                if (await _employeeRepository.EmailExistsAsync(createEmployeeDto.Email))
                {
                    return ServiceResult<EmployeeDTO>.Failure("An employee with this email already exists");
                }

                var employee = MapToEmployee(createEmployeeDto);
                var createdEmployee = await _employeeRepository.CreateEmployeeAsync(employee);
                var employeeDto = MapToEmployeeDto(createdEmployee);

                return ServiceResult<EmployeeDTO>.Success(employeeDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<EmployeeDTO>.Failure($"Error creating employee: {ex.Message}");
            }
        }

        public async Task<ServiceResult<EmployeeDTO>> UpdateEmployeeAsync(int id, UpdateEmployeeDTO updateEmployeeDto)
        {
            try
            {
                var existingEmployee = await _employeeRepository.GetEmployeeByIdAsync(id);
                if (existingEmployee == null)
                {
                    return ServiceResult<EmployeeDTO>.Failure("Employee not found");
                }

                // Validate email uniqueness (excluding current employee)
                if (await _employeeRepository.EmailExistsAsync(updateEmployeeDto.Email, id))
                {
                    return ServiceResult<EmployeeDTO>.Failure("An employee with this email already exists");
                }

                // Update properties without mapper
                existingEmployee.FirstName = updateEmployeeDto.FirstName;
                existingEmployee.LastName = updateEmployeeDto.LastName;
                existingEmployee.Email = updateEmployeeDto.Email;
                existingEmployee.PhoneNumber = updateEmployeeDto.PhoneNumber;
                existingEmployee.Department = updateEmployeeDto.Department;
                existingEmployee.Position = updateEmployeeDto.Position;
                existingEmployee.Salary = updateEmployeeDto.Salary;
                existingEmployee.HireDate = updateEmployeeDto.HireDate;
                existingEmployee.IsActive = updateEmployeeDto.IsActive;

                var updatedEmployee = await _employeeRepository.UpdateEmployeeAsync(existingEmployee);
                var employeeDto = MapToEmployeeDto(updatedEmployee);

                return ServiceResult<EmployeeDTO>.Success(employeeDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<EmployeeDTO>.Failure($"Error updating employee: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteEmployeeAsync(int id)
        {
            try
            {
                var exists = await _employeeRepository.EmployeeExistsAsync(id);
                if (!exists)
                {
                    return ServiceResult<bool>.Failure("Employee not found");
                }

                var deleted = await _employeeRepository.DeleteEmployeeAsync(id);
                return ServiceResult<bool>.Success(deleted);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"Error deleting employee: {ex.Message}");
            }
        }

        // Manual mapping methods (no mapper as requested)
        private static Employee MapToEmployee(CreateEmployeeDTO dto)
        {
            return new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Department = dto.Department,
                Position = dto.Position,
                Salary = dto.Salary,
                HireDate = dto.HireDate,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        private static EmployeeDTO MapToEmployeeDto(Employee employee)
        {
            return new EmployeeDTO
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                FullName = employee.FullName,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                Department = employee.Department,
                Position = employee.Position,
                Salary = employee.Salary,
                HireDate = employee.HireDate,
                IsActive = employee.IsActive,
                CreatedAt = employee.CreatedAt,
                UpdatedAt = employee.UpdatedAt
            };
        }

        private static EmployeeListDTO MapToEmployeeListDto(Employee employee)
        {
            return new EmployeeListDTO
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Email = employee.Email,
                Department = employee.Department,
                Position = employee.Position,
                IsActive = employee.IsActive
            };
        }
    }
}
