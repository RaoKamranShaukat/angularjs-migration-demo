// The page "frame": top bar with the logged-in user and the logout button.
// After migration: AppComponent (app.component.ts / .html).
(function () {
    'use strict';

    angular.module('employeeApp')
        .controller('ShellController', ['$scope', '$location', 'authService',
            function ($scope, $location, authService) {

                $scope.auth = authService;

                // The login page is full-screen, without the top bar.
                $scope.isLoginPage = function () {
                    return $location.path() === '/login';
                };

                $scope.logout = function () {
                    authService.logout().finally(function () {
                        $location.path('/login');
                    });
                };
            }]);
})();
