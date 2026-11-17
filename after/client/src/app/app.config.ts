import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { authInterceptor } from './core/auth.interceptor';
import { routes } from './app.routes';

// BEFORE: angular.module('employeeApp', ['ngRoute']).config(...)
// AFTER:  a list of providers. No zone.js: Angular 22 updates the screen from signals.
export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(withInterceptors([authInterceptor])),
    // withComponentInputBinding: the ":id" route parameter arrives as a component input.
    provideRouter(routes, withComponentInputBinding()),
  ],
};
