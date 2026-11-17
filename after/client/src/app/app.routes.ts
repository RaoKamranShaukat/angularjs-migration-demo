import { Routes } from '@angular/router';
import { authGuard } from './core/auth.guard';
import { EmployeeForm } from './pages/employee-form/employee-form';
import { EmployeeList } from './pages/employee-list/employee-list';
import { LoginPage } from './pages/login/login';

// BEFORE: $routeProvider.when(...) with "#!/employees" URLs.
// AFTER:  Angular Router with clean URLs: /employees, /employees/5/edit.
export const routes: Routes = [
  { path: 'login', component: LoginPage },
  { path: 'employees', component: EmployeeList, canActivate: [authGuard] },
  { path: 'employees/new', component: EmployeeForm, canActivate: [authGuard] },
  { path: 'employees/:id/edit', component: EmployeeForm, canActivate: [authGuard] },
  { path: '', pathMatch: 'full', redirectTo: 'employees' },
  { path: '**', redirectTo: 'employees' },
];
