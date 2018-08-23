(function () {
    "use strict";

    if (typeof Array.isArray === 'undefined') {
        Array.isArray = function (obj) {
            return Object.toString.call(obj) === '[object Array]';
        }
    };

    angular.module('AstoveApp').controller('UpdateCtrl', ['$scope', '$routeParams', 'messageBoxService', 'AstoveService', 'AstoveFilter', 'AstoveData', 'AstoveCommon', 'AstoveFile', 'cfpLoadingBar', 'AstoveFileResource', '$timeout',
            function ($scope, $routeParams, messageBoxService, $service, $filter, $data, $global, $file, cfpLoadingBar, AstoveFileResource, $timeout) {
                $global.loading = true;
                $scope.entityNameUpdate = (typeof $routeParams.entityName === 'undefined' || $routeParams.entityName === null || $routeParams.entityName === '') ? $scope.entityName : $routeParams.entityName;
                $scope.forms = {};
                $scope.service = $service;
                $scope.data = $data;
                $scope.filter = $filter;
                $scope.fileService = $file;
                $scope.messageBoxService = messageBoxService;
                $scope.global = $global;

                if (typeof onLoading === 'function')
                    onLoading();

                //                $scope.loadModel = true;
                //                if (typeof loadEntity === 'function') {
                //                    if (Array.isArray(loadEntity())) {
                //                        $scope.loadModel = loadEntity()[0];
                //                    }
                //                    else {
                //                        $scope.loadModel = !loadEntity();
                //                    }
                //                }

                var getModelTimer;
                function getModel() {
                    if ($scope.loadModel === 'true') {
                        $service.getModel($scope.entityNameUpdate, $routeParams.entityId);
                        $timeout.cancel(getModelTimer);
                    }
                    timer += 1;
                    if (timer === 1)
                        $timeout.cancel(getModelTimer);
                }

                var timer = 0;
                getModelTimer = $timeout(getModel, 1000);


                //console.log($data.model);

                //                $scope.loadFullModel = false;
                //                if (typeof loadEntity === 'function') {
                //                    if (Array.isArray(loadEntity())) {
                //                        $scope.loadFullModel = loadEntity()[1];
                //                    }
                //                    else {
                //                        $scope.loadFullModel = loadEntity();
                //                    }
                //                }

                var getFullModelTimer;
                function getFullModel() {
                    console.log($scope.loadFullModel);
                    if ($scope.loadFullModel === 'true') {
                        $service.getData($scope, $routeParams.entityId, 'select');
                        $timeout.cancel(getFullModelTimer);
                    }
                    timer += 1;
                    if (timer === 1)
                        $timeout.cancel(getFullModelTimer);
                }

                var timer = 0;
                getFullModelTimer = $timeout(getFullModel, 1000);



                //AstoveFileResource.get({ id: $routeParams.entityId }).$promise.then(function (data) {
                //    console.log(JSON.stringify(data));
                //    console.log(data.dpi);
                //    $scope.dpi = data.dpi;
                //});

                $scope.getCep = function (cep) {
                    if (typeof setEndereco === 'function') {
                        $service.getCepAsync(cep)
                            .then(
                                function (data) { setEndereco(($scope.loadModel === true) ? $data.model : $data.entity, data); },
                                function (err) { $error.show('Erro', 'OK', [err]); }
                            );
                    }
                };
            } ]);

})();