using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeDemo.Web.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required, StringLength(50)]
        public string FirstName { get; set; }

        [Required, StringLength(50)]
        public string LastName { get; set; }

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a department.")]
        public int DepartmentId { get; set; }

        // Read-only: filled from the JOIN in the stored procedure, ignored on save.
        public string DepartmentName { get; set; }

        [Range(0, 999999999)]
        public decimal Salary { get; set; }

        public DateTime HireDate { get; set; }

        public bool IsActive { get; set; }
    }
}
