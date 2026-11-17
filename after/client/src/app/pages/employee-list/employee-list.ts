import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { EmployeeService } from '../../core/employee.service';
import { Employee } from '../../core/models';
import { InitialsPipe } from '../../shared/initials.pipe';

// BEFORE: EmployeeListController. $scope.employees, $scope.stats, $scope.load()...
// AFTER:  a component class. $scope.x became a signal, $scope functions became methods,
//         and the stats are a computed() that updates itself when the list changes.
@Component({
  selector: 'app-employee-list',
  imports: [RouterLink, FormsModule, DecimalPipe, DatePipe, InitialsPipe],
  templateUrl: './employee-list.html',
})
export class EmployeeList {
  private readonly employeeService = inject(EmployeeService);

  protected readonly employees = signal<Employee[]>([]);
  protected readonly search = signal('');
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);

  // Numbers for the four summary cards at the top of the page.
  protected readonly stats = computed(() => {
    const list = this.employees();
    return {
      count: list.length,
      active: list.filter((e) => e.isActive).length,
      departments: new Set(list.map((e) => e.departmentId)).size,
      salaries: list.reduce((sum, e) => sum + e.salary, 0),
    };
  });

  constructor() {
    this.load();
  }

  protected load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.employeeService
      .getAll(this.search())
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (data) => this.employees.set(data),
        error: () => this.error.set('Could not load employees.'),
      });
  }

  protected clearSearch(): void {
    this.search.set('');
    this.load();
  }

  protected remove(employee: Employee): void {
    if (!confirm(`Delete ${employee.firstName} ${employee.lastName}?`)) {
      return;
    }

    this.employeeService.remove(employee.employeeId).subscribe({
      next: () => this.load(),
      error: () => this.error.set('Could not delete the employee.'),
    });
  }

  // Same colour for everyone in a department (avatar + pill).
  protected tone(employee: Employee): string {
    return `tone-${employee.departmentId % 6}`;
  }
}
