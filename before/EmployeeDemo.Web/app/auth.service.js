// Login state for the whole app. The real "session" is the HttpOnly cookie set by the server;
// this service only remembers who is logged in so the UI can show the name.
// After migration: auth.service.ts (with a signal for currentUser).
(function () {
    'use strict';

    angular.module('employeeApp')
        .factory('authService', ['$http', '$q', function ($http, $q) {
            var service = {
                currentUser: null,

                login: function (username, password) {
                    return $http.post('api/auth/login', { Username: username, Password: password })
                        .then(function (response) {
                            service.currentUser = response.data;
                            return response.data;
                        });
                },

                logout: function () {
                    return $http.post('api/auth/logout')
                        .finally(function () { service.currentUser = null; });
                },

                // Returns the logged-in user. After a page refresh we don't know yet, so we ask the server.
                requireUser: function () {
                    if (service.currentUser) {
                        return $q.resolve(service.currentUser);
                    }
                    return $http.get('api/auth/me').then(function (response) {
                        service.currentUser = response.data;
                        return response.data;
                    });
                },

                clear: function () {
                    service.currentUser = null;
                }
            };

            return service;
        }]);
})();
