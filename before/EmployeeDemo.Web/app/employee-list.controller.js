// List page: summary cards, search, table, delete.
// After migration: EmployeeListComponent (employee-list.component.ts).
// $scope.x becomes a class property, functions on $scope become class methods.
(function () {
    'use strict';

    // Numbers for the four summary cards at the top of the page.
    function summarize(list) {
        var departments = {};
        var active = 0;
        var salaries = 0;

        list.forEach(function (e) {
            departments[e.DepartmentId] = true;
            if (e.IsActive) { active++; }
            salaries += e.Salary;
        });

        return {
            count: list.length,
            active: active,
            departments: Object.keys(departments).length,
            salaries: salaries
        };
    }

    angular.module('employeeApp')
        .controller('EmployeeListController', ['$scope', '$window', 'employeeService',
            function ($scope, $window, employeeService) {

                $scope.employees = [];
                $scope.stats = summarize([]);
                $scope.search = '';
                $scope.loading = false;
                $scope.error = null;

                $scope.load = function () {
                    $scope.loading = true;
                    $scope.error = null;

                    employeeService.getAll($scope.search)
                        .then(function (data) {
                            $scope.employees = data;
                            $scope.stats = summarize(data);
                        })
                        .catch(function () { $scope.error = 'Could not load employees.'; })
                        .finally(function () { $scope.loading = false; });
                };

                $scope.clearSearch = function () {
                    $scope.search = '';
                    $scope.load();
                };

                $scope.remove = function (employee) {
                    if (!$window.confirm('Delete ' + employee.FirstName + ' ' + employee.LastName + '?')) {
                        return;
                    }

                    employeeService.remove(employee.EmployeeId)
                        .then($scope.load)
                        .catch(function () { $scope.error = 'Could not delete the employee.'; });
                };

                $scope.load();
            }]);
})();
