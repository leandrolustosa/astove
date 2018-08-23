(function () {
    "use strict";

    angular.module('AstoveApp').factory('AstoveApi', ['$resource',
        function ($resource) {
            return function (entityName, antiForgeryToken) {
                var url = '/api/';
                //antiForgeryToken = '';
                var AstoveApi = $resource(url, {}, {
                    'getAll': {
                        url: url + ((entityName.indexOf('/') > -1) ? 'ac/' : 'v1/') + entityName + '/',
                        method: 'GET',
                        headers: {
                            'Content-Type': 'application/json',
                            'Authorization': 'Bearer ' + window.localStorage.getItem('accessToken')
                        },
                        params: { page: 1, take: 20, type: 'Paged' }
                    },
                    'get': {
                        url: url + ((entityName.indexOf('/') > -1) ? 'ac/' : 'v1/') + entityName + '/:id',
                        method: 'GET',
                        headers: {
                            'Content-Type': 'application/json',
                            'Authorization': 'Bearer ' + window.localStorage.getItem('accessToken')
                        },
                        params: { id: "@id" }
                    },
                    'getAction': {
                        url: url + ((entityName.indexOf('/') > -1) ? 'ac/' : 'v1/') + entityName + '/',
                        method: 'GET',
                        headers: {
                            'Content-Type': 'application/json',
                            'Authorization': 'Bearer ' + window.localStorage.getItem('accessToken')
                        }
                    },
                    'query': {
                        url: url + ((entityName.indexOf('/') > -1) ? 'ac/' : 'v1/') + entityName + '/',
                        method: 'GET',
                        headers: {
                            'Content-Type': 'application/json',
                            'Authorization': 'Bearer ' + window.localStorage.getItem('accessToken')
                        },
                        params: { id: 0 }
                    },
                    'insert': {
                        url: url + ((entityName.indexOf('/') > -1) ? 'ac/' : 'v1/') + entityName,
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                            'Authorization': 'Bearer ' + window.localStorage.getItem('accessToken'),
                            'RequestVerificationToken': antiForgeryToken
                        },
                        params: {}
                    },
                    'update': {
                        url: url + ((entityName.indexOf('/') > -1) ? 'ac/' : 'v1/') + entityName + '/:id',
                        method: 'PUT',
                        headers: {
                            'Content-Type': 'application/json',
                            'Authorization': 'Bearer ' + window.localStorage.getItem('accessToken'),
                            'RequestVerificationToken': antiForgeryToken
                        },
                        params: { id: "@id" }
                    },
                    'remove': {
                        url: url + ((entityName.indexOf('/') > -1) ? 'ac/' : 'v1/') + entityName + '/:id',
                        method: 'DELETE',
                        headers: {
                            'Content-Type': 'application/json',
                            'Authorization': 'Bearer ' + window.localStorage.getItem('accessToken')
                        },
                        params: { id: "@id" }
                    }
                });
                return AstoveApi;
            }
        } ]);

})();