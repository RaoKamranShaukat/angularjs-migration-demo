// The AngularJS module: routes, the 401 interceptor and a small filter.
// After migration: app.routes.ts (routes + guard), auth.interceptor.ts, initials.pipe.ts.
(function () {
    'use strict';

    // Route "resolve": the page only opens when authService.requireUser() succeeds.
    // If nobody is logged in, the API answers 401 and the interceptor below sends the user to /login.
    // After migration: a CanActivateFn route guard.
    var requireUser = ['authService', function (authService) {
        return authService.requireUser();
    }];

    angular.module('employeeApp', ['ngRoute'])
        .config(['$routeProvider', '$httpProvider', function ($routeProvider, $httpProvider) {
            $routeProvider
                .when('/login', {
                    templateUrl: 'app/login.html',
                    controller: 'LoginController'
                })
                .when('/employees', {
                    templateUrl: 'app/employee-list.html',
                    controller: 'EmployeeListController',
                    resolve: { user: requireUser }
                })
                .when('/employees/new', {
                    templateUrl: 'app/employee-form.html',
                    controller: 'EmployeeFormController',
                    resolve: { user: requireUser }
                })
                .when('/employees/:id/edit', {
                    templateUrl: 'app/employee-form.html',
                    controller: 'EmployeeFormController',
                    resolve: { user: requireUser }
                })
                .otherwise({ redirectTo: '/employees' });

            $httpProvider.interceptors.push('authInterceptor');
        }])

        // Runs for every $http response. Any 401 (session missing or expired) -> go to the login page.
        .factory('authInterceptor', ['$q', '$location', '$injector', function ($q, $location, $injector) {
            return {
                responseError: function (response) {
                    var isLoginCall = response.config && /api\/auth\/login$/.test(response.config.url);

                    if (response.status === 401 && !isLoginCall) {
                        // $injector.get avoids a circular dependency: $http -> interceptor -> authService -> $http
                        $injector.get('authService').clear();
                        $location.path('/login');
                    }
                    return $q.reject(response);
                }
            };
        }])

        // "Ayesha Khan" -> "AK". Used for the avatar circles.
        .filter('initials', function () {
            return function (name) {
                return (name || '').trim().split(/\s+/).slice(0, 2)
                    .map(function (part) { return part.charAt(0).toUpperCase(); })
                    .join('');
            };
        });
})();
