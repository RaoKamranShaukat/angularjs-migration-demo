import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Department, Employee } from './models';

// BEFORE: employee.service.js, an AngularJS factory using $http (returns promises).
// AFTER:  an injectable class using HttpClient (returns typed Observables).
@Injectable({ providedIn: 'root' })
export class EmployeeService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/employees';

  getAll(search?: string) {
    const term = search?.trim();
    const params = term ? new HttpParams().set('search', term) : undefined;
    return this.http.get<Employee[]>(this.baseUrl, { params });
  }

  getById(id: number) {
    return this.http.get<Employee>(`${this.baseUrl}/${id}`);
  }

  create(employee: Employee) {
    return this.http.post<Employee>(this.baseUrl, employee);
  }

  update(employee: Employee) {
    return this.http.put<void>(`${this.baseUrl}/${employee.employeeId}`, employee);
  }

  remove(id: number) {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  getDepartments() {
    return this.http.get<Department[]>('/api/departments');
  }
}
