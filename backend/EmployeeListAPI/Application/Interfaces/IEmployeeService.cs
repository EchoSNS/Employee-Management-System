using EmployeeListAPI.Application.DTOs.Employee;
using EmployeeManagement.Domain.Common;

namespace EmployeeListAPI.Application.Interfaces
{
    public interface IEmployeeService
    {

        Task<ServiceResult<IEnumerable<EmployeeListDTO>>> GetAllEmployeesAsync();
        Task<ServiceResult<EmployeeDTO>> GetEmployeeByIdAsync(int id);
        Task<ServiceResult<EmployeeDTO>> CreateEmployeeAsync(CreateEmployeeDTO createEmployeeDto);
        Task<ServiceResult<EmployeeDTO>> UpdateEmployeeAsync(int id, UpdateEmployeeDTO updateEmployeeDto);
        Task<ServiceResult<bool>> DeleteEmployeeAsync(int id);
    }
}
