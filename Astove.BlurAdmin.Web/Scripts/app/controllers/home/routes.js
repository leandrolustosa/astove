(function () {
    "use strict";
    
    function $ConfigRoutes ($stateProvider, $urlRouterProvider, AstoveModalProvider) {

        $urlRouterProvider.otherwise("/indicacoes/update");

        AstoveModalProvider.mountRoutes($stateProvider);
    }

    angular
        .module('AstoveApp')
        .config(['$stateProvider', '$urlRouterProvider', 'AstoveModalProvider', $ConfigRoutes])
        .run(['$rootScope', '$state', function ($rootScope, $state) {
            $rootScope.$state = $state;
        }]);

})();