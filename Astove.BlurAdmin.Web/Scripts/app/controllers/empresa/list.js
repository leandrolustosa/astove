(function () {
    "use strict";

    angular.module('AstoveApp').controller('ListCtrl', ['$scope', 'AstoveApi', '$stateParams', 'AstoveService', 'AstoveDataList', 'AstoveFilterList', 'AstoveCommon', 'AstoveUIDate', 'astoveParams',
        function ($scope, $api, $stateParams, $service, $data, $filter, $global, AstoveUIDate, astoveParams) {
            $global.loading = true;
            $scope.service = $service;
            $scope.data = $data;
            $scope.filter = $filter;
            $scope.global = $global;

            if (typeof onLoading === 'function')
                onLoading();

            AstoveUIDate.initialize();

            $scope.entityName = astoveParams.controllerName;
            $service.reloadDataList($scope);

            $data.listen($scope);

            $scope.editarPerfil = function (id) {
                var templateUrl = _.template('/cta/update/<%= id %>');
                $service.redirect(templateUrl({ id: id }));
            };
        } ]);

})();