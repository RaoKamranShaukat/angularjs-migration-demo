// Add / edit page. The same controller handles both: if the route has an :id, it is edit mode.
// After migration: EmployeeFormComponent (employee-form.component.ts) with a Reactive Form.
(function () {
    'use strict';

    angular.module('employeeApp')
        .controller('EmployeeFormController', ['$scope', '$routeParams', '$location', '$filter', 'employeeService',
            function ($scope, $routeParams, $location, $filter, employeeService) {

                var id = $routeParams.id;

                $scope.isEdit = !!id;
                $scope.employee = { IsActive: true, HireDate: new Date() };
                $scope.departments = [];
                $scope.saving = false;
                $scope.error = null;

                employeeService.getDepartments()
                    .then(function (data) { $scope.departments = data; });

                if ($scope.isEdit) {
                    employeeService.getById(id)
                        .then(function (data) {
                            // <input type="date"> needs a real Date object, the API sends a string.
                            data.HireDate = new Date(data.HireDate);
                            $scope.employee = data;
                        })
                        .catch(function () { $scope.error = 'Employee not found.'; });
                }

                $scope.save = function (form) {
                    if (form.$invalid) {
                        form.$setSubmitted();
                        return;
                    }

                    $scope.saving = true;
                    $scope.error = null;

                    // Send the date as "yyyy-MM-dd". Sending the Date object would turn it into UTC
                    // (e.g. 2024-01-01T00:00 in Pakistan becomes 2023-12-31T19:00Z) and save the wrong day.
                    var payload = angular.copy($scope.employee);
                    payload.HireDate = $filter('date')($scope.employee.HireDate, 'yyyy-MM-dd');

                    var request = $scope.isEdit
                        ? employeeService.update(payload)
                        : employeeService.create(payload);

                    request
                        .then(function () { $location.path('/employees'); })
                        .catch(function (response) {
                            $scope.error = (response.data && response.data.Message) || 'Could not save the employee.';
                        })
                        .finally(function () { $scope.saving = false; });
                };
            }]);
})();
