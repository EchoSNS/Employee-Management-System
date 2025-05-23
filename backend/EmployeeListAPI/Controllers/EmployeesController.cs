using EmployeeListAPI.Application.DTOs.Employee;
using EmployeeListAPI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeListAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(IEmployeeService employeeService, ILogger<EmployeesController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        /// <summary>
        /// Get all employees
        /// </summary>
        /// <returns>List of employees</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmployeeListDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<EmployeeListDTO>>> GetAllEmployees()
        {
            _logger.LogInformation("Getting all employees");

            var result = await _employeeService.GetAllEmployeesAsync();

            if (!result.IsSuccess)
            {
                _logger.LogError("Error getting all employees: {ErrorMessage}", result.ErrorMessage);
                return StatusCode(500, new { message = result.ErrorMessage });
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Get employee by ID
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns>Employee details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EmployeeDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EmployeeDTO>> GetEmployee(int id)
        {
            _logger.LogInformation("Getting employee with ID: {EmployeeId}", id);

            var result = await _employeeService.GetEmployeeByIdAsync(id);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Employee with ID {EmployeeId} not found", id);
                return NotFound(new { message = result.ErrorMessage });
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Create a new employee
        /// </summary>
        /// <param name="createEmployeeDTO">Employee creation data</param>
        /// <returns>Created employee</returns>
        [HttpPost]
        [ProducesResponseType(typeof(EmployeeDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EmployeeDTO>> CreateEmployee([FromBody] CreateEmployeeDTO createEmployeeDTO)
        {
            _logger.LogInformation("Creating new employee with email: {Email}", createEmployeeDTO.Email);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _employeeService.CreateEmployeeAsync(createEmployeeDTO);

            if (!result.IsSuccess)
            {
                _logger.LogError("Error creating employee: {ErrorMessage}", result.ErrorMessage);
                return BadRequest(new { message = result.ErrorMessage });
            }

            _logger.LogInformation("Employee created successfully with ID: {EmployeeId}", result.Data?.Id);
            return CreatedAtAction(nameof(GetEmployee), new { id = result.Data?.Id }, result.Data);
        }

        /// <summary>
        /// Update an existing employee
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <param name="updateEmployeeDTO">Employee update data</param>
        /// <returns>Updated employee</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(EmployeeDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EmployeeDTO>> UpdateEmployee(int id, [FromBody] UpdateEmployeeDTO updateEmployeeDTO)
        {
            _logger.LogInformation("Updating employee with ID: {EmployeeId}", id);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _employeeService.UpdateEmployeeAsync(id, updateEmployeeDTO);

            if (!result.IsSuccess)
            {
                _logger.LogError("Error updating employee with ID {EmployeeId}: {ErrorMessage}", id, result.ErrorMessage);

                if (result.ErrorMessage.Contains("not found"))
                {
                    return NotFound(new { message = result.ErrorMessage });
                }

                return BadRequest(new { message = result.ErrorMessage });
            }

            _logger.LogInformation("Employee with ID {EmployeeId} updated successfully", id);
            return Ok(result.Data);
        }

        /// <summary>
        /// Delete an employee (soft delete)
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            _logger.LogInformation("Deleting employee with ID: {EmployeeId}", id);

            var result = await _employeeService.DeleteEmployeeAsync(id);

            if (!result.IsSuccess)
            {
                _logger.LogError("Error deleting employee with ID {EmployeeId}: {ErrorMessage}", id, result.ErrorMessage);

                if (result.ErrorMessage.Contains("not found"))
                {
                    return NotFound(new { message = result.ErrorMessage });
                }

                return StatusCode(500, new { message = result.ErrorMessage });
            }

            _logger.LogInformation("Employee with ID {EmployeeId} deleted successfully", id);
            return NoContent();
        }
    }
}
