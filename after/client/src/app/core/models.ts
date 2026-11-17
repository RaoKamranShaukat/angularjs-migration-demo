// Types for the API data. AngularJS had none: a typo like "e.FristName" just showed nothing.
// Property names are camelCase because ASP.NET Core sends camelCase JSON
// (Web API 2 sent PascalCase: EmployeeId -> employeeId).

export interface Employee {
  employeeId: number;
  firstName: string;
  lastName: string;
  email: string;
  departmentId: number;
  departmentName?: string;
  salary: number;
  hireDate: string; // "yyyy-MM-dd" (DateOnly on the server, so no time zone shifts)
  isActive: boolean;
}

export interface Department {
  departmentId: number;
  name: string;
}

export interface UserInfo {
  username: string;
  fullName: string;
}
