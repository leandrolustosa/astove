(function () {
    "use strict";

    angular.module('AstoveApp').controller('ListCtrl', ['$scope', 'AstoveApi', '$stateParams', 'AstoveService', 'AstoveDataList', 'AstoveFilterList', 'AstoveCommon', 'AstoveUIDate',
        function ($scope, $api, $stateParams, $service, $data, $filter, $global, AstoveUIDate) {
            $global.loading = true;
            $scope.service = $service;
            $scope.data = $data;
            $scope.filter = $filter;
            $scope.global = $global;

            if (typeof onLoading === 'function')
                onLoading();

            AstoveUIDate.initialize();

            $scope.entityName = 'entregalista';
            $service.reloadDataList($scope);

            $data.listen($scope);
        } ]);

})();