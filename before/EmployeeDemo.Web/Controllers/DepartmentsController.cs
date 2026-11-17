using System.Web.Http;
using EmployeeDemo.Web.Data;

namespace EmployeeDemo.Web.Controllers
{
    // GET api/departments  (fills the dropdown on the employee form)
    [RoutePrefix("api/departments")]
    public class DepartmentsController : ApiController
    {
        private readonly EmployeeRepository _repository = new EmployeeRepository();

        [HttpGet, Route("")]
        public IHttpActionResult GetAll()
        {
            return Ok(_repository.GetDepartments());
        }
    }
}
