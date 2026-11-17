// All HTTP calls in one place. Every method returns a promise.
// After migration: employee.service.ts using HttpClient (returns Observables).
(function () {
    'use strict';

    angular.module('employeeApp')
        .factory('employeeService', ['$http', function ($http) {
            var baseUrl = 'api/employees';

            function getData(response) {
                return response.data;
            }

            return {
                getAll: function (search) {
                    return $http.get(baseUrl, { params: { search: search || null } }).then(getData);
                },
                getById: function (id) {
                    return $http.get(baseUrl + '/' + id).then(getData);
                },
                create: function (employee) {
                    return $http.post(baseUrl, employee).then(getData);
                },
                update: function (employee) {
                    return $http.put(baseUrl + '/' + employee.EmployeeId, employee);
                },
                remove: function (id) {
                    return $http.delete(baseUrl + '/' + id);
                },
                getDepartments: function () {
                    return $http.get('api/departments').then(getData);
                }
            };
        }]);
})();
