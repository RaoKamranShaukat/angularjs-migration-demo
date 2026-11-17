using System.Data.SqlClient;
using System.Net;
using System.Web.Http;
using EmployeeDemo.Web.Data;
using EmployeeDemo.Web.Models;

namespace EmployeeDemo.Web.Controllers
{
    // GET    api/employees?search=kh
    // GET    api/employees/5
    // POST   api/employees
    // PUT    api/employees/5
    // DELETE api/employees/5
    [RoutePrefix("api/employees")]
    public class EmployeesController : ApiController
    {
        // No DI container in Web API 2 by default, so the controller creates the repository itself.
        private readonly EmployeeRepository _repository = new EmployeeRepository();

        [HttpGet, Route("")]
        public IHttpActionResult GetAll(string search = null)
        {
            return Ok(_repository.GetAll(search));
        }

        [HttpGet, Route("{id:int}", Name = "GetEmployeeById")]
        public IHttpActionResult GetById(int id)
        {
            var employee = _repository.GetById(id);
            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpPost, Route("")]
        public IHttpActionResult Create(Employee employee)
        {
            if (employee == null)
                return BadRequest("Employee data is required.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int newId;
            try
            {
                newId = _repository.Insert(employee);
            }
            catch (SqlException ex) when (IsDuplicateEmail(ex))
            {
                return DuplicateEmail();
            }

            return CreatedAtRoute("GetEmployeeById", new { id = newId }, _repository.GetById(newId));
        }

        [HttpPut, Route("{id:int}")]
        public IHttpActionResult Update(int id, Employee employee)
        {
            if (employee == null)
                return BadRequest("Employee data is required.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            employee.EmployeeId = id; // the id in the URL wins

            try
            {
                if (!_repository.Update(employee))
                    return NotFound();
            }
            catch (SqlException ex) when (IsDuplicateEmail(ex))
            {
                return DuplicateEmail();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpDelete, Route("{id:int}")]
        public IHttpActionResult Delete(int id)
        {
            if (!_repository.Delete(id))
                return NotFound();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // SQL Server error 2627/2601 = unique constraint violated (UQ_Employees_Email).
        private static bool IsDuplicateEmail(SqlException ex)
        {
            return ex.Number == 2627 || ex.Number == 2601;
        }

        private IHttpActionResult DuplicateEmail()
        {
            return Content(HttpStatusCode.Conflict, new { Message = "An employee with this email already exists." });
        }
    }
}
