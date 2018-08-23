(function () {
    "use strict";

    if (typeof Array.isArray === 'undefined') {
        Array.isArray = function (obj) {
            return Object.toString.call(obj) === '[object Array]';
        }
    };

    angular.module('AstoveApp').controller('InsertCtrl', ['$scope', '$stateParams', 'AstoveService', 'AstoveFilter', 'AstoveData', 'AstoveFile', 'AstoveCommon', 'AstoveFileResource', 'AstoveUIDate',
        function ($scope, $stateParams, $service, $filter, $data, $file, $global, AstoveFileResource, $uiDatePtbr) {
            $scope.entityNameInsert = (typeof $stateParams.entityName === 'undefined' || $stateParams.entityName === null || $stateParams.entityName === '') ? $scope.entityName : $stateParams.entityName;
            $scope.forms = {};
            $scope.service = $service;
            $scope.data = $data;
            $scope.global = $global;
            $scope.filter = $filter;
            $scope.fileService = $file;

            $uiDatePtbr.initialize();

            if (typeof onLoading === 'function')
                onLoading();

            $service.getInsertModel($scope.entityNameInsert);

            $scope.getCep = function (cep) {
                if (typeof setEndereco === 'function') {
                    $service.getCepAsync(cep)
                        .then(
                            function (data) { setEndereco($data.model, data); },
                            function (err) { $error.show('Erro', 'OK', [err]); }
                        );
                }
            };

            $scope.callbackResult = function () {
                window.parent.onResult($data.model);
            };
        } ]);

})();