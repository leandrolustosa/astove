(function () {
    "use strict";

    if (typeof Array.isArray === 'undefined') {
        Array.isArray = function (obj) {
            return Object.toString.call(obj) === '[object Array]';
        }
    };

    angular.module('AstoveApp').controller('InsertCtrl', ['$scope', '$stateParams', 'AstoveService', 'AstoveFilter', 'AstoveData', 'AstoveFile', 'AstoveCommon', 'AstoveFileResource', 'AstoveUIDate', '$uibModal', 'astoveParams',
        function ($scope, $stateParams, $service, $filter, $data, $file, $global, AstoveFileResource, $uiDatePtbr, $uibModal, astoveParams) {
            $scope.entityNameInsert = astoveParams.controllerName;
            $scope.forms = {};
            $scope.service = $service;
            $scope.data = $data;
            $scope.global = $global;
            $scope.filter = $filter;
            $scope.fileService = $file;
            $scope.files = [];
            $scope.empresas =[];
            $scope.permissoes =[];
            $scope.areaCoords = {
                w: 75,
                h: 75
            };
            $scope.areaMinSize = 75;
            $scope.aspectRatio = $scope.areaCoords.w / $scope.areaCoords.h;

            $uiDatePtbr.initialize();

            if (typeof onLoading === 'function')
                onLoading();

            $service.getInsertModel($scope.entityNameInsert);

            $scope.getCep = function (cep) {
                $service.getCepAsync(cep)
                    .then(
                        function (data) {
                            $data.model.logradouro = data.logradouro;
                            $data.model.bairro = data.bairro;
                            $data.model.cidade = data.cidade;
                            $data.model.estado = data.uf;
                        },
                        function (err) { $error.show('Erro', 'OK', [err]); }
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

            $scope.callbackResult = function () {
                window.parent.onResult($data.model);
            };

            $scope.submitForm = function (isValid) {
                $scope.showAlert = false;
                $scope.hasError = false;
                $scope.success = false;

                if (isValid) {
                    var action = 'route';
                    var properties = '/cta/list';

                    $data.model.codigoEmpresas = _.map($data.model.empresaOptions.items, function (obj, key) {
                        if ($scope.empresas[key])
                            return obj.key;
                        else
                            return '';
                    });
                    $data.model.codigoEmpresas = _.reject($data.model.codigoEmpresas, function (value) {
                        return value === '';
                    });

                    $data.model.permissoes = _.map($data.model.permissaoOptions.items, function (obj, key) {
                        if ($scope.permissoes[key])
                            return obj.key;
                        else
                            return '';
                    });
                    $data.model.permissoes = _.reject($data.model.permissoes, function (value) {
                        return value === '';
                    });

                    $file.uploadImageModelAsync($scope, $scope.files, astoveParams.directory, 'image').then(function (data) {
                        if (data !== null)
                            $data.model.fotoUrl = data.url;

                        $service.addEntityForm($scope, astoveParams.postControllerName, $data.model, 'redirect', '/cta/list');

                    }, function (reason) {
                        console.log(reason);
                        $scope.message = 'Erro inesperado';
                        $scope.showAlert = true;
                    }, function (update) {
                        alert('Got notification: ' + update);
                    });
                }
            };
        }]);

})();