import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../../core/auth.service';

// BEFORE: LoginController ($scope.credentials, $scope.login) + login.html with ng-model.
// AFTER:  a standalone component with a typed Reactive Form and signals for UI state.
@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule],
  templateUrl: './login.html',
})
export class LoginPage {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly form = inject(NonNullableFormBuilder).group({
    username: ['', Validators.required],
    password: ['', Validators.required],
  });

  protected readonly showPassword = signal(false);
  protected readonly busy = signal(false);
  protected readonly error = signal<string | null>(null);

  protected login(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.busy.set(true);
    this.error.set(null);

    const { username, password } = this.form.getRawValue();
    this.auth
      .login(username, password)
      .pipe(finalize(() => this.busy.set(false)))
      .subscribe({
        next: () => this.router.navigate(['/employees']),
        error: (err: HttpErrorResponse) =>
          this.error.set(err.error?.message ?? 'Login failed. Please try again.'),
      });
  }

  // Public demo only: fills in the demo account so visitors can try the app.
  protected useDemoAccount(): void {
    this.form.setValue({ username: 'admin', password: 'Admin@123' });
  }

  protected invalid(name: 'username' | 'password'): boolean {
    const control = this.form.controls[name];
    return control.invalid && control.touched;
  }
}
