using System.ComponentModel.DataAnnotations;

namespace EmployeeDemo.Api.Models;

public class Employee
{
    public int EmployeeId { get; set; }

    [Required, StringLength(50)]
    public string FirstName { get; set; } = "";

    [Required, StringLength(50)]
    public string LastName { get; set; } = "";

    [Required, EmailAddress, StringLength(100)]
    public string Email { get; set; } = "";

    [Range(1, int.MaxValue, ErrorMessage = "Please select a department.")]
    public int DepartmentId { get; set; }

    // Read-only: filled from the JOIN in the stored procedure, ignored on save.
    public string? DepartmentName { get; set; }

    [Range(0, 999999999)]
    public decimal Salary { get; set; }

    // BEFORE: DateTime, sent as "2019-03-11T00:00:00" (time zone bugs waiting to happen).
    // AFTER: DateOnly, sent as "2019-03-11". A hire date has no time, so it can't shift a day.
    public DateOnly HireDate { get; set; }

    public bool IsActive { get; set; }
}
