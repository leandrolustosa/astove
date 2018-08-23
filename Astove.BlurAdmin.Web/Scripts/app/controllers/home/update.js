(function () {
    "use strict";

    if (typeof Array.isArray === 'undefined') {
        Array.isArray = function (obj) {
            return Object.toString.call(obj) === '[object Array]';
        }
    };

    angular.module('AstoveApp').controller('UpdateCtrl', ['$scope', '$stateParams', 'messageBoxService', 'AstoveService', 'AstoveFilter', 'AstoveData', 'AstoveDataList', 'AstoveFilterList', 'AstoveCommon', 'AstoveError', 'AstoveFile', 'AstoveUIDate', '$uibModal', '$timeout', 'astoveParams',
            function ($scope, $stateParams, messageBoxService, $service, $filter, $data, $datalist, $filterlist, $global, $error, $file, AstoveUIDate, $modal, $timeout, astoveParams) {
                $global.loading = true;
                $scope.forms = {};
                $scope.service = $service;
                $scope.data = $data;
                $scope.datalist = $datalist;
                $scope.filter = $filter;
                $scope.fileService = $file;
                $scope.messageBoxService = messageBoxService;
                $scope.global = $global;
                $scope.files = [];
                $scope.areaCoords = {
                    w: 75,
                    h: 75
                };
                $scope.areaMinSize = 75;
                $scope.aspectRatio = $scope.areaCoords.w / $scope.areaCoords.h;
                $scope.evento = _.noop;
                $scope.eventoSelecionado = false;
                $scope.carregando = true;
                $scope.indicacao = {};

                if (typeof onLoading === 'function')
                    onLoading();

                $datalist.clear();

                $scope.defaultSort = {
                    field: 'EventoNome',
                    direction: 'asc'
                };

                $datalist.columnsToSort = [$scope.defaultSort.field];
                $datalist.directionsToSort = [$scope.defaultSort.direction];

                $scope.sortBy = function (field, fieldClass) {
                    $datalist.entity.sortOptions = null;
                    $datalist.sortBy($scope, field, $scope.defaultSort);
                    $filter.filtrar($scope);
                    $scope.rowSortClass(field);
                };

                if (typeof onLoading === 'function')
                    onLoading();

                AstoveUIDate.initialize();

                $scope.entityName = astoveParams.controllerName;
                var promise = $filterlist.filtrarAsync($scope);
                promise.then(
                    function (resp) {
                        if (resp.items.length === 1)
                            $scope.selecionarEvento(resp.items[0]);

                        $scope.carregando = false;
                    },
                    function (err) {
                        $error.show(err);
                        $scope.carregando = false;
                    }
                );

                $datalist.listen($scope);
                $filterlist.listen($scope);

                $scope.getCep = function (cep) {
                    if (typeof setEndereco === 'function') {
                        $service.getCepAsync(cep)
                            .then(
                                function (data) { setEndereco($data.model, data); },
                                function (err) { $error.show('Erro', 'OK', [err]); }
                            );
                    }
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

                $scope.rowEventoClass = function (evento) {
                    if (evento.numeroIndicados > evento.eventoNumeroMaximoIndicacoes)
                        return 'danger';
                    else if (evento.numeroIndicados === evento.eventoNumeroMaximoIndicacoes)
                        return 'info';
                    else
                        return '';
                };

                $scope.selecionarEvento = function (evento) {
                    var promise = $service.getUpdateModelAsync(astoveParams.controllerName, [evento.eventoId.toString(), evento.expositorId.toString()]);
                    promise.then(function (resp) {
                        $scope.evento = evento;
                        $scope.eventoSelecionado = true;
                        $scope.indicacao = _.clone($data.model.novaIndicacao);
                    },
                    function (err) {
                        $error.show(err);
                    });
                };

                $scope.voltarSelecionarEvento = function () {
                    $scope.evento = _.noop;
                    $scope.eventoSelecionado = false;
                };

                $scope.deleteIndicacao = function (index) {
                    var model = $data.model.indicacoes[index];
                    if (model.habilitado) {
                        model.excluida = true;
                        $service.updateEntityAsync($scope, astoveParams.controllerName, model).then(
                            function (resp) {
                                var evento = _.findWhere($datalist.entity.items, { eventoId: parseInt($data.model.eventoParentId), expositorId: $data.model.expositorId });
                                if (typeof evento !== 'undefined')
                                    evento.numeroIndicados -= 1;
                                $data.model.indicacoes.splice(index, 1);
                            },
                            function (error) {
                                $error.show(error);
                            }
                        );
                    }
                };

                $scope.setEstadoId = function () {
                    $data.modalModel.estadoId = parseInt($data.modalModel.estadoKV.key);
                    $data.modalModel.estadoKV = _.findWhere($data.modalModel.estado.items, { key: $data.modalModel.estadoId.toString() });
                    if (typeof $data.modalModel.estadoKV !== 'undefined')
                        $data.modalModel.siglaUF = $data.modalModel.estadoKV.value;
                };

                $scope.getCidadesByParentId = function () {
                    $service.getDataWithParametersAsync('cidades/options', { parentId: $data.modalModel.estadoId })
                            .then(
                                function (data) {
                                    $data.modalModel.cidadeOptions = data;
                                },
                                function (err) { $error.show(er); }
                            );
                };

                window.onResult = function (screen, model) {
                    if (screen.indexOf('addindicacao') != -1) {
                        $service.addEntityAsync($scope, astoveParams.controllerName, model).then(
                            function (resp) {
                                $data.model.indicacoes.push(resp);
                                var evento = _.findWhere($datalist.entity.items, { eventoId: parseInt($data.model.eventoParentId), expositorId: $data.model.expositorId });
                                if (typeof evento !== 'undefined')
                                    evento.numeroIndicados += 1;
                                $data.model.novaIndicacao = _.clone($scope.indicacao);
                            },
                            function (error) {
                                $error.show(error);
                            }
                        );
                    }
                    else if (screen.indexOf('editindicacao') != -1) {
                        $service.updateEntityAsync($scope, astoveParams.controllerName, model).then(
                            function (resp) {
                                var obj = _.findWhere($data.model.indicacoes, { id: model.id });
                                obj = _.extendOwn(obj, model);
                            },
                            function (error) {
                                $error.show(error);
                            }
                        );
                    }
                };

                $scope.openAddIndicacao = function () {
                    if ($scope.evento.numeroIndicados !== $scope.evento.eventoNumeroMaximoIndicacoes && $scope.indicacao.habilitado) {
                        $data.modalModel = $data.model.novaIndicacao;
                        $scope.openIndicacao('addindicacao');
                    }
                };

                $scope.openEditIndicacao = function (model) {
                    if (model.habilitado) {
                        $data.modalModel = model;
                        $scope.openIndicacao('editindicacao');
                    }
                };

                $scope.openIndicacao = function (screen) {
                    var modalInstance = $modal.open({
                        templateUrl: '/Modal/AddOrEditIndicacao',
                        controller: 'AddOrEditIndicacaoCtrl',
                        resolve: {
                            astoveParams: function () {
                                return { screen: screen };
                            }
                        }
                    });
                };

                $scope.submitForm = function (isValid) {
                    $scope.showAlert = false;
                    $scope.hasError = false;
                    $scope.success = false;

                    if (isValid) {
                        $service.addEntityForm($scope, astoveParams.controllerName, $data.model, 'message', 'As indicações foram salvas com sucesso');
                    }
                };
            }]);

})();