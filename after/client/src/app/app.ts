import { Component, computed, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterLink, RouterOutlet } from '@angular/router';
import { filter, finalize, map } from 'rxjs';
import { AuthService } from './core/auth.service';
import { InitialsPipe } from './shared/initials.pipe';

// The page "frame": top bar with the logged-in user and the sign-out button.
// BEFORE: ShellController on <body> + <div ng-view>.  AFTER: the root component + <router-outlet>.
@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, InitialsPipe],
  templateUrl: './app.html',
})
export class App {
  protected readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  private readonly url = toSignal(
    this.router.events.pipe(
      filter((e): e is NavigationEnd => e instanceof NavigationEnd),
      map((e) => e.urlAfterRedirects),
    ),
    { initialValue: '' },
  );

  // The login page is full-screen, without the top bar.
  protected readonly isLoginPage = computed(() => this.url().startsWith('/login'));

  protected logout(): void {
    this.auth
      .logout()
      .pipe(finalize(() => this.router.navigate(['/login'])))
      .subscribe({ error: () => {} });
  }
}
