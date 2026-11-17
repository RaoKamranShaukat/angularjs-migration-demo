using EmployeeDemo.Api.Data;
using EmployeeDemo.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace EmployeeDemo.Api.Controllers;

// GET    api/employees?search=kh
// GET    api/employees/5
// POST   api/employees
// PUT    api/employees/5
// DELETE api/employees/5
//
// BEFORE: ApiController + [RoutePrefix], repository created with "new", manual ModelState checks.
// AFTER:  ControllerBase + [ApiController], repository injected, invalid models get an automatic 400.
[ApiController]
[Route("api/employees")]
public class EmployeesController(EmployeeRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Employee>>> GetAll(string? search) =>
        await repository.GetAllAsync(search);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Employee>> GetById(int id)
    {
        var employee = await repository.GetByIdAsync(id);
        return employee is null ? NotFound() : employee;
    }

    [HttpPost]
    public async Task<ActionResult<Employee>> Create(Employee employee)
    {
        int newId;
        try
        {
            newId = await repository.InsertAsync(employee);
        }
        catch (SqlException ex) when (IsDuplicateEmail(ex))
        {
            return DuplicateEmail();
        }

        return CreatedAtAction(nameof(GetById), new { id = newId }, await repository.GetByIdAsync(newId));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Employee employee)
    {
        employee.EmployeeId = id; // the id in the URL wins

        try
        {
            if (!await repository.UpdateAsync(employee))
                return NotFound();
        }
        catch (SqlException ex) when (IsDuplicateEmail(ex))
        {
            return DuplicateEmail();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        await repository.DeleteAsync(id) ? NoContent() : NotFound();

    // SQL Server error 2627/2601 = unique constraint violated (UQ_Employees_Email).
    private static bool IsDuplicateEmail(SqlException ex) => ex.Number is 2627 or 2601;

    private ConflictObjectResult DuplicateEmail() =>
        Conflict(new { message = "An employee with this email already exists." });
}
