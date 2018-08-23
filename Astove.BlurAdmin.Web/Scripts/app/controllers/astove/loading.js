(function () {
    "use strict";
    
    angular.module('AstoveApp').controller('LoadingCtrl', ['$scope', 'AstoveCommon', 
        function ($scope, $global) {
            $scope.global = $global;
        } ]);

    angular.module('AstoveApp').controller('ProfileCtrl', ['$scope', 'AstoveService', 'AstoveCommon', 'AstoveData', 'AstoveError', function ($scope, $service, $global, $data, $error) {
        $scope.data = $data;
        $scope.apresentaMenuIndicacao = true;
        $scope.apresentaMenuReservas = false;
        $scope.apresentaMenuConfiguracoes = true;
        $scope.apresentaMenuDashboard = true;
        $scope.apresentaMenuEmpresa = false;
        
        $scope.getProfile = function () {
            $service.executeGetActionAsync('pessoas/profile').then(function (resp) {
                $data.profile = resp;
                $global.loading = false;
                $scope.apresentaMenuIndicacao = $data.profile.empresaTipoAcesso === 0 || $data.profile.empresaTipoAcesso === 3 || $data.profile.empresaTipoAcesso === 4;
                $scope.apresentaMenuReservas = $data.profile.empresaTipoAcesso === 3 || $data.profile.empresaTipoAcesso === 4;
                $scope.apresentaMenuConfiguracoes = $data.profile.empresaTipoAcesso === 3 || $data.profile.empresaTipoAcesso === 4;
                $scope.apresentaMenuDashboard = $data.profile.empresaTipoAcesso === 0 || $data.profile.empresaTipoAcesso === 3 || $data.profile.empresaTipoAcesso === 4;
                $scope.apresentaMenuEmpresa = $data.profile.empresaTipoAcesso === 0 || $data.profile.empresaTipoAcesso === 3 || $data.profile.empresaTipoAcesso === 4;
            }, function (errResponse) {
                $error.show('Error', 'OK', errResponse);
                $global.loading = false;
            });
        };

        $scope.getProfile();

        $scope.apresentaMenu = function (permissao) {
            return _.filter($data.profile.permissoes, function (value) { return value.indexOf(permissao) != -1 }).length > 0;
        };

        $scope.editarPerfil = function (id) {
            var templateUrl = _.template('/epr/update/<%= id %>');
            $service.redirect(templateUrl({ id: id }));
        };
    }]);

})();