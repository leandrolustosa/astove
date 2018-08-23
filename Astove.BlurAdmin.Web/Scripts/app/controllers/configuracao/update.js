(function () {
    "use strict";

    if (typeof Array.isArray === 'undefined') {
        Array.isArray = function (obj) {
            return Object.toString.call(obj) === '[object Array]';
        }
    };

    angular.module('AstoveApp').controller('UpdateCtrl', ['$scope', '$stateParams', 'messageBoxService', 'AstoveService', 'AstoveFilter', 'AstoveData', 'AstoveCommon', 'AstoveFile', '$timeout', 'astoveParams',
        function ($scope, $stateParams, messageBoxService, $service, $filter, $data, $global, $file, $timeout, astoveParams) {
            $global.loading = true;
            $scope.forms = {};
            $scope.service = $service;
            $scope.data = $data;
            $scope.filter = $filter;
            $scope.fileService = $file;
            $scope.messageBoxService = messageBoxService;
            $scope.global = $global;
            $scope.files = [];
            $scope.areaCoords = {
                w: 867,
                h: 293
            };
            $scope.areaMinSize = 293;
            $scope.aspectRatio = $scope.areaCoords.w / $scope.areaCoords.h;

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

            if (typeof onLoading === 'function')
                onLoading();

            var promise = $service.getUpdateModel(astoveParams.controllerName);

            $scope.getCep = function (cep) {
                $service.getCepAsync(cep)
                    .then(
                        function (data) {
                            $data.model.logradouro = data.logradouro;
                            $data.model.bairro = data.bairro;
                            $data.model.cidade = data.cidade;
                            $data.model.siglaUF = data.uf.trim();
                        },
                        function (err) { $error.show(err); }
                    );
            };

            $scope.openModal = function (id) {
                var modalInstance = $uibModal.open({
                    templateUrl: '/Modal/AddOrEditTipoQuarto',
                    controller: 'ViewCtrl',
                    resolve: {
                        astoveParams: function () {
                            return { controllerName: astoveParams.controllerName, id: id };
                        }
                    }
                });
            };
                
            $scope.submitForm = function (isValid) {
                $scope.showAlert = false;
                $scope.hasError = false;
                $scope.success = false;

                if (isValid) {
                    $file.uploadImageModelAsync($scope, $scope.files, astoveParams.directory, 'image').then(function (data) {
                        if (data !== null)
                            $data.model.bannerVoucherUrl = data.url;

                        $service.updateEntityForm($scope, astoveParams.putControllerName, $data.model, 'message', 'As configurações foram salvas com sucesso');

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