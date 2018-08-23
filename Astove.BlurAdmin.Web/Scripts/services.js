(function () {
    "use strict";

    angular.module('astove.modal',[]).provider('AstoveModal',
        function AstoveModalProvider() {
            this.mountRoutes = function ($stateProvider) {
                $stateProvider
                    .state('indicacoes', {
                        abstract: true,
                        url: "/indicacoes",
                        templateUrl: '/Common/Content',
                        title: 'Indicações',
                        sidebarMeta: {
                            icon: 'ion-compose',
                            order: 0
                        },
                    })
                    .state('indicacoes.atualizar', {
                        url: "/update",
                        templateUrl: "/Home/Update",
                        controller: "UpdateCtrl",
                        title: 'Alterar',
                        sidebarMeta: {
                            order: 0,
                        },
                        resolve: {
                            astoveParams: function () {
                                return { controllerName: 'pessoas/getprofiles', putControllerName: 'pessoas/changeusuario', directory: 'pessoas' };
                            }
                        }
                    })
                    .state('epr', {
                        abstract: true,
                        url: "/epr",
                        templateUrl: '/Common/Content',
                        title: 'Empresa',
                        sidebarMeta: {
                            icon: 'ion-compose',
                            order: 100
                        },
                    })
                    .state('epr.atualizar', {
                        url: "/update",
                        templateUrl: "/Empresa/Update",
                        controller: "UpdateCtrl",
                        title: 'Alterar',
                        sidebarMeta: {
                            order: 0,
                        },
                        resolve: {
                            astoveParams: function () {
                                return { controllerName: 'pessoas/getprofiles', putControllerName: 'pessoas/changeusuario', directory: 'pessoas' };
                            }
                        }
                    }).state('cfg', {
                        abstract: true,
                        url: "/cfg",
                        templateUrl: '/Common/Content',
                        title: 'Configurações',
                        sidebarMeta: {
                            icon: 'ion-compose',
                            order: 200
                        },
                    }).state('cfg.atualizar', {
                        url: "/update",
                        templateUrl: "/configuracao/Update/",
                        controller: "UpdateCtrl",
                        title: 'Alterar',
                        sidebarMeta: {
                            order: 0,
                        },
                        resolve: {
                            astoveParams: function () {
                                return { controllerName: 'configuracao/GetEditConfiguracao', putControllerName: 'configuracao/changeconfiguracao', addEditFilialModal: 'modal/addeditfilial', addEditFilialController: 'AddEditFilialCtrl' };
                            }
                        }
                    });

                
            };

            this.$get = function AstoveModalFactory() {

                // let's assume that the UnicornLauncher constructor was also changed to
                // accept and use the useTinfoilShielding argument
                return new AstoveModal();
            };
        });

})();