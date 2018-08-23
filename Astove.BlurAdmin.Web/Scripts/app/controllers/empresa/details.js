(function () {
    "use strict";

    if (typeof Array.isArray === 'undefined') {
        Array.isArray = function (obj) {
            return Object.toString.call(obj) === '[object Array]';
        }
    };

    //deps: ['./services/astove.filter', './services/ui-date-ptbr', './services/messagebox']
    angular.module('AstoveApp').controller('DetailsCtrl', ['$scope', '$stateParams', 'messageBoxService', 'AstoveService', 'AstoveFilter', 'AstoveData', 'AstoveFile', 'AstoveCommon', 'AstoveFileResource', '$timeout', 'astoveParams',
            function ($scope, $stateParams, messageBoxService, $service, $filter, $data, $file, $global, AstoveFileResource, $timeout, astoveParams) {
                $global.loading = true;
                $scope.entityNameUpdate = astoveParams.controllerName;
                $scope.forms = {};
                $scope.service = $service;
                $scope.data = $data;
                $scope.filter = $filter;
                $scope.fileService = $file;
                $scope.messageBoxService = messageBoxService;
                $scope.global = $global;

                if (typeof onLoading === 'function')
                    onLoading();

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
                getModelTimer = $timeout(getModel, 100);

                var getFullModelTimer;
                function getFullModel() {
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


                //                $scope.loadModel = true;
                //                if (typeof loadEntity === 'function') {
                //                    if (Array.isArray(loadEntity())) {
                //                        $scope.loadModel = loadEntity()[0];
                //                    }
                //                    else {
                //                        $scope.loadModel = !loadEntity();
                //                    }
                //                }

//                if ($('#loadModel').val() === 'true')
//                    $service.getModel($scope.entityNameUpdate, $routeParams.entityId);

                //                $scope.loadFullModel = false;
                //                if (typeof loadEntity === 'function') {
                //                    if (Array.isArray(loadEntity())) {
                //                        $scope.loadFullModel = loadEntity()[1];
                //                    }
                //                    else {
                //                        $scope.loadFullModel = loadEntity();
                //                    }
                //                }
                

//                if ($('#loadFullModel').val() === 'true')
//                    $service.getData($scope, $routeParams.entityId, 'select');
//                else
//                    $service.getModel($scope.entityNameUpdate, $routeParams.entityId);

                //                AstoveFileResource.get({ id: $routeParams.entityId }).$promise.then(function (data) {
                //                    console.log(JSON.stringify(data));
                //                    console.log(data.dpi);
                //                    $scope.dpi = data.dpi;
                //                });

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