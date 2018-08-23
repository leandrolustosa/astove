(function () {
    "use strict";

    if (typeof Array.isArray === 'undefined') {
        Array.isArray = function (obj) {
            return Object.toString.call(obj) === '[object Array]';
        }
    };

    angular.module('AstoveApp').controller('AddOrEditIndicacaoCtrl', ['$scope', '$uibModalInstance', 'messageBoxService', 'AstoveService', 'AstoveFilter', 'AstoveData', 'AstoveError', 'AstoveFile', 'AstoveCommon', 'AstoveFileResource', '$timeout', 'astoveParams',
        function ($scope, $uibModalInstance, messageBoxService, $service, $filter, $data, $error, $file, $global, AstoveFileResource, $timeout, astoveParams) {
            $scope.forms = {};
            $scope.service = $service;
            $scope.data = $data;
            $scope.filter = $filter;
            $scope.fileService = $file;
            $scope.messageBoxService = messageBoxService;
            $scope.global = $global;
            $scope.celularMask = '(99)9999-9999?9';

            $scope.ok = function () {
                $uibModalInstance.close();
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };

            $scope.setEstadoId = function () {
                $data.modalModel.estadoId = parseInt($data.modalModel.estadoKV.key);
                $data.modalModel.estadoKV = _.findWhere($data.modalModel.estado.items, { key: $data.modalModel.estadoId.toString() });
                if (typeof $data.modalModel.estadoKV !== 'undefined')
                    $data.modalModel.siglaUF = $data.modalModel.estadoKV.value;
                $scope.getCidadesByParentId();
            };

            $scope.setCidadeId = function () {
                $data.modalModel.cidadeId = parseInt($data.modalModel.cidadeOptionsKV.key);
                $data.modalModel.cidadeOptionsKV = _.findWhere($data.modalModel.cidadeOptions.items, { key: $data.modalModel.cidadeId.toString() });
                if (typeof $data.modalModel.cidadeOptionsKV !== 'undefined')
                    $data.modalModel.cidadeNome = $data.modalModel.cidadeOptionsKV.value;
            };

            $scope.setCelularMask = function () {
                $timeout(function () {
                    if (typeof $data.modalModel.celular !== 'undefined') {
                        if ($data.modalModel.celular.replace('_','').length > 13) {
                            $scope.celular = $data.modalModel.celular;
                            $scope.celularMask = '(99)99999-999?9';
                            $data.modalModel.celular = $scope.celular;
                        }
                        else
                            $scope.celularMask = '(99)9999-9999?9';
                    }
                });
            };

            $scope.getCidadesByParentId = function () {
                $service.getDataWithParametersAsync('cidades/options', { parentId: $data.modalModel.estadoId })
                        .then(
                            function (data) {
                                $data.modalModel.cidadeOptions = data;
                                $global.loading = false;
                            },
                            function (err) {
                                $error.show(err);
                                $global.loading = false;
                            }
                        );
            };

            $scope.submitForm = function (isValid) {
                window.parent.onResult(astoveParams.screen, $data.modalModel);
                $uibModalInstance.close();
            };
        }]);

    angular.module('AstoveApp').controller('EditIndicacaoExpositorAgenciaCtrl', ['$scope', '$uibModalInstance', 'messageBoxService', 'AstoveService', 'AstoveFilter', 'AstoveData', 'AstoveError', 'AstoveFile', 'AstoveCommon', 'AstoveFileResource', '$timeout', 'astoveParams',
        function ($scope, $uibModalInstance, messageBoxService, $service, $filter, $data, $error, $file, $global, AstoveFileResource, $timeout, astoveParams) {
            $scope.forms = {};
            $scope.service = $service;
            $scope.data = $data;
            $scope.filter = $filter;
            $scope.fileService = $file;
            $scope.messageBoxService = messageBoxService;
            $scope.global = $global;

            $scope.ok = function () {
                $uibModalInstance.close();
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };

            $scope.submitForm = function (isValid) {
                if (isValid) {
                    window.parent.onResult('editindicacaoagencia', $data.modalModel);
                    $uibModalInstance.close();
                }
            };
        }]);

    angular.module('AstoveApp').controller('AssociarHotelEventoCtrl', ['$scope', '$uibModalInstance', 'messageBoxService', 'AstoveService', 'AstoveFilter', 'AstoveData', 'AstoveError', 'AstoveFile', 'AstoveCommon', 'AstoveFileResource', '$timeout', 'astoveParams',
        function ($scope, $uibModalInstance, messageBoxService, $service, $filter, $data, $error, $file, $global, AstoveFileResource, $timeout, astoveParams) {
            $scope.forms = {};
            $scope.service = $service;
            $scope.data = $data;
            $scope.filter = $filter;
            $scope.fileService = $file;
            $scope.messageBoxService = messageBoxService;
            $scope.global = $global;
            
            $scope.ok = function () {
                $uibModalInstance.close();
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };

            $service.getDataWithParametersAsync(astoveParams.controllerName, { id: astoveParams.hotelId }).then(
                function (response) {
                    $data.modalModel = response;
                    $global.loading = false;
                },
                function (errorResponse) {
                    $error.show(errorResponse, _.noop, true);
                    $global.loading = false;
                }
            );

            $scope.setEventoId = function () {
                $data.modalModel.eventoId = parseInt($data.modalModel.eventoKV.key);
                $data.modalModel.eventoKV = _.findWhere($data.modalModel.eventoOptions.items, { key: $data.modalModel.eventoId.toString() });
            };

            $scope.submitForm = function (isValid) {
                $scope.showAlert = false;
                $scope.hasError = false;
                $scope.success = false;

                if (isValid) {
                    $service.updateEntityFormAsync($scope, astoveParams.controllerName, $data.modalModel).then(
                        function (response) {
                            $global.loading = false;
                            $error.show(errorResponse, _.noop, true);
                            $uibModalInstance.close();
                        },
                        function (errorResponse) {
                            $error.show(errorResponse, _.noop, true);
                            $global.loading = false;
                        }
                    );
                }
            };
        }]);

    angular.module('AstoveApp').controller('AddOrEditTipoQuartoCtrl', ['$scope', '$uibModalInstance', 'messageBoxService', 'AstoveService', 'AstoveFilter', 'AstoveData', 'AstoveError', 'AstoveFile', 'AstoveCommon', 'AstoveFileResource', '$timeout', 'astoveParams',
        function ($scope, $uibModalInstance, messageBoxService, $service, $filter, $data, $error, $file, $global, AstoveFileResource, $timeout, astoveParams) {
            $scope.forms = {};
            $scope.service = $service;
            $scope.data = $data;
            $scope.filter = $filter;
            $scope.fileService = $file;
            $scope.messageBoxService = messageBoxService;
            $scope.global = $global;

            $scope.ok = function () {
                $uibModalInstance.close();
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };

            $scope.setTipoQuarto = function () {
                $data.modalModel.tipoQuarto = parseInt($data.modalModel.tipoQuartoOptions.selected.key);
                $data.modalModel.tipoQuartoOptions.selected = _.findWhere($data.modalModel.tipoQuartoOptions.items, { key: $data.modalModel.tipoQuarto });
                if (typeof $data.modalModel.tipoQuartoOptions.selected !== 'undefined')
                    $data.modalModel.nome = $data.modalModel.tipoQuartoOptions.selected.value;
            };

            $scope.submitForm = function (isValid) {
                $data.modalModel.tipoQuarto = parseInt($data.modalModel.tipoQuartoOptions.selected.key);
                $data.modalModel.tipoQuartoOptions.selected = _.findWhere($data.modalModel.tipoQuartoOptions.items, { key: $data.modalModel.tipoQuarto });
                if (typeof $data.modalModel.tipoQuartoOptions.selected !== 'undefined')
                    $data.modalModel.nome = $data.modalModel.tipoQuartoOptions.selected.value;

                window.parent.onResult(astoveParams.screen, $data.modalModel);
                $uibModalInstance.close();
            };
        }]);

    angular.module('AstoveApp').controller('AddOrEditAgendaCtrl', ['$scope', '$uibModalInstance', 'messageBoxService', 'AstoveService', 'AstoveFilter', 'AstoveData', 'AstoveError', 'AstoveFile', 'AstoveCommon', 'AstoveFileResource', '$timeout', 'astoveParams',
        function ($scope, $uibModalInstance, messageBoxService, $service, $filter, $data, $error, $file, $global, AstoveFileResource, $timeout, astoveParams) {
            $scope.forms = {};
            $scope.service = $service;
            $scope.data = $data;
            $scope.filter = $filter;
            $scope.fileService = $file;
            $scope.messageBoxService = messageBoxService;
            $scope.global = $global;

            $scope.ok = function () {
                $uibModalInstance.close();
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };

            $scope.setEventoExpositorId = function () {
                $data.modalModel.eventoExpositorId = parseInt($data.modalModel.expositorOptions.selected.key);
                if (!_.isUndefined($data.modalModel.expositorOptions.selected))
                    $data.modalModel.expositorNome = $data.modalModel.expositorOptions.selected.value;

                if ($data.modalModel.eventoId > 0 && $data.modalModel.eventoExpositorId > 0 && $data.modalModel.data !== null)
                    $scope.getHorariosByExpositorId();
            };

            $scope.onChangeData = function () {
                if ($data.modalModel.eventoId > 0 && $data.modalModel.eventoExpositorId > 0 && $data.modalModel.data !== null)
                    $scope.getHorariosByExpositorId();
            };

            $scope.setDataHora = function () {
                var data = moment($data.modalModel.data);
                var hora = parseInt($data.modalModel.dataHoraOptions.selected.key);
                data.hour(hora);
                $data.modalModel.dataHora = data.utc().format();
            };

            $scope.getHorariosByExpositorId = function () {
                var horariosEscolhidos = _.map(astoveParams.agendas, function(item) { return moment(item.dataHora).hour(); });
                $service.getDataWithParametersAsync('reservas/datahoraoptions', { eventoId: $data.modalModel.eventoId, eventoExpositorId: $data.modalModel.eventoExpositorId, data: $data.modalModel.data, horariosEscolhidos: horariosEscolhidos })
                        .then(
                            function (data) {
                                $data.modalModel.dataHoraOptions = data;
                                $data.modalModel.dataHoraOptions.selected = _.first($data.modalModel.dataHoraOptions.items);
                                $global.loading = false;
                            },
                            function (err) {
                                $error.show(err, _.noop, true);
                                $global.loading = false;
                            }
                        );
            };

            $scope.submitForm = function (isValid) {
                $scope.setDataHora();
                var objByExpo = _.findWhere(astoveParams.agendas, { eventoExpositorId: $data.modalModel.eventoExpositorId });
                var objByData = _.findWhere(astoveParams.agendas, { dataHora: $data.modalModel.dataHora });
                if (astoveParams.screen.indexOf('addagenda') !== -1 && !_.isUndefined(objByExpo)) {
                    $error.showAlert('Não é possível agendar duas pretenções para o mesmo expositor.', true);
                }
                else if (astoveParams.screen.indexOf('addagenda') !== -1 && !_.isUndefined(objByData)) {
                    $error.showAlert('Não é possível agendar duas pretenções para o mesmo horário.', true);
                }
                else {
                    window.parent.onResult(astoveParams.screen, $data.modalModel);
                    $uibModalInstance.close();
                }
            };
        }]);

    angular.module('AstoveApp').controller('AddOrEditHospedeCtrl', ['$scope', '$uibModalInstance', 'messageBoxService', 'AstoveService', 'AstoveFilter', 'AstoveData', 'AstoveError', 'AstoveFile', 'AstoveCommon', 'AstoveFileResource', '$timeout', 'astoveParams',
        function ($scope, $uibModalInstance, messageBoxService, $service, $filter, $data, $error, $file, $global, AstoveFileResource, $timeout, astoveParams) {
            $scope.forms = {};
            $scope.service = $service;
            $scope.data = $data;
            $scope.filter = $filter;
            $scope.fileService = $file;
            $scope.messageBoxService = messageBoxService;
            $scope.global = $global;

            $scope.ok = function () {
                $uibModalInstance.close();
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };

            $scope.submitForm = function (isValid) {
                if (isValid) {
                    window.parent.onResult(astoveParams.screen, $data.modalModel);
                    $uibModalInstance.close();
                }
            };
        }]);

    angular.module('AstoveApp').controller('AssociarIndicacaoCtrl', ['$scope', '$uibModalInstance', 'messageBoxService', 'AstoveService', 'AstoveFilter', 'AstoveData', 'AstoveError', 'AstoveFile', 'AstoveCommon', 'AstoveFileResource', '$timeout', 'astoveParams',
        function ($scope, $uibModalInstance, messageBoxService, $service, $filter, $data, $error, $file, $global, AstoveFileResource, $timeout, astoveParams) {
            $scope.forms = {};
            $scope.service = $service;
            $scope.data = $data;
            $scope.filter = $filter;
            $scope.fileService = $file;
            $scope.messageBoxService = messageBoxService;
            $scope.global = $global;

            $scope.ok = function () {
                $uibModalInstance.close();
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };

            $service.getDataWithParametersAsync(astoveParams.controllerName, { id: astoveParams.id }).then(
                function (response) {
                    $data.modalModel = response;
                    $global.loading = false;
                },
                function (errorResponse) {
                    $error.show(errorResponse, _.noop, true);
                    $global.loading = false;
                }
            );

            $scope.submitForm = function (isValid) {
                $scope.showAlert = false;
                $scope.hasError = false;
                $scope.success = false;

                if (isValid) {
                    window.parent.onResult('associarindicacao', $data.modalModel);
                    $uibModalInstance.close();
                }
            };
        }]);

    angular.module('AstoveApp').controller('EnviarEmailCtrl', ['$scope', '$uibModalInstance', 'messageBoxService', 'AstoveService', 'AstoveFilter', 'AstoveData', 'AstoveError', 'AstoveFile', 'AstoveCommon', 'AstoveFileResource', '$timeout', 'astoveParams',
       function ($scope, $uibModalInstance, messageBoxService, $service, $filter, $data, $error, $file, $global, AstoveFileResource, $timeout, astoveParams) {
           $scope.forms = {};
           $scope.service = $service;
           $scope.data = $data;
           $scope.filter = $filter;
           $scope.fileService = $file;
           $scope.messageBoxService = messageBoxService;
           $scope.global = $global;

           $scope.ok = function () {
               $uibModalInstance.close();
           };

           $scope.cancel = function () {
               $uibModalInstance.dismiss('cancel');
           };

           $data.modalModel = { id: astoveParams.id, email: '' };

           $scope.submitForm = function (isValid) {
               $scope.showAlert = false;
               $scope.hasError = false;
               $scope.success = false;

               if (isValid) {
                   $service.updateEntityAsync($scope, astoveParams.controllerName, $data.modalModel).then(
                       function (resp) {
                           window.alert('E-mail enviado com sucesso!');
                           $uibModalInstance.close();
                           $global.loading = false;
                       },
                       function (error) {
                           $error.show(error, _.noop, true);
                           $global.loading = false;
                       }
                    );
               }
           };
       }]);
})();