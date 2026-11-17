using EmployeeDemo.Api.Data;
using EmployeeDemo.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeDemo.Api.Controllers;

// GET api/departments  (fills the dropdown on the employee form)
[ApiController]
[Route("api/departments")]
public class DepartmentsController(EmployeeRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Department>>> GetAll() =>
        await repository.GetDepartmentsAsync();
}
