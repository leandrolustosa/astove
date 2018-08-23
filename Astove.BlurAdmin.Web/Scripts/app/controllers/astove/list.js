(function () {
    "use strict";

    angular.module('AstoveApp').controller('ListCtrl', ['$scope', 'AstoveApi', '$routeParams', 'AstoveService', 'AstoveDataList', 'AstoveFilterList', 'AstoveCommon', 'AstoveUIDate', 'cfpLoadingBar',
        function ($scope, $api, $routeParams, $service, $data, $filter, $global, AstoveUIDate, cfpLoadingBar) {
            $global.loading = true;
            $scope.service = $service;
            $scope.data = $data;
            $scope.filter = $filter;
            $scope.global = $global;

            if (typeof onLoading === 'function')
                onLoading();

            AstoveUIDate.initialize();

            $service.reloadDataList($scope);

            $data.listen($scope);
        } ]);

})();