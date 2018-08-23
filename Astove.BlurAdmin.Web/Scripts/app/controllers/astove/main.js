function MainCtrl($scope, $http, $state, $stateParams, $service, $data, $filter, $global, AstoveUIDate, $uibModal) {
    $scope.service = $service;
    $scope.data = $data;
    $scope.filter = $filter;
    $scope.global = $global;
};

/**
 *
 * Pass all functions into module
 */
angular
    .module('AstoveApp')
    .controller('MainCtrl', ['$scope', '$http', '$state', '$stateParams', 'AstoveService', 'AstoveDataList', 'AstoveFilterList', 'AstoveCommon', 'AstoveUIDate', '$uibModal', MainCtrl]);