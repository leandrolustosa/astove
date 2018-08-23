(function () {
    "use strict";

    if (typeof Array.isArray === 'undefined') {
        Array.isArray = function (obj) {
            return Object.toString.call(obj) === '[object Array]';
        }
    };

    angular.module('AstoveApp').controller('UpdateCtrl', ['$scope', '$stateParams', 'messageBoxService', 'AstoveService', 'AstoveFilter', 'AstoveData', 'AstoveCommon', 'AstoveFile', 'AstoveFileResource', '$timeout', 'astoveParams',
            function ($scope, $stateParams, messageBoxService, $service, $filter, $data, $global, $file, AstoveFileResource, $timeout, astoveParams) {
                $global.loading = true;
                $scope.entityNameUpdate = astoveParams.controllerName;
                $scope.forms = {};
                $scope.service = $service;
                $scope.data = $data;
                $scope.filter = $filter;
                $scope.fileService = $file;
                $scope.messageBoxService = messageBoxService;
                $scope.global = $global;
                $scope.files = [];
                $scope.empresas = [];
                $scope.permissoes = [];
                $scope.celularMask = '(99)9999-9999?9';
                $scope.areaCoords = {
                    w: 75,
                    h: 75
                };
                $scope.areaMinSize = 75;
                $scope.aspectRatio = $scope.areaCoords.w / $scope.areaCoords.h;

                if (typeof onLoading === 'function')
                    onLoading();

                var promise = $service.getUpdateModelAsync($scope.entityNameUpdate);
                promise.then(
                    function (resp) {
                        $data.model = resp;
                    },
                    function (reason) {
                        console.log(reason);
                    }
                );

                $scope.getCep = function (cep) {
                    $service.getCepAsync(cep)
                        .then(
                            function (data) {
                                $data.model.cidade = data.cidade;
                                $data.model.siglaUF = data.uf.trim();
                            },
                            function (err) { $error.show(err); }
                        );
                };

                $scope.onImageSelect = function (files, evt) {
                    console.log(files);
                    if (files.length > 0) {
                        var file = files[0];
                        var _this = this;
                        var reader = new FileReader();

                        var id = evt.currentTarget.id;

                        var propertyName = id.replace('fileInput$', '');
                        var format = _.template("<%= propertyName %>Source");
                        console.log(format({ propertyName: propertyName }));

                        reader.onloadend = function () {
                            $scope.$apply(function ($scope) {
                                $scope.data.model[format({ propertyName: propertyName })] = reader.result;
                            });
                        }

                        reader.readAsDataURL(file);
                    }
                };

                $scope.onChanged = function (dataURI, imageData, id) {
                    console.log(dataURI);
                    var file = _.find($scope.files, function (f) { return f.id == id });
                    if (typeof file === 'undefined')
                        $scope.files.push({ id: id, data: dataURI });
                    else {
                        file.id = id;
                        file.data = dataURI;
                    }
                };

                $scope.setCelularMask = function () {
                    $timeout(function () {
                        if (typeof $data.model.celular !== 'undefined') {
                            if ($data.model.celular.replace('_', '').length > 13) {
                                $scope.celular = $data.model.celular;
                                $scope.celularMask = '(99)99999-999?9';
                                $data.model.celular = $scope.celular;
                            }
                            else
                                $scope.celularMask = '(99)9999-9999?9';
                        }
                    });
                };

                $scope.submitForm = function (isValid) {
                    $scope.showAlert = false;
                    $scope.hasError = false;
                    $scope.success = false;

                    if (isValid) {
                        var action = 'message';
                        var properties = 'Os dados da empresa foram salvos com sucesso!';

                        $file.uploadImageModelAsync($scope, $scope.files, astoveParams.directory, 'image').then(function (data) {
                            if (data !== null)
                                $data.model.fotoUrl = data.url;

                            $service.updateEntityForm($scope, astoveParams.putControllerName, $data.model, 'message', 'Os dados da empresa foram salvos com sucesso');

                        }, function (reason) {
                            console.log(reason);
                            $scope.message = 'Erro inesperado';
                            $scope.showAlert = true;
                        }, function (update) {
                            alert('Got notification: ' + update);
                        });
                    }
                };
            } ]);

})();