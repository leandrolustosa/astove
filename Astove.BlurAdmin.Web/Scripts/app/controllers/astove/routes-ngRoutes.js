//(function () {
//    "use strict";

//    angular.module('AstoveApp').config(['$routeProvider',
//        function ($routeProvider) {
//            $routeProvider.
//                when('/list', {
//                    templateUrl: function (params) { return '/' + $('#route-name').val() + '/list'; },
//                    controller: 'ListCtrl'
//                }).
//                when('/details/:entityId', {
//                    templateUrl: '/' + $('#route-name').val() + '/details',
//                    controller: 'DetailsCtrl'
//                }).
//                when('/success/:entityId', {
//                    templateUrl: '/' + $('#route-name').val() + '/success',
//                    controller: 'DetailsCtrl'
//                }).
//                when('/update/:entityId/:entityName?', {
//                    templateUrl: function (params) { return '/' + $('#route-name').val() + '/update/' + params.entityId; },
//                    controller: 'UpdateCtrl'
//                }).
//                when('/profile/:entityId/:entityName?', {
//                    templateUrl: function (params) { return '/' + $('#route-name').val() + '/profile/' + params.entityId; },
//                    controller: 'UpdateCtrl'
//                }).
//                when('/step/:entityId/:entityName?', {
//                    templateUrl: function (params) { return '/' + $('#route-name').val() + '/step/' + params.entityId; },
//                    controller: 'UpdateCtrl'
//                }).
//                when('/statement/:entityId/:entityName?', {
//                    templateUrl: function (params) { return '/' + $('#route-name').val() + '/statement/' + params.entityId; },
//                    controller: 'UpdateCtrl'
//                }).
//                when('/option/:entityId/:entityName?', {
//                    templateUrl: function (params) { return '/' + $('#route-name').val() + '/option/' + params.entityId; },
//                    controller: 'UpdateCtrl'
//                }).
//                when('/insert', {
//                    templateUrl: function (params) { return '/' + $('#route-name').val() + '/insert'; },
//                    controller: 'InsertCtrl'
//                }).
//                when('/register', {
//                    templateUrl: function (params) { return '/' + $('#route-name').val() + '/insert'; },
//                    controller: 'InsertCtrl'
//                }).
//                when('/imageeditor/:imagemUrl/:largura/:altura/:larguraMin?/:alturaMin?/:razao?', {
//                    templateUrl: function (params) { return '/' + $('#route-name').val() + '/imageeditor?imagemUrl=' + params.imagemUrl + '&largura=' + params.largura + '&altura=' + params.altura + ((typeof params.larguraMin === 'undefined') ? '' : '&larguraMin=' + params.larguraMin) + ((typeof params.alturaMin === 'undefined') ? '' : '&alturaMin=' + params.alturaMin); },
//                    controller: 'ImageEditorCtrl'
//                }).
//                when('/filter', {
//                    templateUrl: '/' + $('#route-name').val() + '/filter',
//                    controller: 'FilterCtrl'
//                }).
//                otherwise({
//                    redirectTo: '/list'
//                });
//        } ]);

//})();