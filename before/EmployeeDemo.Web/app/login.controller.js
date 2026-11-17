// Login page.
// After migration: LoginComponent (login.component.ts) with a Reactive Form.
(function () {
    'use strict';

    angular.module('employeeApp')
        .controller('LoginController', ['$scope', '$location', 'authService',
            function ($scope, $location, authService) {

                $scope.credentials = { username: '', password: '' };
                $scope.showPassword = false;
                $scope.busy = false;
                $scope.error = null;

                $scope.login = function (form) {
                    if (form.$invalid) {
                        form.$setSubmitted();
                        return;
                    }

                    $scope.busy = true;
                    $scope.error = null;

                    authService.login($scope.credentials.username, $scope.credentials.password)
                        .then(function () { $location.path('/employees'); })
                        .catch(function (response) {
                            $scope.error = (response.data && response.data.Message) || 'Login failed. Please try again.';
                        })
                        .finally(function () { $scope.busy = false; });
                };

                // Public demo only: fills in the demo account so visitors can try the app.
                $scope.useDemoAccount = function () {
                    $scope.credentials.username = 'admin';
                    $scope.credentials.password = 'Admin@123';
                };
            }]);
})();
