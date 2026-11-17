import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, computed, inject, input, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Observable, finalize } from 'rxjs';
import { EmployeeService } from '../../core/employee.service';
import { Employee } from '../../core/models';

// Add / edit page. The same component handles both: if the route has an :id, it is edit mode.
// BEFORE: EmployeeFormController + ng-model + validation rules written in the HTML.
// AFTER:  a typed Reactive Form. The rules live in TypeScript, next to the data.
@Component({
  selector: 'app-employee-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './employee-form.html',
})
export class EmployeeForm implements OnInit {
  private readonly employeeService = inject(EmployeeService);
  private readonly router = inject(Router);

  // The ":id" route parameter (thanks to withComponentInputBinding). Undefined on /employees/new.
  // BEFORE: $routeParams.id
  readonly id = input<string>();

  protected readonly isEdit = computed(() => !!this.id());
  protected readonly departments = toSignal(this.employeeService.getDepartments(), { initialValue: [] });
  protected readonly saving = signal(false);
  protected readonly error = signal<string | null>(null);

  private readonly fb = inject(NonNullableFormBuilder);
  protected readonly form = this.fb.group({
    firstName: ['', [Validators.required, Validators.maxLength(50)]],
    lastName: ['', [Validators.required, Validators.maxLength(50)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(100)]],
    departmentId: [0, Validators.min(1)],
    salary: this.fb.control<number | null>(null, [Validators.required, Validators.min(0)]),
    // <input type="date"> works with a "yyyy-MM-dd" string: no Date object, so no UTC conversion.
    // BEFORE: we had to convert with $filter('date') before saving to avoid losing a day.
    hireDate: [new Date().toLocaleDateString('en-CA'), Validators.required], // en-CA = yyyy-MM-dd
    isActive: [true],
  });

  ngOnInit(): void {
    const id = this.id();
    if (id) {
      this.employeeService.getById(+id).subscribe({
        next: (employee) => this.form.patchValue(employee),
        error: () => this.error.set('Employee not found.'),
      });
    }
  }

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    this.error.set(null);

    const value = this.form.getRawValue();
    const employee: Employee = {
      ...value,
      employeeId: this.isEdit() ? +this.id()! : 0,
      salary: value.salary ?? 0,
    };

    const request: Observable<unknown> = this.isEdit()
      ? this.employeeService.update(employee)
      : this.employeeService.create(employee);

    request.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => this.router.navigate(['/employees']),
      error: (err: HttpErrorResponse) =>
        this.error.set(err.error?.message ?? 'Could not save the employee.'),
    });
  }

  protected invalid(name: keyof typeof this.form.controls): boolean {
    const control = this.form.controls[name];
    return control.invalid && control.touched;
  }
}
