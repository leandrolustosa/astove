function config($stateProvider, $urlRouterProvider, AstoveModalProvider) {

    $urlRouterProvider.otherwise("/dashboards/painel");

    AstoveModalProvider.mountRoutes($stateProvider);
}
angular
    .module('AstoveApp')
    .config(['$stateProvider', '$urlRouterProvider', 'AstoveModalProvider', config])
    .run(['$rootScope', '$state', function ($rootScope, $state) {
        $rootScope.$state = $state;
    }]);